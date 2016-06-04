// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

namespace AutoQuest.ViewModels
{
    public class StreetTypeViewModel : BaseViewModel
    {
        private bool _enable;
        private string _name;

        public StreetTypeViewModel(string name)
        {
            Name = name;
        }

        public string Name
        {
            get { return _name; }
            private set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool Enable
        {
            get { return _enable; }
            set
            {
                if (value == _enable)
                    return;
                _enable = value;
                OnPropertyChanged();
            }
        }
    }
}