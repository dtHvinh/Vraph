using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// An object that can be tracked by the binding client.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
