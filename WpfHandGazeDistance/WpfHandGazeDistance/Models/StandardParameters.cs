using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHandGazeDistance.Models
{
    public static class StandardParameters
    {
        public static readonly Parameter _longActionDuration = new Parameter("Long Action Duration [s]", 2f, 0f, 10f);

        public static readonly Parameter _stdDevWindowDuration = new Parameter("Standard Deviation Window Duration [s]", 2f, 0f, 10f);

        public static readonly Parameter _bufferDuration = new Parameter("Buffer Duration [s]", 0.5f, 0f, 2f);

        public static readonly Parameter _medianWindowLength = new Parameter("Median Window Length [frames]", 10, 0, 20);

        public static readonly Parameter _pixelThreshold = new Parameter("Hand Pixel Threshold [px]", 10000, 0, 100000);

        public static readonly Parameter _hueThreshold1 = new Parameter("Hue Threshold 1 [-]", 0, 0, 180);

        public static readonly Parameter _hueThreshold2 = new Parameter("Hue Threshold 2 [-]", 30, 0, 180);

        public static readonly Parameter _hueThreshold3 = new Parameter("Hue Threshold 3 [-]", 160, 0, 180);

        public static readonly Parameter _hueThreshold4 = new Parameter("Hue Threshold 4 [-]", 180, 0, 180);

        public static readonly Parameter _erosionSize = new Parameter("Erosion Size [px]", 5, 0, 10);

        public static readonly Parameter _erosionIterations = new Parameter("Erosion Iterations [-]", 3, 0, 10);

        public static List<Parameter> GetParameters()
        {
            return new List<Parameter>()
            {
                _longActionDuration,
                _stdDevWindowDuration,
                _bufferDuration,
                _medianWindowLength,
                //_pixelThreshold,
                //_hueThreshold1,
                //_hueThreshold2,
                //_hueThreshold3,
                //_hueThreshold4,
                //_erosionSize,
                //_erosionIterations
            };
        }
    }
}
