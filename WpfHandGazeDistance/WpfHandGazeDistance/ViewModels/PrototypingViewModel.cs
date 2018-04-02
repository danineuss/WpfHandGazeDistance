using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class PrototypingViewModel : BaseViewModel
    {
        #region Private Properties

        private string _imagePath;

        private string _videoPath;

        private string _beGazePath;

        private string _hgdPath;

        private Video _video;

        private Image<Bgr, byte> _inputImage;

        private Image<Gray, byte> _outputImage;

        private BitmapSource _inputBitmap;

        private BitmapSource _outputBitmap;

        private int _numberOfHands;

        private float _distance;

        private int _currentFrameCount;

        private int _maxFrameCount;

        private int _frameStep = 60;

        private BeGazeData _beGazeData;

        private HgdData _hgdData;

        private float _videoDuration;

        #endregion

        #region Public Properties

        public HgdViewModel HgdViewModel { get; set; }
        
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                ChangeAndNotify(value, ref _imagePath);
                InputImage = new Image<Bgr, byte>(_imagePath);
            }
        }

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                ChangeAndNotify(value, ref _videoPath);
                Video = new Video(_videoPath);
            }
        }

        //public string BeGazePath
        //{
        //    get => _beGazePath;
        //    set => ChangeAndNotify(value, ref _beGazePath);
        //}

        public string HgdPath
        {
            get => _hgdPath;
            set => ChangeAndNotify(value, ref _hgdPath);
        }

        public Video Video
        {
            get => _video;
            set
            {
                ChangeAndNotify(value, ref _video);
                MaxFrameCount = Video.FrameCount;
                InputImage = _video.GetImageFrame();
            }
        }

        public Image<Bgr, byte> InputImage
        {
            get => _inputImage;
            set
            {
                ChangeAndNotify(value, ref _inputImage);
                InputBitmap = BitMapConverter.ToBitmapSource(_inputImage);
            }
        }

        public Image<Gray, byte> OutputImage
        {
            get => _outputImage;
            set
            {
                ChangeAndNotify(value, ref _outputImage);
                OutputBitmap = BitMapConverter.ToBitmapSource(_outputImage);
            }
        }

        public BitmapSource InputBitmap
        {
            get => _inputBitmap;
            set => ChangeAndNotify(value, ref _inputBitmap);
        }

        public BitmapSource OutputBitmap
        {
            get => _outputBitmap;
            set => ChangeAndNotify(value, ref _outputBitmap);
        }

        public int NumberOfHands
        {
            get => _numberOfHands;
            set => ChangeAndNotify(value, ref _numberOfHands);
        }

        public float Distance
        {
            get => _distance;
            set => ChangeAndNotify(value, ref _distance);
        }

        public int CurrentFrameCount
        {
            get => _currentFrameCount;
            set => ChangeAndNotify(value, ref _currentFrameCount);
        }

        public int MaxFrameCount
        {
            get => _maxFrameCount;
            set => ChangeAndNotify(value, ref _maxFrameCount);
        }

        public BeGazeData BeGazeData
        {
            get => _beGazeData;
            set => ChangeAndNotify(value, ref _beGazeData);
        }

        public HgdData HgdData
        {
            get => _hgdData;
            set => ChangeAndNotify(value, ref _hgdData);
        }

        public float VideoDuration
        {
            get => _videoDuration;
            set => ChangeAndNotify(value, ref _videoDuration);
        }

        #endregion

        #region Constructor

        public PrototypingViewModel()
        {
            NumberOfHands = 0;
            VideoDuration = 10f;
        }

        #endregion

        #region Commands

        public ICommand LoadImageCommand => new RelayCommand(LoadImage, true);

        //public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        //public ICommand LoadBeGazeCommand => new RelayCommand(LoadBeGaze, true);

        //public ICommand SetSavePathCommand => new RelayCommand(SetSavePath, true);

        public ICommand AnalyseImageCommand => new RelayCommand(AnalyseImage, true);

        public ICommand NextImageCommand => new RelayCommand(NextImage, true);

        //public ICommand AnalyseDataCommand => new RelayCommand(AnalyseRawDistance, true);

        public ICommand CutVideoCommand => new RelayCommand(CutVideo, true);

        public ICommand LoadHgdCommand => new RelayCommand(LoadHgd, true);

        public ICommand SaveHgdCommand => new RelayCommand(SaveHgd, true);

        #endregion

        private void LoadImage()
        {
            ImagePath = FileDialog.OpenFileDialog();
        }

        //private void LoadVideo()
        //{
        //    VideoPath = FileDialog.OpenFileDialog();
        //}

        //private void LoadBeGaze()
        //{
        //    BeGazePath = FileDialog.OpenFileDialog();
        //    BeGazeData = new BeGazeData(BeGazePath);
        //}

        private void SetSavePath()
        {
            HgdPath = FileDialog.SaveFileDialog();
        }

        private void NextImage()
        {
            if (VideoPath == null) return;

            for (int i = 0; i < _frameStep; i++)
            {
                Video.GetMatFrame();
            }

            CurrentFrameCount += _frameStep;
            InputImage = Video.GetImageFrame();
        }

        private void AnalyseImage()
        {
            OutputImage = HandDetector.AnalyseImage(InputImage);
            NumberOfHands = HandDetector.FindHands(InputImage).Size;
            OutputImage = HandDetector.AnalyseImage(InputImage);
            Distance = HandDetector.MeasureHgd(InputImage, new PointF(0, 0));
        }

        private void AnalyseRawDistance()
        {
            HandDetector handDetector = new HandDetector(BeGazeData, Video);

            HgdData = handDetector.MeasureRawHgd();

            SaveData();
        }

        private void SaveData()
        {
            HgdData.SaveData(HgdPath);
        }

        private void CutVideo()
        {
            VideoEditor videoEditor = new VideoEditor(VideoPath);
            videoEditor.CutVideo(HgdPath, 0f, VideoDuration);
        }

        private void LoadHgd()
        {
            HgdData = new HgdData();
            HgdData.LoadData(FileDialog.OpenFileDialog());
        }

        private void SaveHgd()
        {
            HgdData.SaveData(FileDialog.SaveFileDialog());
        }
    }
};