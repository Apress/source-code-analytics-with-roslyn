<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code =
@"class A
{
public void f(int a,int b,int c,
int d,bool x,bool z,float t)
{
}
public void f3(int a,int b,int c)
{
}
}
class B
{
public void f3b(int a,int b,int c,
float d,bool x,bool z,float t)
{
}
public void fb(int a,int b,int c)
{
}
}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
.OfType<ClassDeclarationSyntax>()
.Select(cds =>
new
{
   ClassName = cds.Identifier.ValueText,//#1
   Methods = cds.Members.OfType<MethodDeclarationSyntax>()
				.Select(mds => new
				{
					MethodName = mds.Identifier.ValueText, //#2	
					Parameters = mds.ParameterList.Parameters.Count//#3
				 })
}).Dump();
