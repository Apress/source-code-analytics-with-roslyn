<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

//Magic numbers in numeric condition is bad
//because they are hard to understand and thus replace 
string code =
@"class PasswordManager
{
  int x = 8;
bool IsGood(string password)
     {
    if(password.Length < 5)
return false;
return password.Length >= 7;
}
int fun()
{
return g[x]; 
}
bool zun()
{
if(z > 3.4)
return false;
else 
return true;
}
}";

var tree = CSharpSyntaxTree.ParseText(code);

var operators = new List<SyntaxKind>()
{
SyntaxKind.GreaterThanToken,
SyntaxKind.GreaterThanEqualsToken,
SyntaxKind.LessThanEqualsToken,
SyntaxKind.LessThanToken,
SyntaxKind.EqualsEqualsToken,
SyntaxKind.LessThanLessThanEqualsToken,
SyntaxKind.GreaterThanGreaterThanEqualsToken

};

tree.GetRoot()
	.DescendantNodes()
.Where(t => t.Kind() == SyntaxKind.ClassDeclaration)
.Cast<ClassDeclarationSyntax>()
.Select(t =>
 new
 {
	 ClassName = t.Identifier.ValueText,//#1
	 MethodTokens  = t.Members
					.Where(m => m.Kind() == SyntaxKind.MethodDeclaration)
					.Cast<MethodDeclarationSyntax>()
					.Select(mds =>
							new
							{
			   						MethodName = mds.Identifier.ValueText,
			   							Tokens = CSharpSyntaxTree.ParseText(mds.ToFullString())
											.GetRoot()
											 .DescendantTokens()
											 .Select(c => c.Kind())
				})
.Select(w =>
new
{
   MethodName = w.MethodName, //#2
	Toks = w.Tokens.Zip(w.Tokens.Skip(1), (a, b) =>
	 operators.Any(op => op == a) && b
	 == SyntaxKind.NumericLiteralToken)//#3
})
.Where(w => w.Toks.Any(to => to == true))//#4
.Select(w => w.MethodName)
 }).Dump();
