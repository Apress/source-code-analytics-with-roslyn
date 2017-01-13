<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"class A {
 					void fun() {}
 					void fun1(int a) {}
 					void fun2(int a, int a) {}
 					public int Age {
  							get;
  							set;
 						}
					 public string Name {
					  get;
					  set;
					 }
					}
					class B {
					 public int Age {
					  get;
					  set;
					 }
					 public string Name {
					  get;
					  set;
					 }
					}
					class C {
					 public double RateOfInterest {
					  get;
					  set;
					 }
					}";

var tree = CSharpSyntaxTree.ParseText(code);

var classes = tree

.GetRoot()

.DescendantNodes()

.OfType<ClassDeclarationSyntax>()//#1

.Select(cds => new //#2
{
	//Name of the class
	Name = cds.Identifier.ValueText,
	//Number of members of the class
	MemberCount = cds.Members.Count,
	//Number of public properties
	PublicPropertyCount = cds.Members
							 .OfType<PropertyDeclarationSyntax>()
							 .Count(pds => pds.Modifiers
							 .Any(m => m.ValueText == "public"))
})

.Where(cds => cds.MemberCount == cds.PublicPropertyCount)//#3
.Dump("Data Classes");