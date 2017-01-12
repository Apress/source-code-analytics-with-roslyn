<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code =
@"class A {
 void Test() {
  for (int i = 1; i < 101; i++) {
   if (i % 3 == 0 && i % 5 == 0) {
    Console.WriteLine(""
     FizzBuzz "");
   } else if (i % 3 == 0) {
    Console.WriteLine(""
     Fizz "");
   } else if (i % 5 == 0) {
    Console.WriteLine(""
     Buzz "");
   } else {
    Console.WriteLine(i);
   }
  }
 }
}
class Z {
 int funny = 1;
 void fun2() {
  updateThat();
 }
 void funny2() {
  Console.WriteLine(funny);
 }
}";

var tree = CSharpSyntaxTree.ParseText(code);
						
tree.GetRoot()
   .DescendantNodes()
   .Where(t => t.Kind() == SyntaxKind.ClassDeclaration)
   .Cast <ClassDeclarationSyntax>()
   .Select(cds =>
   new {
      ClassName = cds.Identifier.ValueText, //#1
      Methods = cds.Members
                   .OfType <MethodDeclarationSyntax> ()
                   .Select(mds =>
						   new 
						   {
						
						     MethodName = mds.Identifier.ValueText, //#2
						     LOC = mds.Body.SyntaxTree
						 		           .GetLineSpan(mds.FullSpan).EndLinePosition.Line //#3
						          - mds.Body.SyntaxTree.GetLineSpan(mds.FullSpan).StartLinePosition.Line - 3 //#4
						
						   })

 }
)
.Dump();