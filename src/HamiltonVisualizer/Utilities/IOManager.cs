using HamiltonVisualizer.Commands;
using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Contracts;
using HamiltonVisualizer.Extensions;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace HamiltonVisualizer.Utilities;
internal sealed class IOManager : ISetupInputBinding, IImplementKeyBindings, IImplementCommand
{
    private readonly MainWindow _window;
    private readonly SaveFileDialog _saveFileDialog;
    private readonly OpenFileDialog _openFileDialog;

    private ICommand _saveFileCommand = null!;
    private ICommand _openFileCommand = null!;

    private KeyBinding _saveFileKeyBiding = null!;
    private KeyBinding _openFileKeyBiding = null!;

    public IOManager(MainWindow window)
    {
        _window = window;

        _saveFileDialog = new()
        {
            FileName = "Graph",
            DefaultExt = ".csv",
            Filter = "Text documents (.csv)|*.csv",
            OverwritePrompt = true,
            AddExtension = true,
            AddToRecent = true,
        };
        _openFileDialog = new()
        {
            AddExtension = true,
            FileName = "Graph",
            DefaultExt = ".csv"
        };
        InitializeCommands();
        InitializeKeyBindings();
        SetupInputBindings();
        ImplementDrop();
    }
    //
    public void InitializeCommands()
    {
        _saveFileCommand = new ActionCommand(SaveFileCore);
        _openFileCommand = new ActionCommand(OpenFileCore);
    }
    public void InitializeKeyBindings()
    {
        _saveFileKeyBiding = new KeyBinding
        {
            Command = _saveFileCommand,
            Gesture = ConstantValues.KeyCombination.SaveFile,
        };
        _openFileKeyBiding = new KeyBinding
        {
            Command = _openFileCommand,
            Gesture = ConstantValues.KeyCombination.OpenFile,
        };
    }
    public void SetupInputBindings()
    {
        _window.InputBindings.Add(_saveFileKeyBiding);
        _window.InputBindings.Add(_openFileKeyBiding);
    }
    private void ImplementDrop()
    {
        _window._canvas.Drop += (sender, e) =>
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (_window.ElementCollection.Nodes.Any())
                {
                    if (ActionGuard.BeforeImport(out bool needSaveFile))
                    {
                        if (needSaveFile)
                        {
                            SaveFileCore();
                        }
                        _window.DeleteAllCore();
                        OpenFileFrom(dropFiles[0]);
                    }
                }
                else
                    OpenFileFrom(dropFiles[0]);
            }
        };
    }

    public void OpenFileCore()
    {
        _openFileDialog.ShowDialog();
        if (_window.ElementCollection.Nodes.Any())
        {
            if (ActionGuard.BeforeImport(out bool needSaveFile))
            {
                if (needSaveFile)
                {
                    SaveFileCore();
                }
                _window.DeleteAllCore();
                OpenFileFrom(_openFileDialog.FileName);
            }
        }
        else
            OpenFileFrom(_openFileDialog.FileName);
    }
    public void SaveFileCore()
    {
        _saveFileDialog.ShowDialog();
        FileExporter.WriteTo(_saveFileDialog.FileName, _window.ElementCollection);
    }
    public void OpenFileFrom(string path)
    {
        FileImporter.ReadFrom(_window, path);
    }
}

