using CSLibraries.DataStructure.Graph.Base;
using CSLibraries.DataStructure.Graph.Component;
using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Events.EventArgs.ForAlgorithm;
using HamiltonVisualizer.Events.EventHandlers.ForAlgorithm;
using HamiltonVisualizer.Events.EventHandlers.ForGraph;
using HamiltonVisualizer.Utilities;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private bool _skipTransition = false;
        private bool _isDirectionalGraph = true;
        private GraphBase<int> _graph = new DirectedGraph<int>();
        private readonly NodeMap _map = new();

        private ReadOnlyCollection<Node> _nodes = null!; // nodes in the list are guaranteed to be unique due to the duplicate check in view
        private ReadOnlyCollection<GraphLine> _edges = null!;
        private SelectedNodeCollection _selectedNode = null!;

        public event PresentingTraversalAlgorithmEventHandler? PresentingTraversalAlgorithm;
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
                int intNum = _map.LookUp(node);

                IEnumerable<int> result = _graph.Algorithm.DFS(intNum);

                IEnumerable<Node> nodes = result.Select(_map.LookUp);

                PresentingTraversalAlgorithm?.Invoke(this, new PresentingTraversalAlgorithmEventArgs()
                {
                    Name = nameof(DisplayDFS),
                    Data = nodes,
                    SkipTransition = SkipTransition,
                });
            }
            catch (Exception) { }
        }
        public void DisplayBFS(Node node)
        {
            try
            {
                int intNum = _map.LookUp(node);

                IEnumerable<int> result = _graph.Algorithm.BFS(intNum);

                IEnumerable<Node> nodes = result.Select(_map.LookUp);

                PresentingTraversalAlgorithm?.Invoke(this, new()
                {
                    Name = nameof(DisplayBFS),
                    Data = nodes,
                    SkipTransition = SkipTransition,
                });
            }
            catch (Exception) { }
        }
        public void DisplaySCC()
        {
            try
            {
                IEnumerable<SCC<int>> result = _graph.Algorithm.GetComponents();

                IEnumerable<IEnumerable<Node>> nodes = result.Select(Convert);

                PresentingSCCAlgorithm?.Invoke(this, new()
                {
                    Name = nameof(DisplaySCC),
                    Data = nodes,
                    SkipTransition = SkipTransition,
                });
            }
            catch (Exception) { }
        }

        private IEnumerable<Node> Convert(SCC<int> scc)
        {
            return scc.Vertices.Select(_map.LookUp);
        }

        /// <summary>
        /// Should invoke this method when:
        /// <list type="bullet">
        /// <item>Modify the collection of selected nodes.</item>
        /// <item>Modify the collection edges.</item>
        /// <item>Modify the collection vertices.</item>
        /// </list>
        /// </summary>
        public void Refresh()
        {
            OnPropertyChanged(nameof(NoSN));
            OnPropertyChanged(nameof(NoE));
            OnPropertyChanged(nameof(NoV));
        }

        /// <summary>
        /// When add new <see cref="GraphLine"/>, refresh view model.
        /// </summary>
        /// <param name="line">The newly added <see cref="GraphLine"/>.</param>
        public void Refresh(GraphLine line)
        {
            Refresh();

            var u = _map.LookUp(line.From);
            var v = _map.LookUp(line.To);

            _graph.AddEdge(u, v);
        }

        public void OnGraphModeChanged()
        {
            OnPropertyChanged(nameof(IsDirectionalGraph));
            _graph = _graph.Change();
            GraphModeChanged?.Invoke(this, new());
        }
    }
}
