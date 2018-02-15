using System.Windows.Input;
using Microsoft.Win32;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class VideoViewModel : BaseViewModel
    {
        private string _videoPath;

        private Video _video;

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                ChangeAndNotify(value, ref _videoPath);
                _video = new Video(value);
            }
        }

        public ICommand LoadCommand => new RelayCommand(LoadVideo, true);

        private void LoadVideo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                VideoPath = openFileDialog.FileName;
            }
        }
    }
}
