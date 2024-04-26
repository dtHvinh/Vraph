using HamiltonVisualizer.Commands;
using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Extensions;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace HamiltonVisualizer.Utilities;
internal class IOManager
{
    private readonly MainWindow _window;
    private readonly SaveFileDialog _saveFileDialog;
    private readonly OpenFileDialog _openFileDialog;

    private ICommand SaveFileCommand = null!;
    private ICommand OpenFileCommand = null!;

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
        };
        InitializeCommands();
        SetUpInputBindings();
        ImplementDrop();
    }
    //
    private void InitializeCommands()
    {
        SaveFileCommand = new ActionCommand(SaveFileCore);
        OpenFileCommand = new ActionCommand(OpenFileCore);
    }
    private void SetUpInputBindings()
    {
        _window.InputBindings.Add(new KeyBinding
        {
            Command = SaveFileCommand,
            Gesture = ConstantValues.KeyCombination.SaveFile,
        });
        _window.InputBindings.Add(new KeyBinding
        {
            Command = OpenFileCommand,
            Gesture = ConstantValues.KeyCombination.OpenFile,
        });
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

