<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

var code = @"using System;
namespace DoFactory.GangOfFour.Abstract.Structural {
 class MainApp {
  public static void Main() {
   AbstractFactory factory1 = new ConcreteFactory1();
   Client client1 = new Client(factory1);
   client1.Run();
   AbstractFactory factory2 = new ConcreteFactory2();
   Client client2 = new Client(factory2);
   client2.Run();
   Console.ReadKey();
  }
 }
 abstract class AbstractFactory {
  public abstract AbstractProductA CreateProductA();
  public abstract AbstractProductB CreateProductB();
 }
 class ConcreteFactory1: AbstractFactory {
  public override AbstractProductA CreateProductA() {
   return new ProductA1();
  }
  public override AbstractProductB CreateProductB() {
   return new ProductB1();
  }
 }
 class ConcreteFactory2: AbstractFactory {
  public override AbstractProductA CreateProductA() {
   return new ProductA2();
  }
  public override AbstractProductB CreateProductB() {
   return new ProductB2();
  }
 }
 abstract class AbstractProductA {}
 abstract class AbstractProductB {
  public abstract void Interact(AbstractProductA a);
 }
 class ProductA1: AbstractProductA {}
 class ProductB1: AbstractProductB {
  public override void Interact(AbstractProductA a) {
   Console.WriteLine(this.GetType().Name + ""
    interacts with "" + a.GetType().Name);
  }
 }
 class ProductA2: AbstractProductA {}
 class ProductB2: AbstractProductB {
  public override void Interact(AbstractProductA a) {
   Console.WriteLine(this.GetType().Name + ""
    interacts with "" + a.GetType().Name);
  }
 }
 class Client {
  private AbstractProductA _abstractProductA;
  private AbstractProductB _abstractProductB;
  public Client(AbstractFactory factory) {
   _abstractProductB = factory.CreateProductB();
   _abstractProductA = factory.CreateProductA();
  }
  public void Run() {
   _abstractProductB.Interact(_abstractProductA);
  }
 }
}";

var tree = CSharpSyntaxTree.ParseText(code);

//"ConcreteCreatorA" class overrides abstract method //"FactoryMethod" of abstract class "Creator"

var abstractClasses = tree.GetRoot()
							.DescendantNodes()
							.OfType<ClassDeclarationSyntax>()
							.Where(cds => cds.Modifiers
							.Select(m => m.Text).Contains("abstract"))//#1
							.Select(cds => new
							{
								ClassName = cds.Identifier.ValueText,
								AbstractMethods = cds.Members.OfType<MethodDeclarationSyntax>().Where(mds => mds.Modifiers.Select(m => m.Text)
								                 .Contains("abstract")).Select(mds => mds.Identifier.ValueText)
							})//#2

.Dump("Abstract classes and Methods");

var relations =
			tree.GetRoot()
				.DescendantNodes()
				.OfType<ClassDeclarationSyntax>()
				.Select(cds => new //#3
				{
					ClassName = cds.Identifier.ValueText,
					OverriddenMethods = cds.Members.OfType<MethodDeclarationSyntax>().Where(mds => mds.Modifiers.Select(m => m.Text).Contains("override") 
					&& mds.Modifiers.Select(m => m.Text).Contains("public"))
						.Select(mds => new 
						{
							ReturnType = mds.ReturnType.ToFullString(),
							Name = mds.Identifier.ValueText
						}),//#4

				//Elvis operator saved the day
				InheritedFrom = cds.BaseList?.Types.Select(t => t.ToFullString())
			})//#5

.Where
(
	cds => cds.InheritedFrom?.Count() == 1
	&& cds.OverriddenMethods?.Count() == 1
)//#6
.Dump(@"Abstract Factory Pattern Detected with following settings");