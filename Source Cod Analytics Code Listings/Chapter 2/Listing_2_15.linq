<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code =

@"class A

{

int fun(int x)

{

//update x

x++;

return x - 3;

}

int add(int x,int y)

{

//add these two

//it might lead to exception

return x + y;

}

}

class B

{

int fun3(int x)

{

//update x

x++;

return x - 3;

}

int add2(int x,int y)

{

//add these two

//it might lead to exception

return x + y;

}

}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()

.DescendantNodes()

.Where(t => t.Kind() == SyntaxKind.ClassDeclaration)

.Cast<ClassDeclarationSyntax>()

.Select(t =>

new

{

   ClassName = t.Identifier.ValueText,

   Methods =

t.Members.OfType<MethodDeclarationSyntax>()

})//#1

.Select(t =>

new
{
   ClassName = t.ClassName,

   MethodDetails = t.Methods

.Select(m => new
{
   Name = m.Identifier.ValueText,

   Lines = m.Body.Statements.Count, //#2

	Comments = m.Body.DescendantTrivia()

.Count(b => b.Kind() ==

SyntaxKind.SingleLineCommentTrivia

|| b.Kind() == SyntaxKind.MultiLineCommentTrivia) //#3

})
})

.Dump("Code and Comment per method per class");