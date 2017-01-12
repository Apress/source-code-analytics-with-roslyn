<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var code = @"struct Vector: IEquatable < Vector > {
			 public int X {
			  get;
			  set;
			 }
			 public int Y {
			  get;
			  set;
			 }
			 public int Z {
			  get;
			  set;
			 }
			 public int Magnitude {
			  get;
			  set;
			 }
			 public override bool Equals(object obj) {
			  if (obj == null) {
			   return false;
			  }
			  if (obj.GetType() != this.GetType()) {
			   return false;
			  }
			  return this.Equals((Vector) obj);
			 }
			 public bool Equals(Vector other) {
			  return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.Magnitude == other.Magnitude;
			 }
			}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

tree.GetRoot()
	.DescendantNodes()
	.OfType<StructDeclarationSyntax>()//#2
	.Select(sds => new //#3
	{
		StructName = sds.Identifier.ValueText,
		//Flag if “Equals” is overridden
		OverridenEquals = sds.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.ValueText == "Equals" && 
		                        m.Modifiers.Any(mo => mo.ValueText == "override")) != null,
		//Flag if “GetHashCode” is overridden
		OverridenGetHashCode = sds.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.ValueText == "GetHashCode" &&
		                       m.Modifiers.Any(mo => mo.ValueText == "override")) != null

	})
.Where(sds => !sds.OverridenEquals || !sds.OverridenGetHashCode)//#4
.Dump("Defaulter Structs");