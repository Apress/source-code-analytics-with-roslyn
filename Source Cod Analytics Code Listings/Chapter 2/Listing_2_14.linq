<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code =

@"class BaseClass {
 bool cflag = false;
 void update() {
  if (!flag)
   if (thatThing())
    flag = true;
 }
 void thatOtherThing() {
  bool flag = false;
  if (flag) {} else {
   flag = false;
  }
 }";

//Find all boolean variables in the class

//Find all if statements that solely rely on those

//Find the methods in which these if statements are

var tree = CSharpSyntaxTree.ParseText(code);
var bools = tree.GetRoot()
                .DescendantNodes()
                .Where(t => t.Kind() == SyntaxKind.VariableDeclaration
                          && t.ToFullString().Trim().StartsWith("bool"))
                .Select(t =>
                               new
                               {
                                      VariableName = t.ToFullString().Trim()
                                                      .Split(' ')[1],
                                      Class = t.Ancestors()
                                               .Where(x => x.Kind() == SyntaxKind.ClassDeclaration)
                                               .Cast<ClassDeclarationSyntax>()
                                               .First().Identifier.ValueText
                               })

                .Select(t => t.VariableName);//#1

tree.GetRoot()

.DescendantNodes()
.Where(t => t.Kind() == SyntaxKind.IfStatement)

.Cast<IfStatementSyntax>()

.Select(ifs =>

new

{

   If = ifs.ToFullString(), //#2

	Condition = ifs.Condition.ToFullString(),//#3

	Line = ifs.SyntaxTree

.GetLineSpan(ifs.FullSpan)

.StartLinePosition.Line + 1//#4

})

.Where(ifs => ifs.Condition.Split(new string[] { "&&", "||", "==", "(", ")", " " }, StringSplitOptions.RemoveEmptyEntries)

//If a control flag is used,

.Any(c => bools.Contains(c) ||

//it can also be used in a negative way. skipping "!"

bools.Contains(c.Substring(1))))//#5

.Dump("if nodes with control flags");