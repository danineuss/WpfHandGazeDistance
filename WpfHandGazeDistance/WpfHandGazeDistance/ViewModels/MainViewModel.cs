using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.ViewModels.Base;
using WpfHandGazeDistance.Views;

namespace WpfHandGazeDistance.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<HgdViewModel> _hgdViewModels;

        private BackgroundWorker _backgroundWorker;

        private DelegateCommand _instigateWorkCommand;

        #region Public Properties

        public HgdViewModel HgdViewModel { get; set; }

        public ParametersViewModel ParametersViewModel { get; set; }

        public ObservableCollection<HgdViewModel> HgdViewModels
        {
            get => _hgdViewModels;
            set => ChangeAndNotify(value, ref _hgdViewModels);
        }

        public bool Running { get; set; }

        public bool StopBool { get; set; }

        #endregion

        public MainViewModel()
        {
            ParametersViewModel = new ParametersViewModel();
            HgdViewModel = new HgdViewModel(ParametersViewModel.ParameterList, this);
            HgdViewModels = new ObservableCollection<HgdViewModel> {HgdViewModel};

            _backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
            };
            _backgroundWorker.DoWork += DoWork;
            _instigateWorkCommand =
                new DelegateCommand(o => _backgroundWorker.RunWorkerAsync(), o => !_backgroundWorker.IsBusy);
        }

        #region Public Members and Commands 

        public ICommand InstigateWorkCommand => _instigateWorkCommand;

        public ICommand AnalyseAllDataCommand => new RelayCommand(AnalyseAllData, true);

        public ICommand StopCommand => new RelayCommand(Stop, true);

        public ICommand AddExperimentCommand => new RelayCommand(AddHgdViewModel, true);

        public ICommand OpenParametersCommand => new RelayCommand(OpenParameters, true);

        public void RemoveHgdViewModel(HgdViewModel hgdViewModel)
        {
            HgdViewModels.Remove(hgdViewModel);
        }

        #endregion

        #region Private Members

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            AnalyseAllData();
        }

        private void AddHgdViewModel()
        {
            if (Running)
            {
                MessageBox("You can't add experiments while the programm is running. Please add all the experiments before you run the code.");
                return;
            }

            HgdViewModel hgdViewModel = new HgdViewModel(ParametersViewModel.ParameterList, this);
            HgdViewModels.Add(hgdViewModel);
        }

        private void AnalyseAllData()
        {
            Running = true;
            StopBool = false;

            foreach (HgdViewModel hgdViewModel in _hgdViewModels)
            {
                hgdViewModel.AnalyseData();
            }

            Running = false;
            StopBool = false;
        }

        private void Stop()
        {
            StopBool = true;

            foreach (HgdViewModel hgdViewModel in _hgdViewModels)
            {
                if (hgdViewModel.Running)
                {
                    hgdViewModel.StopCommand.Execute(null);
                }
            }

            Running = false;
        }

        private void OpenParameters()
        {
            if (Running)
            {
                MessageBox("You can't open the Parameters while the programm is running. Please choose parameters before you run the code.");
                return;
            }

            if (ParametersViewModel.ParameterList == null)
            {
                ParametersWindow parametersWindow = new ParametersWindow();
                parametersWindow.Show();
            }
            else
            {
                ParametersWindow parametersWindow = new ParametersWindow(ParametersViewModel.ParameterList);
                parametersWindow.Show();
            }
        }

        private void MessageBox(string message)
        {
            MessageBox messageBox = new MessageBox(message);
            messageBox.Show();
        }

        #endregion
    }
}
