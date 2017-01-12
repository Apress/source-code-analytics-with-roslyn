<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code =

@"using System;
namespace PerformanceLibrary
{
	public class Test
	{
		string [] nameValues;
		public Test()
		{
			nameValues = new string[100];
			for (int i = 0; i< 100; i++)
			{
				nameValues[i] = ""Sample"";
			}
		}
		public string[] Names
		{
			get
			{
				return (string[])nameValues.Clone();
			}
		}

		public static void Main()
		{
		// Using the property in the following manner
		// results in 201 copies of the array.
		// One copy is made each time the loop executes,
		// and one copy is made each time the condition is
		// tested.
		Test t = new Test();
		for (int i = 0; i < t.Names.Length; i++)
		{
			if (t.Names[i] == (""SomeName""))
			{
				// Perform some operation.
			}
		}
	}
}
}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()//#2
	.Select(cds => new //#3
	{
		ClassName = cds.Identifier.ValueText,
		Properties = cds.Members.OfType<PropertyDeclarationSyntax>()
			.Select(pds => new //#4
			{
					PropertyName = pds.Identifier.ValueText,
					PropertyType = pds.Type.ToFullString().Trim()
			})
		})
	.Where(cds => cds.Properties //#5
	.Any(p => p.PropertyType.Contains("[")))
	.Dump("Properties returning an array");