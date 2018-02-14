using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace WpfHandGazeDistance.Models
{
    /// <summary>
    /// Contains a string with the file path of the video source as well as a Emgu VideoCapture class
    /// to contain the video.
    /// </summary>
    public class Video
    {
        #region Private Properties

        private string _videoSource;

        private VideoCapture _capture;

        #endregion

        #region Public Properties

        /// <summary>
        /// Automatically creates a new VideoCapture when the video source is set.
        /// </summary>
        public string VideoSource
        {
            get => _videoSource;
            set
            {
                _videoSource = value;
                _capture = new VideoCapture(value);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor simply calls the VideoSource property setter which instantiates a new video capture.
        /// </summary>
        /// <param name="videoSource"></param>
        public Video(string videoSource = null)
        {
            if (videoSource != null)
            {
                VideoSource = videoSource;
            }
        }

        #endregion

        /// <summary>
        /// Queries the first image of the video and returns it as a Mat.
        /// </summary>
        /// <returns>Mat containing the first frame.</returns>
        public Mat GetMat()
        {
            Mat frame = _capture.QueryFrame();
            return frame;
        }

        /// <summary>
        /// Queries the first image of the video and returns it as a BitmapSource which can be displayed
        /// in the View.
        /// </summary>
        /// <returns>BitmapSource containing the first frame.</returns>
        public BitmapSource GetBitmap()
        {
            return BitMapConverter.ToBitmapSource(GetMat());
        }

        public double NumberOfFrames()
        {
            return _capture.GetCaptureProperty(CapProp.FrameCount);
        }
    }
}
