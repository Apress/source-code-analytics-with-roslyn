<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

string code =@"	public void fun(int a) 
                {
				 switch (a) {
				  case 1:
				   print(1);
				   break;
				  case 2:
				   print(5);
				   break;
				  case 3:
				   print(4);
				   break;
				  case 4:
				   print(2);
				   break;
				  case 5:
				   print(8);
				   break;
				  case 6:
				   print(7);
				   break;
				  case 7:
				   print(7);
				   break;
				  default:
				   print(nothing);
				   break;
				 }
				}
				public void fun2(int a) {
				 switch (a + 1) {
				  case 1:
				   dothat();
				   break;
				  case 2:
				   dothese();
				   break;
				 }
				 switch (g) {
				  case 1:
				   print(1);
				   break;
				  case 2:
				   print(5);
				   break;
				  case 3:
				   print(4);
							break;
						case 4:
							print(2);
							break;
						case 5:
							print(8);
							break;
						case 6:
							print(7);
							break;
						case 7:
							print(7);
							break;
						default:
							print(nothing);
							break;
					}
				}
";//long switch cases

var tree = CSharpSyntaxTree.ParseText(code);//#1
				
tree.GetRoot()
	.DescendantNodes()
	.OfType<MethodDeclarationSyntax>() //#2
	.Select(mds => //#3
	new
	{
		Name = mds.Identifier.ValueText,
		Switches = mds.Body
					  .DescendantNodes()
					  .OfType<SwitchStatementSyntax>()
					  //How many switch sections are there in the switch statement.
					  .Select(st => new { SwitchStatement = st.ToFullString(),Sections = st.Sections.Count})
					  .OrderByDescending(st => st.Sections)//#4

})

.Dump("Switch statements per functions");