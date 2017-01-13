<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"using System;
				namespace PerformanceLibrary {
				 public class StringTester {
				  string s1 = ""
				  test "";
				  public void EqualsTest() {
				   if (s1 == """") {
				    Console.WriteLine(@ ""
				     s1 equals empty string.
				     "");
				   }
				  }
				  public void LengthTest() {
				   if (s1 != null && s1.Length == 0) {
				    Console.WriteLine(""
				     s1.Length == 0. "");
				   }
				  }
				  public void NullOrEmptyTest() {
				   if (!String.IsNullOrEmpty(s1)) {
				    Console.WriteLine(""
				     s1 != null and s1.Length != 0. "");
				   }
				  }
				 }
				}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

//Finding all instances of “string” in source code.

var strings = tree.GetRoot()
				  .DescendantNodes()
				.OfType<VariableDeclarationSyntax>()//#2
				.Where(vds => vds.Type.ToFullString().Trim() == "string")//#3
				.SelectMany(vds => vds.Variables.Select(v => v.Identifier.ValueText));//#4
			
var results = tree.GetRoot()
				.DescendantNodes()
				.OfType<IfStatementSyntax>()
				.Where(iss => strings.Any(s => iss.Condition.ToFullString().Contains(s + " == \"\""))) //#5
				.Select(iss => new //#6
				{
					MethodName = iss.Ancestors()
									.OfType<MethodDeclarationSyntax>()
									.First()
									.Identifier.ValueText,
					Condition = iss.ToFullString()
				});

if (results.Any())
	results.Dump();