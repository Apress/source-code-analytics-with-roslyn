<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

//Avoid using ToLower(), ToUpper() on string literals

var code = @"class A
{
	public string fun()
	{
		int x = ""EDGE"".Length;
		string s = ""Edge"".Substring(1,4);
		int y = ""234"".TryParse();
		return ""EDGE"".ToLower();
	}
	public string GetRep(string upper)
	{
		return upper.ToLower();
	}
}";

var tree = CSharpSyntaxTree.ParseText(code); //#1

//Finding all the literals in the code.

var literals = tree.GetRoot()
					.DescendantNodes()
					.OfType<LiteralExpressionSyntax>()//#2
					.Select(les => les.ToFullString())
					.Distinct();
					
tree.GetRoot()
	.DescendantNodes()
	.OfType<InvocationExpressionSyntax>()
	.Select(ies => new //#3
	{
		MethodName = ies.Ancestors().OfType<MethodDeclarationSyntax>()?.First()?.Identifier.ValueText,
		Expression = ies.Expression.ToFullString(),
		CallTokens = ies.Expression.ChildNodes().Select(e => e.ToFullString())
	})
	//#4
	.Where(ies => ies.CallTokens.Any(ct => literals.Contains(ct)))
	//#5
	.Select(ies =>
	new
	{
		MethodName = ies.MethodName,
		Expression = ies.Expression
	})
	.Dump("Methods using ToUpper or ToLower on string literals");