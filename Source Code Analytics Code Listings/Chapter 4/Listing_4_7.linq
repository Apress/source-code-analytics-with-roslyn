<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code = @"class A {
 void fun(IEnumerable < int > nums) {
  var vals = nums.ToList();
  foreach(var v in vals) {...
  }
 }
}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

var decls = tree
			.GetRoot()
			.DescendantNodes()
			.OfType<LocalDeclarationStatementSyntax>();//#2

var projectors = new string[] {".ToList();",".ToArray();"};

if (decls.Count() > 0) //#3
{
	var defaulters = decls.Select(ldss => new //#4
	{
		ClassName = ldss.Ancestors()
						.OfType<ClassDeclarationSyntax>()
						.FirstOrDefault()
						?.Identifier
						.ValueText,
		Method = ldss.Ancestors()
					.OfType<MethodDeclarationSyntax>()
					.FirstOrDefault()?.Identifier
					.ValueText,
		Statement = ldss.ToFullString().Trim()
	})
	//#5
	.Where(ldss => projectors.Any(projector => ldss.Statement.Trim().EndsWith(projector)));

if (defaulters.Count() > 0)
	defaulters.Dump("Un-necessary projections");

}