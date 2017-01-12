<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = 
@"int fun(int b)
{
    int x = 323;
int z = dic[x] + x + dic[323];
return z + b;
}
float funny(float c)
{
int d = 234;
Dictionary<float,string> dic = getDic();
float z = dic[d];
return z;
}

Dictionary<float,string> getDic()
{
return new Dictionary<float,string>();
}
 ";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
.DescendantNodes()
.OfType<BracketedArgumentListSyntax>()
.Select(bals =>
   new 
      { 
         Method = bals.Ancestors()
               .OfType<MethodDeclarationSyntax>()
               .First()
               .Identifier.ValueText, 
			   
		Indices = bals.Arguments
                    .Select(a => a.GetText()
                       .Container
                       .CurrentText
                       .ToString())
  		}
	)

//Find defaulter methods that use magic indices
.Where(bals =>
bals.Indices
        .Any(i => Regex.Match(i,"[0-9]+").Success))
.Dump("Methods using magic indices");
