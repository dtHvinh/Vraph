using HamiltonVisualizer.Commands;
using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventArgs.ForFeature.FeatureEventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using System.Windows.Input;

namespace HamiltonVisualizer.Utilities;
internal class Feature : ISetupInputBinding, IImplementCommand, IImplementKeyBindings
{
    private readonly MainWindow _mainWindow;
    private readonly SearchBox _searchBox;

    private ICommand FindCommand = null!;
    private ICommand DeleteCommand = null!;

    protected virtual KeyBinding FindKeyBinding { get; set; } = null!;
    protected virtual KeyBinding DeleteKeyBinding { get; set; } = null!;

    public EventHandler? FindEventHandler;
    public EventHandler? DeleteEventHandler;
    public NotificationEventHandler? NotificationEventHandler;

    public Feature(MainWindow window)
    {
        _mainWindow = window;
        _searchBox = new(this);

        _mainWindow.MainContainer.Children.Add(_searchBox);

        InitializeCommands();
        InitializeKeyBindings();
        SetupInputBindings();
    }

    public void InitializeCommands()
    {
        FindCommand = new ActionCommand(FindNodes);
        DeleteCommand = new ActionCommand(DeleteNodes);
    }
    public void SetupInputBindings()
    {
        _mainWindow.InputBindings.Add(FindKeyBinding);
        _mainWindow.InputBindings.Add(DeleteKeyBinding);
    }
    public virtual void InitializeKeyBindings()
    {
        FindKeyBinding = new KeyBinding
        {
            Command = FindCommand,
            Gesture = ConstantValues.KeyCombination.FindNodes,
        };
        DeleteKeyBinding = new KeyBinding
        {
            Command = DeleteCommand,
            Gesture = ConstantValues.KeyCombination.DeleteNodes,
        };
    }

    private void FindNodes()
    {
        OpenSearchBoxWithMode(SearchBoxMode.Find);
    }
    private void DeleteNodes()
    {
        OpenSearchBoxWithMode(SearchBoxMode.Delete);
    }

    internal void OpenSearchBoxWithMode(SearchBoxMode mode)
    {
        _searchBox.ShowWithMode(mode);
    }
    internal void CollapseSearchBox()
    {
        _searchBox.Collapse();
    }

    public void OnFind(FindEventArgs e)
    {
        FindEventHandler?.Invoke(this, e);
    }
    public void OnDelete(DeleteEventArgs e)
    {
        DeleteEventHandler?.Invoke(this, e);
    }
    public void OnNotify(NotificationEventArgs e)
    {
        NotificationEventHandler?.Invoke(this, e);
    }
}
