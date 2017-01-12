<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"using System;
			namespace DesignLibrary {
			 public abstract class BadAbstractClassWithConstructor {
			  public BadAbstractClassWithConstructor() {}
			  public abstract void fun() {}
			 }
			 public abstract class GoodAbstractClassWithConstructor {
			  protected GoodAbstractClassWithConstructor() {}
			 }
			}";

var tree = CSharpSyntaxTree.ParseText(code);

var abstractTypes =
		tree.GetRoot()
			.DescendantNodes()
			.OfType<ClassDeclarationSyntax>()
			.Where(cds => cds.Modifiers.Any(m => m.ValueText == "abstract"))//#1
			.Select(cds => new //#2
			{
				ClassName = cds.Identifier.ValueText,
				PublicConstructors = cds.Members.OfType<ConstructorDeclarationSyntax>()
												.Any(c => c.Modifiers
												.Any(m => m.ValueText == "public"))
			})
.Where(cds => cds.PublicConstructors)//#3
.Dump("AbstractTypesShouldNotHaveConstructors Violators");