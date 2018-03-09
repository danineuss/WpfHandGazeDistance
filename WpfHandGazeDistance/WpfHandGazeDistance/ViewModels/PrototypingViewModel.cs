using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
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

        private BitmapSource _inputImage;

        private BitmapSource _outputImage;

        private int _imageIndex;

        #endregion

        #region Public Properties

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

        public int ImageIndex
        {
            get => _imageIndex;
            set
            {
                ChangeAndNotify(value, ref _imageIndex);
                if (value == 0)
                {
                    ColorSegment();
                }
                else if (value == 1)
                {
                    OutputImage = BitMapConverter.ToBitmapSource(MinimumSegment());
                }
                else
                {
                    OutputImage = BitMapConverter.ToBitmapSource(HsvSegment());
                }
            }
        }

        #endregion

        public ICommand LoadImageCommand => new RelayCommand(LoadImage, true);

        public ICommand AnalyseCommand => new RelayCommand(AnalyseImage, true);

        public ICommand SegmentCommand => new RelayCommand(ColorSegment, true);

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

        private void ColorSegment()
        {
            Image<Bgr, byte> inputImage = new Image<Bgr, byte>(ImagePath);
            var minimumSegment = MinimumSegment();
            var hsvSegment = HsvSegment();

            Image<Gray, byte> segmentedImage = new Image<Gray, byte>(inputImage.Size);
            CvInvoke.BitwiseAnd(minimumSegment, hsvSegment, segmentedImage);

            OutputImage = BitMapConverter.ToBitmapSource(segmentedImage);
        }

        private Image<Gray, byte> MinimumSegment()
        {
            Image<Bgr, byte> inputImage = new Image<Bgr, byte>(ImagePath);
            Mat deltaOne = new Mat();
            Mat deltaTwo = new Mat();

            VectorOfMat bgrChannels = new VectorOfMat(3);
            CvInvoke.Split(inputImage, bgrChannels);
            CvInvoke.Subtract(bgrChannels[2], bgrChannels[1], deltaOne);
            CvInvoke.Subtract(bgrChannels[2], bgrChannels[0], deltaTwo);

            Mat mixedMat = new Mat();
            CvInvoke.Min(deltaOne, deltaTwo, mixedMat);

            Image<Gray, byte> outputImage = mixedMat.ToImage<Gray, byte>().InRange(new Gray(10), new Gray(200));
            return outputImage;
        }

        private Image<Gray, byte> HsvSegment()
        {
            Image<Hsv, byte> hsvImage = new Image<Hsv, byte>(ImagePath);
            Image<Gray, byte> outputImage = new Image<Gray, byte>(hsvImage.Size);

            Hsv hsvThresholdOne = new Hsv(0, 0, 0);
            Hsv hsvThresholdTwo = new Hsv(30, 255, 255);
            Hsv hsvThresholdThree = new Hsv(160, 0, 0);
            Hsv hsvThresholdFour = new Hsv(180, 255, 255);

            Image<Gray, byte> lowerThreshold = hsvImage.InRange(hsvThresholdOne, hsvThresholdTwo);
            Image<Gray, byte> upperThreshold = hsvImage.InRange(hsvThresholdThree, hsvThresholdFour);

            CvInvoke.BitwiseOr(lowerThreshold, upperThreshold, outputImage);
            return outputImage;
        }
    }
};