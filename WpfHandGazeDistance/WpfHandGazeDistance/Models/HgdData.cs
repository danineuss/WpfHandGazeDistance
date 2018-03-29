using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfHandGazeDistance.Models
{
    public class HgdData
    {
        #region Private Properties

        private readonly List<string> _headerList;

        private List<List<float>> _dataList;

        private char _csvDelimiter = ';';

        private const int _longActionCount = 120;

        private const int _stdDevPeriod = 120;

        private const int _medianPeriod = 10;

        private const int _stdDevThreshold = 60;

        private const float _buffer = 0.5f;

        private List<float> _recordingTime;

        private List<float> _rawDistance;

        private List<float> _medianDistance;

        private List<float> _longActions;

        private List<float> _standardDeviation;

        private List<float> _rigidActions;

        private List<float> _usabilityIssues;

        private List<float> _bufferedUsabilityIssues;

        #endregion

        #region Public Properties

        public List<float> RecordingTime
        {
            get => _recordingTime;
            set { _recordingTime = value; UpdateDataList(); } 
        }

        public List<float> RawDistance
        {
            get => _rawDistance;
            set { _rawDistance = value; UpdateDataList(); }
        }

        public List<float> MedianDistance
        {
            get => _medianDistance;
            set { _medianDistance = value; UpdateDataList(); }
        }

        public List<float> LongActions
        {
            get => _longActions;
            set { _longActions = value; UpdateDataList(); }
        }

        public List<float> StandardDeviation
        {
            get => _standardDeviation;
            set { _standardDeviation = value; UpdateDataList(); }
        }

        public List<float> RigidActions
        {
            get => _rigidActions;
            set { _rigidActions = value; UpdateDataList(); }
        }

        public List<float> UsabilityIssues
        {
            get => _usabilityIssues;
            set { _usabilityIssues = value; UpdateDataList(); }
        }

        public List<float> BufferedUsabilityIssues
        {
            get => _bufferedUsabilityIssues;
            set { _bufferedUsabilityIssues = value; UpdateDataList(); }
        }

        #endregion

        #region Constructor

        public HgdData()
        {
            RecordingTime = new List<float>();
            RawDistance = new List<float>();
            MedianDistance = new List<float>();
            LongActions = new List<float>();
            StandardDeviation = new List<float>();
            RigidActions = new List<float>();
            UsabilityIssues = new List<float>();
            BufferedUsabilityIssues = new List<float>();

            _headerList = new List<string>()
            {
                "RecordingTime [ms]",
                "RawDistance [px]",
                "MedianDistance [px]",
                "LongActions [px]",
                "StandardDeviation [px]",
                "RigidActions [px]",
                "UsabilityIssues [-]",
                "BufferedUsabilityIssues [-]"
        };

            _dataList = new List<List<float>>()
            {
                RecordingTime,
                RawDistance,
                MedianDistance,
                LongActions,
                StandardDeviation,
                RigidActions,
                UsabilityIssues,
                BufferedUsabilityIssues
            };
        }

        #endregion

        #region Public Members

        /// <summary>
        /// This will load the HGD Data from a .csv file. The first line is a header
        /// which is simply ignored. The rest is filled into the various list of floats
        /// containing the data.
        /// </summary>
        /// <param name="loadPath">The path to the .csv file.</param>
        public void LoadData(string loadPath)
        {
            using (var streamReader = new StreamReader(loadPath))
            {
                string headerLine = streamReader.ReadLine();
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    var values = line.Split(_csvDelimiter);

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] != "") _dataList[i].Add(float.Parse(values[i]));
                    }
                }
            }
        }

        /// <summary>
        /// This will save the HGD Data into a .csv file. The first line is a header
        /// which written out as strings. Below that the data is filled into each column.
        /// </summary>
        /// <param name="savePath">The path to the .csv file.</param>
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

            for (int index = 0; index < RecordingTime.Count; index++)
            {
                string line = "";
                foreach (var list in _dataList)
                {
                    if (list != null && list.Count > 0)
                    {
                        if (line == "")
                            line = $"{list[index]}";
                        else
                            line += _csvDelimiter + $" {list[index]}";
                    }
                }
                stringBuilder.AppendLine(line);
            }

            File.WriteAllText(savePath, stringBuilder.ToString());
        }

        public void AnalyseData()
        {
            AnalyseMedian();
            AnalyseLongActions();
            AnalyseStdDev();
            AnalyseRigidActions();
        }

        public void AnalyseMedian()
        {
            MedianDistance = MovingMedian(RawDistance);
        }

        public void AnalyseLongActions()
        {
            LongActions = LowPass(MedianDistance);
        }

        public void AnalyseStdDev()
        {
            StandardDeviation = MovingStdDev(LongActions);
        }

        public void AnalyseRigidActions()
        {
            RigidActions = Threshold(StandardDeviation);
        }

        public void BufferRigidActions()
        {

        }

        #endregion

        #region Private Members

        /// <summary>
        /// This will update the list of values for easy access.
        /// </summary>
        private void UpdateDataList()
        {
            _dataList = new List<List<float>>()
            {
                RecordingTime,
                RawDistance,
                MedianDistance,
                LongActions,
                StandardDeviation,
                RigidActions,
                UsabilityIssues,
                BufferedUsabilityIssues
            };
        }

        /// <summary>
        /// This function will take a list of floats and apply a moving median on it. 
        /// Within a window of length 'period' the values are replaced by the median value within that window.
        /// The new list of floats is then returned.
        /// 
        /// The median is calculated by taking the middle value (for odd number of values) or the two neighboring
        /// values and taking the average of the two (i.e. 10 values = (5th value + 6th value) / 2).
        /// </summary>
        /// <param name="inputValues">A list of float values which should be filtered.</param>
        /// <param name="period">The size of the rolling median window.</param>
        /// <returns></returns>
        private static List<float> MovingMedian(List<float> inputValues, int period = _medianPeriod)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count(); i++)
            {
                if (i < period - 1)
                    outputValues.Add(float.NaN);
                else
                {
                    var values = new List<float>();
                    for (int x = i; x > i - period; x--)
                        values.Add(inputValues[x]);
                    values.Sort();
                    
                    float median;
                    if (period % 2 == 0)
                        median = (values[period / 2] + values[period / 2 - 1]) / 2;
                    else
                        median =  values[(int)(period / 2 + 0.5)];
                    outputValues.Add(median);
                }
            }
            return outputValues;
        }

        /// <summary>
        /// This function will filter out short 'actions', intervals of HGD which are shorter than the
        /// minimum value. A temporary list is created and values are appended until a NaN value is hit.
        /// The length of the temporary list is then checked and added to the main list if its long enough.
        /// </summary>
        /// <param name="inputValues">A list of input float values (MedianDistance).</param>
        /// <param name="longActionsCount">The minimum amount of values required to count (e.g. 120)</param>
        /// <returns>A list of float with only the long actions.</returns>
        private static List<float> LowPass(List<float> inputValues, int longActionsCount = _longActionCount)
        {
            List<float> outputValues = new List<float>();
            
            for (int i = 0; i < inputValues.Count; i++)
            {
                if (float.IsNaN(inputValues[i]))
                {
                    outputValues.Add(float.NaN);
                    continue;
                }

                List<float> currentWindow = new List<float>();
                while (!float.IsNaN(inputValues[i]))
                {
                    currentWindow.Add(inputValues[i]);
                    if (float.IsNaN(inputValues[i + 1])) break;
                    if (i + 1 < inputValues.Count) i++;
                }

                if (currentWindow.Count > longActionsCount)
                {
                    outputValues.AddRange(currentWindow);
                }
                else
                {
                    List<float> nanList = Enumerable.Range(0, currentWindow.Count).Select(n => float.NaN).ToList();
                    outputValues.AddRange(nanList);
                }
            }

            return outputValues;
        }

        /// <summary>
        /// This function will compute the standard deviation of a set of input values within a moving window.
        /// The window is advanced along the data set if enough values are available, the standard deviation
        /// is written at the beginning of said window.
        /// </summary>
        /// <param name="inputValues">A set of float values (e.g. LongActions).</param>
        /// <param name="period">The size of the moving window (e.g. 120).</param>
        /// <returns>Standard deviation in a list of float.</returns>
        private static List<float> MovingStdDev(List<float> inputValues, int period = _stdDevPeriod)
        {
            List<float> outputValues = new List<float>();
            
            for (int i = 0; i < inputValues.Count; i++)
            {
                if (float.IsNaN(inputValues[i]))
                {
                    outputValues.Add(float.NaN);
                    continue;
                }

                List<float> currentWindow = new List<float>();
                for (int j = 0; j < period; j++)
                {
                    currentWindow.Add(inputValues[i + j]);
                }
                outputValues.Add(CalculateStdDev(currentWindow));

                // Fill up the remaining values with NaN and jump to the end of the window.
                if (float.IsNaN(inputValues[i + period]))
                {
                    outputValues.AddRange(Enumerable.Range(0, period - 1).Select(n => float.NaN).ToList());
                    i += period - 1;
                }
            }

            return outputValues;
        }

        /// <summary>
        /// This function will apply a upper threshold to all values within a list of floats. Any value
        /// above the threshold is replace with NaN.
        /// </summary>
        /// <param name="inputValues">List of floats (e.g. StandardDeviation)</param>
        /// <param name="threshold">The upper threshold (e.g. 60px)</param>
        /// <returns>A list of floats.</returns>
        private static List<float> Threshold(List<float> inputValues, float threshold = _stdDevThreshold)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (inputValues[i] > threshold)
                {
                    outputValues.Add(float.NaN);
                }
                else
                {
                    outputValues.Add(inputValues[i]);
                }
            }

            return outputValues;
        }

        private static List<float> Buffer(List<float> inputValues, float buffer = _buffer)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (float.IsNaN(inputValues[i]))
                {
                    outputValues.Add(float.NaN);
                    continue;
                }

                List<float> currentWindow = new List<float>();
                while (!float.IsNaN(inputValues[i]))
                {
                    currentWindow.Add(inputValues[i]);
                    if (float.IsNaN(inputValues[i + 1])) break;
                    if (i + 1 < inputValues.Count) i++;
                }
            }

            return outputValues;
        }

        /// <summary>
        /// Calculates the standard deviation of a list of floats.
        /// </summary>
        /// <param name="inputValues">A list of float values.</param>
        /// <returns>Standard deviation as a float.</returns>
        private static float CalculateStdDev(List<float> inputValues)
        {
            float variance = 0f;
            float average = inputValues.Sum() / inputValues.Count;

            for (int i = 0; i < inputValues.Count; i++)
            {
                variance += (float)Math.Pow(inputValues[i] - average, 2);
            }

            variance /= inputValues.Count - 1;

            return (float)Math.Sqrt(variance);
        }

        #endregion
    }
}
