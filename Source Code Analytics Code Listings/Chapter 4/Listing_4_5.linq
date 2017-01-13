<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

var code = @"class VolatileTest {
				 public volatile int i;
				 public void Test(int _i) {
				  i = _i;
				 }
				}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

tree.GetRoot()
	.DescendantNodes()
	.OfType<FieldDeclarationSyntax>()//#2
	.Where(vds => vds.Modifiers
	.Any(m => m.ValueText == "volatile"))//#3
	.Select(vds => new //#4
	{
		ClassName = vds.Ancestors().OfType<ClassDeclarationSyntax>().First()?.Identifier.ValueText,
		VolatileDeclaration = vds.ToFullString()
	})
	.Dump();