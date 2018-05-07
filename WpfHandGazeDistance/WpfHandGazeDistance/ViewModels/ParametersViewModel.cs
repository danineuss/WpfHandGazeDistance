using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class ParametersViewModel : BaseViewModel
    {
        #region Private Properties

        private readonly char _csvDelimiter = ',';

        private readonly string _defaultParameterPath = @"C:\Users\mouseburglar\Desktop\ET_Data\HgdDefaultParameters.csv";

        private readonly List<string> _headerList = new List<string>()
        {
            "Parameter Name",
            "Value",
            "Minimum",
            "Maximum",
            "Type"
        };

        private ObservableCollection<Parameter> _parameterList;

        #endregion

        #region Public Properties

        public ObservableCollection<Parameter> ParameterList
        {
            get => _parameterList;
            set => ChangeAndNotify(value, ref _parameterList);
        }

        #endregion

        public ParametersViewModel()
        {
            ParameterList = new ObservableCollection<Parameter>();
            ParameterList.Clear();
            LoadDefaultParameters();
        }

        public ParametersViewModel(ObservableCollection<Parameter> parameterList)
        {
            ParameterList = parameterList;
        }

        public ICommand LoadParametersCommand => new RelayCommand(LoadParameters, true);

        public ICommand SaveParametersCommand => new RelayCommand(SaveParameters, true);

        public ICommand ResetParametersCommand => new RelayCommand(LoadDefaultParameters, true);

        private void LoadParameters()
        {
            string inputPath = FileManager.OpenFileDialog(".csv");

            LoadParameters(inputPath);
        }

        private void LoadParameters(string inputPath)
        {
            if (inputPath != null)
            {
                ParameterList = new ObservableCollection<Parameter>();
                ParameterList.Clear();

                using (var streamReader = new StreamReader(inputPath))
                {
                    string headerLine = streamReader.ReadLine();
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        var values = line.Split(_csvDelimiter);

                        Parameter parameter = new Parameter(values[0], values[1], values[2], values[3], values[4]);
                        ParameterList.Add(parameter);
                    }
                }
            }
        }

        private void LoadDefaultParameters()
        {
            LoadParameters(_defaultParameterPath);
        }

        private void SaveParameters()
        {
            string savePath = FileManager.SaveFileDialog();

            if (savePath != null)
            {
                var stringBuilder = new StringBuilder();

                string headerLine = "";
                foreach (var header in _headerList)
                {
                    if (headerLine == "")
                        headerLine = $"{header}";
                    else
                        headerLine += _csvDelimiter + $" {header}";
                }
                stringBuilder.AppendLine(headerLine);


                foreach (Parameter parameter in ParameterList)
                {
                    string line = $"{parameter.Name}";
                    line += _csvDelimiter + $"{parameter.Value}" +
                            _csvDelimiter + $"{parameter.Minimum}" +
                            _csvDelimiter + $"{parameter.Maximum}" +
                            _csvDelimiter + $"{parameter.Type}";
                    stringBuilder.AppendLine(line);
                }

                File.WriteAllText(savePath, stringBuilder.ToString());
            }
        }
    }
}
