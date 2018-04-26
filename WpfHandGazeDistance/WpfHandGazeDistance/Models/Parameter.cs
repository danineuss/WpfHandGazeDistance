using WpfHandGazeDistance.ViewModels.Base;

namespace WpfHandGazeDistance.Models
{
    public class Parameter : BaseViewModel
    {
        private string _name;

        private object _value;

        public string Name
        {
            get => _name;
            set => ChangeAndNotify(value, ref _name);
        }

        public object Value
        {
            get => _value;
            set => ChangeAndNotify(value, ref _value);
        }

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
