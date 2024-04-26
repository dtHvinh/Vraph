using HamiltonVisualizer.Constants;
using System.Windows;

namespace HamiltonVisualizer.Utilities;
internal static class ActionGuard
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

    public static bool BeforeImport(out bool needSaveFile)
    {
        var result = MessageBox.Show(ConstantValues.Messages.ConfirmBeforeImport, "Cảnh Báo", MessageBoxButton.YesNoCancel);
        if (result == MessageBoxResult.Cancel)
        {
            needSaveFile = false;
            return false;
        }
        needSaveFile = result == MessageBoxResult.Yes;
        return true;
    }
}
