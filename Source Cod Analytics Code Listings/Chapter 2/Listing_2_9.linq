<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code =
@"int fun(int x)
{
x++; 
if (x == 0) 
return x 
else 
return x+2;
}
double funny3(int x)
{
return x/12;
}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.Where(t => t.Kind() == SyntaxKind.MethodDeclaration)
	.Cast<MethodDeclarationSyntax>()
	.Select(t =>
	new
	{
	   Name = t.Identifier.ValueText, //#1
		Returns = t.Body.DescendantTokens()
	.Count(st => st.Kind() == SyntaxKind.ReturnKeyword)//#2
	})
	//Method should ideally have one return statement
	//That way it is easier to refactor them later.
	.Where(t => t.Returns > 1).Dump("Multiple return statements");

