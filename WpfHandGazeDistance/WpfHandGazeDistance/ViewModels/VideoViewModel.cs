using System.Windows.Input;
using Microsoft.Win32;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class VideoViewModel : BaseViewModel
    {
        #region Private Properties

        private string _videoPath;

        private Video _video;

        private VideoEditor _videoEditor;

        #endregion

        #region Public Properties

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                ChangeAndNotify(value, ref _videoPath);
                _video = new Video(value);
                _videoEditor = new VideoEditor(value);
            }
        }

        public Video Video
        {
            get => _video;
        }

        public bool ReadyToAnalyse => _videoPath != null;

        #endregion

        #region Constructor

        public VideoViewModel(string videoPath)
        {
            VideoPath = videoPath;
        }

        #endregion

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