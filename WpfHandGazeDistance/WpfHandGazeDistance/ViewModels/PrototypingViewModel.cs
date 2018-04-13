using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class PrototypingViewModel : BaseViewModel
    {
        public ObservableCollection<MyObject> MyList { get; set; }

        public class MyObject
        {
            public int MyID { get; set; }
            public string MyString { get; set; }
        }

        #region Constructor

        public PrototypingViewModel()
        {
            InitializeMyList();
            TestCommand = new RelayCommand(Print, true);
        }

        #endregion

        public ICommand TestCommand { get; set; }
        

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
            MyObject theObject = new MyObject();
            theObject.MyID = i;
            theObject.MyString = "The object " + i;
            return theObject;
        }

        private void Print()
        {
            Debug.Print("Juhuu");
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.Print("Clicked.");
        }

        private void ShowWindow(int i)
        {
            // Just as an exammple, here I just show a MessageBox
            MessageBox.Show("You clicked on object " + i + "!!!");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var data = new Test { Test1 = "Test1", Test2 = "Test2" };

            //DataGridTest.Items.Add(data);
        }

        private void DataGridTest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.Print("MouseDown");
        }
        
        public class Test
        {
            public string Test1 { get; set; }
            public string Test2 { get; set; }
        }
    }
};