using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using WpfHandGazeDistance.Models;

namespace WpfHandGazeDistance.Helpers
{
    /// <summary>
    /// This is the Computer Vision class of the project. All image manipulation functionality is concentrated here.
    /// The class is only called in the HgdViewModel.Analyse() function in order to find the hands in the image.
    /// 
    /// The individual functions and its purposes/reasoning are detailed below.
    /// 
    /// This is not a static class, so when you need its services you first have to instatiate it, so that is has
    /// all the required data for performing its job:     
    ///     HandDetector handDetector = new HandDetector(...)
    /// </summary>
    public class HandDetector
    {
        #region Private Properties

        private const int PixelThreshold = 10000;

        private const int NumberOfContours = 2;

        private BackgroundWorker _backgroundWorker;

        #endregion

        #region Public Properties

        public BeGazeData BeGazeData { get; }

        public Video Video { get; }

        public HgdData HgdData { get; }

        public int Progress;

        public bool StopBool;

        #endregion

        #region Constructor

        public HandDetector(BeGazeData beGazeData, Video video, BackgroundWorker backgroundWorker)
        {
            BeGazeData = beGazeData;
            Video = video;
            _backgroundWorker = backgroundWorker;
            HgdData = new HgdData();

            StopBool = false;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// This will measure the distance between the gaze point and the closest hand for each frame in the video.
        /// For each time step the hands are detected and compared to the (x, y)-coordinates of the gaze. The
        /// distance between the two is recorded and stored in the HgdData.
        /// </summary>
        /// <returns></returns>
        public HgdData MeasureRawHgd()
        {
            StopBool = false;

            List<float> rawDistance = new List<float>();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            for (int index = 0; index < Video.FrameCount; index++)
            {
                if (StopBool) break;

                PointF coordinates = BeGazeData.GetCoordinatePoint(index);
                Image<Bgr, byte> frame = Video.GetBgrImageFrame();

                float distance = float.NaN;
                if (frame != null)
                {
                    distance = MeasureHgd(frame, coordinates);
                    frame.Dispose();
                }

                rawDistance.Add(distance);

                Progress = Convert.ToInt32(((double) index / Video.FrameCount) * 100);

                if (Progress % 2 == 0)
                {
                    _backgroundWorker.ReportProgress(Progress);
                }
            }

            HgdData.RawDistance = rawDistance;
            return HgdData;
        }

        /// <summary>
        /// This function will segment, erode and filter a BGR image to find the hands
        /// and then return the hand contours as white contours on a black background.
        /// This is useful for testing or demonstrating the hand detection algorithm.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <returns>A grayscale image with the hands as white.</returns>
        public static Image<Gray, byte> AnalyseImage(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(inputImage.Size);
            VectorOfVectorOfPoint handContours = FindHands(inputImage);
            CvInvoke.DrawContours(outputImage, handContours, -1, new MCvScalar(255), -1);

            handContours.Dispose();
            return outputImage;
        }

        /// <summary>
        /// This will find the hands and measure the distance from the top-left corner of the image.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <param name="point">The distance to the hand is measured from this point. In (x, y) coordinates from the top left.</param>
        /// <returns>The distance between the top-left corner and the closest hand (in pixels).</returns>
        public static float MeasureHgd(Image<Bgr, byte> inputImage, PointF point)
        {
            VectorOfVectorOfPoint largestContours = FindHands(inputImage);
            float distance = MeasureDistance(largestContours, point);

            largestContours.Dispose();
            return distance;
        }

        /// <summary>
        /// This function will segment and erode a BGR image into a grayscale image with the hands
        /// and then figure out the two largest contours above a certain size within that image.
        /// By querying handContours.Size you can figure out the number of hands in the image.
        /// </summary>
        /// <param name="inputImage">Standard BGR image</param>
        /// <returns>Vector of points with the two largest contours.</returns>
        public static VectorOfVectorOfPoint FindHands(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> segmentedImage = ColorSegment(inputImage);
            Image<Gray, byte> outputImage = Erode(segmentedImage);
            VectorOfVectorOfPoint handContours = LargestContours(outputImage);

            segmentedImage.Dispose();
            outputImage.Dispose();
            return handContours;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The two color segmentations ('Minimum' and 'HSV') are combined to give a better prediction.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <returns>A grayscale image with skin being white pixels.</returns>
        private static Image<Gray, byte> ColorSegment(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> minimumSegment = MinimumSegment(inputImage);
            Image<Gray, byte> hsvSegment = HsvSegment(inputImage);

            Image<Gray, byte> segmentedImage = new Image<Gray, byte>(inputImage.Size);
            CvInvoke.BitwiseAnd(minimumSegment, hsvSegment, segmentedImage);

            minimumSegment.Dispose();
            hsvSegment.Dispose();
            return segmentedImage;
        }

        /// <summary>
        /// This function compares the separte Blue, Green and Red values of each pixel and, using a clever
        /// subtraction, tries to determine if it is skin-colored. The idea is taken from this paper:
        /// "In-air gestures around unmodified mobile devices" by Song et al.
        /// </summary>
        /// <param name="inputImage">Standard BGR image.</param>
        /// <returns>Grayscale image with the white pixels containing skin.</returns>
        private static Image<Gray, byte> MinimumSegment(Image<Bgr, byte> inputImage)
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

            bgrChannels.Dispose();
            mixedMat.Dispose();
            return outputImage;
        }

        /// <summary>
        /// This function will use thresholds on the Hue value in the Hue-Saturation-Value (HSV) color space to find caucasian skin within an image.
        /// It will then return a grayscale image with the hand-containing pixels colored white.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <returns>Grayscale image with white pixels containing white skin.</returns>
        private static Image<Gray, byte> HsvSegment(Image<Bgr, byte> inputImage)
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

            hsvImage.Dispose();
            lowerThreshold.Dispose();
            upperThreshold.Dispose();
            return outputImage;
        }

        /// <summary>
        /// This function will erode a grayscale image using a standard rectangular kernel, five by five.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <param name="iterations">How often the image is eroded. Standard value is 3.</param>
        /// <returns></returns>
        private static Image<Gray, byte> Erode(IImage inputImage, int iterations = 3)
        {
            Image<Gray, byte> erodedImage = new Image<Gray, byte>(inputImage.Size);

            Mat kernelMat = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Erode(inputImage, erodedImage, kernelMat, new Point(-1, -1), iterations, BorderType.Default, CvInvoke.MorphologyDefaultBorderValue);

            kernelMat.Dispose();
            return erodedImage;
        }

        /// <summary>
        /// This function will dilate a grayscale image using a standard rectangular kernel, five by five.
        /// </summary>
        /// <param name="inputImage">A standard BGR image.</param>
        /// <param name="iterations">How often the image is dilated. Standard value is 3.</param>
        private static Image<Gray, byte> Dilate(IImage inputImage, int iterations = 3)
        {
            Image<Gray, byte> dilatedImage = new Image<Gray, byte>(inputImage.Size);

            Mat kernelMat = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Dilate(inputImage, dilatedImage, kernelMat, new Point(-1, -1), iterations, BorderType.Default, CvInvoke.MorphologyDefaultBorderValue);

            kernelMat.Dispose();
            return dilatedImage;
        }

        /// <summary>
        /// This function will take a segmented grayscale image and the likeliest candidates for hands.
        /// They are chosen as the two largest contours with a size of at least 10'000 pixels.
        /// </summary>
        /// <param name="inputImage">Already segmented grayscale image.</param>
        /// <param name="pixelThreshold">Number of pixels required to be counted as a hand.</param>
        /// <param name="numberOfContours">The n largest contours which will be picked from the list.</param>
        /// <returns>Vector of contours</returns>
        public static VectorOfVectorOfPoint LargestContours(Image<Gray, byte> inputImage, 
            int pixelThreshold = PixelThreshold, int numberOfContours = NumberOfContours)
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
            }

            hierarchyMat.Dispose();
            contours.Dispose();
            return sortedContours;
        }

        /// <summary>
        /// This function measures the minimum distance between any point and the hand contours.
        /// Hereby, the shortest of the distances is picked as the relevant one.
        /// </summary>
        /// <param name="handContours">The hand contour[s] as vector of vector of points.</param>
        /// <param name="point">The desired [gaze] point</param>
        /// <returns>Distance in (float) number of pixels.</returns>
        private static float MeasureDistance(VectorOfVectorOfPoint handContours, PointF point)
        {
            if (handContours.Size == 0) return Single.NaN;

            List<double> distances = new List<double>();
            for (int i = 0; i < handContours.Size; i++)
            {
                double distance = Math.Abs(CvInvoke.PointPolygonTest(handContours[i], point, true));
                distances.Add(distance);
            }

            return (float)distances.Min();
        }

        #endregion
    }
}
