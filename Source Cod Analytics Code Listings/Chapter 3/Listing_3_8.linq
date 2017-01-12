<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code = @"class DocumentHome {
			 (...)
			 public Document createDocument(String name) {
			  return createDocument(name, -1);
			 }
			 public Document createDocument(String name, i nt minPagesCount) {
			  return createDocument(name, minPagesCount, false);
			 }
			 public Document createDocument(String name, int minPagesCount, boolean firstPageBlank) {
			  return createDocument(name, minPagesCount, false, "");
			 }
			 public Document createDocument(String name, int minPagesCount, boolean firstPageBlank, String title) {
			   (...)
			  }
			  (...)
			}";

var tree = CSharpSyntaxTree.ParseText(code);
				
tree.GetRoot()
	.DescendantNodes()
	.Where(t => t.Kind() == SyntaxKind.ClassDeclaration)
	.Cast<ClassDeclarationSyntax>()
	.Select(
	
		cds =>
		new 
		{
			ClassName = cds.Identifier.ValueText,//#1
			Methods = cds.Members.OfType<MethodDeclarationSyntax>()//#2
								 .Select(mds => mds.Identifier.ValueText)
		})
	.Select(cds => 
		new
		{
			ClassName = cds.ClassName,
			Overloads = cds.Methods.ToLookup(m => m).ToDictionary(m => m.Key, m => m.Count())
		})//#3

.Dump("Overloaded Methods");