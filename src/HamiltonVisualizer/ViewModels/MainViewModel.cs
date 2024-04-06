using CSLibraries.DataStructure.Graph.Component;
using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Events.EventArgs.ForAlgorithm;
using HamiltonVisualizer.Events.EventHandlers.ForAlgorithm;
using HamiltonVisualizer.Utilities;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private bool _skipTransition = false;
        private readonly DirectedGraph<int> _graph = new();
        private readonly NodeMap _map = new();

        private ReadOnlyCollection<Node> _nodes = null!; // nodes in the list are guaranteed to be unique due to the duplicate check in view
        private ReadOnlyCollection<GraphLine> _edges = null!;
        private SelectedNodeCollection _selectedNode = null!;

        public event PresentingTraversalAlgorithmEventHandler? OnPresentingTraversalAlgorithm;
        public event PresentingSCCEventHandler? OnPresentingSCCAlgorithm;

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

                OnPresentingTraversalAlgorithm?.Invoke(this, new PresentingTraversalAlgorithmEventArgs()
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

                OnPresentingTraversalAlgorithm?.Invoke(this, new()
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

                OnPresentingSCCAlgorithm?.Invoke(this, new()
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

        /// <summary>
        /// When remove a node, refresh the view model.
        /// </summary>
        public void Refresh(Node node, out List<GraphLineConnectInfo> pendingRemove)
        {
            Refresh();
            pendingRemove = node.Adjacent;
        }

    }
}
