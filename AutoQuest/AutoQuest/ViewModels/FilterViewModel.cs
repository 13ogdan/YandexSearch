// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using AutoQuest.API;

namespace AutoQuest.ViewModels
{
    public class FilterViewModel : BaseViewModel, IFilter
    {
        private readonly ILocationService _locationService;

        private readonly string[] _requiredRecalculatingProperty =
        {
            nameof(IFilter.SearchQuery),
            nameof(IFilter.CurrentLocation),
            nameof(IFilter.CheckAlternativeName),
            nameof(IFilter.MaxDistance),
            nameof(IFilter.PossibleTypes)
        };

        private bool _checkAlternativeName;
        private GeoPoint _currentLocation;
        private string _error;
        private double _maxDistance;
        private string _maxDistanceRepresentation;
        private IEnumerable<string> _possibleTypes;
        private string _search;
        private Regex _searchQuery;
        private ObservableCollection<StreetTypeViewModel> _types;

        public event EventHandler FilterChanged;

        public FilterViewModel() {}

        public FilterViewModel(ILocationService locationService)
        {
            _locationService = locationService;
            if (_locationService != null)
            {
                CurrentLocation = locationService.Location;
                locationService.LocationChanged += LocationServiceOnLocationChanged;
            }
        }

        public string Search
        {
            get { return _search; }
            set
            {
                if (value == _search)
                    return;
                _search = value;
                CreateQuery();
            }
        }

        public ObservableCollection<StreetTypeViewModel> Types
        {
            get { return _types; }
            set
            {
                if (Equals(value, _types))
                    return;
                if (_types != null)
                    foreach (var streetTypeViewModel in _types)
                        streetTypeViewModel.PropertyChanged -= StreetTypeViewModelOnPropertyChanged;

                _types = value;

                foreach (var streetTypeViewModel in _types)
                    streetTypeViewModel.PropertyChanged += StreetTypeViewModelOnPropertyChanged;

                OnPropertyChanged();
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                if (value == _error)
                    return;
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        public string MaxDistanceRepresentation
        {
            get { return _maxDistanceRepresentation; }
            private set
            {
                if (value == _maxDistanceRepresentation)
                    return;
                _maxDistanceRepresentation = value;
                OnPropertyChanged(MaxDistanceRepresentation);
            }
        }

        /// <summary>Gets the current location. Changed by <see cref = "ILocationService"/></summary>
        public GeoPoint CurrentLocation
        {
            get { return _currentLocation; }
            private set
            {
                if (value.Equals(_currentLocation))
                    return;
                _currentLocation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets the maximum distance. Zero if we should skip this parameter.</summary>
        public double MaxDistance
        {
            get { return _maxDistance; }
            set
            {
                if (value.Equals(_maxDistance))
                    return;
                _maxDistance = value;
                _maxDistanceRepresentation = StreetViewModel.GetDistanceRepresentation(_maxDistance);
                OnPropertyChanged();
            }
        }

        /// <summary>Gets a value indicating whether check RegEx for alternative name.</summary>
        public bool CheckAlternativeName
        {
            get { return _checkAlternativeName; }
            set
            {
                if (value == _checkAlternativeName)
                    return;
                _checkAlternativeName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets the search query for creating RegEx. Created due to <see cref = "Search"/>.</summary>
        public Regex SearchQuery
        {
            get { return _searchQuery; }
            private set
            {
                if (Equals(value, _searchQuery))
                    return;
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        /// <summary>Gets the possible types. Empty if all types available. Created due to <see cref = "Types"/>.</summary>
        public IEnumerable<string> PossibleTypes
        {
            get { return _possibleTypes; }
            private set
            {
                if (Equals(value, _possibleTypes))
                    return;
                _possibleTypes = value;
                OnPropertyChanged(nameof(PossibleTypes));
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (_requiredRecalculatingProperty.Contains(propertyName))
                FilterChanged?.Invoke(this, new EventArgs());
        }

        private void StreetTypeViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            PossibleTypes = _types.Where(model => model.Enable).Select(model => model.Name).ToArray();
        }

        private void LocationServiceOnLocationChanged(object sender, EventArgs eventArgs)
        {
            CurrentLocation = _locationService.Location;
        }

        private void CreateQuery()
        {
            if (string.IsNullOrWhiteSpace(_search))
                SearchQuery = null;
            Error = null;
            try
            {
                SearchQuery = new Regex(Search, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            finally
            {
                if (Error != null)
                    OnPropertyChanged();
            }
        }
    }
}