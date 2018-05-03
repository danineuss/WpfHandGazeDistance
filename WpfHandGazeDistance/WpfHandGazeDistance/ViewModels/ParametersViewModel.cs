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
            get => (float)ParameterList[0].Value;
            set
            {
                if (value < (float)ParameterList[0].Minimum) value = (float)ParameterList[0].Minimum;
                if (value > (float)ParameterList[0].Maximum) value = (float)ParameterList[0].Maximum;

                ChangeAndNotify(value, ref _longActionDuration.Value);
                UpdateParameterList();
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

        //public int PixelThreshold
        //{
        //    get => (int)ParameterList[4].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[4].Minimum) value = (int)ParameterList[4].Minimum;
        //        if (value > (int)ParameterList[4].Maximum) value = (int)ParameterList[4].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[4].Value);
        //        UpdateParameterList();
        //    }
        //}

        //public int HueThreshold1
        //{
        //    get => (int)ParameterList[5].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[5].Minimum) value = (int)ParameterList[5].Minimum;
        //        if (value > (int)ParameterList[5].Maximum) value = (int)ParameterList[5].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[5].Value);
        //        UpdateParameterList();
        //    }
        //}

        //public int HueThreshold2
        //{
        //    get => (int)ParameterList[6].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[6].Minimum) value = (int)ParameterList[6].Minimum;
        //        if (value > (int)ParameterList[6].Maximum) value = (int)ParameterList[6].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[6].Value);
        //        UpdateParameterList();
        //    }
        //}

        //public int HueThreshold3
        //{
        //    get => (int)ParameterList[7].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[7].Minimum) value = (int)ParameterList[7].Minimum;
        //        if (value > (int)ParameterList[7].Maximum) value = (int)ParameterList[7].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[7].Value);
        //        UpdateParameterList();
        //    }
        //}

        //public int HueThreshold4
        //{
        //    get => (int)ParameterList[8].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[8].Minimum) value = (int)ParameterList[8].Minimum;
        //        if (value > (int)ParameterList[8].Maximum) value = (int)ParameterList[8].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[8].Value);
        //        UpdateParameterList();
        //    }
        //}

        //public int ErosionSize
        //{
        //    get => (int)ParameterList[9].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[9].Minimum) value = (int)ParameterList[9].Minimum;
        //        if (value > (int)ParameterList[9].Maximum) value = (int)ParameterList[9].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[9].Value);
        //        UpdateParameterList();
        //    }
        //}

        //public int ErosionIterations
        //{
        //    get => (int)ParameterList[10].Value;
        //    set
        //    {
        //        if (value < (int)ParameterList[10].Minimum) value = (int)ParameterList[10].Minimum;
        //        if (value > (int)ParameterList[10].Maximum) value = (int)ParameterList[10].Maximum;

        //        ChangeAndNotify(value, ref ParameterList[10].Value);
        //        UpdateParameterList();
        //    }
        //}

        #endregion

        public ParametersViewModel()
        {
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
