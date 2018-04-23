using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class PrototypingViewModel : BaseViewModel
    {
        private BitmapSource _publicImage;
        private string _videoPath;
        private HgdExperiment _hgdExperiment;

        private readonly BackgroundWorker _backgroundWorker;
        private readonly ICommand instigateWorkCommand;
        private int _currentProgress;

        public string VideoPath
        {
            get => _videoPath;
            set => ChangeAndNotify(value, ref _videoPath);
        }

        public BitmapSource PublicImage
        {
            get => _publicImage;
            set => ChangeAndNotify(value, ref _publicImage);
        }

        public ObservableCollection<HgdExperiment> HgdExperiments { get; set; }

        #region Constructor

        public PrototypingViewModel()
        {
            instigateWorkCommand =
                new DelegateCommand(o => _backgroundWorker.RunWorkerAsync(), o => !_backgroundWorker.IsBusy);
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += DoWork;
            _backgroundWorker.ProgressChanged += ProgressChanged;

            InitializeMyList();
        }

        #endregion

        public ICommand InstigateWorkCommand => this.instigateWorkCommand;

        public int CurrentProgress
        {
            get => _currentProgress;
            private set => ChangeAndNotify(value, ref _currentProgress);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // do time-consuming work here, calling ReportProgress as and when you can
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }


        public ICommand AnalyseAllDataCommand => new RelayCommand(AnalyseAllData, true);

        public ICommand StopCommand => new RelayCommand(Stop, true);

        public ICommand LoadPublicVideoCommand => new RelayCommand(LoadVideo, true);

        public ICommand AddExperimentCommand => new RelayCommand(AddExperiment, true);

        public void InitializeMyList()
        {
            HgdExperiments = new ObservableCollection<HgdExperiment>();
            HgdExperiments.Clear();
            HgdExperiments.Add(new HgdExperiment());
        }
        
        private void LoadVideo()
        {
            VideoPath = FileManager.OpenFileDialog();
            _hgdExperiment.VideoPath = VideoPath;
            _hgdExperiment.Video = new Video(VideoPath);
            _hgdExperiment.Video.ThumbnailImage.Resize(0.1);
            _hgdExperiment.Thumbnail = _hgdExperiment.Video.ThumbnailImage.BitMapImage;
            PublicImage = _hgdExperiment.Thumbnail;

            foreach (HgdExperiment hgdExperiment in HgdExperiments)
            {
                hgdExperiment.Thumbnail = PublicImage;
            }
        }

        private void AddExperiment()
        {
            HgdExperiments.Add(new HgdExperiment());
        }

        private void AnalyseAllData()
        {
            foreach(HgdExperiment hgdExperiment in HgdExperiments)
            {
                hgdExperiment.Analyse();
            }
        }

        private void Stop()
        {

        }
    }
};