<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

var code =

@"class A

{

	public int g {get;set;}
	
	public void f1(){ }
	
	public void f2(){ }
	
	public void f3(){ }
	
	public void f4(){ }
	
	public void f5(){ }
	
	public void f6(){ }

}

class B

{

	public void f22(){ }
	
	public void f32(){ }

}";

var tree = CSharpSyntaxTree.ParseText(code);

var classAndMembers = tree.GetRoot()
							.DescendantNodes()
							.Where(t => t.Kind() == SyntaxKind.ClassDeclaration)
							.Cast<ClassDeclarationSyntax>()//#1
							.Select(cds =>
							new
							{
							   ClassName = cds.Identifier.ValueText,//#2
							   Size = cds.Members.Count//#3
							});

var averageLength =  classAndMembers.Select(classDetails => classDetails.Size)
                                    .Average();//#4

classAndMembers.Where(am => am.Size > averageLength)//#5
               .Dump("Large Class");
			   
			  