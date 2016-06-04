// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

namespace AutoQuest.ViewModels
{
    public class SearchPageViewModel : BaseViewModel
    {
        private FilterViewModel _filterViewModel;
        private SteetsViewModel _streets;

        public FilterViewModel FilterViewModel
        {
            get { return _filterViewModel; }
            set
            {
                if (Equals(value, _filterViewModel))
                    return;
                _filterViewModel = value;
                OnPropertyChanged();
            }
        }

        public SteetsViewModel Streets
        {
            get { return _streets; }
            set
            {
                if (Equals(value, _streets))
                    return;
                _streets = value;
                OnPropertyChanged();
            }
        }
    }
}