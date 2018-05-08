using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Office.Core;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Public Properties

        public VideoViewModel VideoViewModel { get; set; }

        public BeGazeViewModel BeGazeViewModel { get; set; }

        public HgdViewModel HgdViewModel { get; set; }

        public PrototypingViewModel PrototypingViewModel { get; }

        public ParametersViewModel ParametersViewModel { get; set; }

        #endregion

        public MainViewModel()
        {
            PrototypingViewModel = new PrototypingViewModel();
            ParametersViewModel = new ParametersViewModel();
            HgdViewModel = new HgdViewModel(ParametersViewModel.ParameterList, PrototypingViewModel);
        }
    }
}
