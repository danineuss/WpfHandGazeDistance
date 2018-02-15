using System.Diagnostics;
using System.Windows.Input;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Public Properties

        public VideoViewModel VideoViewModel { get; }

        public BeGazeViewModel BeGazeViewModel { get; }

        public HgdViewModel HgdViewModel { get; }

        public bool ReadyToAnalyse => VideoViewModel.ReadyToAnalyse && BeGazeViewModel.ReadyToAnalyse;

        #endregion

        public MainViewModel()
        {
            VideoViewModel = new VideoViewModel();
            BeGazeViewModel = new BeGazeViewModel();
            HgdViewModel = new HgdViewModel();
        }

        public ICommand AnalyseCommand => new RelayCommand(AnalyseData, ReadyToAnalyse);

        private void AnalyseData()
        {
            Debug.Print(ReadyToAnalyse.ToString());
        }
    }
}
