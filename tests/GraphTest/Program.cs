using HamiltonVisualizer.DataStructure.Implements;

DirectedGraph<int> a = new();

a.AddEdge(1, 2);
a.AddEdge(1, 3);
a.AddEdge(1, 4);
a.AddEdge(2, 5);
a.AddEdge(2, 7);
a.AddEdge(2, 8);

foreach (var layer in a.Algorithm.BFSLayered(1))
{
    foreach (var i in layer)
    {
        Console.Write(i + " ");
    }
    Console.WriteLine();
}

Console.ReadKey();
