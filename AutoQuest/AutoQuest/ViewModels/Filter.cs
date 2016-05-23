// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AutoQuest.API;

namespace AutoQuest.ViewModels
{
    public class Filter : BaseViewModel, IFilter
    {
        private readonly ILocationService _locationService;
        private bool _checkAlternativeName;
        private ObservableCollection<string> _checkedItems;
        private GeoPoint _currentLocation;
        private double _maxDistance;
        private string _search;
        private Regex _searchQuery;
        private string _error;

        public event EventHandler FilterChanged;

        public Filter()
        {

        }

        public Filter(ILocationService locationService)
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

        public ObservableCollection<string> CheckedItems
        {
            get { return _checkedItems; }
            set
            {
                if (Equals(value, _checkedItems))
                    return;

                _checkedItems = value;
                PossibleTypes = _checkedItems;
                OnPropertyChanged();
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

        /// <summary>Gets the possible types. Empty if all types available. Created due to <see cref = "CheckedItems"/>.</summary>
        public IEnumerable<string> PossibleTypes { get; private set; }

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

        public string Error { get { return _error; } set { _error = value; } }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            FilterChanged?.Invoke(this, new EventArgs());
        }
    }
}