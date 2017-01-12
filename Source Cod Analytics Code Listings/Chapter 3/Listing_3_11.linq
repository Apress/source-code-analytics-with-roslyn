<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code =

@"class A < T > {
 public static int fun() {
  return 10;
 }
 public int funny < T > () {
  return 0;
 }
}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.Where(cds => cds.Arity > 0)//#1
	.Select(cds => //#2
		new
		{
			//Name of the generic class
			GenericClassName = cds.Identifier.ValueText,
			//Static methods in the generic class
			StaticMethods = cds.Members
								.OfType<MethodDeclarationSyntax>()
								.Where(mds => mds.Modifiers
								.Any(m => m.ValueText == "static"))
								.Select(mds => mds.Identifier.ValueText)
		})

	.Where(cds => cds.StaticMethods.Count() > 0)//#3
	.Dump("Static methods on generic types");