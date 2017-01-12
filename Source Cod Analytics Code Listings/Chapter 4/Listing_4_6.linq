<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"class A 
				{
				 bool someSearchWithObjParams(params object[] searchTerms) {}
				 bool someSearch(params objectsome[] searchCriteria) {}
				 bool search(int a, int b, params string[] arra) {}
				 bool search(string code, int length) {}
				}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

tree.GetRoot()
	.DescendantNodes()
	.OfType<MethodDeclarationSyntax>()//#2
	.Where(mds => mds.ParameterList.Parameters
	.Any(p => p.Modifiers
	.Any(m => m.Text == "params" && //#3
		p.Type.ToFullString().Replace(" ", string.Empty).Contains("object[]"))))
	
	.Select(mds => new //#4
	{
		//Name of the class
		ClassName = mds.Ancestors()
						.OfType<ClassDeclarationSyntax>()
						.First()
						.Identifier
						.ValueText,
		//The name of defaulter method
		MethodName = mds.Identifier.ValueText
	})
	.Dump("Methods with param objects");