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
                Mat handsImage = FindHands(Video.GetMatFrame());
                Tuple<float, float> gazeCoordinates = new Tuple<float, float>(BeGazeData.XGaze[i], BeGazeData.YGaze[i]);

                float distance = MeasureDistance(handsImage, gazeCoordinates);
                HgdData.RawData.Add(distance);
            }

            return HgdData;
        }

        private Mat FindHands(Mat inputImage)
        {
            throw new NotImplementedException();

            var segmentedImage = ColorSegment(inputImage);
            var contours = FindContours(segmentedImage);

        }

        private Image<Gray, byte> ColorSegment(Mat inputImage)
        {
            throw new NotImplementedException();
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

        private float MeasureDistance(Mat handsImage, Tuple<float, float> gazeCoordinates)
        {
            throw new NotImplementedException();
        }
    }
}
