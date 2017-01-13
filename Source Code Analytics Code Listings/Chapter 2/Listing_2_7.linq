<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code = @"int fun(int x)
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
											.Where(d => d.Kind() == SyntaxKind.MethodDeclaration)
											.Cast<MethodDeclarationSyntax>()
											.ToList();//#1

methods
		.Select(z =>
		new
		{
			MethodName = z.Identifier.ValueText,//#2
			NBLocal = z.Body.Statements
		   //#3 
		   .Count(x => x.Kind() == SyntaxKind.LocalDeclarationStatement)
		})
		.OrderByDescending(x => x.NBLocal)
		.ToList()
		.ForEach(x =>
		Console.WriteLine(x.MethodName + " " + x.NBLocal));
