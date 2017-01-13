<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"void check(int x) {
		 if (x < 10)
		  doSomeThing();
		}
		void fun2(int x, int y) {
		 if (x < y)
		  if (x + y < 20)
		   doThat();
		}
		void fun(int x) {
		 int x = 20;
		 if (x < 10) {
		  if (x - 1 < 10) {
		   if (x - 2 < 10) {
		    if (x - 4 < 10)
		     doThat();
		    else
		     doOther();
		   }
		  }
		 }
		}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

var loopTypes = new List<SyntaxKind>()
{
	SyntaxKind.IfStatement
};//#2

tree.GetRoot()
	.DescendantNodes()
	.Where(t => loopTypes.Any(l => t.Kind() == l))//#3
	.Select(t => new //#4
	{
		Method = t.Ancestors()
				  .OfType<MethodDeclarationSyntax>()
				  .First()
				  .Identifier.ValueText,
		Nesting = 1 + t.Ancestors()
					   .Count(z => loopTypes.Any(l => z.Kind() == l))
	})
.ToLookup(t => t.Method)
//#5
.ToDictionary(t => t.Key,t => t.Select(m => m.Nesting).Max())
.Select(t => new { Method = t.Key, Nesting = t.Value })
//Find only if blocks that are deeper than 3 levels.
.Where(t => t.Nesting >= 3)//#6
.Dump("Deeply nested if-statements");