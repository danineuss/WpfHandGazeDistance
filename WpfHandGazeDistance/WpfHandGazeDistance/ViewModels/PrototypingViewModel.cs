using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
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

        #endregion

        #region Public Properties

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

        public Video Video
        {
            get => _video;
            set
            {
                ChangeAndNotify(value, ref _video);
                MaxFrameCount = Video.NumberOfFrames();
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

        #endregion

        #region Constructor

        public PrototypingViewModel()
        {
            NumberOfHands = 0;
        }

        #endregion

        #region Commands

        public ICommand LoadImageCommand => new RelayCommand(LoadImage, true);

        public ICommand AnalyseCommand => new RelayCommand(AnalyseImage, true);

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public ICommand NextImageCommand => new RelayCommand(NextImage, true);

        #endregion

        private void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        private void AnalyseImage()
        {
            OutputImage = HandDetector.AnalyseImage(InputImage);
            NumberOfHands = HandDetector.FindHands(InputImage).Size;
            OutputImage = HandDetector.AnalyseImage(InputImage);
            Distance = HandDetector.MeasureDistance(InputImage);
        }

        private void LoadVideo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                VideoPath = openFileDialog.FileName;
            }
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
    }
};