<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

//Following script finds magic lines for +=, -=, *=, /= and + , //- , * amd / cases

string code =

@"float fun(int g)

{

int size = 10000;

g+=23456;//bad code. magic number 23456 is used.

g+=size;

return g/10;

}

decimal updateRate(decimal rate)

{

return rate / 0.2345M;

}

decimal updateRateM(decimal rateM)

{

decimal basis = 0.2345M;

return rateM/basis;

}";

List<SyntaxKind> kinds = new List<SyntaxKind>()

{

	SyntaxKind.AddAssignmentExpression,//+=

	SyntaxKind.SubtractAssignmentExpression,//-=

	SyntaxKind.MultiplyAssignmentExpression,//*=

	SyntaxKind.DivideAssignmentExpression, // /=

	SyntaxKind.AddExpression, // +

	SyntaxKind.SubtractExpression,// -

	SyntaxKind.MultiplyExpression,// *

	SyntaxKind.DivideExpression // /

};

CSharpSyntaxTree.ParseText(code)

.GetRoot()

.DescendantNodes()

.Where(st => st.Kind() == SyntaxKind.MethodDeclaration)

.Cast<MethodDeclarationSyntax>()

.Select(st =>

new

{

	MethodName = st.Identifier.ValueText,//#1

	MagicLines =

CSharpSyntaxTree.ParseText(st.ToFullString())

.GetRoot()

.DescendantNodes()

.Where(z => kinds

.Any(k => k == z.Kind()))

.Select(q => q.ToFullString().Trim())

.Where(w => CSharpSyntaxTree

.ParseText("void dummy(){" + w.ToString() + "}")

.GetRoot()

.DescendantTokens()

.Any(s => //#2

s.Kind() == SyntaxKind.NumericLiteralToken))
}).Dump("Magic lines. Please avoid these");