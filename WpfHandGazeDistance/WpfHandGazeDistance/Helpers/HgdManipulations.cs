using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfHandGazeDistance.Helpers
{
    /// <summary>
    /// This class contains all the core data manipulation knowledge. It is a static class as it will be called
    /// by the HgdViewModel to basically transfer lists of numbers into different lists of numbers depending
    /// on which action and filter is required. It is mostly used in the HgdViewModel.Analyse() function.
    /// 
    /// The individual functions and its purposes/reasoning are detailed below.
    /// </summary>
    public static class HgdManipulations
    {
        public static List<float> RecordingTimeFromVideo(HandDetector handDetector)
        {
            List<float> recordingTime = new List<float>();

            if (handDetector != null)
            {
                float frameStep = 1000 / handDetector.Video.Fps;
                for (int i = 0; i < handDetector.Video.FrameCount; i++)
                {
                    recordingTime.Add(frameStep * i);
                }
            }

            return recordingTime;
        }

        public static List<float> RecordingTimeFromConstant(int length, float fps)
        {
            List<float> recordingTime = new List<float>();

            float frameStep = 1000 / fps;
            for (int i = 0; i < length; i++)
            {
                recordingTime.Add(frameStep * i);
            }

            return recordingTime;
        }

        public static List<float> PruneValues(List<float> inputValues)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                outputValues.Add(Math.Abs(inputValues[i] - (-1f)) < 0.0001f ? float.NaN : inputValues[i]);
            }

            return outputValues;
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
        public static List<float> MovingMedian(List<float> inputValues, int period)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
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
        public static List<float> LowPass(List<float> inputValues, int longActionsCount)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (float.IsNaN(inputValues[i]))
                {
                    outputValues.Add(float.NaN);
                    continue;
                }

                List<float> currentWindow = ValueWindow(inputValues, i);

                if (currentWindow.Count > longActionsCount)
                {
                    outputValues.AddRange(currentWindow);
                }
                else
                {
                    List<float> nanList = Enumerable.Range(0, currentWindow.Count).Select(n => float.NaN).ToList();
                    outputValues.AddRange(nanList);
                }

                i += currentWindow.Count - 1;
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
        public static List<float> MovingStdDev(List<float> inputValues, int period)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count - period; i++)
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
            }
            outputValues.AddRange(Enumerable.Range(0, period).Select(n => float.NaN).ToList());

            return outputValues;
        }

        /// <summary>
        /// This function will apply a upper threshold to all values within a list of floats. Any value
        /// above the threshold is replace with NaN.
        /// </summary>
        /// <param name="inputValues">List of floats (e.g. StandardDeviation)</param>
        /// <param name="threshold">The upper threshold (e.g. 60px)</param>
        /// <returns>A list of floats.</returns>
        public static List<float> Threshold(List<float> inputValues, float threshold)
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

        /// <summary>
        /// This will replace any values into 1 and keep the NaN for the rest.
        /// </summary>
        /// <param name="inputValues">List of floats (e.g. RigidActions</param>
        /// <returns>List of floats</returns>
        public static List<float> ConvertToBinary(List<float> inputValues)
        {
            List<float> outputValues = new List<float>();

            foreach (var value in inputValues)
            {
                outputValues.Add(float.IsNaN(value) ? float.NaN : 1);
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
        public static List<float> Buffer(List<float> inputValues, int windowLength, int bufferLength)
        {
            List<float> outputValues = new List<float>();

            for (int i = 0; i < inputValues.Count; i++)
            {
                if (float.IsNaN(inputValues[i]))
                {
                    outputValues.Add(float.NaN);
                    continue;
                }

                List<float> currentWindow = ValueWindow(inputValues, i);
                List<float> bufferList = Enumerable.Repeat(currentWindow.Average(), bufferLength).ToList();
                List<float> stdDevWindow = Enumerable.Repeat(currentWindow.Average(), windowLength).ToList();

                // The first action is at the very start of the video and the output is not
                // far enough to contain more values than bufferLength
                if (outputValues.Count > bufferLength)
                {
                    outputValues.RemoveRange(outputValues.Count - bufferLength, bufferLength);
                    outputValues.AddRange(bufferList);
                }

                outputValues.AddRange(currentWindow);
                i += currentWindow.Count - 1;

                if (inputValues.Count > outputValues.Count + windowLength + bufferLength)
                {
                    outputValues.AddRange(stdDevWindow);
                    outputValues.AddRange(bufferList);
                    i += windowLength + bufferLength;
                }
                else
                {
                    if (inputValues.Count > outputValues.Count + windowLength)
                    {
                        outputValues.AddRange(stdDevWindow);
                    }
                    List<float> fillerList = Enumerable.Repeat(currentWindow.Average(), inputValues.Count - outputValues.Count).ToList();
                    outputValues.AddRange(fillerList);
                    break;
                }
            }

            return outputValues;
        }

        /// <summary>
        /// Calculates the standard deviation of a list of floats.
        /// </summary>
        /// <param name="inputValues">A list of float values.</param>
        /// <returns>Standard deviation as a float.</returns>
        public static float CalculateStdDev(List<float> inputValues)
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

        public static List<float> ValueWindow(List<float> inputValues, int index)
        {
            List<float> currentWindow = new List<float>();
            while (!float.IsNaN(inputValues[index]))
            {
                currentWindow.Add(inputValues[index]);
                if (index + 1 < inputValues.Count)
                {
                    if (float.IsNaN(inputValues[index + 1])) break;
                    index++;
                }
                else
                {
                    break;
                }
            }

            return currentWindow;
        }
    }
}
