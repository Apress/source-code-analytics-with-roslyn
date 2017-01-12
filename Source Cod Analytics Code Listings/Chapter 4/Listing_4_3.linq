<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code = @"void fun()
	{
		int x;
		int y;
		int z;
		int w;
		int ws;
	}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

//The recommended value is 64;

//But for deomonstration purpose it is changed to 4

const int MAX_LOCALS_ALLOWED = 4; //#2

tree.GetRoot()
	.DescendantNodes()
	.OfType<MethodDeclarationSyntax>() //#3
	.Where(mds =>mds.Body.Statements.OfType<LocalDeclarationStatementSyntax>().Count() >= MAX_LOCALS_ALLOWED) //#4
	.Select(mds => mds.Identifier.ValueText)//#5
	.Dump("Methods with many local variable declarations");