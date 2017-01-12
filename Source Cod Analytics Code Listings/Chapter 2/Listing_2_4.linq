<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code =
@"void fun()
{
//Call a function only once
if(c1() == 1)
f1();
if(c1() == 2)
f2();
if(c1() == 3)
f3();
if(c1() == 4)
f4();
if(co() == 23)
f22();
if(co() == 24)
f21();
}
void funny()
{
read_that();
if(c1() == 3)
c13();
if(c2() == 34)
c45();
}
";


var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
.Where(t => t.Kind() == SyntaxKind.MethodDeclaration)
.Cast<MethodDeclarationSyntax>()
.Select(t =>
new
{
   Name = t.Identifier.ValueText,
   IfStatements = t.Body.Statements
.Where(s => s.Kind() == SyntaxKind.IfStatement)
.Cast<IfStatementSyntax>()
})
.Select(t =>
new
{
   Name = t.Name,
   Ladders = t.IfStatements
.Select(i => i.Condition.ToFullString())
.ToLookup(i => i.Substring(0, i.IndexOf('=')))
.Where(i => i.Count() >= 2)
})
.Dump();
