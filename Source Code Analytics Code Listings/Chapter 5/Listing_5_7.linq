<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code = @"
namespace CompositePattern
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    //Client
    class Program
    {
        static void Main(string[] args)
        {
            // initialize variables
            var compositeGraphic = new CompositeGraphic();
            var compositeGraphic1 = new CompositeGraphic();
            var compositeGraphic2 = new CompositeGraphic();

            //Add 1 Graphic to compositeGraphic1
            compositeGraphic1.Add(new Ellipse());

            //Add 2 Graphic to compositeGraphic2
            compositeGraphic2.AddRange(new Ellipse(), 
                new Ellipse());

            /*Add 1 Graphic, compositeGraphic1, and 
              compositeGraphic2 to compositeGraphic */
            compositeGraphic.AddRange(new Ellipse(), 
                compositeGraphic1, 
                compositeGraphic2);

            
            compositeGraphic.Print();
Console.ReadLine();
        }
    }
    //Component
    public interface IGraphic
{
	void Print();
}
//Leaf
public class Ellipse : IGraphic
{
	//Prints the graphic
	public void Print()
	{
		Console.WriteLine(Ellipse);
	}
}
//Composite
public class CompositeGraphic : IGraphic
{
	//Collection of Graphics.
	private readonly List<IGraphic> graphics;

	//Constructor 
	public CompositeGraphic()
	{
		//initialize generic Collection(Composition)
		graphics = new List<IGraphic>();
	}
	//Adds the graphic to the composition
	public void Add(IGraphic graphic)
	{
		graphics.Add(graphic);
	}
	//Adds multiple graphics to the composition
	public void AddRange(params IGraphic[] graphic)
	{
		graphics.AddRange(graphic);
	}
	//Removes the graphic from the composition
	public void Delete(IGraphic graphic)
	{
		graphics.Remove(graphic);
	}
	//Prints the graphic.
	public void Print()
	{
		foreach (var childGraphic in graphics)
		{
			childGraphic.Print();
		}
	}
}
}
";


var tree = CSharpSyntaxTree.ParseText(code);

tree.GetRoot()
	.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.Select(cds => new
	{
		ClassName = cds.Identifier.ValueText,
		InheritsFrom = cds.BaseList?.Types.Select(t => t.ToFullString().Trim()),
		VariableTypes = cds.Members.OfType<FieldDeclarationSyntax>().Select(fds => fds.Declaration.Type.ToFullString().Trim())
	})//#1
	.Where(cds => cds.InheritsFrom?.Count() > 0 && cds?.VariableTypes.Count() > 0)
	.Where(cds => cds.InheritsFrom.Any(b => cds.VariableTypes.Any(v =>
	//This assumes that the composite type will hold the leaf(s) in //a generic collection
				v.Contains("<" + b.Trim() + ">"))))//#2
	.Dump("Probable Composite Pattern Detected");