using System.Windows;
using WpfHandGazeDistance.ViewModels;

namespace WpfHandGazeDistance.Views
{
    /// <summary>
    /// Interaction logic for ParametersWindow.xaml
    /// </summary>
    public partial class ParametersWindow : Window
    {
        public ParametersWindow()
        {
            InitializeComponent();
            DataContext = new PrototypingViewModel();
        }
    }
}
