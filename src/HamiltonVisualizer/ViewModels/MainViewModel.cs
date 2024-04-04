using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using HamiltonVisualizer.Utilities;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject, IRefSetter
    {
        private readonly DirectedGraph<int> _graph = new();
        private readonly NodeMap _map = new();

        private ReadOnlyCollection<Node> _nodes = null!; // nodes in the list are guaranteed to be unique due to the duplicate check in view
        private ReadOnlyCollection<GraphLine> _edges = null!;
        private SelectedNodeCollection _selectedNode = null!;

        public event PresentingAlgorithmEventHandler? OnPresentingAlgorithm;

        private bool _skipTransition = false;

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

        public void VM_NodeAdded()
        {
            OnPropertyChanged(nameof(NoV));
        }

        /// <summary>
        /// Update counter. Return a collection of <see cref="GraphLine"> objects need to be remove.
        /// </summary>
        /// 
        /// <remarks>
        /// Affect:
        /// <list type="bullet">
        /// <item>Decrease NoV counter => Update UI.</item>
        /// </list>
        /// </remarks>
        /// 
        ///  <param name="pendingRemove">The <see cref="GraphLine"/> objects that related to this object.</param>
        public void VM_NodeRemoved(Node node, out List<GraphLineConnectInfo> pendingRemove)
        {
            OnPropertyChanged(nameof(NoV));
            OnPropertyChanged(nameof(NoSN)); // a node while selecting may be deleted
            pendingRemove = node.Adjacent;
        }

        public void VM_EdgeAdded(GraphLine line)
        {
            OnPropertyChanged(nameof(NoE));

            var u = _map.LookUp(line.From);
            var v = _map.LookUp(line.To);

            _graph.AddEdge(u, v);
        }

        public void VM_EdgeRemoved()
        {
            OnPropertyChanged(nameof(NoE));
        }

        /// <summary>
        /// Tell the client to update the view data.
        /// </summary>
        public void VM_NodeSelectedOrRelease()
        {
            OnPropertyChanged(nameof(NoSN));
        }

        public void DFS(Node node)
        {
            try
            {
                int intNum = _map.LookUp(node);

                IEnumerable<int> result = _graph.Algorithm.DFS(intNum);

                IEnumerable<Node> nodes = result.Select(_map.LookUp);

                OnPresentingAlgorithm?.Invoke(this, new PresentingAlgorithmEventArgs()
                {
                    Name = nameof(DFS),
                    Data = nodes,
                    SkipTransition = SkipTransition,
                });
            }
            catch (Exception) { }
        }

        public void BFS(Node node)
        {
            try
            {
                int intNum = _map.LookUp(node);

                IEnumerable<int> result = _graph.Algorithm.BFS(intNum);

                IEnumerable<Node> nodes = result.Select(_map.LookUp);

                OnPresentingAlgorithm?.Invoke(this, new()
                {
                    Name = nameof(BFS),
                    Data = nodes,
                    SkipTransition = SkipTransition,
                });
            }
            catch (Exception) { }
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(NoSN));
            OnPropertyChanged(nameof(NoE));
            OnPropertyChanged(nameof(NoV));
        }
    }
}
