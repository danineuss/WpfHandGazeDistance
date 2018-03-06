using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace WpfHandGazeDistance.Models
{
    public class BeGazeData
    {
        #region Private Properties

        private List<float> _recordingTime;

        private List<float> _xGaze;

        private List<float> _yGaze;

        private Dictionary<string, List<float>> _columnsBeGaze;

        #endregion

        #region Public Properties



        #endregion

        #region Constructor

        public BeGazeData(string filePath)
        {
            _recordingTime = new List<float>();
            _xGaze = new List<float>();
            _yGaze = new List<float>();

            _columnsBeGaze = new Dictionary<string, List<float>>()
            {
                { "RecordingTime [ms]", _recordingTime },
                { "Point of Regard Binocular X [px]", _xGaze },
                { "Point of Regard Binocular Y [px]", _yGaze }
            };
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
        public void LoadBeGazeFile(string beGazePath)
        {
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(beGazePath)))
            {
                var myWorksheet = excelPackage.Workbook.Worksheets.First();
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                for (int i = 1; i <= totalColumns; i++)
                {
                    string currentKey = myWorksheet.GetValue<string>(1, i);

                    if (_columnsBeGaze.ContainsKey(currentKey))
                    {
                        for (int j = 2; j <= totalRows; j++)
                        {
                            _columnsBeGaze[currentKey].Add(myWorksheet.GetValue<float>(j, i));
                        }
                    }
                }                
            }
        }

        #endregion
    }
}
