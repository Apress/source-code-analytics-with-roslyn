<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

//avoid goto

string code = @"class SwitchTest

{

public static void message()

{

}

public void gotoFun()

{

// Search:

for (int i = 0; i < x; i++)

{

for (int j = 0; j < y; j++)

{

if (array[i, j].Equals(myNumber))

{

goto Found;

}

}

}

}

static void Main()

{

Console.WriteLine(@""Coffee sizes: 1=Small 2=Medium

3=Large"");

Console.Write(""Please enter your selection: "");

string s = Console.ReadLine();

int n = int.Parse(s);

int cost = 0;

switch (n)

{

case 1:

cost += 25;

break;

case 2:

cost += 25;

goto case 1;

case 3:

cost += 50;

goto case 1;

default:

Console.WriteLine(""Invalid selection."");

break;

}

Console.ReadKey();

}

}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()

.DescendantNodes()

.Where (t => t.Kind() == SyntaxKind.ClassDeclaration)

.Cast<ClassDeclarationSyntax>()

.Select (cds =>

new

{

ClassName = cds.Identifier.ValueText,

Methods =

cds.Members

.Where (m => m.Kind() ==

SyntaxKind.MethodDeclaration)

.Cast<MethodDeclarationSyntax>()

.Select (mds =>

new

{

MethodName = mds.Identifier.ValueText,

HasGoto =  CSharpSyntaxTree.ParseText(mds.ToString())
                           .GetRoot()
                           .DescendantTokens()
							//Checking whether the method uses "goto" labels or not 
							.Any (st => st.Kind() == SyntaxKind.GotoKeyword)})
							.Where (mds => mds.HasGoto)
							.Select(mds => mds.MethodName)
})

.Dump("Classwise methods which use goto");