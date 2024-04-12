namespace HamiltonVisualizer.DataStructure.Impl.Events.EventArgs;

public class GraphModifyEventArgs
{
    public GraphModifyMethod Method { get; set; }
    public object? Data { get; set; }
}

public enum GraphModifyMethod
{
    AddEdge,
    AddVertex,
    RemoveEdge,
    RemoveVertex,
}
