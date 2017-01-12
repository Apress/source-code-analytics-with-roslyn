<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"using System;
namespace PerformanceLibrary {
 public class ArrayHolder {
  int[][] jaggedArray = {
   new int[] {   1,    2,    3,    4   },
   new int[] {    5,    6,    7   },
   new int[] {    8   },   
   new int[] {    9   }
  };
  int[, ] multiDimArray = {   {    1,    2,    3,    4   },   {    5,    6,    7,    0   },   {    8,    0,    0,    0   },   
  {    9,    0,    0,    0   }  };
 }
}
";

var tree = CSharpSyntaxTree.ParseText(code);//#1

tree.GetRoot()
	.DescendantNodes()	
	.OfType<ArrayRankSpecifierSyntax>()//#2	
	.Select(ats => new //#3
	{
		BelongsTo = ats.Ancestors().OfType<ClassDeclarationSyntax>()
						.First()?.Identifier.ValueText,
		ArrayType = ats.ToFullString()
	})
	.Where(ats => ats.ArrayType.Contains(","))//#4
	.Dump("Classes with multi-dimentional array");