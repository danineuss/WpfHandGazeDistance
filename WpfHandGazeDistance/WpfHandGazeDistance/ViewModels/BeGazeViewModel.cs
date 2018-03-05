using System.Windows.Input;
using Microsoft.Win32;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class BeGazeViewModel : BaseViewModel
    {
        private string _beGazePath;

        private BeGazeData _beGazeData;

        public string BeGazePath
        {
            get => _beGazePath;
            set
            {
                ChangeAndNotify(value, ref _beGazePath);
                _beGazeData = new BeGazeData(value);
            }
        }

        public bool ReadyToAnalyse => _beGazeData != null;

        public ICommand LoadCommand => new RelayCommand(LoadBeGaze, true);

        private void LoadBeGaze()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                BeGazePath = openFileDialog.FileName;
                _beGazeData.LoadBeGazeFile(BeGazePath);
            }
        }
    }
}
