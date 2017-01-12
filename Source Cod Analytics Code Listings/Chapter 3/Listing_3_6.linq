<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = //Here is a toy sample code

@"class A {
 class localA {}
}
class B {
 void fun() {}
}
class C {
 void funny() {}
}";

var tree = CSharpSyntaxTree.ParseText(code);

tree
	.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()//#1
	.Select(cds =>
			new
			{
			   ClassName = cds.Identifier.ValueText,
			   LocalClasses = cds.Members
						         .OfType<ClassDeclarationSyntax>()
								 .Select(m => m.Identifier.ValueText)
			}			
)

.Where(cds => cds.LocalClasses.Count() >= 1)//#3
.Dump("Local Classes");