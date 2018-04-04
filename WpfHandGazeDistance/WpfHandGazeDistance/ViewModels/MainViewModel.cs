﻿using System.Windows.Input;
using Microsoft.Office.Core;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region

        private string _videoPath;

        private string _beGazePath;

        private string _hgdPath;

        #endregion

        #region Public Properties

        public VideoViewModel VideoViewModel { get; set; }

        public BeGazeViewModel BeGazeViewModel { get; set; }

        public HgdViewModel HgdViewModel { get; set; }

        public PrototypingViewModel PrototypingViewModel { get; }

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                ChangeAndNotify(value, ref _videoPath);
                VideoViewModel = new VideoViewModel(value);
                PrototypingViewModel.VideoPath = value;
            }
        }

        public string BeGazePath
        {
            get => _beGazePath;
            set
            {
                ChangeAndNotify(value, ref _beGazePath);
                BeGazeViewModel = new BeGazeViewModel(value);
            }
        }

        public string HgdPath
        {
            get => _hgdPath;
            set
            {
                ChangeAndNotify(value, ref _hgdPath);
                HgdViewModel = new HgdViewModel(BeGazeViewModel.BeGazeData, VideoViewModel.Video);
            }
        }

        //public bool ReadyToAnalyse
        //{
        //    get => VideoViewModel.ReadyToAnalyse && BeGazeViewModel.ReadyToAnalyse;
        //}

        #endregion

        public MainViewModel()
        {
            HgdViewModel = new HgdViewModel();
            PrototypingViewModel = new PrototypingViewModel();
        }

        #region Commands

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public ICommand LoadBeGazeCommand => new RelayCommand(LoadBeGaze, true);

        public ICommand LoadHgdCommand => new RelayCommand(LoadHgd, true);

        public ICommand SetHgdPathCommand => new RelayCommand(SetHgdPath, true);

        public ICommand AnalyseDataCommand => new RelayCommand(AnalyseData, true);

        #endregion

        private void LoadVideo()
        {
            string loadPath = FileManager.OpenFileDialog();
            if (loadPath != null && loadPath.EndsWith(".avi")) VideoPath = loadPath;
        }

        private void LoadBeGaze()
        {
            string loadPath = FileManager.OpenFileDialog();
            if (loadPath != null && loadPath.EndsWith(".txt")) BeGazePath = loadPath;
        }

        private void LoadHgd()
        {
            string loadPath = FileManager.OpenFileDialog();
            if (loadPath != null && loadPath.EndsWith(".csv")) HgdViewModel.LoadHgd(loadPath);
        }

        private void SetHgdPath()
        {
            string savePath = FileManager.SaveFileDialog();
            if (savePath != null && savePath.EndsWith(".csv")) HgdPath = savePath;
        }

        private void AnalyseData()
        {
            HgdViewModel.AnalyseData();
            HgdViewModel.SaveHgd(HgdPath);
        }
    }
}
