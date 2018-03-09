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
            VectorOfMat channels = new VectorOfMat(3);
            CvInvoke.Split(inputImage, channels);
            return outputImage;
        }

        private Image<Gray, byte> HsvSegment(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> outputImage = inputImage.Copy().Convert<Gray, byte>();
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
    }
}
