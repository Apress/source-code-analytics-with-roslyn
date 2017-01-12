<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code =

@"public static void Main()
	{
		Thread newThread = new Thread(new ThreadStart(TestMethod));
		newThread.Start();
		Thread.Sleep(1000);
		// Abort newThread.
		Console.WriteLine(""Main aborting new thread."");
		newThread.Abort(""Information from Main."");
		// Wait for the thread to terminate.
		newThread.Join();
		Console.WriteLine(@""New thread terminated –
		Main exiting."");
	}";

var tree = CSharpSyntaxTree.ParseText(code);//#1

//Finding names of all “Thread” objects
var allThreadNames =	tree.GetRoot()
							.DescendantNodes()
							.OfType<LocalDeclarationStatementSyntax>()//#2
							.Where(ldss => ldss.Declaration.Type.ToFullString().Trim() == "Thread" ||
										ldss.Declaration.Type.ToFullString().Trim()== "System.Threading.Thread")//#3
							.SelectMany(ldss => ldss.Declaration.Variables
							.Select(v => v.Identifier.ValueText));//#4

//Finding all the method invocations
tree.GetRoot()
	.DescendantNodes()
	.OfType<InvocationExpressionSyntax>()//#5
	.Where(ies => allThreadNames.Any(tn => ies.Expression.ToFullString().Trim().StartsWith(tn + ".Abort")//#6
	  || ies.Expression.ToFullString().Trim().StartsWith(tn + ".Suspend")))
	.Select(d => new //#7
	{
		Method = d.Ancestors()
				  .OfType<MethodDeclarationSyntax>()
				  .First().Identifier.ValueText,
		Line = d.Expression.ToFullString().Trim()
	})
	.Dump("Defaulters");