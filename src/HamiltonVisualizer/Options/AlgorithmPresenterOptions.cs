using HamiltonVisualizer.Constants;
using System.Windows.Media;

namespace HamiltonVisualizer.Options;

public class AlgorithmPresenterOptions
{
    public bool IsDirectedGraph { get; set; } = true;
    public bool SkipTransition { get; set; } = false;
    public int NodeTransition { get; set; } = ConstantValues.Time.Transition;
    public int EdgeTransition { get; set; } = ConstantValues.Time.Transition;
    public SolidColorBrush ColorizedNode { get; set; } = ConstantValues.ControlColors.NodeTraversalBackground;
    public SolidColorBrush ColorizedLine { get; set; } = ConstantValues.ControlColors.NodeTraversalBackground;
}
