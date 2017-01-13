<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code1 = 
@"public class PayRoll {
 public double calcAvgSal(List < double > salaries) {
  double sum = 0;
  for (int i = 0; i < salaries.Count; i++)
   sum += salaries[i];
  return sum / salaries.Count;
 }
}";

string code2 = @"public class BonusManager {
 public double calcAvgBonus(List < double > bonuses) {
  double sum = 0;
  for (int i = 0; i < bonuses.Count; i++)
   sum += bonuses[i];
  return sum / bonuses.Count;
 }
}";

var tree1 = CSharpSyntaxTree.ParseText(code1);

var tree2 = CSharpSyntaxTree.ParseText(code2);

var tokens1 = tree1
				.GetRoot()
				.DescendantTokens()
				.Select(t => t.Kind().ToString());
				
var tokens2 = tree2
				.GetRoot()
				.DescendantTokens()
				.Select(t => t.Kind().ToString());
				
tokens1.SequenceEqual(tokens2).Dump("100% Match");