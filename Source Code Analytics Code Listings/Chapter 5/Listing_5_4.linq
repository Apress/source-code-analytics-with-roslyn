<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>


var code = @"/// <summary>
/// Sample singleton object.
/// </summary>
public sealed class SiteStructure
{
    /// <summary>
    /// This is an expensive resource.
    /// We need to only store it in one place.
    /// </summary>
    object[] _data = new object[10];

    /// <summary>
    /// Allocate ourselves.
    /// We have a private constructor, so no one else can.
    /// </summary>
    static readonly SiteStructure _instance = new SiteStructure();

    /// <summary>
    /// Access SiteStructure.Instance to get the singleton object.
    /// Then call methods on that instance.
    /// </summary>
    public static SiteStructure Instance
    {
	get { return _instance; }
    }

    /// <summary>
    /// This is a private constructor, meaning no outsiders have access.
    /// </summary>
    private SiteStructure()
    {
	// Initialize members here.
    }
}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

//Bottom up approach.

//Finding the return statements and then finding our path

//up till we find a class that seems to be a singleton.

var modifiers = new List<string>() { "public", "static" };

var staticFields = tree
				.GetRoot()
				.DescendantNodes()
				.OfType<ClassDeclarationSyntax>()
				.Select(cds => new
				{
					ClassName = cds.Identifier.ValueText,
					Fields = cds.Members.OfType<FieldDeclarationSyntax>().Where(fds => modifiers.Any(mod =>fds.Modifiers.Select(m => m.Text).Contains(mod)))
																		 .SelectMany(fds => fds.Declaration.Variables.Select(v => v.Identifier.ValueText.Trim()))

				}).Dump("static fields");//#2

var staticReturns = tree.GetRoot()
						.DescendantNodes()
						.OfType<ReturnStatementSyntax>()
						.Where(rss => staticFields.SelectMany(f => f.Fields).Any(f => rss.ChildNodes().Select(r => r.ToFullString()).Contains(f)))
						.Dump("static returns");//#3

staticReturns
	.Select(r => new
	{
		//The name of the variable
		VariableName = r.Expression.GetLastToken().Text,
		//Type of the public method or public property
		//that exposes the singleton instance
		ParentType = r.Ancestors().First(ty =>ty.Kind() == SyntaxKind.PropertyDeclaration || ty.Kind() == SyntaxKind.MethodDeclaration)
								  .ChildNodes().First().ToFullString(),
		//Name of the class
		//if the class is singleton, then this name and the ParentType
		//Should match
		Name = r?.Ancestors()?.OfType<ClassDeclarationSyntax>().First().Identifier.ValueText
	})//#4

.Where(r =>
	//Checking whether the return type of the publicly
	//exposed method or property match up the name of the class or //not
	r.ParentType.Trim() == r.Name.Trim()
	//Checking whether the variable name used actually is of the //type being returned or not. It may be possible for a class to //have many such functions
	&& staticFields.Any(f => f.ClassName == r.Name
	&& f.Fields.Contains(r.VariableName.Trim())))//#5
//Select just the names of the classes which seems to be //Singleton
.Select(r => r.Name)
//Dump those names.
.Dump("Probable Singleton Classes");