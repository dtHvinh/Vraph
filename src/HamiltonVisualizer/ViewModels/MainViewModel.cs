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

namespace HamiltonVisualizer.ViewModels
{
    /**
     * ***************************************************************************************************
     * ** Node or GraphLine Should not be added directly from inside this class, it should be add       **
     * ** using method like CreateNode from MainView instead                                            **
     * ***************************************************************************************************
     */
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
        public event PresentingHamiltonAlgorithmEventHandler? PresentingHamiltonCycleAlgorithm;
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
                OnPresentingTraversal("DFS", nodes);
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
                OnPresentingTraversal("BFS", nodes);
            }
            catch (Exception) { }
        }
        public void DisplaySCC()
        {
            try
            {
                IEnumerable<SCC<int>> result = _graph.Algorithm.GetComponents();
                IEnumerable<IEnumerable<Node>> nodes = result.Select(Convert);
                OnPresentingSCC(nodes);
            }
            catch (Exception) { }
        }
        public void DisplayHamiltonCycle()
        {
            IEnumerable<Node> nodes = _graph.Algorithm.HamiltonianCycle().Select(_map.LookUp);
            if (nodes.Count() == NoV)
            {
                OnPresentingTraversal("Hamilton", nodes);
            }
            else
                MessageBox.Show("Không tìm thấy chu trình", "Thông báo");
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
        public void RefreshWhendAdd(GraphLine line)
        {
            Refresh();

            var u = _map.LookUp(line.From);
            var v = _map.LookUp(line.To);

            _graph.AddEdge(u, v);
        }
        public void RefreshWhendRemove(Node node)
        {
            Refresh();

            var u = _map.LookUp(node);
            _graph.Adjacent.RemoveVertex(u);
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
    }
}
