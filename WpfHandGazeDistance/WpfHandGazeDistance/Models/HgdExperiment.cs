using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.Models
{
    public class HgdExperiment : BaseViewModel
    {
        #region Private Properties

        private BitmapSource _thumbnail;

        private string _shortVideoPath = "Select";

        private string _shortBeGazePath = "Select";

        private string _shortOutputPath = "Select";

        #endregion

        #region Public Properties

        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            set => ChangeAndNotify(value, ref _thumbnail);
        }

        public Video Video { get; set; }

        public HgdData HgdData;

        public BeGazeData BeGazeData;
        
        public string VideoPath;

        public string BeGazePath;

        public string OutputPath;

        public string ShortVideoPath
        {
            get => _shortVideoPath;
            set => ChangeAndNotify(value, ref _shortVideoPath);
        }

        public string ShortBeGazePath
        {
            get => _shortBeGazePath;
            set => ChangeAndNotify(value, ref _shortBeGazePath);
        }

        public string ShortOutputPath
        {
            get => _shortOutputPath;
            set => ChangeAndNotify(value, ref _shortOutputPath);
        }

        public int Progress { get; set; }

        public bool HgdFlags { get; set; }

        #endregion

        #region Commands

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public ICommand LoadBeGazeCommand => new RelayCommand(LoadBeGaze, true);

        public ICommand SetOutputPathCommand => new RelayCommand(SetOutputPath, true);

        public ICommand AnalyseCommand => new RelayCommand(Analyse, true);

        public ICommand StopCommand => new RelayCommand(Stop, true);

        #endregion

        #region Private Members

        private void LoadVideo()
        {
            VideoPath = FileManager.OpenFileDialog(".avi");
            if (VideoPath != null)
            {
                Video = new Video(VideoPath);
                Video.ThumbnailImage.Resize(0.12);
                Thumbnail = Video.ThumbnailImage.BitMapImage;

                ShortVideoPath = ShortenPath(VideoPath);
            }
        }

        private void LoadBeGaze()
        {
            BeGazePath = FileManager.OpenFileDialog(".txt");
            if (BeGazePath != null)
            {
                BeGazeData = new BeGazeData(BeGazePath);

                ShortBeGazePath = ShortenPath(BeGazePath);
            }
        }

        private void SetOutputPath()
        {
            OutputPath = FileManager.SelectFolderDialog();
            if (OutputPath != null)
            {
                ShortOutputPath = ShortenPath(OutputPath) + @"\";
            }
        }

        private void Analyse()
        {

        }

        private void Stop()
        {

        }

        private string ShortenPath(string inputPath)
        {
            string fileName = inputPath.Substring(inputPath.LastIndexOf("\\") + 1);
            inputPath = inputPath.Remove(inputPath.LastIndexOf("\\"));
            string parentFolder = inputPath.Substring(inputPath.LastIndexOf("\\") + 1);

            return @"...\" + parentFolder + @"\" + fileName;
        }

        #endregion
    }
}
