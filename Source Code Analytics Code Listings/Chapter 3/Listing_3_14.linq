<Query Kind="Program">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

void Main()

{

	string code = @"void fun(object a,int x,float d)
					{}
					void funny(int x)
					{}
					object soFunny(object one)
					{return one;}";

	var tree = CSharpSyntaxTree.ParseText(code);

	var result = tree.GetRoot()
					.DescendantNodes()
					.OfType<MethodDeclarationSyntax>()//#1
					.Where(thisMethod => //#2
							//This method is not an event declaration
							!thisMethod.IsEventDeclaration()
							&& thisMethod.TakesOrReturnsObject())
	.Select(thisMethod =>thisMethod.Identifier.ValueText); //#3

	if (result.Count() > 0)
	{

		result.Dump(@"Methods that aren't event handlers but takes or returns objects");

	}

}

public static class MethoDeclEx
{
	public static bool IsEventDeclaration(this MethodDeclarationSyntax mds)
	{
		return mds.ParameterList.Parameters.Any(p =>p.Type.ToFullString().EndsWith("EventArgs"));
	}
	public static bool TakesOrReturnsObject(this MethodDeclarationSyntax mds)
	{
		return mds.ParameterList.Parameters.Any(p =>
		//If any parameter is of type object

		p.Type.ToFullString().ToLower().Trim() == "object")

		//if return type is of type object
		|| mds.ReturnType.ToFullString().ToLower().Trim() == "object";
	}
}