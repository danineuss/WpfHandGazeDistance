using System;
using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.Models
{
    public class Parameter : BaseViewModel
    {
        public string _name;
        public object _value;
        private object _minimum;
        private object _maximum;
        private string _type;

        public string Name
        {
            get => _name;
            set => ChangeAndNotify(value, ref _name);
        }

        public double Value
        {
            get => Convert.ToDouble(_value);
            set
            {
                if (Type == "double")
                {
                    ChangeAndNotify(Math.Round(value, 2), ref _value);
                }
                else
                {
                    ChangeAndNotify(Convert.ToInt32(Math.Round(value)), ref _value);
                }
            }
        }

        public double Minimum
        {
            get => Convert.ToDouble(_minimum);
            set => ChangeAndNotify(value, ref _minimum);
        }

        public double Maximum
        {
            get => Convert.ToDouble(_maximum);
            set => ChangeAndNotify(value, ref _maximum);
        }

        public string Type
        {
            get => _type;
            set => ChangeAndNotify(value, ref _type);
        }

        public Parameter(string name, string value, string minimum, string maximum, string type)
        {
            Name = name;
            Type = type;

            if (type == "double")
            {
                Value = double.Parse(value);
                Minimum = double.Parse(minimum);
                Maximum = double.Parse(maximum);
            }
            else if (type == "int")
            {
                Value = int.Parse(value);
                Minimum = int.Parse(minimum);
                Maximum = int.Parse(maximum);
            }
        }
    }
}
