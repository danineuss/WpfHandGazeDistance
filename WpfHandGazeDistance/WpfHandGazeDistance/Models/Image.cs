using System.Drawing;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WpfHandGazeDistance.Helpers;

namespace WpfHandGazeDistance.Models
{
    /// <summary>
    /// This could at some point become the parent class of BgrImage and HsvImage etc.
    /// </summary>
    public class Image
    {
        public Image<Bgr, byte> BgrImage { get; set; }

        public Image<Gray, byte> GrayImage { get; set; }

        public BitmapSource BitMapImage { get; set; }

        public Image(IInputArray inputImage)
        {
            BgrImage = inputImage.GetInputArray().GetMat().ToImage<Bgr, byte>();
            GrayImage = inputImage.GetInputArray().GetMat().ToImage<Gray, byte>();
            BitMapImage = BitMapConverter.Convert(BgrImage.ToBitmap());
        }

        public void Resize(double scale)
        {
            BgrImage = BgrImage.Resize(scale, Inter.Linear);
            GrayImage = GrayImage.Resize(scale, Inter.Linear);
            BitMapImage = BitMapConverter.Convert(BgrImage.ToBitmap());
        }
    }
}
