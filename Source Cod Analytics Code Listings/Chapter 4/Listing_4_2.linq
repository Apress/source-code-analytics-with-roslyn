<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code = @"static void Main(string[] args)
			{
				dynamic a = 13;
				dynamic b = 14;
				dynamic c = a + b;
				Console.WriteLine(c);
			}";

var tree = CSharpSyntaxTree.ParseText(code);//#1


tree.GetRoot()
	.DescendantNodes()
	.OfType<VariableDeclarationSyntax>()//#2
	//#3
	.Where(vds => vds.Type.ToFullString().Trim() == "dynamic")
	.Select(vds => vds.ToFullString())
	.Dump("All usages of dynamic. Some may not be required");