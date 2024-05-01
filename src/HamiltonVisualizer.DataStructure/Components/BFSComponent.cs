namespace HamiltonVisualizer.DataStructure.Components;

public class BFSComponent<T>(T root)
{
    private readonly T _root = root;
    private readonly List<T> _children = [];

    public T Root() => _root;
    public void AddChild(T child) => _children.Add(child);
    public List<T> Children => _children;

    public bool AnyChild() => _children.Count != 0;
}
