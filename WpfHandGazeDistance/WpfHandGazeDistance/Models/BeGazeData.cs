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

        private List<string> _columnsBeGaze;

        #endregion

        #region Public Properties



        #endregion

        #region Constructor

        public BeGazeData(string filePath)
        {
            _recordingTime = new List<float>();
            _xGaze = new List<float>();
            _yGaze = new List<float>();

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
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(beGazePath)))
            {
                var myWorksheet = excelPackage.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                var content = myWorksheet.Cells[1, 1].Value;
                var contents = myWorksheet.Cells[1, 1, 11, 1].Value;
                
            }

            //string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", beGazePath);

            //var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1]", connectionString);
            //var dataSet = new DataSet();

            //adapter.Fill(dataSet, "myDataSet");

            //DataTable dataTable = dataSet.Tables["myDataSet"];

            //string HDR = "No";
            //string oleString;
            //if (beGazePath.Substring(beGazePath.LastIndexOf('.')).ToLower() == ".xlsx")
            //    oleString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + beGazePath + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"";
            //else
            //    oleString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + beGazePath + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";
            //OleDbConnection oledbConnection = new OleDbConnection(oleString);
            //oledbConnection.Open();
            //DataTable dataTable = oledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

        }

        #endregion
    }
}
