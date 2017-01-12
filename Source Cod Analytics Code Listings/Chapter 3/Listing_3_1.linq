<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"void fun2(int x) {
			 for (int i = 0; i < 10; i++) {
			  for (int j = 0; j < 10; j++)
			   list.Add(i + j);
			 }
			}
			void fun(int x) {
			 for (int i = 0; i < 10; i++) {
			  for (int j = 0; j < 10; j++) {
			   for (int k = 2; k < 20; k++)
			    list.Add(i + j + k);
			  }
			 }
			}
			void straightLoop() {
			 for (int j = 0; j < 10; j++)
			  doThat(j);
			}
			void loopingTheLoopWhile() {
			 while (true)
			  for (int x = 0; x < 10; x++)
			   foreach(var z in z[x])
			 doSome(z);
			}
			void loopingTheLoop() {
			 foreach(var m in newItems)
			 foreach(var z in oldItems)
			 for (int i = 0; i < z.Items.Count; i++)
			  doThat(i, z, m);
			}
			void fun4(int x) {
			 for (int m = 0; m < 10; m += 2)
			  for (int i = 0; i < 10; i++) {
			   for (int j = 0; j < 10; j++) {
			    for (int k = 2; k < 20; k++)
			     list.Add(i + j + k);
			   }
			  }
			}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

var loopTypes = new List<SyntaxKind>()
{
	SyntaxKind.ForStatement,
	SyntaxKind.ForEachStatement,
	SyntaxKind.WhileStatement
};//#2

tree.GetRoot()
	.DescendantNodes()
	.Where(t => loopTypes.Any(l => t.Kind() == l))//#3
	.Select(t => new
	{
	    //#4
		Method = t.Ancestors().OfType<MethodDeclarationSyntax>()
				.First()
        	    .Identifier.ValueText,
		Nesting = 1 + t.Ancestors()
                   .Count(z => loopTypes.Any(l => z.Kind() == l))
  	})//#5

.ToLookup(t => t.Method)
.ToDictionary(t => t.Key,t => t.Select(m => m.Nesting).Max())//#6
.Select(t => new { Method = t.Key, Nesting = t.Value })
.Where(t => t.Nesting >= 3)//#7
.Dump("Deeply Nested Loops");