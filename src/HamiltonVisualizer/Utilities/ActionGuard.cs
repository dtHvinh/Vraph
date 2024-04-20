using System.Windows;

namespace HamiltonVisualizer.Utilities;
public static class ActionGuard
{
    public static bool ShouldContinue(string message)
    {
        var result = MessageBox.Show(message, "Cảnh Báo", MessageBoxButton.OKCancel);
        return result == MessageBoxResult.OK;
    }

    public static bool ShouldContinue(string message, Delegate action, object[]? args)
    {
        var result = MessageBox.Show(message, "Cảnh Báo", MessageBoxButton.OKCancel);
        var should = result == MessageBoxResult.OK;
        if (should)
            action.DynamicInvoke();
        return should;
    }
}
