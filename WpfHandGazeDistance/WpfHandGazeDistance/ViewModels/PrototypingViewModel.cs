using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class PrototypingViewModel : BaseViewModel
    {
        private BitmapSource _publicImage;
        private string _videoPath;
        private HgdExperiment _hgdExperiment;

        public string VideoPath
        {
            get => _videoPath;
            set => ChangeAndNotify(value, ref _videoPath);
        }

        public BitmapSource PublicImage
        {
            get => _publicImage;
            set => ChangeAndNotify(value, ref _publicImage);
        }

        public ObservableCollection<HgdExperiment> HgdExperiments { get; set; }

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
                VideoPath = FileManager.OpenFileDialog();
                Video = new Video(VideoPath);
                Video.ThumbnailImage.Resize(0.1);
                Thumbnail = Video.ThumbnailImage.BitMapImage;
            }
        }

        public class MyObject : BaseViewModel
        {
            private Image _image;

            private BitmapSource _bitmapImage;

            private Uri _uriString;

            public Image Image
            {
                get => _image;
                set
                {
                    ChangeAndNotify(value, ref _image);
                    _image.Resize(0.1);
                    BitmapImage = _image.BitMapImage;
                    //UriString = new Uri(Image.BgrImage.ToString());
                }
            }

            public BitmapSource BitmapImage
            {
                get => _bitmapImage;
                set => ChangeAndNotify(value, ref _bitmapImage);
            }

            public Uri UriString
            {
                get => _uriString;
                set => ChangeAndNotify(value, ref _uriString);
            }

            public Video Video { get; set; }
            
            public string Name { get; set; }

            public bool HgdFlags { get; set; }
        }

        #region Constructor

        public PrototypingViewModel()
        {
            InitializeMyList();
        }

        #endregion

        public ICommand LoadPublicVideoCommand => new RelayCommand(LoadVideo, true);

        public void InitializeMyList()
        {
            HgdExperiments = new ObservableCollection<HgdExperiment>();
            for (int i = 0; i < 5; i++)
            {
                HgdExperiments.Add(InitializeMyObject(i));
            }
        }

        public HgdExperiment InitializeMyObject(int i)
        {
            _hgdExperiment = new HgdExperiment
            {
                VideoPath = i.ToString()
            };

            return _hgdExperiment;
        }

        private void LoadVideo()
        {
            VideoPath = FileManager.OpenFileDialog();
            _hgdExperiment.VideoPath = VideoPath;
            _hgdExperiment.Video = new Video(VideoPath);
            _hgdExperiment.Video.ThumbnailImage.Resize(0.1);
            _hgdExperiment.Thumbnail = _hgdExperiment.Video.ThumbnailImage.BitMapImage;
            PublicImage = _hgdExperiment.Thumbnail;

            foreach (HgdExperiment hgdExperiment in HgdExperiments)
            {
                hgdExperiment.Thumbnail = PublicImage;
            }
        }

        private void Print()
        {
            Debug.Print("Juhuu");
        }

        //private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Debug.Print("Clicked.");
        //}

        //private void ShowWindow(int i)
        //{
        //    // Just as an exammple, here I just show a MessageBox
        //    MessageBox.Show("You clicked on object " + i + "!!!");
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    var data = new Test { Test1 = "Test1", Test2 = "Test2" };

        //    //DataGridTest.Items.Add(data);
        //}

        //private void DataGridTest_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Debug.Print("MouseDown");
        //}
        
        //public class Test
        //{
        //    public string Test1 { get; set; }
        //    public string Test2 { get; set; }
        //}
    }
};