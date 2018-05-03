using System;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.Models
{
    public class Parameter
    {
        public string Name;

        public object Value;

        public object Minimum;

        public object Maximum;

        public Parameter()
        {

        }

        public Parameter(string name, object value, object minimum, object maximum)
        {
            Name = name;
            Value = value;
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
