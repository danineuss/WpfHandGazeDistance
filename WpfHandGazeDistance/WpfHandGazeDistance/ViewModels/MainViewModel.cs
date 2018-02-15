using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public VideoViewModel VideoViewModel { get; }

        public BeGazeViewModel BeGazeViewModel { get; }

        public MainViewModel()
        {
            VideoViewModel = new VideoViewModel();
            BeGazeViewModel = new BeGazeViewModel();
        }
    }
}
