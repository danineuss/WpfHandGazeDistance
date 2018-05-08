using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;
using WpfHandGazeDistance.Views;

namespace WpfHandGazeDistance.ViewModels
{
    public class PrototypingViewModel : BaseViewModel
    {
        private ParametersViewModel _parametersViewModel;

        private ObservableCollection<HgdViewModel> _hgdViewModels;

        private float _startTime = 660f;
        private float _duration = 0f;

        private string _videoPath;

        private readonly BackgroundWorker _backgroundWorker;
        private readonly ICommand instigateWorkCommand;
        private int _currentProgress;

        public float StartTime
        {
            get => _startTime;
            set => ChangeAndNotify(value, ref _startTime);
        }

        public float Duration
        {
            get => _duration;
            set => ChangeAndNotify(value, ref _duration);
        }

        public string VideoPath
        {
            get => _videoPath;
            set => ChangeAndNotify(value, ref _videoPath);
        }

        public ObservableCollection<HgdExperiment> HgdExperiments { get; set; }

        public ObservableCollection<HgdViewModel> HgdViewModels
        {
            get => _hgdViewModels;
            set => ChangeAndNotify(value, ref _hgdViewModels);
        }

        #region Constructor

        public PrototypingViewModel()
        {
            //instigateWorkCommand =
            //    new DelegateCommand(o => _backgroundWorker.RunWorkerAsync(), o => !_backgroundWorker.IsBusy);
            //_backgroundWorker = new BackgroundWorker();
            //_backgroundWorker.DoWork += DoWork;
            //_backgroundWorker.ProgressChanged += ProgressChanged;

            _parametersViewModel = new ParametersViewModel();
            InitializeMyList();
        }

        #endregion

        //public ICommand InstigateWorkCommand => this.instigateWorkCommand;

        //public int CurrentProgress
        //{
        //    get => _currentProgress;
        //    private set => ChangeAndNotify(value, ref _currentProgress);
        //}

        //private void DoWork(object sender, DoWorkEventArgs e)
        //{
        //    // do time-consuming work here, calling ReportProgress as and when you can
        //}

        //private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    CurrentProgress = e.ProgressPercentage;
        //}


        public ICommand AnalyseAllDataCommand => new RelayCommand(AnalyseAllData, true);

        public ICommand StopCommand => new RelayCommand(Stop, true);

        public ICommand AddExperimentCommand => new RelayCommand(AddExperiment, true);

        public ICommand OpenParametersCommand => new RelayCommand(OpenParameters, true);

        public ICommand GoCommand => new RelayCommand(Go, true);

        public void InitializeMyList()
        {
            HgdExperiments = new ObservableCollection<HgdExperiment>();
            HgdExperiments.Clear();
            HgdExperiments.Add(new HgdExperiment());

            HgdViewModels = new ObservableCollection<HgdViewModel>();
            HgdViewModels.Clear();
            AddHgdViewModel();
        }

        public void RemoveHgdViewModel(HgdViewModel hgdViewModel)
        {
            HgdViewModels.Remove(hgdViewModel);
        }

        //private void LoadVideo()
        //{
        //    VideoPath = FileManager.OpenFileDialog();
        //    _hgdExperiment.VideoPath = VideoPath;
        //    _hgdExperiment.Video = new Video(VideoPath);
        //    _hgdExperiment.Video.ThumbnailImage.Resize(0.1);
        //    _hgdExperiment.Thumbnail = _hgdExperiment.Video.ThumbnailImage.BitMapImage;
        //}

        private void AddExperiment()
        {
            HgdExperiments.Add(new HgdExperiment());

            HgdViewModels.Add(new HgdViewModel(_parametersViewModel.ParameterList, this));
        }

        private void AddHgdViewModel()
        {
            HgdViewModel hgdViewModel = new HgdViewModel(_parametersViewModel.ParameterList, this);
            HgdViewModels.Add(hgdViewModel);
        }

        private void AnalyseAllData()
        {
            //foreach(HgdExperiment hgdExperiment in HgdExperiments)
            //{
            //    hgdExperiment.Analyse();
            //}

            foreach (HgdViewModel hgdViewModel in HgdViewModels)
            {
                hgdViewModel.AnalyseData();
            }
        }

        private void Stop()
        {

        }

        private void OpenParameters()
        {
            if (_parametersViewModel.ParameterList == null)
            {
                ParametersWindow parametersWindow = new ParametersWindow();
                parametersWindow.Show();
            }
            else
            {
                ParametersWindow parametersWindow = new ParametersWindow(_parametersViewModel.ParameterList);
                parametersWindow.Show();
            }
        }

        private void Go()
        {
            Debug.Print("Goooo!");
        }
    }
};