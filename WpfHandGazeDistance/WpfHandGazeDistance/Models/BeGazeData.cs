using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace WpfHandGazeDistance.Models
{
    public class BeGazeData
    {
        #region Private Properties

        private readonly Dictionary<string, List<float>> _columnsBeGaze;

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

            _columnsBeGaze = new Dictionary<string, List<float>>()
            {
                { "RecordingTime [ms]", RecordingTime },
                { "Point of Regard Binocular X [px]", XGaze },
                { "Point of Regard Binocular Y [px]", YGaze }
            };

            LoadBeGazeFile(filePath);
        }

        #endregion

        #region Public Members

        #endregion

        #region Private Members
        
        /// <summary>
        /// This loads a .xlsx file and fills the values of specific columns, which can be found in <see cref="_columnsBeGaze"/>.
        /// The top row contains the name of the column, the rest are values.
        /// </summary>
        /// <param name="beGazePath">The windows path to the BeGaze data file. Is selected from the GUI.</param>
        private void LoadBeGazeFile(string beGazePath)
        {
            using (var excelPackage = new ExcelPackage(new FileInfo(beGazePath)))
            {
                var myWorksheet = excelPackage.Workbook.Worksheets.First();
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                for (var i = 1; i <= totalColumns; i++)
                {
                    var currentKey = myWorksheet.GetValue<string>(1, i);

                    if (!_columnsBeGaze.ContainsKey(currentKey)) continue;

                    for (var j = 2; j <= totalRows; j++)
                    {
                        _columnsBeGaze[currentKey].Add(myWorksheet.GetValue<float>(j, i));
                    }
                }
            }
        }

        #endregion
    }
}
