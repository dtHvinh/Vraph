using System.Windows.Input;

namespace HamiltonVisualizer.Core;

public class RelayCommand(
    Action<object> execute,
    Predicate<object>? canExecute = null) : ICommand
{
#nullable disable
    private readonly Predicate<object> _canExecute = canExecute;

    private readonly Action<object> _execute = execute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return parameter is not null && _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}
