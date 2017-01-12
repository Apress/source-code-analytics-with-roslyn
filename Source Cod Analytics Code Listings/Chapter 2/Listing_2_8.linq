<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
</Query>

string code = @"int fun(int x,int z)
				{ 
					int y = 0; 
					x++; 
					return x+1;
				} 
                double funny(double x)
				{ 
					return x/2.13;
				}";

SyntaxTree tree = CSharpSyntaxTree.ParseText(code);

List<MethodDeclarationSyntax> methods = tree.GetRoot()
								            .DescendantNodes()
								            .OfType<MethodDeclarationSyntax>()
											.ToList();

methods.Select(z =>
				  {
					  var parameters = z.ParameterList.Parameters.Select(p => p.Identifier.ValueText);
					  return
							new
							{
							    MethodName =  z.Identifier.ValueText,//#1
								//#2 
							    IsUsingAllParameter =  parameters.All
				                                     (x => z.Body.Statements.SelectMany(s => s.DescendantTokens())
													            .Select(s => s.ValueText).Distinct().Contains(x))
							};

				  })
.Where(x => !x.IsUsingAllParameter)
.ToList()
.ForEach(x => Console.WriteLine(x.MethodName + " " + x.IsUsingAllParameter));
