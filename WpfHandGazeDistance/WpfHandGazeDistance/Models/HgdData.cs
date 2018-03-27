using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace WpfHandGazeDistance.Models
{
    public class HgdData
    {
        #region Private Properties

        private readonly Dictionary<string, List<float>> _columnsHgdData;

        #endregion

        #region Public Properties

        public List<float> RecordingTime { get; set; }

        public List<float> RawDistance { get; set; }

        public List<float> MedianDistance { get; set; }

        public List<float> LongActions { get; set; }

        public List<float> StandardDeviation { get; set; }

        public List<float> RigidActions { get; set; }

        public List<float> UsabilityIssues { get; set; }

        public List<float> BufferedUsabilityIssues { get; set; }

        #endregion

        #region Constructor

        public HgdData()
        {
            RecordingTime = new List<float>();
            RawDistance = new List<float>();

            _columnsHgdData = new Dictionary<string, List<float>>()
            {
                { "RecordingTime [ms]", RecordingTime },
                { "RawDistance [px]", RawDistance}
            };
        }

        #endregion

        public void SaveData(string savePath)
        {
            File.WriteAllLines(savePath, RawDistance.Select(d => d.ToString()));
        }

        public void AnalyseData()
        {
            MedianDistance = MovingMedian(RawDistance);
        }

        #region Private Members

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
        private static List<float> MovingMedian(List<float> inputValues, int period = 10)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count(); i++)
            {
                if (i < period - 1)
                    outputValues.Add(Single.NaN);
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

        #endregion
    }
}
