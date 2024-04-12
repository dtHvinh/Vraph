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

    public bool IsDirectional { get => _isDirectional; set => _isDirectional = value; }

    #region Constructors

    public ObservableGraph() : this(true, EqualityComparer<T>.Default) { }
    public ObservableGraph(IEqualityComparer<T> comparer) : this(true, comparer) { }

    #endregion Constructors
    private Dictionary<T, HashSet<T>> ProcessTransposition(Dictionary<T, HashSet<T>> source)
    {
        Dictionary<T, HashSet<T>> result = [];

        foreach (KeyValuePair<T, HashSet<T>> pair in source)
        {
            foreach (T v in pair.Value)
            {
                if (result.TryGetValue(v, out var adj))
                {
                    adj.Add(pair.Key);
                }
                else
                    result.Add(v, new(comparer)
                    {
                        pair.Key,
                    });
            }
        }
        return result;
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
                _combineAdjacent.Add(vertex, (HashSet<T>)_adjacent[vertex].Union(GetTranspose()[vertex]));
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

    public IEnumerable<T> GetAdjacent(T vertex)
    {
        _adjacent.TryGetValue(vertex, out var adj1);

        if (IsDirectional)
        {
            return adj1 is not null ? adj1 : [];
        }
        else
        {
            if (!_isCombineAdjacentUpdated)
                UpdateCombineAdjacent();
            return _combineAdjacent[vertex];
        }
    }
    public IEnumerable<T> GetVertices() => _adjacent.Keys;
    public Dictionary<T, HashSet<T>> GetTranspose()
    {
        if (_isModified)
        {
            _transpose = ProcessTransposition(_adjacent);
        }
        _isModified = false;
        return _transpose;
    }

    public IEnumerable<T> BFS(T start)
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
    public IEnumerable<T> DFS(T start)
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
        if (_adjacent.TryGetValue(u, out HashSet<T>? adj1))
        {
            adj1.Add(v);
        }
        else
        {
            _adjacent.Add(u, new(comparer)
            {
                v
            });
        }
        OnModified();
    }

    public void OnModified()
    {
        _isModified = true;
        _isCombineAdjacentUpdated = false;
    }
}
