<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

//Avoid hungarian notation. It's a code smell now
//refer http://web.mst.edu/~cpp/common/hungarian.html
//applicable to 
//class level variables
//temporary variables in methods
//method parameters 

string code =
@"class A
{
   float fIntRate = 4.456;
float intRate = 4.53;
long  liX = 342;
   bool bCondi = false;
   string name = ""Sam"";
string strTitle = ""Mr"";
}";

//#1 
Func<string, string, bool> IsHungarian = (varName, typeName) =>
  {
	  bool result = false;
	  string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	  if (typeName == "bool"
	&& varName.StartsWith("b")
	&& upperCase.Contains(varName[1]))
		  result = true;
	  if (typeName == "char"
	&& varName.StartsWith("c")
	&& upperCase.Contains(varName[1]))
		  result = true;
	  if (typeName == "string"
	&& varName.StartsWith("str")
	&& upperCase.Contains(varName[1]))
		  result = true;
	  if (typeName == "int"
	&& varName.StartsWith("i")
	&& upperCase.Contains(varName[1]))
		  result = true;
	  if (typeName == "float"
	&& varName.StartsWith("f")
	&& upperCase.Contains(varName[1]))
		  result = true;
	  if (typeName == "short"
	&& varName.StartsWith("s")
	&& upperCase.Contains(varName[1]))
		  result = true;
	  if (typeName == "long"
	&& varName.StartsWith("l")
	&& upperCase.Contains(varName[1]))
		  result = true;

	  return result;
  };

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
.Where(t => t.Kind() == SyntaxKind.FieldDeclaration)
	.Cast<FieldDeclarationSyntax>()
.Select(fds =>
 new
 {
	 //#2
	 TypeName = fds.Declaration.Type.ToFullString().Trim(),
	 //#3
	 VarName = fds.Declaration.Variables
				.Select(v => v.Identifier.Value).First()
 })
//#4
.Where(fds => IsHungarian(fds.VarName.ToString(),
						  fds.TypeName.ToString()))
.Dump("Hungaranian Notations");

