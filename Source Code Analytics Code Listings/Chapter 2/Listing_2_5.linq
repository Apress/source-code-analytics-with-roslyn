<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

//Find if statements in each functions where
//they can be clubbed.
string code = @"public class A{int maybe_do_something(...) {
    if(something != -1)
        return 0;
    if(somethingelse != -1)
        return 0;
    if(etc != -1)
        return 0;
    do_something();
}
int otherFun()
{
    if(bailIfIEqualZero == 0)
  return;
if(string.IsNullOrEmpty(shouldNeverBeEmpty))
  return;

if(betterNotBeNull == null || betterNotBeNull.RunAwayIfTrue)
  return;
    return 1;
}}";

//pull up 

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
.Where(t => t.Kind() == SyntaxKind.MethodDeclaration)
.Cast<MethodDeclarationSyntax>()//#1
.Select(t => new
{
	Name = t.Identifier.ValueText,
	IfStatements = t.Body.Statements
.Where(m => m.Kind() == SyntaxKind.IfStatement)
.Cast<IfStatementSyntax>()
.Select(iss =>
//#2
new
{
   Statement = iss.Statement.ToFullString(),
	//#3
	IfStatement = iss.Condition.ToFullString()
})
//#4
.ToLookup(iss => iss.Statement)
})
.Dump("Fragmented conditions");
