using System.Windows.Input;

namespace HamiltonVisualizer.Commands;
public class ActionCommand(Action action) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        action.Invoke();
    }
}
