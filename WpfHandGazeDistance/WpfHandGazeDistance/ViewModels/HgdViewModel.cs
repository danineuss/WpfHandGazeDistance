using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class HgdViewModel : BaseViewModel
    {
        #region Private Properties

        private const float Fps = 60;

        private int _longActionCount;

        private int _stdDevPeriod;

        private int _bufferLength;

        private int _medianPeriod;

        private int _stdDevThreshold;

        private HandDetector _handDetector;

        private VideoEditor _videoEditor;

        private int _progress;

        private BitmapSource _thumbnail;

        private string _shortVideoPath = "Select";

        private string _shortBeGazePath = "Select";

        private string _shortFolderPath = "Select";

        private PrototypingViewModel _prototypingViewModel;

        #endregion

        #region Public Properties

        public ObservableCollection<Parameter> ParameterList;

        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            set => ChangeAndNotify(value, ref _thumbnail);
        }

        public Video Video { get; set; }

        public HgdData HgdData;

        public BeGazeData BeGazeData;

        public string VideoPath;

        public string BeGazePath;

        public string FolderPath;

        public string OutputPath;

        public string ShortVideoPath
        {
            get => _shortVideoPath;
            set => ChangeAndNotify(value, ref _shortVideoPath);
        }

        public string ShortBeGazePath
        {
            get => _shortBeGazePath;
            set => ChangeAndNotify(value, ref _shortBeGazePath);
        }

        public string ShortFolderPath
        {
            get => _shortFolderPath;
            set => ChangeAndNotify(value, ref _shortFolderPath);
        }

        public int Progress
        {
            get => _progress;
            set => ChangeAndNotify(value, ref _progress);
        }

        #endregion

        #region Constructors

        public HgdViewModel(ObservableCollection<Parameter> parameterList, PrototypingViewModel prototypingViewModel)
        {
            HgdData = new HgdData();

            ParameterList = parameterList;
            _longActionCount = (int)(ParameterList[0].Value * Fps);
            _stdDevPeriod = (int)(ParameterList[1].Value * Fps);
            _bufferLength = (int)(ParameterList[2].Value * Fps);
            _medianPeriod = (int)ParameterList[3].Value;
            _stdDevThreshold = (int)ParameterList[4].Value;

            _prototypingViewModel = prototypingViewModel;
        }

        #endregion

        #region Commands

        public ICommand AnalyseCommand => new RelayCommand(AnalyseData, true);

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public ICommand LoadBeGazeCommand => new RelayCommand(LoadBeGaze, true);

        public ICommand SetFolderPathCommand => new RelayCommand(SetFolderPath, true);

        public ICommand RemoveCommand => new RelayCommand(RemoveHgdViewModel, true);

        #endregion

        #region Public Members

        public void LoadHgd(string loadPath)
        {
            HgdData = new HgdData();
            if (loadPath != null)
            {
                HgdData.LoadData(loadPath);
                HgdData.RecordingTime = HgdManipulations.RecordingTimeFromConstant(HgdData.RecordingTime.Count, Fps);
            }
        }

        public void SaveHgd()
        {
            string savePath = FileManager.SaveFileDialog();
            if (savePath != null) HgdData.SaveData(savePath);
        }

        public void SaveHgd(string savePath)
        {
            HgdData.SaveData(savePath);
        }

        public void AnalyseData()
        {
            _handDetector = new HandDetector(BeGazeData, Video);
            HgdData = _handDetector.MeasureRawHgd();
            HgdData.RecordingTime = HgdManipulations.RecordingTimeFromVideo(_handDetector);
            HgdData.RawDistance = HgdManipulations.PruneValues(HgdData.RawDistance);
            HgdData.MedianDistance = HgdManipulations.MovingMedian(HgdData.RawDistance, _medianPeriod);
            HgdData.LongActions = HgdManipulations.LowPass(HgdData.MedianDistance, _longActionCount);
            HgdData.StandardDeviation = HgdManipulations.MovingStdDev(HgdData.LongActions, _stdDevPeriod);
            HgdData.RigidActions = HgdManipulations.Threshold(HgdData.StandardDeviation, _stdDevThreshold);
            HgdData.UsabilityIssues = HgdManipulations.ConvertToBinary(HgdData.RigidActions);
            HgdData.BufferedUsabilityIssues = HgdManipulations.Buffer(
                HgdData.UsabilityIssues, _stdDevPeriod, _bufferLength);

            SaveHgd(OutputPath);
            _videoEditor = new VideoEditor(VideoPath);
            _videoEditor.CutSnippets(FolderPath, HgdData);
        }

        #endregion

        #region Private Members

        private void LoadVideo()
        {
            VideoPath = FileManager.OpenFileDialog(".avi");
            if (VideoPath != null)
            {
                Video = new Video(VideoPath);
                //Video.ThumbnailImage.Resize(0.25);
                Thumbnail = Video.ThumbnailImage.BitMapImage;

                ShortVideoPath = ShortenPath(VideoPath);
            }
        }

        private void LoadBeGaze()
        {
            BeGazePath = FileManager.OpenFileDialog(".txt");
            if (BeGazePath != null)
            {
                BeGazeData = new BeGazeData(BeGazePath);

                ShortBeGazePath = ShortenPath(BeGazePath);
            }
        }

        private void SetFolderPath()
        {
            FolderPath = FileManager.SelectFolderDialog() + @"\";
            if (FolderPath != null)
            {
                ShortFolderPath = ShortenPath(FolderPath);

                string fileName = Path.GetFileNameWithoutExtension(VideoPath.Substring(VideoPath.LastIndexOf("\\") + 1));
                OutputPath = FolderPath + fileName + ".csv";
            }
        }

        private void RemoveHgdViewModel()
        {
            _prototypingViewModel.RemoveHgdViewModel(this);
        }

        private string ShortenPath(string inputPath)
        {
            string fileName = inputPath.Substring(inputPath.LastIndexOf("\\") + 1);
            inputPath = inputPath.Remove(inputPath.LastIndexOf("\\"));
            string parentFolder = inputPath.Substring(inputPath.LastIndexOf("\\") + 1);

            return @"...\" + parentFolder + @"\" + fileName;
        }

        #endregion
    }
}
