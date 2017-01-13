<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code = @"using System;
			namespace DoFactory.GangOfFour.Proxy.RealWorld {
			 class MainApp {
			  static void Main() {
			   MathProxy proxy = new MathProxy();
			   Console.WriteLine(""
			    4 + 2 = "" + proxy.Add(4, 2));
			   Console.WriteLine(""
			    4 - 2 = "" + proxy.Sub(4, 2));
			   Console.WriteLine(""
			    4 * 2 = "" + proxy.Mul(4, 2));
			   Console.WriteLine(""
			    4 / 2 = "" + proxy.Div(4, 2));
			   Console.ReadKey();
			  }
			 }
			 public interface IMath {
			  double Add(double x, double y);
			  double Sub(double x, double y);
			  double Mul(double x, double y);
			  double Div(double x, double y);
			 }
			 class Math: IMath {
			  public double Add(double x, double y) {
			   return x + y;
			  }
			  public double Sub(double x, double y) {
			   return x - y;
			  }
			  public double Mul(double x, double y) {
			   return x * y;
			  }
			  public double Div(double x, double y) {
			   return x / y;
			  }
			 }
			 class MathProxy: IMath {
			  private Math _math = new Math();
			  public double Add(double x, double y) {
			   return _math.Add(x, y);
			  }
			  public double Sub(double x, double y) {
			   return _math.Sub(x, y);
			  }
			  public double Mul(double x, double y) {
			   return _math.Mul(x, y);
			  }
			  public double Div(double x, double y) {
			   return _math.Div(x, y);
			  }
			 }
			}";

var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.Select(cds => new //#1
	{
		ClassName = cds.Identifier.ValueText,
		InheritedFrom = cds.BaseList?.Types.Select(t => t.ToFullString().Trim()),
		Fields = cds.Members.OfType<FieldDeclarationSyntax>()
		.Select(fds => new
		{
			TypeName = fds.Declaration.Type.ToFullString(),
			Variables = fds.Declaration.Variables.Select(v => v.Identifier.ValueText)
		}),
		Invocations = cds.Members.OfType<MethodDeclarationSyntax>().Select(mds => mds.Body.ToFullString())
	})//#2
.Where(cds => cds.ClassName != null && cds.InheritedFrom != null && cds.Fields != null && cds.Invocations != null )//#3
.Dump("Proxy Design Pattern Participants");