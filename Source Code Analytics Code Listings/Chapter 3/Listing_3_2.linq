<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

//avoid out parameter

string code = @"class OutExample {
 static void Method(out int i) {
  i = 44;
 }
 static void Main() {
  int value;
  Method(out value);
 }
}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()//#1
	.Select(cds => new
	{
		ClassName = cds.Identifier.ValueText,
		Methods = cds.Members.OfType<MethodDeclarationSyntax>()//#2
					.Where(mds => //#3 
							mds.ParameterList.Parameters.Any(z => z.Modifiers.Any(m => m.ValueText == "out")))
					.Select(mds => mds.Identifier.ValueText)

	})

.Dump("Methods with \"out\" parameters");