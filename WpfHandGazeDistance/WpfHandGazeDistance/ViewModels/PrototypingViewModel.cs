using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
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

        private Image<Bgr, byte> _inputImage;

        private Image<Gray, byte> _outputImage;

        private BitmapSource _inputBitmap;

        private BitmapSource _outputBitmap;

        private int _imageIndex;

        private int _numberOfHands;

        private float _distance;

        #endregion

        #region Public Properties

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                ChangeAndNotify(value, ref _imagePath);
                Mat image = new Mat(_imagePath);
                InputBitmap = BitMapConverter.ToBitmapSource(image);
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

        public int ImageIndex
        {
            get => _imageIndex;
            set
            {
                ChangeAndNotify(value, ref _imageIndex);
                AnalyseImage();
            }
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

        #endregion

        public PrototypingViewModel()
        {
            NumberOfHands = 0;
        }

        public ICommand LoadImageCommand => new RelayCommand(LoadImage, true);

        public ICommand AnalyseCommand => new RelayCommand(AnalyseImage, true);

        public ICommand AddCommand => new RelayCommand(AddIndex, true);

        public ICommand SubtractCommand => new RelayCommand(SubtractIndex, true);


        private void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                _inputImage = new Image<Bgr, byte>(ImagePath);
            }
        }

        private void AnalyseImage()
        {
            ColorSegment();
            OutputImage = Erode(_outputImage);
            VectorOfVectorOfPoint sortedContours = FindHands(_outputImage);
            Distance = MeasureDistance(sortedContours, new PointF(0, 0));
            Debug.Print(Distance.ToString());
        }

        private float MeasureDistance(VectorOfVectorOfPoint handContours, PointF gazeCoordinates)
        {
            if (handContours.Size == 0) return -1.0f;

            List<double> distances = new List<double>();
            for (int i = 0; i < handContours.Size; i++)
            {
                double distance = Math.Abs(CvInvoke.PointPolygonTest(handContours[i], gazeCoordinates, true));
                distances.Add(distance);
            }

            return (float)distances.Min();
        }

        private void ColorSegment()
        {
            Image<Bgr, byte> inputImage = new Image<Bgr, byte>(ImagePath);
            Image<Gray, byte> minimumSegment = MinimumSegment();
            Image<Gray, byte> hsvSegment = HsvSegment();

            Image<Gray, byte> segmentedImage = new Image<Gray, byte>(inputImage.Size);
            CvInvoke.BitwiseAnd(minimumSegment, hsvSegment, segmentedImage);

            _outputImage = segmentedImage;
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

        private Image<Gray, byte> Erode(Image<Gray, byte> inputImage, int iterations = 3)
        {
            Image<Gray, byte> erodedImage = new Image<Gray, byte>(inputImage.Size);

            Mat kernelMat = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Erode(inputImage, erodedImage, kernelMat, new Point(-1, -1), iterations, BorderType.Default, CvInvoke.MorphologyDefaultBorderValue);

            return erodedImage;
        }

        private VectorOfVectorOfPoint FindHands(Image<Gray, byte> inputImage, int pixelThreshold = 10000, int numberOfContours = 2)
        {
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfVectorOfPoint sortedContours = new VectorOfVectorOfPoint();
            Mat hierarchyMat = new Mat();

            CvInvoke.FindContours(inputImage, contours, hierarchyMat, RetrType.Tree, ChainApproxMethod.ChainApproxNone);

            if (contours.Size > 0)
            {
                Dictionary<VectorOfPoint, double> contourDict = new Dictionary<VectorOfPoint, double>();
                for (int i = 0; i < contours.Size; i++)
                {
                    double contourArea = CvInvoke.ContourArea(contours[i]);
                    contourDict.Add(contours[i], contourArea);
                }

                var orderedDict = contourDict.OrderByDescending(area => area.Value).TakeWhile(area => area.Value > pixelThreshold);
                if (orderedDict.Count() > numberOfContours) orderedDict = orderedDict.Take(numberOfContours);
                foreach (var contour in orderedDict) sortedContours.Push(contour.Key);

                var singleContour = new Image<Gray, byte>(inputImage.Size);
                CvInvoke.DrawContours(singleContour, sortedContours, ImageIndex > sortedContours.Size - 1 ? sortedContours.Size - 1 : ImageIndex, new MCvScalar(255), -1);
                OutputImage = singleContour;

                NumberOfHands = sortedContours.Size;
            }

            return sortedContours;
        }

        private void AddIndex()
        {
            ImageIndex += 1;
        }

        private void SubtractIndex()
        {
            ImageIndex -= 1;
        }
    }
};