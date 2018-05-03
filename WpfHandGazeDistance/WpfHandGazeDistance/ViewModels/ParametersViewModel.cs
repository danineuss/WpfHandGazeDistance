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

        private Parameter _longActionDuration;

        private Parameter _stdDevWindowDuration;

        private Parameter _bufferDuration;

        private Parameter _medianWindowLength;

        private Parameter _pixelThreshold;

        private Parameter _hueThreshold1;

        private Parameter _hueThreshold2;

        private Parameter _hueThreshold3;

        private Parameter _hueThreshold4;

        private Parameter _erosionSize;

        private Parameter _erosionIterations;

        //private string parameterPath = @"C:\Users\mouseburglar\Desktop\Parameters.csv";

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

        public List<Parameter> ParameterList;

        public ObservableCollection<Parameter> ParameterCollection;

        public float LongActionDuration
        {
            get => _parameters.ParameterList.LongActionsDuration.Value;
            set
            {
                if (value < _parameters.ParameterList.LongActionsDuration.Minimum)
                    value = _parameters.ParameterList.LongActionsDuration.Minimum;
                if (value > _parameters.ParameterList.LongActionsDuration.Maximum)
                    value = _parameters.ParameterList.LongActionsDuration.Maximum;

                ChangeAndNotify(value, ref _parameters.ParameterList.LongActionsDuration.Value);
            }
        }

        public float StdDevWindowDuration
        {
            get => (float)ParameterList[1].Value;
            set
            {
                if (value < (float)ParameterList[1].Minimum) value = (float)ParameterList[1].Minimum;
                if (value > (float)ParameterList[1].Maximum) value = (float)ParameterList[1].Maximum;

                ChangeAndNotify(value, ref _stdDevWindowDuration.Value);
                UpdateParameterList();
            }
        }

        public float BufferDuration
        {
            get => (float)ParameterList[2].Value;
            set
            {
                if (value < (float)ParameterList[2].Minimum) value = (float)ParameterList[2].Minimum;
                if (value > (float)ParameterList[2].Maximum) value = (float)ParameterList[2].Maximum;

                ChangeAndNotify(value, ref _bufferDuration.Value);
                UpdateParameterList();
            }
        }

        public int MedianWindowLength
        {
            get => (int)ParameterList[3].Value;
            set
            {
                if (value < (int)ParameterList[3].Minimum) value = (int)ParameterList[3].Minimum;
                if (value > (int)ParameterList[3].Maximum) value = (int)ParameterList[3].Maximum;

                ChangeAndNotify(value, ref _medianWindowLength.Value);
                UpdateParameterList();
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
                ParameterList.Clear();

                using (var streamReader = new StreamReader(inputPath))
                {
                    string headerLine = streamReader.ReadLine();
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        var values = line.Split(_csvDelimiter);

                        Parameter parameter = new Parameter(values[0], values[1], values[2], values[3]);

                        ParameterList.Add(parameter);
                    }
                }

                UpdateProperties();
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
                            _csvDelimiter + $"{parameter.Maximum}";
                    stringBuilder.AppendLine(line);
                }

                File.WriteAllText(savePath, stringBuilder.ToString());
            }
        }

        private void LoadDefaultParameters()
        {
            ParameterList = StandardParameters.GetParameters();
            UpdateProperties();
            Parameters parameters = new Parameters();
        }

        private void UpdateParameterList()
        {
            ParameterList = new List<Parameter>()
            {
                _longActionDuration,
                _stdDevWindowDuration,
                _bufferDuration,
                _medianWindowLength,
                //_pixelThreshold, 
                //_hueThreshold1,
                //_hueThreshold2,
                //_hueThreshold3,
                //_hueThreshold4,
                //_erosionSize,
                //_erosionIterations
            };
        }

        private void UpdateProperties()
        {
            _longActionDuration = ParameterList[0];
            _stdDevWindowDuration = ParameterList[1];
            _bufferDuration = ParameterList[2];
            _medianWindowLength = ParameterList[3];

            LongActionDuration = (float)ParameterList[0].Value;
            StdDevWindowDuration = (float)ParameterList[1].Value;
            BufferDuration = (float)ParameterList[2].Value;
            MedianWindowLength = (int)ParameterList[3].Value;
            //PixelThreshold = (int)ParameterList[4].Value;
            //HueThreshold1 = (int)ParameterList[5].Value;
            //HueThreshold2 = (int)ParameterList[6].Value;
            //HueThreshold3 = (int)ParameterList[7].Value;
            //HueThreshold4 = (int)ParameterList[8].Value;
            //ErosionSize = (int)ParameterList[9].Value;
            //ErosionIterations = (int)ParameterList[10].Value;
        }
    }
}
