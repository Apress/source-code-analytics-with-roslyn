<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"private static T FromString < T > (string s) where T: struct {
				 if (typeof(T).Equals(typeof(decimal))) {
				  var x = (decimal) System.Convert.ToInt32(s) / 100;
				  return (T) Convert.ChangeType(x, typeof(T));
				 }
				 if (typeof(T).Equals(typeof(int))) {
				  var x = System.Convert.ToInt32(s);
				  return (T) Convert.ChangeType(x, typeof(T));
				 }
				 if (typeof(T).Equals(typeof(DateTime)))...etc...
				}
				public string name() {
				 return string.Empty;
				}
				private static T FromString2 < T, T, T > (string s) where T: struct {
				 if (typeof(T).Equals(typeof(decimal))) {
				  var x = (decimal) System.Convert.ToInt32(s) / 100;
				  return (T) Convert.ChangeType(x, typeof(T));
				 }
				 if (typeof(T).Equals(typeof(int))) {
				  var x = System.Convert.ToInt32(s);
				  return (T) Convert.ChangeType(x, typeof(T));
				 }
				 if (typeof(T).Equals(typeof(DateTime)))...etc...
				}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<MethodDeclarationSyntax>()
	.Select(mds => new
	{
		Name = mds.Identifier.ValueText,
		Arity = mds.Arity
	})
	.Where(mds => mds.Arity > 2)
	.Dump("Generic Methods with lots of generic attribute");