using System.Windows.Input;
using Microsoft.Win32;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class BeGazeViewModel : BaseViewModel
    {
        #region Private Members

        private string _beGazePath;

        private BeGazeData _beGazeData;

        #endregion

        #region Public Properties

        public string BeGazePath
        {
            get => _beGazePath;
            set
            {
                ChangeAndNotify(value, ref _beGazePath);
                _beGazeData = new BeGazeData(value);
            }
        }

        #endregion

        public bool ReadyToAnalyse => _beGazeData != null;

        public ICommand LoadCommand => new RelayCommand(LoadBeGaze, true);

        private void LoadBeGaze()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                BeGazePath = openFileDialog.FileName;
                _beGazeData = new BeGazeData(BeGazePath);
            }
        }
    }
}
