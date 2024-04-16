namespace HamiltonVisualizer.DataStructure.Impl;


public class ObservableGraph<T>(bool isDirectional, IEqualityComparer<T> comparer)
    where T : notnull
{
    private bool _isDirectional = isDirectional;
    private bool _isModified = false; // need to check this before retrieve value of _transpose.
    private bool _isCombineAdjacentUpdated = true;
    private readonly Dictionary<T, HashSet<T>> _adjacent = new(comparer);
    private Dictionary<T, HashSet<T>> _transpose = new(comparer); // lazily loading transpose adjacent.
    private readonly Dictionary<T, HashSet<T>> _combineAdjacent = new(comparer);

    public bool IsDirectional
    {
        get => _isDirectional;
        set
        {
            if (value == false)
            {
                ProcessTransposition(_adjacent);
                UpdateCombineAdjacent();
            }
            _isDirectional = value;
        }
    }

    #region Constructors

    public ObservableGraph() : this(true, EqualityComparer<T>.Default) { }
    public ObservableGraph(IEqualityComparer<T> comparer) : this(true, comparer) { }

    #endregion Constructors
    private void ProcessTransposition(Dictionary<T, HashSet<T>> source)
    {
        Dictionary<T, HashSet<T>> result = [];

        foreach (T v in _adjacent.Keys)
        {
            result.TryAdd(v, new(comparer));
        }

        foreach (KeyValuePair<T, HashSet<T>> pair in source)
        {
            foreach (T v in pair.Value)
            {
                result[v].Add(pair.Key);
            }
        }
        _transpose = result;
    }
    private void UpdateCombineAdjacent()
    {
        foreach (T vertex in _adjacent.Keys)
        {
            if (_combineAdjacent.ContainsKey(vertex))
            {
                _combineAdjacent[vertex] = (HashSet<T>)_adjacent[vertex].Union(GetTranspose()[vertex]);
            }
            else
            {
                var ta = GetTranspose()[vertex];
                _combineAdjacent.Add(vertex, [.. _adjacent[vertex].Union(ta)]);
            }
        }
    }

    internal void DFSAndThen(T source, Action<T> action) // after travel all vertices, execute action recursively
    {
        HashSet<T> visited = [];

        void DFSTraversal(T source)
        {
            if (visited.Contains(source)) return;
            visited.Add(source);
            foreach (T dst in GetAdjacent(source))
            {
                DFSTraversal(dst);
            }
            action.Invoke(source);
        }

        DFSTraversal(source);
    }
    internal void DFSAnd(T source, Action<T> action) // traversal and execute action on-the-fly
    {
        HashSet<T> visited = [];

        void DFSTraversal(T source)
        {
            if (visited.Contains(source)) return;
            action.Invoke(source);
            visited.Add(source);
            foreach (T dst in GetAdjacent(source))
            {
                DFSTraversal(dst);
            }
        }

        DFSTraversal(source);
    }

    public IEnumerable<T> GetVertices() => _adjacent.Keys;
    public IEnumerable<T> GetAdjacent(T vertex) // get adjacent of a vertex, may execute update method
    {
        if (IsDirectional)
        {
            return _adjacent[vertex];
        }
        else
        {
            if (!_isCombineAdjacentUpdated)
                UpdateCombineAdjacent();
            return _combineAdjacent[vertex];
        }
    }
    public Dictionary<T, HashSet<T>> GetTranspose() // get transpose graph, may execute update method
    {
        if (_isModified)
        {
            ProcessTransposition(_adjacent);
        }
        _isModified = false;
        return _transpose;
    }

    public IEnumerable<T> BreathFirstSearch(T start)
    {
        List<T> list = [];
        HashSet<T> hashSet = [];

        Queue<T> queue = new();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            T val = queue.Dequeue();
            if (!hashSet.Add(val))
            {
                continue;
            }

            list.Add(val);
            foreach (T item in GetAdjacent(val))
            {
                queue.Enqueue(item);
            }
        }

        return list;
    }
    public IEnumerable<T> DepthFirstSearch(T start)
    {
        List<T> list = [];
        HashSet<T> hashSet = [];
        Stack<T> stack = [];
        stack.Push(start);
        while (stack.Count > 0)
        {
            T val = stack.Pop();
            if (!hashSet.Add(val))
            {
                continue;
            }

            list.Add(val);
            foreach (T item in GetAdjacent(val))
            {
                if (!hashSet.Contains(item))
                {
                    stack.Push(item);
                }
            }
        }

        return list;
    }

    public void Add(T u)
    {
        _adjacent.TryAdd(u, new(comparer));
        OnModified();
    }
    public void Add(T u, T v)
    {
        Add(u);
        Add(v);
        _adjacent[u].Add(v);
        OnModified();
    }

    public void Remove(T u)
    {
        _adjacent.Remove(u);
        _transpose.Remove(u);

        foreach (var pair in _adjacent)
        {
            pair.Value.Remove(u);
        }

        foreach (var pair in _transpose)
        {
            pair.Value.Remove(u);
        }

        OnModified();
    }
    public void Remove(T u, T v)
    {
        _adjacent[u].Remove(v);
        _adjacent[v].Remove(u);
        _transpose[u].Remove(v);
        _transpose[v].Remove(u);
        OnModified();
    }

    public void OnModified()
    {
        _isModified = true;
        _isCombineAdjacentUpdated = false;
    }
}
