<Query Kind="Expression">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

public class Profile

{

	//Directory which stores code written by this author
	public string CodeDirectory { get; internal set; }
	//Representation of the profile
	//The keys represent the elements of style
	//and values represent the values calculated for each
	//code file
	private Dictionary<string, List<double>> Rows = new Dictionary<string, List<double>>();
	//All the source code syntax trees for all the sources
	//in the given directory.
	private List<SyntaxTree> CSharpForest =	new List<SyntaxTree>();
	//Name of the author who’s profile it is.
	public string Name { get; set; }
	public Profile(string name)
	{
		Name = name;
	}
	//Other methods to get the profile vector values
	//goes here.
	private void PopulateRows(string name, int parts, int all)
	{

		if (!Rows.ContainsKey(name))
			Rows.Add(name, new List<double>(){ (double)parts / (double)all });
		else
			Rows[name].Add((double)parts / (double)all);
	}
	//Gets all the CSharp Syntax Trees in the given directory
	public void GetTrees(int n)
	{
		foreach (var codeFile in Directory
		.GetFiles(CodeDirectory, "*.cs", SearchOption.AllDirectories).Take(n))
		{
			try
			{
				CSharpForest.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(codeFile)));
			}
			catch
			{
				//Nothing to do.
			}
		}
	}
}