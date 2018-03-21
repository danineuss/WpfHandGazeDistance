using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfHandGazeDistance.Models
{
    public class HgdData
    {
        #region Public Properties

        public List<float> RecordingTime { get; set; }

        public List<float> RawDistance { get; set; }

        #endregion

        #region Constructor

        public HgdData()
        {
            RecordingTime = new List<float>();
            RawDistance = new List<float>();
        }

        #endregion

        public void SaveData(string savePath)
        {
            File.WriteAllLines(savePath, RawDistance.Select(d => d.ToString()));
        }
    }
}
