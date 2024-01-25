#nullable disable
using HamiltonVisualizer.Core;

namespace HamiltonVisualizer.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public DrawViewModel DrawVM { get; set; }
        public ConnectedCompViewModel ConCompVM { get; set; }
        public HamiltonCycleViewModel HamiltonCVM { get; set; }

        public RelayCommand DrawViewCmd { get; set; }
        public RelayCommand ConCompCmd { get; set; }
        public RelayCommand HamiltonCCmd { get; set; }

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
            ConCompVM = new();
            DrawVM = new();
            HamiltonCVM = new();
        }

        private void InitializeCommand()
        {
            DrawViewCmd = new RelayCommand(o =>
            {
                CurrentView = DrawVM;
            });

            ConCompCmd = new RelayCommand(o =>
            {
                CurrentView = ConCompVM;
            });

            HamiltonCCmd = new RelayCommand(o =>
            {
                CurrentView = HamiltonCVM;
            });
        }
    }
}
