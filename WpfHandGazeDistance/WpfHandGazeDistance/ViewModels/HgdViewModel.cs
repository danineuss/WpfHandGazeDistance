using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WpfHandGazeDistance.Helpers;
using WpfHandGazeDistance.Models;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.ViewModels
{
    public class HgdViewModel : BaseViewModel
    {
        #region Private Properties

        private BeGazeData _beGazeData;

        private readonly Video _video;

        private readonly HandDetector _handDetector;

        private readonly int _longActionCount;

        private readonly int _stdDevPeriod;

        private readonly int _bufferLength;

        private readonly int _medianPeriod = 10;

        private readonly int _stdDevThreshold = 60;

        #endregion

        public HgdData HgdData { get; set; }

        public HgdViewModel(BeGazeData beGazeData, Video video)
        {
            HgdData = new HgdData();
            _beGazeData = beGazeData;
            _video = video;
            _handDetector = new HandDetector(beGazeData, video);

            _longActionCount = (int) (2f * _video.Fps);
            _stdDevPeriod = (int)(2f * _video.Fps);
            _bufferLength = (int)(0.5f * _video.Fps);
        }

        public ICommand AnalyseCommand => new RelayCommand(AnalyseData, true);

        private void AnalyseData()
        {
            HgdData = _handDetector.MeasureRawHgd();
            HgdData.MedianDistance = MovingMedian(HgdData.RawDistance, _medianPeriod);
            HgdData.LongActions = LowPass(HgdData.MedianDistance, _longActionCount);
            HgdData.StandardDeviation = MovingStdDev(HgdData.LongActions, _stdDevPeriod);
            HgdData.RigidActions = Threshold(HgdData.StandardDeviation, _stdDevThreshold);
            HgdData.UsabilityIssues = ConvertToBinary(HgdData.RigidActions);
            HgdData.BufferedUsabilityIssues = Buffer(HgdData.UsabilityIssues, _stdDevPeriod, _bufferLength);
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
        private static List<float> MovingMedian(List<float> inputValues, int period)
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
                        median = values[(int)(period / 2 + 0.5)];
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
        private static List<float> LowPass(List<float> inputValues, int longActionsCount)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (Single.IsNaN(inputValues[i]))
                {
                    outputValues.Add(Single.NaN);
                    continue;
                }

                List<float> currentWindow = new List<float>();
                while (!Single.IsNaN(inputValues[i]))
                {
                    currentWindow.Add(inputValues[i]);
                    if (Single.IsNaN(inputValues[i + 1])) break;
                    if (i + 1 < inputValues.Count) i++;
                }

                if (currentWindow.Count > longActionsCount)
                {
                    outputValues.AddRange(currentWindow);
                }
                else
                {
                    List<float> nanList = Enumerable.Range(0, currentWindow.Count).Select(n => Single.NaN).ToList();
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
        private static List<float> MovingStdDev(List<float> inputValues, int period)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (Single.IsNaN(inputValues[i]))
                {
                    outputValues.Add(Single.NaN);
                    continue;
                }

                List<float> currentWindow = new List<float>();
                for (int j = 0; j < period; j++)
                {
                    currentWindow.Add(inputValues[i + j]);
                }
                outputValues.Add(CalculateStdDev(currentWindow));

                // Fill up the remaining values with NaN and jump to the end of the window.
                if (Single.IsNaN(inputValues[i + period]))
                {
                    outputValues.AddRange(Enumerable.Range(0, period - 1).Select(n => Single.NaN).ToList());
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
        private static List<float> Threshold(List<float> inputValues, float threshold)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (inputValues[i] > threshold)
                {
                    outputValues.Add(Single.NaN);
                }
                else
                {
                    outputValues.Add(inputValues[i]);
                }
            }

            return outputValues;
        }

        /// <summary>
        /// This will replace any values into 1 and keep the NaN for the rest.
        /// </summary>
        /// <param name="inputValues">List of floats (e.g. RigidActions</param>
        /// <returns>List of floats</returns>
        private static List<float> ConvertToBinary(List<float> inputValues)
        {
            List<float> outputValues = new List<float>();

            foreach (var value in inputValues)
            {
                outputValues.Add(Single.IsNaN(value) ? Single.NaN : 1);
            }

            return outputValues;
        }

        /// <summary>
        /// This will make the length of the standard deviation data longer again, so that it
        /// more closely matches the actual duration of the original action. This means adding
        /// a window with the same length as the rolling window of the standard deviation. In addition,
        /// a buffer is added before and after each action (typically 0.5s each). In both cases, 
        /// the average value of the data within that action is added in the buffers.
        /// </summary>
        /// <param name="inputValues">List of floats (e.g. RigidActions)</param>
        /// <param name="windowLength">The length of the standard deviation rolling window, 
        /// used back in MovingStdDev().</param>
        /// <param name="bufferLength">The length of the buffer window.</param>
        /// <returns></returns>
        private static List<float> Buffer(List<float> inputValues, int windowLength, int bufferLength)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (Single.IsNaN(inputValues[i]))
                {
                    outputValues.Add(Single.NaN);
                    continue;
                }

                List<float> currentWindow = new List<float>();
                while (!Single.IsNaN(inputValues[i]))
                {
                    currentWindow.Add(inputValues[i]);
                    if (Single.IsNaN(inputValues[i + 1])) break;
                    if (i + 1 < inputValues.Count) i++;
                }

                List<float> bufferList = Enumerable.Repeat(currentWindow.Average(), bufferLength).ToList();
                List<float> stdDevWindow = Enumerable.Repeat(currentWindow.Average(), windowLength).ToList();
                currentWindow.InsertRange(0, bufferList);
                currentWindow.InsertRange(currentWindow.Count, stdDevWindow);
                currentWindow.InsertRange(currentWindow.Count, bufferList);

                outputValues.RemoveRange(outputValues.Count - bufferLength, bufferLength);
                outputValues.AddRange(currentWindow);
                i += bufferLength;
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
    }
}
