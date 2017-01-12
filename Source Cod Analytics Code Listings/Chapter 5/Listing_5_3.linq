<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code1 = @"class A {
				 void doThat() {}
				 void doThis() {}
				 void fun() {
				  if (x < 1)
				   doThat();
				  else
				   doThis();
				 }
				}";

string code2 = @"class B {
			 void doThat() {}
			 void doThis() {}
			 void fun() {
			  int y = 0;
			  doThose(y);
			 }
			 void doThose(int y) {
			  if (y < 1)
			   doThat();
			  else
			   doThis();
			 }
			}";

var tokens1 = CSharpSyntaxTree.ParseText(code1)
					  		.GetRoot().DescendantTokens()
							.Select(d => d.Kind().ToString());

var tokens2 = CSharpSyntaxTree.ParseText(code2).GetRoot()
							.DescendantTokens().Select(d => d.Kind().ToString());

//Calculates the percentage of matching tokens.

Func<IEnumerable<string>, IEnumerable<string>, double> PercentMatch =

(tokenStream1, tokenStream2) =>
{
	int match = 0;
	for (int i = 0; i < tokenStream1.Count(); i++)
	{
		if (tokenStream1.ElementAt(i) == tokenStream2.ElementAt(i))
			match++;
	}
	match *= 100;
	return ((float)match / (float)tokenStream1.Count());
};
PercentMatch.Invoke(tokens1, tokens2)
.Dump("Percentage Match");