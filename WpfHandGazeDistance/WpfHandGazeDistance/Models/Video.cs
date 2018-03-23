using System;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WpfHandGazeDistance.Helpers;

namespace WpfHandGazeDistance.Models
{
    /// <summary>
    /// Contains a string with the file path of the video source as well as a Emgu VideoCapture class
    /// to contain the video.
    /// </summary>
    public class Video
    {
        #region Private Properties
        
        private VideoCapture _capture;

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor simply calls the VideoSource property setter which instantiates a new video capture.
        /// </summary>
        /// <param name="videoPath"></param>
        public Video(string videoPath = null)
        {
            if (videoPath == null) return;

            _capture = new VideoCapture(videoPath);
        }

        #endregion

        #region Public Members

        public Mat GetMatFrame()
        {
            return _capture.QueryFrame();
        }

        public Image<Bgr, byte> GetImageFrame()
        {
            Mat frameMat = GetMatFrame();
            return frameMat.ToImage<Bgr, byte>();
        }

        /// <summary>
        /// Queries the first image of the video and returns it as a BitmapSource which can be displayed
        /// in the View.
        /// </summary>
        /// <returns></returns>
        public BitmapSource GetBitmapFrame()
        {
            Mat frame = GetMatFrame();
            return BitMapConverter.ToBitmapSource(frame);
        }

        public int NumberOfFrames()
        {
            return (int)Math.Floor(_capture.GetCaptureProperty(CapProp.FrameCount));
        }

        public double GetFps()
        {
            return _capture.GetCaptureProperty(CapProp.Fps);
        }

        #endregion
    }
}
