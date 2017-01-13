<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

//it scans only the given source. It doesn't search beyond the //given local file resource.

string code = @"using System;
using System.Collections.Generic;
namespace DoFactory.GangOfFour.Strategy.RealWorld {
 class MainApp {
  static void Main() {
   SortedList studentRecords = new SortedList();
   studentRecords.Add(""Samual"");
   studentRecords.Add(""Jimmy"");
studentRecords.Add(""Sandra"");
studentRecords.Add(""Vivek"");
studentRecords.Add(""Anna"");
studentRecords.SetSortStrategy(new QuickSort());
studentRecords.Sort();
studentRecords.SetSortStrategy(new ShellSort());
studentRecords.Sort();
studentRecords.SetSortStrategy(new MergeSort());
studentRecords.Sort();
Console.ReadKey();
  }
 }
 abstract class SortStrategy
{
	public abstract void Sort(List<string> list);
}
class QuickSort : SortStrategy
{
	public override void Sort(List<string> list)
	{
		list.Sort();
		Console.WriteLine(""QuickSorted list "");
	}
}
class ShellSort : SortStrategy
{
	public override void Sort(List<string> list)
	{
		Console.WriteLine(""ShellSorted list "");
	}
}
class MergeSort : SortStrategy
{
	public override void Sort(List<string> list)
	{
		Console.WriteLine(""MergeSorted list "");
	}
}
class SortedList
{
	private List<string> _list = new List<string>();
	private SortStrategy _sortstrategy;
	public void SetSortStrategy(SortStrategy sortstrategy)
	{
		this._sortstrategy = sortstrategy;
	}
	public void Add(string name)
	{
		_list.Add(name);
	}
	public void Sort()
	{
		_sortstrategy.Sort(_list);
		foreach (string name in _list)
		{
			Console.WriteLine("" "" + name);
		}
		Console.WriteLine();
	}
}
}";

var tree = CSharpSyntaxTree.ParseText(code);
		
var abstractStrategy = tree.GetRoot()
						.DescendantNodes()
						.OfType<ClassDeclarationSyntax>()
						.Where(cds => cds.Modifiers.Select(m => m.Text).Contains("abstract"))//#1
						.Select(cds => //#2
									new
						{
								Name = cds.Identifier.ValueText,
								AbstractMethod = cds.Members
													.OfType<MethodDeclarationSyntax>()
													.Where(mds => mds.Modifiers
													.Select(m => m.Text).Contains("abstract"))
													.Select(mds => mds.Identifier.ValueText)
						})

.Dump("Strategies");

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.Select(cds => //#3
	new
	{
		ClassName = cds.Identifier.ValueText,
		InheritedFrom = cds.BaseList?.Types.Select(t => t.ToFullString().Trim()),	
		OverriddenMethods = cds.Members.OfType<MethodDeclarationSyntax>().Where(mds => mds.Modifiers.Select(m => m.Text).Contains("override"))
								.Select(mds => mds.Identifier.ValueText)
	})
.Where //#4
(
//Finds those classes that
cds => cds.InheritedFrom?.Count() > 0).Where(cds => abstractStrategy
	.Any(s => cds.InheritedFrom.Contains(s.Name.Trim()))
)
	.Dump("Concrete Strategies");