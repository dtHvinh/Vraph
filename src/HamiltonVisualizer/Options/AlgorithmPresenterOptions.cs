using HamiltonVisualizer.Constants;
using System.Windows.Media;

namespace HamiltonVisualizer.Options;

internal sealed class AlgorithmPresenterOptions
{
    public bool IsDirectedGraph { get; set; } = true;
    public bool SkipTransition { get; set; } = false;
    public int NodeTransition { get; set; } = ConstantValues.Time.TransitionDefault;
    public int EdgeTransition { get; set; } = ConstantValues.Time.TransitionDefault;
    public SolidColorBrush ColorizedNode { get; set; } = ConstantValues.ControlColors.NodeTraversalBackground;
    public SolidColorBrush ColorizedLine { get; set; } = ConstantValues.ControlColors.NodeTraversalBackground;
}
