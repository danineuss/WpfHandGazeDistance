using System.Collections.Generic;

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

        private void LoadBeGazeFile(string beGazePath)
        {

        }

        #endregion
    }
}
