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

        private Parameters _parameters;

        private readonly char _csvDelimiter = ',';

        private readonly List<string> _headerList = new List<string>()
        {
            "Parameter Name",
            "Value",
            "Minimum",
            "Maximum"
        };

        #endregion

        #region Public Properties

        public float LongActionDuration
        {
            get => _parameters.LongActionsDuration.Value;
            set
            {
                if (value < _parameters.LongActionsDuration.Minimum)
                    value = _parameters.LongActionsDuration.Minimum;
                if (value > _parameters.LongActionsDuration.Maximum)
                    value = _parameters.LongActionsDuration.Maximum;

                ChangeAndNotify(value, ref _parameters.LongActionsDuration.Value);
            }
        }

        public float StdDevWindowDuration
        {
            get => _parameters.StdDevWindowDuration.Value;
            set
            {
                if (value < _parameters.StdDevWindowDuration.Minimum)
                    value = _parameters.StdDevWindowDuration.Minimum;
                if (value > _parameters.StdDevWindowDuration.Maximum)
                    value = _parameters.StdDevWindowDuration.Maximum;

                ChangeAndNotify(value, ref _parameters.StdDevWindowDuration.Value);
            }
        }

        public float BufferDuration
        {
            get => _parameters.BufferDuration.Value;
            set
            {
                if (value < _parameters.BufferDuration.Minimum)
                    value = _parameters.BufferDuration.Minimum;
                if (value > _parameters.BufferDuration.Maximum)
                    value = _parameters.BufferDuration.Maximum;

                ChangeAndNotify(value, ref _parameters.BufferDuration.Value);
            }
        }

        public int MedianWindowLength
        {
            get => _parameters.MedianWindowLength.Value;
            set
            {
                if (value < _parameters.MedianWindowLength.Minimum)
                    value = _parameters.MedianWindowLength.Minimum;
                if (value > _parameters.MedianWindowLength.Maximum)
                    value = _parameters.MedianWindowLength.Maximum;

                ChangeAndNotify(value, ref _parameters.MedianWindowLength.Value);
            }
        }
        

        #endregion

        public ParametersViewModel()
        {
            _parameters = new Parameters();
            LoadDefaultParameters();
        }

        public ICommand LoadParametersCommand => new RelayCommand(LoadParameters, true);

        public ICommand SaveParametersCommand => new RelayCommand(SaveParameters, true);

        public ICommand ResetParametersCommand => new RelayCommand(LoadDefaultParameters, true);

        public void LoadParameters()
        {
            string inputPath = FileManager.OpenFileDialog(".csv");

            if (inputPath != null)
            {
                using (var streamReader = new StreamReader(inputPath))
                {
                    string headerLine = streamReader.ReadLine();
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        var values = line.Split(_csvDelimiter);

                        //Parameter parameter = new Parameter(values[0], values[1], values[2], values[3]);

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


                //foreach (Parameter parameter in ParameterList)
                //{
                //    string line = $"{parameter.Name}";
                //    line += _csvDelimiter + $"{parameter.Value}" + 
                //            _csvDelimiter + $"{parameter.Minimum}" + 
                //            _csvDelimiter + $"{parameter.Maximum}";
                //    stringBuilder.AppendLine(line);
                //}

                File.WriteAllText(savePath, stringBuilder.ToString());
            }
        }

        private void LoadDefaultParameters()
        {
            //ParameterList = StandardParameters.GetParameters();
            //UpdateProperties();
            Parameters parameters = new Parameters();
        }

        private void CheckMinMax(object value, object parameter)
        {

        }
    }
}
