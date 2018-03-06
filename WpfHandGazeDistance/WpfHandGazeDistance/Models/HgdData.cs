using System.Collections.Generic;

namespace WpfHandGazeDistance.Models
{
    public class HgdData
    {
        #region Private Properties

        private List<float> _recordingTime;

        #endregion

        #region Public Properties

        public List<float> RawData { get; set; }

        #endregion

        #region Constructor

        public HgdData()
        {
            _recordingTime = new List<float>();
            RawData = new List<float>();
        }

        #endregion

        #region Public Members



        #endregion

        #region Private Members

        

        #endregion
    }
}
