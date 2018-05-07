using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Office.Core;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Properties

        private string _videoPath;

        private string _beGazePath;

        private string _hgdPath;

        private string _snippetsPath;

        private VideoEditor _videoEditor;

        #endregion

        #region Public Properties

        public VideoViewModel VideoViewModel { get; set; }

        public BeGazeViewModel BeGazeViewModel { get; set; }

        public HgdViewModel HgdViewModel { get; set; }

        public PrototypingViewModel PrototypingViewModel { get; }

        public ParametersViewModel ParametersViewModel { get; set; }

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                ChangeAndNotify(value, ref _videoPath);
                VideoViewModel = new VideoViewModel(value);
                _videoEditor = new VideoEditor(value);
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
                HgdViewModel = new HgdViewModel(BeGazeViewModel.BeGazeData, VideoViewModel.Video, 
                    ParametersViewModel.ParameterList);
            }
        }

        public string SnippetsPath
        {
            get => _snippetsPath;
            set => ChangeAndNotify(value, ref _snippetsPath);
        }

        //public bool ReadyToAnalyse
        //{
        //    get => VideoViewModel.ReadyToAnalyse && BeGazeViewModel.ReadyToAnalyse;
        //}

        #endregion

        public MainViewModel()
        {
            PrototypingViewModel = new PrototypingViewModel();
            ParametersViewModel = new ParametersViewModel();
            HgdViewModel = new HgdViewModel(ParametersViewModel.ParameterList);
        }

        #region Commands

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public ICommand LoadBeGazeCommand => new RelayCommand(LoadBeGaze, true);

        public ICommand LoadHgdCommand => new RelayCommand(LoadHgd, true);

        public ICommand SetHgdPathCommand => new RelayCommand(SetHgdPath, true);

        public ICommand SetSnippetsPathCommand => new RelayCommand(SetSnippetsPath, true);

        public ICommand AnalyseDataCommand => new RelayCommand(AnalyseData, true);

        public ICommand CutSnippetsCommand => new RelayCommand(CutSnippets, true);

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

        private void SetSnippetsPath()
        {
            string folderPath = FileManager.SelectFolderDialog();
            SnippetsPath = folderPath + "\\";
        }

        private void AnalyseData()
        {
            HgdViewModel.AnalyseData();
            HgdViewModel.SaveHgd(HgdPath);
            CutSnippets();
        }

        private void CutSnippets()
        {
            if (SnippetsPath != null && HgdViewModel.HgdData != null)
            {
                _videoEditor.CutSnippets(SnippetsPath, HgdViewModel.HgdData);
            }
        }
    }
}
