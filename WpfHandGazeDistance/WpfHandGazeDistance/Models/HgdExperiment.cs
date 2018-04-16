using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.Models
{
    public class HgdExperiment : BaseViewModel
    {
        private BitmapSource _thumbnail;

        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            set => ChangeAndNotify(value, ref _thumbnail);
        }

        public Video Video { get; set; }

        public string VideoPath { get; set; }

        public string BeGazePath { get; set; }

        public string OutputPath { get; set; }

        public bool HgdFlags { get; set; }

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public void LoadVideo()
        {

        }
    }
}
