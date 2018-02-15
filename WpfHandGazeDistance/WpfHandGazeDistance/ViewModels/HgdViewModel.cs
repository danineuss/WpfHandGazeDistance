using System;
using System.Windows.Input;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class HgdViewModel : BaseViewModel
    {
        private HgdData _hgdData;

        public HgdViewModel()
        {
            _hgdData = new HgdData();
        }

        public ICommand AnalyseCommand => new RelayCommand(AnalyseData, true);

        private void AnalyseData()
        {
            throw new NotImplementedException();
        }
    }
}
