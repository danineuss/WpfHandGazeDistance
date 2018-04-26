using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class ParametersViewModel : BaseViewModel
    {
        public ObservableCollection<Parameter> ParameterCollection { get; set; }

        string parameterPath = @"C:\Users\mouseburglar\Desktop\Parameters.csv";

        private readonly char _csvDelimiter = ',';

        private readonly List<string> _headerList = new List<string>()
        {
            "Parameter Name",
            "Value"
        };

        public ParametersViewModel()
        {
            ParameterCollection = new ObservableCollection<Parameter>();
            LoadParameters(parameterPath);
        }

        public void LoadParameters(string inputPath)
        {
            using (var streamReader = new StreamReader(inputPath))
            {
                string headerLine = streamReader.ReadLine();
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    var values = line.Split(_csvDelimiter);

                    Parameter parameter = new Parameter(values[0], values[1]);

                    ParameterCollection.Add(parameter);
                }
            }
        }

        public void SaveData(string savePath)
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


            foreach (Parameter parameter in ParameterCollection)
            {
                string line = $"{parameter.Name}";
                line += _csvDelimiter + $" {parameter.Value}";
                stringBuilder.AppendLine(line);
            }

            File.WriteAllText(savePath, stringBuilder.ToString());
        }

    }
}
