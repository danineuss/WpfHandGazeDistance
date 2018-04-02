using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WpfHandGazeDistance.Models
{
    public class HgdData
    {
        #region Private Properties

        private readonly List<string> _headerList;

        private List<List<float>> _dataList;

        private char _csvDelimiter = ';';

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
                // Remove top line as it only contains the name of the parameters.
                streamReader.ReadLine();

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
        
        #endregion
    }
}
