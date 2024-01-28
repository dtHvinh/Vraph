using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.GraphUIComponents;
using HamiltonVisualizer.ViewModels;
using Libraries.Geometry;
using System.Windows;
using System.Windows.Input;

namespace HamiltonVisualizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainViewModel VM { get; set; }
    public List<Node> Nodes { get; set; } = [];

    public MainWindow()
    {
        InitializeComponent();

        VM = (MainViewModel)DataContext;

        ArgumentNullException.ThrowIfNull(VM);
    }

    #region Navbar behaviors

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void Exit(object sender, MouseButtonEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MaximizeAndRestore(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
            WindowState = WindowState.Maximized;
    }

    private void MinimizeToTaskbar(object sender, MouseButtonEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    #endregion Navbar behaviors

    #region Drawing

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var mPos = e.GetPosition(DrawingCanvas);

            Node node = new(mPos, DrawingCanvas);
            node.OnNodeDelete += Node_OnNodeDelete;

            if (EnsureNoCollision(node))
            {
                AddToCanvas(node);
                VM.AddNewNode(node);
            }
        }
    }


    #endregion Drawing

    #region Events

    private void Node_OnNodeDelete(object sender, NodeDeleteEventArgs e)
    {
        var node = sender as Node;
        VM.RemoveNode(node!);
        Nodes.Remove(node!);
    }

    #endregion Events

    #region Helper class

    /// <summary>
    /// Make sure there no collision happen between nodes.
    /// </summary>
    /// <param name="node">The node to check.</param>
    private bool EnsureNoCollision(Node node)
    {
        if (Nodes.Count == 0)
            return true;

        foreach (Node n in Nodes)
        {
            if (CollisionHelper.IsCircleCollide(Libraries.Geometry.Point.ConvertFrom(node.Position),
                                                Libraries.Geometry.Point.ConvertFrom(n.Position),
                                                Node.NodeWidth / 2))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Add Graph node to the view ui.
    /// </summary>
    /// <param name="node"></param>
    private void AddToCanvas(Node node)
    {
        DrawingCanvas.Children.Add(node);
        Nodes.Add(node);
    }

    #endregion Helper class
}