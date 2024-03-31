using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using HamiltonVisualizer.Extensions;
using System.Collections.ObjectModel;
using System.Windows;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DirectedGraph<int> _graph = new();
        private readonly PointMap _map = new();

        private ReadOnlyCollection<Node> _nodes = null!; // nodes in the list are guaranteed to be unique due to the duplicate check in view
        private ReadOnlyCollection<LinePolygonWrapper> _edges = null!;
        private SelectedNodeCollection _selectedNode = null!;

        public event FinishedExecuteAlgorithmEventHandler? OnFinishedExecuteAlgorithm;
        public event CanvasStateChangeEventHandler? OnCanvasStateChanged;

        private bool _skipAnimation = false;
        private bool _selectMode = false;

        public bool SkipAnimation
        {
            get
            {
                return _skipAnimation;
            }
            set
            {
                _skipAnimation = value;
                OnPropertyChanged();
            }
        }
        public bool IsSelectMode
        {
            get
            {
                return _selectMode;
            }
            set
            {
                _selectMode = value;
                OnPropertyChanged();
                OnCanvasStateChanged?.Invoke(this, new() { State = value ? CanvasState.Select : CanvasState.Draw });
            }
        } // for view binding do not rename!

        public void ProvideRef(RefBag refBag)
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
        /// Update counter. Return a collection of <see cref="LinePolygonWrapper"> objects need to be remove.
        /// </summary>
        /// 
        /// <remarks>
        /// Affect:
        /// <list type="bullet">
        /// <item>Decrease NoV counter => Update UI.</item>
        /// </list>
        /// </remarks>
        /// 
        ///  <param name="pendingRemove">The <see cref="LinePolygonWrapper"/> objects that related to this object.</param>
        public void VM_NodeRemoved(Node node, out List<LinePolygonWrapper> pendingRemove)
        {
            OnPropertyChanged(nameof(NoV));
            pendingRemove = _edges.Where(e => e.From.TolerantEquals(node.Origin) || e.To.TolerantEquals(node.Origin)).ToList();
        }

        public void VM_EdgeAdded(LinePolygonWrapper line)
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
                int intNum = _map.LookUp(node.Origin);

                IEnumerable<int> result = _graph.Algorithm.DFS(intNum);

                IEnumerable<Point> points = result.Select(_map.LookUp);

                List<Node> nodes = _nodes.IntersectBy(points, e => e.Origin, PointComparer.Instance).ToList();

                OnFinishedExecuteAlgorithm?.Invoke(this, new Events.EventArgs.FinishedExecuteEventArgs()
                {
                    Name = nameof(DFS),
                    Data = nodes,
                    SkipAnimation = SkipAnimation,
                });
            }
            catch (Exception) { }
        }

        public void BFS(Node node)
        {
            try
            {
                int intNum = _map.LookUp(node.Origin);

                IEnumerable<int> result = _graph.Algorithm.BFS(intNum);

                IEnumerable<Point> points = result.Select(_map.LookUp);

                List<Node> nodes = _nodes.IntersectBy(points, e => e.Origin, PointComparer.Instance).ToList();

                OnFinishedExecuteAlgorithm?.Invoke(this, new()
                {
                    Name = nameof(BFS),
                    Data = nodes,
                    SkipAnimation = SkipAnimation,
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
