using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace WpfHandGazeDistance.Models
{
    public class BeGazeData
    {
        #region Private Properties

        private Dictionary<int, float> _recordingTime;

        private Dictionary<int, float> _xGaze;

        private Dictionary<int, float> _yGaze;

        private List<string> _columnsBeGaze;

        #endregion

        #region Public Properties



        #endregion

        #region Constructor

        public BeGazeData(string filePath)
        {
            _recordingTime = new Dictionary<int, float>();
            _xGaze = new Dictionary<int, float>();
            _yGaze = new Dictionary<int, float>();

            _columnsBeGaze = new List<string>()
            {
                "RecordingTime [ms]",
                "Point of Regard Binocular X [px]",
                "Point of Regard Binocular Y [px]"
            };
        }

        #endregion

        #region Public Members



        #endregion

        #region Private Members

        public void LoadBeGazeFile(string beGazePath)
        {
            string HDR = "No";
            string oleString;
            if (beGazePath.Substring(beGazePath.LastIndexOf('.')).ToLower() == ".xlsx")
                oleString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + beGazePath + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"";
            else
                oleString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + beGazePath + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";
            OleDbConnection oledbConnection = new OleDbConnection(oleString);
            oledbConnection.Open();
            DataTable dataTable = oledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
        }

        #endregion
    }
}
