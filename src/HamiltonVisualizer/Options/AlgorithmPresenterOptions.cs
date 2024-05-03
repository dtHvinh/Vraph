using HamiltonVisualizer.Constants;
using System.Windows.Media;

namespace HamiltonVisualizer.Options;

internal sealed class AlgorithmPresenterOptions
{
    public bool IsDirectedGraph { get; set; } = true;
    public bool SkipTransition { get; set; } = false;
    public int NodeTransition { get; set; } = ConstantValues.Time.NodeTransitionDefault;
    public int EdgeTransition { get; set; } = ConstantValues.Time.LineTransitionDefault;
    public SolidColorBrush ColorizedNode { get; set; } = ConstantValues.ControlColors.NodeTraversalColor;
    public SolidColorBrush ColorizedLine { get; set; } = ConstantValues.ControlColors.LineTraversalColor;
}
