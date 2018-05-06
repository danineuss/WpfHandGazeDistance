using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Input;
using Microsoft.Office.Core;
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

        #endregion

        #region Public Properties

        public ObservableCollection<Parameter> ParameterList { get; set; }

        #endregion

        public ParametersViewModel()
        {
            ParameterList = new ObservableCollection<Parameter>();
            ParameterList.Clear();
            LoadDefaultParameters();
        }

        public ICommand LoadParametersCommand => new RelayCommand(LoadParameters, true);

        public ICommand SaveParametersCommand => new RelayCommand(SaveParameters, true);

        public ICommand ResetParametersCommand => new RelayCommand(LoadDefaultParameters, true);

        public void LoadParameters()
        {
            string inputPath = FileManager.OpenFileDialog(".csv");

            LoadParameters(inputPath);
        }

        public void LoadParameters(string inputPath)
        {
            if (inputPath != null)
            {
                ParameterList = new ObservableCollection<Parameter>();

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

        public void SaveParameters()
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

        private void LoadDefaultParameters()
        {
            LoadParameters(_defaultParameterPath);
        }
    }
}
