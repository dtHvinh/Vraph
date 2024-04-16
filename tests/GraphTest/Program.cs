//ObservableGraph<int> e = new(true, EqualityComparer<int>.Default);
//GraphBase<int> graph = new DirectedGraph<int>().Change();

//e.Add(0, 1);
//e.Add(1, 2);
//e.Add(2, 0);
//e.Add(2, 3);
//e.Add(3, 4);
//e.Add(4, 5);
//e.Add(4, 7);
//e.Add(5, 6);
//e.Add(6, 7);
//e.Add(6, 4);

//graph.AddEdge(0, 1);
//graph.AddEdge(1, 2);
//graph.AddEdge(2, 0);
//graph.AddEdge(2, 3);
//graph.AddEdge(3, 4);
//graph.AddEdge(4, 5);
//graph.AddEdge(4, 7);
//graph.AddEdge(5, 6);
//graph.AddEdge(6, 7);
//graph.AddEdge(6, 4);

//foreach (var item in e.DFS(3))
//{
//    Console.Write(item);
//};
//Console.WriteLine();

//foreach (var item in graph.Algorithm.DFS(3))
//{
//    Console.Write(item);
//};

//foreach (var t in e.GetAdjacent(11))
//{
//    Console.WriteLine(t);
//}

//Console.ReadKey();
//record class User(string? Name, string? Email);

//765:231,590:495
using System.Text.RegularExpressions;

var regex = new Regex("^[0-9]*:[0-9]*,[0-9]*:[0-9]*$");
Console.WriteLine(regex.IsMatch("123:313,213145:12312"));
Console.ReadKey();

