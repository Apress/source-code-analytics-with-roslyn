<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code = @"using System;
			namespace DesignLibrary {
			 public sealed class SealedClass {
			  protected void ProtectedMethod() {}
			 }
			}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.Where(cds => cds.Modifiers.Any(m => m.ValueText == "sealed")) //#1
	.Select
	(
		cds => //#2
			new
			{
				ClassName = cds.Identifier.ValueText,
				ProtectedMembers = cds.Members
 									 .OfType<MethodDeclarationSyntax>()
 									 .Where(mds => mds.Modifiers.Any(m => m.ValueText == "protected"))
									 .Select(mds => mds.Identifier.ValueText)

			}

)
.Where(cds => cds.ProtectedMembers.Count() > 0)//#3
.Dump("CA1047 Defaulters");