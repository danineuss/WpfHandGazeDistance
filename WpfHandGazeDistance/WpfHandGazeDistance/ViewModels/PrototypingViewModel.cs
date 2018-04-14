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
        private string _videoPath;
        private MyObject _object;

        public string VideoPath
        {
            get => _videoPath;
            set => ChangeAndNotify(value, ref _videoPath);
        }

        public ObservableCollection<MyObject> MyList { get; set; }

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

        public ICommand LoadVideoCommand => new RelayCommand(LoadVideo, true);

        public void InitializeMyList()
        {
            MyList = new ObservableCollection<MyObject>();
            for (int i = 0; i < 5; i++)
            {
                MyList.Add(InitializeMyObject(i));
            }
        }

        public MyObject InitializeMyObject(int i)
        {
            _object = new MyObject
            {
                Name = "The object " + i,
                HgdFlags = false
            };

            return _object;
        }

        private void LoadVideo()
        {
            VideoPath = FileManager.OpenFileDialog();
            _object.Video = new Video(VideoPath);
            _object.Image = _object.Video.GetImageFrame();
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