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
    public class VideoViewModel : BaseViewModel
    {
        #region Private Properties

        private string _videoPath;

        private Video _video;

        // Testing grounds start here:

        private string _imagePath;

        private BitmapSource _inputImage;

        private BitmapSource _outputImage;

        #endregion

        #region Public Properties

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                ChangeAndNotify(value, ref _videoPath);
                _video = new Video(value);
            }
        }

        public bool ReadyToAnalyse => _video != null;

        // Testing grounds start here:

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                ChangeAndNotify(value, ref _imagePath);
                Mat image = new Mat(_imagePath);
                InputImage = BitMapConverter.ToBitmapSource(image);
            }
        }

        public BitmapSource InputImage
        {
            get => _inputImage;
            set => ChangeAndNotify(value, ref _inputImage);
        }

        public BitmapSource OutputImage
        {
            get => _outputImage;
            set => ChangeAndNotify(value, ref _outputImage);
        }

        #endregion

        public ICommand LoadCommand => new RelayCommand(LoadVideo, true);

        private void LoadVideo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                VideoPath = openFileDialog.FileName;
            }
        }

        // Testing grounds start here:

        public ICommand LoadImageCommand => new RelayCommand(LoadImage, true);
        
        public ICommand AnalyseCommand => new RelayCommand(AnalyseImage, true);

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
            Image<Gray, byte> outputImage = new Image<Gray, byte>(_imagePath);
            var contours = HandDetector.FindContours(new Image<Gray, byte>(_imagePath));

            Image<Gray, byte> blackImage = new Image<Gray, byte>(outputImage.Width, outputImage.Height, new Gray(0));

            CvInvoke.DrawContours(blackImage, contours, -1, new MCvScalar(255, 0, 0));
            OutputImage = BitMapConverter.ToBitmapSource(blackImage);
        }
    }
}