#nullable disable
using HamiltonVisualizer.Core;

namespace HamiltonVisualizer.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public DrawViewModel DrawVM { get; set; }

        public RelayCommand SetInitialView { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            InitializeViewModels();
            InitializeCommand();

            CurrentView = DrawVM;
        }

        private void InitializeViewModels()
        {
            DrawVM = new();
        }

        private void InitializeCommand()
        {
            SetInitialView = new RelayCommand(o =>
            {
                CurrentView = DrawVM;
            });
        }
    }
}
