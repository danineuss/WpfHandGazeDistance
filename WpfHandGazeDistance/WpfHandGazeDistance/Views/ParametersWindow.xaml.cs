using System.Collections.ObjectModel;
using System.Windows;
using WpfHandGazeDistance.Models;
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
            DataContext = new ParametersViewModel();
        }

        public ParametersWindow(ObservableCollection<Parameter> parameterList)
        {
            InitializeComponent();
            DataContext = new ParametersViewModel(parameterList);
        }
    }
}
