<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"using System;
				namespace DesignLibrary {
				 public interface IGoodInterface {
				  void funny();
				 }
				 public interface IBadInterface {}
				}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<InterfaceDeclarationSyntax>()//#1
	.Select(ids => //#2
	 new
	{
		InterfaceName = ids.Identifier.ValueText,
		IsEmpty = ids.Members.Count == 0
	})
.Where(thisInterface => thisInterface.IsEmpty)//#3
.Dump("Empty Interfaces");