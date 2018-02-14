using System.Collections.Generic;

namespace WpfHandGazeDistance.Models
{
    public class HgdData
    {
        #region Private Properties

        private Dictionary<int, float> _recordingTime;

        private Dictionary<int, float> _hgdRaw;

        #endregion

        #region Public Properties



        #endregion

        #region Constructor

        public HgdData()
        {
            _recordingTime = new Dictionary<int, float>();
            _hgdRaw = new Dictionary<int, float>();
        }

        #endregion

        #region Public Members



        #endregion

        #region Private Members

        

        #endregion
    }
}
