using System;
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
            throw new NotImplementedException();

            var segmentedImage = ColorSegment(inputImage);
            var contours = FindContours(segmentedImage);

        }

        private Image<Gray, byte> ColorSegment(Image<Bgr, byte> inputImage)
        {
            throw new NotImplementedException();

            var minimumSegment = MinimumSegment(inputImage);
            var hsvSegment = HsvSegment(inputImage);
            Image<Gray, byte> segmentedImage = new Image<Gray, byte>(inputImage.Size);
            CvInvoke.BitwiseAnd(minimumSegment, hsvSegment, segmentedImage);
        }

        private Image<Gray, byte> MinimumSegment(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> outputImage = inputImage.Copy().Convert<Gray, byte>();
            VectorOfMat channels = SplitChannels(inputImage);
            return outputImage;
        }

        private Image<Gray, byte> HsvSegment(Image<Bgr, byte> inputImage)
        {
            Image<Hsv, byte> hsvImage = inputImage.Copy().Convert<Hsv, byte>();
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

        public static VectorOfVectorOfPoint FindContours(Image<Gray, byte> inputImage)
        {
            //This still needs work, at the moment it has a binary threshold, simply from the tutorial.
            Image<Gray, byte> outputImage = inputImage.Copy().ThresholdBinary(new Gray(100), new Gray(255));

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchyMat = new Mat();
            CvInvoke.FindContours(outputImage, contours, hierarchyMat, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            return contours;
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
