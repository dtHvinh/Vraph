using HamiltonVisualizer.Constants;
using System.Diagnostics;
using System.Windows.Media;

namespace HamiltonVisualizer.Utilities;

internal static class ColorPalate
{
    private static readonly List<ColorState> _colors;

    static ColorPalate()
    {
        _colors = [];

        _colors.AddRange(
            [
                new ColorState(Brushes.Beige),
                new ColorState(Brushes.Cyan),
                new ColorState(Brushes.Blue),
                new ColorState(Brushes.Green),
                new ColorState(Brushes.Magenta),
                new ColorState(Brushes.Orange),
                new ColorState(Brushes.Orchid),
                new ColorState(Brushes.SlateBlue),
                new ColorState(Brushes.SlateGray),
                new ColorState(Brushes.SteelBlue),
                new ColorState(Brushes.Yellow),
                new ColorState(Brushes.YellowGreen),
                new ColorState(Brushes.DarkGreen),
                new ColorState(Brushes.DarkOrange),
                new ColorState(Brushes.DarkOrchid),
                new ColorState(Brushes.DarkRed),
                new ColorState(Brushes.DarkSalmon),
                new ColorState(Brushes.DarkSeaGreen),
            ]);

        SetUsed(ConstantValues.ControlColors.NodeDeleteBackground);
        SetUsed(ConstantValues.ControlColors.NodeDefaultBackground);
        SetUsed(ConstantValues.ControlColors.NodeDefaultBackground);
        SetUsed(ConstantValues.ControlColors.NodeTraversalBackground);
        SetUsed(Brushes.Black);
    }

    public static SolidColorBrush GetUnusedColor()
    {
        var state = _colors.FirstOrDefault(e => e.IsUsed == false);
        if (state != null)
        {
            SetUsed(state);
            return state.Color;
        }
        else
        {
            return null!;
        }
    }

    public static void SetUsed(SolidColorBrush color)
    {
        var c = _colors.FirstOrDefault(e => e.Color.Equals(color));
        if (c != null) c.IsUsed = true;
    }

    public static void SetUsed(ColorState state)
    {
        state.IsUsed = true;
    }

    public static void Reset()
    {
        foreach (var color in _colors)
        {
            color.IsUsed = false;
        }
        SetUsed(ConstantValues.ControlColors.NodeDeleteBackground);
        SetUsed(ConstantValues.ControlColors.NodeDefaultBackground);
        SetUsed(ConstantValues.ControlColors.NodeDefaultBackground);
        SetUsed(ConstantValues.ControlColors.NodeTraversalBackground);
        SetUsed(Brushes.Black);
    }
}

[DebuggerDisplay("Color={Color.ToString()}, Used={IsUsed.ToString()}")]
public class ColorState(SolidColorBrush color)
{
    public SolidColorBrush Color { get; set; } = color;
    public bool IsUsed { get; set; } = false;
}