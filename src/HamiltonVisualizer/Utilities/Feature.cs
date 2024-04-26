using HamiltonVisualizer.Commands;
using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Views;
using System.Windows.Input;

namespace HamiltonVisualizer.Utilities;
internal sealed class Feature : ISetupInputBinding, IImplementCommand, IImplementKeyBindings
{
    private readonly MainWindow _mainWindow;
    private readonly FindNodeWindow _findNodeWindow;

    private ICommand FindNodeCommand = null!;

    public Feature(MainWindow window)
    {
        _mainWindow = window;
        _findNodeWindow = new();
        InitializeCommands();
        InitializeKeyBindings();
        SetupInputBindings();
    }

    public void InitializeCommands()
    {
        FindNodeCommand = new ActionCommand(FindNode);
    }

    public void InitializeKeyBindings()
    {

    }

    public void SetupInputBindings()
    {
        _mainWindow.InputBindings.Add(new KeyBinding
        {
            Command = FindNodeCommand,
            Gesture = ConstantValues.KeyCombination.SaveFile,
        });
    }

    private void FindNode()
    {

    }
}
