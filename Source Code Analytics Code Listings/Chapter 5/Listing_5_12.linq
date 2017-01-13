<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

string code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace ConsoleApplication1 {
 class Program {
  static void Main(string[] args) {
   TcpListener serverSocket = new TcpListener(8888);
   int requestCount = 0;
   TcpClient clientSocket =
    default (TcpClient);
   serverSocket.Start();
   Console.WriteLine("" >> Server Started "");
   clientSocket = serverSocket.AcceptTcpClient();
   Console.WriteLine("" >> Accept connection from client "");
   requestCount = 0;
   while ((true)) {
    try {
     requestCount = requestCount + 1;
     NetworkStream networkStream = clientSocket.GetStream();
     byte[] bytesFrom = new byte[10025];
     networkStream.Read(bytesFrom, 0, (int) clientSocket.ReceiveBufferSize);
     string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
     dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf(""
      $ ""));
     Console.WriteLine("" >> Data from client - "" + dataFromClient);
     string serverResponse = ""
     Last Message from client "" + dataFromClient;
     Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
     networkStream.Write(sendBytes, 0, sendBytes.Length);
     networkStream.Flush();
     Console.WriteLine("" >> "" + serverResponse);
    } catch (Exception ex) {
     Console.WriteLine(ex.ToString());
    }
   }
   clientSocket.Close();
   serverSocket.Stop();
   Console.WriteLine("" >> exit "");
   Console.ReadLine();
  }
 }
}";

var tree = CSharpSyntaxTree.ParseText(code);

var methodCalls = tree.GetRoot()
					  .DescendantNodes()
					  .OfType<InvocationExpressionSyntax>();//#1
	
var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);//#2

var System = MetadataReference.CreateFromFile(typeof(System.Net.Sockets.Socket).Assembly.Location);

var compilation = CSharpCompilation.Create("MyCompilation",syntaxTrees: new[] { tree },references: new[] { Mscorlib, System });//#3

var model = compilation.GetSemanticModel(tree);//#4

Dictionary<string, Dictionary<string, int>> methodCallMap = new Dictionary<string, Dictionary<string, int>>();//#5

foreach (var mc in methodCalls)
{
	var methodSymbol = model.GetSymbolInfo(mc);//#6
	string typeName = methodSymbol.Symbol?.ContainingSymbol?.ContainingNamespace +"."+methodSymbol.Symbol?.ContainingSymbol?.Name; //#7
	string methodName = methodSymbol.Symbol?.Name;//#8
	if (typeName != null)//#9
	{
		try
		{
			if (!methodCallMap.ContainsKey(typeName))
			{
				methodCallMap.Add(typeName, new Dictionary<string, int>());
				methodCallMap[typeName].Add(methodName, 1);
			}
			else
			{
				if (methodName != null)
				{
					if (!methodCallMap[typeName].ContainsKey(methodName))
						methodCallMap[typeName].Add(methodName, 1);
					else
						methodCallMap[typeName][methodName]++;
				}
			}
		}
		catch
		{
			continue;
		}
	}
}
methodCallMap
.Where(cm => cm.Key.StartsWith("System.Net.Sockets"))
.Dump("Call map");