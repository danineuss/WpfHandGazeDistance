namespace WpfHandGazeDistance.Models
{
    public class Parameters
    {
        public FloatParameter LongActionsDuration;

        public FloatParameter StdDevWindowDuration;

        public FloatParameter BufferDuration;

        public IntegerParameter MedianWindowLength;

        public Parameters()
        {
            LongActionsDuration = new FloatParameter("Long Actions Duration [s]", 2f, 0f, 10f);
            StdDevWindowDuration = new FloatParameter("Standard Deviation Window Duration [s]", 2f, 0f, 10f);
            BufferDuration = new FloatParameter("Buffer Duration [s]", 0.5f, 0f, 3f);
            MedianWindowLength = new IntegerParameter("Median Window Length [frames]", 10, 0, 100);
        }

        public struct FloatParameter
        {
            public string Name;
            public float Value;
            public float Minimum;
            public float Maximum;

            public FloatParameter(string name, float value, float minimum, float maximum)
            {
                Name = name;
                Value = value;
                Minimum = minimum;
                Maximum = maximum;
            }
        }

        public struct IntegerParameter
        {
            public string Name;
            public int Value;
            public int Minimum;
            public int Maximum;

            public IntegerParameter(string name, int value, int minimum, int maximum)
            {
                Name = name;
                Value = value;
                Minimum = minimum;
                Maximum = maximum;
            }
        }
    }
}
