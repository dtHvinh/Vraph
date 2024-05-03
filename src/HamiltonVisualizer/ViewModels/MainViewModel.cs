using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.DataStructure.Base;
using HamiltonVisualizer.DataStructure.Components;
using HamiltonVisualizer.DataStructure.Implements;
using HamiltonVisualizer.Events.EventHandlers.ForAlgorithm;
using HamiltonVisualizer.Events.EventHandlers.ForGraph;
using HamiltonVisualizer.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace HamiltonVisualizer.ViewModels;

/**
 * ***************************************************************************************************
 * ** Node or GraphLine Should not be added directly from inside this class, it should be add       **
 * ** using method like CreateNodeAtPosition from MainView instead                                  **
 * ***************************************************************************************************
 */
internal sealed class MainViewModel : ObservableObject
{
    private bool _skipTransition = false;
    private bool _isDirectionalGraph = true;
    private GraphBase<Node> _graph = new DirectedGraph<Node>();

    private ReadOnlyCollection<Node> _nodes = null!; // _nodes in the list are guaranteed to be unique due to the duplicate check in view
    private ReadOnlyCollection<GraphLine> _edges = null!;
    private SelectedNodePair _selectedNode = null!;

    public event PresentingTraversalAlgorithmEventHandler? PresentingTraversalAlgorithm;
    public event PresentingLayeredBFSEventHandler? PresentingLayeredBFSAlgorithm;
    public event PresentingSCCEventHandler? PresentingSCCAlgorithm;
    public event GraphModeChangeEventHandler? GraphModeChanged;

    public bool SkipTransition
    {
        get
        {
            return _skipTransition;
        }
        set
        {
            _skipTransition = value;
            OnPropertyChanged();
        }
    }

    public bool IsDirectionalGraph
    {
        get
        {
            return _isDirectionalGraph;
        }
        set
        {
            _isDirectionalGraph = value;
            OnGraphModeChanged();
        }
    }

    public void SetRefs(RefBag refBag)
    {
        _nodes = refBag.Nodes;
        _edges = refBag.Edges;
        _selectedNode = refBag.SelectedNodeCollection;
    }

    public int NoE // for view binding do not rename!
    {
        get
        {
            return _edges is null ? 0 : _edges.Count;
        }
    }
    public int NoV // for view binding do not rename!
    {
        get
        {
            return _nodes is null ? 0 : _nodes.Count;
        }
    }
    public int NoSN // for view binding do not rename!
    {
        get
        {
            return _selectedNode is null ? 0 : _selectedNode.Count;
        }
    }

    public void DisplayDFS(Node node)
    {
        try
        {
            IEnumerable<Node> nodes = _graph.Algorithm.DFS(node);
            OnPresentingTraversal(ConstantValues.AlgorithmNames.DFS, nodes);
        }
        catch (Exception) { }
    }
    public void DisplayBFS(Node node)
    {
        try
        {
            IEnumerable<BFSComponent<Node>> nodes = _graph.Algorithm.BFSLayered(node);
            OnPresentingLayeredBFS(nodes);
        }
        catch (Exception) { }
    }
    public void DisplaySCC()
    {
        try
        {
            IEnumerable<IEnumerable<Node>> nodes = _graph.Algorithm.GetComponents().Select(e => e.Vertices);
            OnPresentingSCC(nodes);
        }
        catch (Exception) { }
    }
    public void DisplayHamiltonCycle()
    {
        IEnumerable<Node> nodes = _graph.Algorithm.HC();
        if (nodes.Count() == NoV)
        {
            OnPresentingTraversal(ConstantValues.AlgorithmNames.Hamilton, nodes);
        }
        else
            MessageBox.Show("Không tìm thấy chu trình", "Thông báo");
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(NoSN));
        OnPropertyChanged(nameof(NoE));
        OnPropertyChanged(nameof(NoV));
    }
    public void RefreshWhenAdd(GraphLine line)
    {
        Refresh();
        _graph.AddEdge(line.From, line.To);
    }
    public void RefreshWhenAdd(Node node)
    {
        Refresh();
        _graph.Adjacent.AddVertex(node);
    }
    public void RefreshWhenRemove(Node node)
    {
        Refresh();
        _graph.Adjacent.RemoveVertex(node);
    }

    public void Clear()
    {
        _graph.Clear();
        Refresh();
    }
    public void SkipTransitionSwitch()
    {
        SkipTransition = !SkipTransition;
    }
    public void GraphModeSwitch()
    {
        IsDirectionalGraph = !IsDirectionalGraph;
    }

    public void OnGraphModeChanged()
    {
        OnPropertyChanged(nameof(IsDirectionalGraph));
        Mouse.OverrideCursor = Cursors.Wait;
        _graph = _graph.Change();
        Mouse.OverrideCursor = Cursors.Arrow;
        GraphModeChanged?.Invoke(this, new());
    }
    public void OnPresentingSCC(IEnumerable<IEnumerable<Node>> nodes)
    {
        PresentingSCCAlgorithm?.Invoke(this, new()
        {
            Name = nameof(DisplaySCC),
            Data = nodes,
            SkipTransition = SkipTransition,
        });
    }
    public void OnPresentingTraversal(string name, IEnumerable<Node> nodes)
    {
        PresentingTraversalAlgorithm?.Invoke(this, new()
        {
            Name = name,
            Data = nodes,
            SkipTransition = SkipTransition,
        });
    }
    public void OnPresentingLayeredBFS(IEnumerable<BFSComponent<Node>> layeredNodes)
    {
        PresentingLayeredBFSAlgorithm?.Invoke(this, new()
        {
            Data = layeredNodes,
        });
    }
}
