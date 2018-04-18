using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace WpfHandGazeDistance.Models
{
    public class BeGazeData
    {
        #region Private Properties

        private readonly Dictionary<int, List<float>> _indexBeGaze;

        private char _csvDelimiter = '\t';

        #endregion

        #region Public Properties
        
        public List<float> RecordingTime { get; }

        public List<float> XGaze { get; }

        public List<float> YGaze { get; }

        #endregion

        #region Constructor

        public BeGazeData(string filePath)
        {
            RecordingTime = new List<float>();
            XGaze = new List<float>();
            YGaze = new List<float>();

            _indexBeGaze = new Dictionary<int, List<float>>()
            {
                { 0, RecordingTime },
                { 9, XGaze },
                { 10, YGaze }
            };

            LoadBeGazeFile(filePath);
        }

        #endregion

        #region Public Members
        
        public PointF GetCoordinatePoint(int index)
        {
            if (index > XGaze.Count - 1)
            {
                Debug.Print("Index: " + index + "/ XGaze.Count: " + XGaze.Count);
                index = XGaze.Count - 1;
            }
            return new PointF(XGaze[index], YGaze[index]);
        }

        #endregion

        #region Private Members
        
        /// <summary>
        /// This loads a .xlsx file and fills the values of specific columns, which can be found in <see cref="_indexBeGaze"/>.
        /// The top row contains the name of the column, the rest are values.
        /// </summary>
        /// <param name="beGazePath">The windows path to the BeGaze data file. Is selected from the GUI.</param>
        private void LoadBeGazeFile(string beGazePath)
        {
            using (var streamReader = new StreamReader(beGazePath))
            {
                string headerLine = streamReader.ReadLine();
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    var values = line.Split(_csvDelimiter);

                    foreach (KeyValuePair<int, List<float>> keyValuePair in _indexBeGaze)
                    {
                        keyValuePair.Value.Add(Single.Parse(values[keyValuePair.Key]));
                    }
                }
            }
        }

        #endregion
    }
}
