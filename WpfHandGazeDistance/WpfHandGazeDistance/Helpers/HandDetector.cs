using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using WpfHandGazeDistance.Models;

namespace WpfHandGazeDistance.Helpers
{
    public class HandDetector
    {
        public BeGazeData BeGazeData { get; }

        public Video Video { get; }

        public HgdData HgdData { get; }

        public HandDetector(BeGazeData beGazeData, Video video)
        {
            BeGazeData = beGazeData;
            Video = video;
            HgdData = new HgdData();
        }

        public HgdData AnalyseData()
        {
            var lengthBeGaze = BeGazeData.RecordingTime.Count;
            var lengthVideo = Video.NumberOfFrames();

            if (lengthVideo != lengthBeGaze) throw new FormatException("Video and BeGaze Data not of same length.");

            throw new NotImplementedException();

            for (var i = 0; i < lengthBeGaze; i++)
            {
                Image<Bgr, byte> handsImage = FindHands(Video.GetImageFrame());
                Tuple<float, float> gazeCoordinates = new Tuple<float, float>(BeGazeData.XGaze[i], BeGazeData.YGaze[i]);

                float distance = MeasureDistance(handsImage, gazeCoordinates);
                HgdData.RawData.Add(distance);
            }

            return HgdData;
        }

        private Image<Bgr, byte> FindHands(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> handsContourImage = new Image<Bgr, byte>(inputImage.Size);

            var segmentedImage = ColorSegment(inputImage);
            var contours = FindHands(segmentedImage);

            for (int i = 0; i < contours.Size; i++)
            {
                CvInvoke.DrawContours(handsContourImage, contours, i, new MCvScalar(255), -1);
            }

            return handsContourImage;
        }

        /// <summary>
        /// The two color segmentations ('Minimum' and 'HSV') are combined to give a better prediction.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <returns>A grayscale image with skin being white pixels.</returns>
        private Image<Gray, byte> ColorSegment(Image<Bgr, byte> inputImage)
        {
            var minimumSegment = MinimumSegment(inputImage);
            var hsvSegment = HsvSegment(inputImage);

            Image<Gray, byte> segmentedImage = new Image<Gray, byte>(inputImage.Size);
            CvInvoke.BitwiseAnd(minimumSegment, hsvSegment, segmentedImage);

            return segmentedImage;
        }

        /// <summary>
        /// This function compares the separte Blue, Green and Red values of each pixel and, using a clever
        /// subtraction, tries to determine if it is skin-colored. The idea is taken from this paper:
        /// "In-air gestures around unmodified mobile devices" by Song et al.
        /// </summary>
        /// <param name="inputImage">Standard BGR image.</param>
        /// <returns>Grayscale image with the white pixels containing skin.</returns>
        private Image<Gray, byte> MinimumSegment(Image<Bgr, byte> inputImage)
        {
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

        /// <summary>
        /// This function will use thresholds on the Hue value in the Hue-Saturation-Value (HSV) color space to find caucasian skin within an image.
        /// It will then return a grayscale image with the hand-containing pixels colored white.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <returns>Grayscale image with white pixels containing white skin.</returns>
        private Image<Gray, byte> HsvSegment(Image<Bgr, byte> inputImage)
        {
            var hsvImage = inputImage.Copy().Convert<Hsv, byte>();
            var outputImage = new Image<Gray, byte>(hsvImage.Size);

            var hsvThresholdOne = new Hsv(0, 0, 0);
            var hsvThresholdTwo = new Hsv(30, 255, 255);
            var hsvThresholdThree = new Hsv(160, 0, 0);
            var hsvThresholdFour = new Hsv(180, 255, 255);

            Image<Gray, byte> lowerThreshold = hsvImage.InRange(hsvThresholdOne, hsvThresholdTwo);
            Image<Gray, byte> upperThreshold = hsvImage.InRange(hsvThresholdThree, hsvThresholdFour);
            CvInvoke.BitwiseOr(lowerThreshold, upperThreshold, outputImage);

            return outputImage;
        }

        /// <summary>
        /// This function will erode a grayscale image using a standard rectangular kernel, five by five.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <param name="iterations">How often the image is eroded. Standard value is 3.</param>
        /// <returns></returns>
        private Image<Gray, byte> Erode(Image<Gray, byte> inputImage, int iterations=3)
        {
            Image<Gray, byte> erodedImage = new Image<Gray, byte>(inputImage.Size);

            Mat kernelMat = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Erode(inputImage, erodedImage, kernelMat, new Point(-1, -1), iterations, BorderType.Default, CvInvoke.MorphologyDefaultBorderValue);

            return erodedImage;
        }

        /// <summary>
        /// This function will dilate a grayscale image using a standard rectangular kernel, five by five.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <param name="iterations">How often the image is dilated. Standard value is 3.</param>
        private Image<Gray, byte> Dilate(Image<Gray, byte> inputImage, int iterations = 3)
        {
            Image<Gray, byte> dilatedImage = new Image<Gray, byte>(inputImage.Size);

            Mat kernelMat = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Dilate(inputImage, dilatedImage, kernelMat, new Point(-1, -1), iterations, BorderType.Default, CvInvoke.MorphologyDefaultBorderValue);

            return dilatedImage;
        }

        /// <summary>
        /// This function will take a segmented grayscale image and the likeliest candidates for hands.
        /// They are chosen as the two largest contours with a size of at least 10'000 pixels.
        /// </summary>
        /// <param name="inputImage">Already segmented grayscale image.</param>
        /// <param name="pixelThreshold">Number of pixels required to be counted as a hand.</param>
        /// <returns>Vector of contours</returns>
        public VectorOfVectorOfPoint FindHands(Image<Gray, byte> inputImage, int pixelThreshold = 10000)
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
                if (orderedDict.Count() > 2) orderedDict = orderedDict.Take(2);
                foreach (var contour in orderedDict) sortedContours.Push(contour.Key);
            }

            return sortedContours;
        }

        private float MeasureDistance(Image<Bgr, byte> handsImage, Tuple<float, float> gazeCoordinates)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This will split an image into its three color channels, storing them in a three-dimensional array of Mats.
        /// </summary>
        /// <param name="inputImage">Any 3-channel color image (HSV, BGR, ...)</param>
        /// <returns></returns>
        private VectorOfMat SplitChannels(IInputArray inputImage)
        {
            VectorOfMat channels = new VectorOfMat(3);
            CvInvoke.Split(inputImage, channels);
            return channels;
        }
    }
}
