<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code = @"public void fun() {
						 int x = 32;
						 Point p = new Point(10, 10);
						 object box = p;
						 p.x = 20;
						 Console.Write(((Point) box).x);
						 object o = x;
						}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

//There are couple of boxing calls in the provided code sample

//These should have been avoided

//x - Int

//o -> object

var objects = tree.GetRoot()
				  .DescendantNodes()
				  .OfType<VariableDeclarationSyntax>()//#2

.SelectMany(aes => aes.Variables.Select(v => 
        new //#3
		{
				Type = aes.GetFirstToken().ValueText,
				Name = v.Identifier.ValueText,
				Value = aes.GetLastToken().ValueText
		})
);

var defaulters = objects //#4
				.Where(aes => aes.Type == "object" && objects.FirstOrDefault(d => d.Name == aes.Value && d.Type != "object") != null)
				.Dump("Boxing calls");