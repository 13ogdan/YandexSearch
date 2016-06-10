// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace AutoQuest.ViewModels
{
    public class StreetViewModel : BaseViewModel
    {
        private readonly GeoPoint _centerStreetPoint;
        private readonly Street _street;
        private double _distance;
        private string _distanceRepr = string.Empty;
        private bool _isVisible = true;
        private string _name;
        private Command<StreetViewModel> _navigateCommand;
        
        public StreetViewModel(Street street)
        {
            _street = street;
            _centerStreetPoint = new GeoPoint(_street.Lat, _street.Long);
            Name = street.ToString();
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            private set
            {
                if (value == _isVisible)
                    return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string DistanceRepr
        {
            get { return _distanceRepr; }
            private set
            {
                if (value == _distanceRepr)
                    return;
                _distanceRepr = value;
                OnPropertyChanged();
            }
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

        public double Distance => _distance;

        public Command<StreetViewModel> NavigateCommand
        {
            get { return _navigateCommand; }
            set
            {
                if (Equals(value, _navigateCommand))
                    return;
                _navigateCommand = value;
                OnPropertyChanged();
            }
        }

        public GeoPoint CenterStreetPoint { get { return _centerStreetPoint; } }

        public static string GetDistanceRepresentation(double distance)
        {
            if (distance <= 0)
                return string.Empty;

            return distance >= 1 ? $"{distance:F2} км." : $"{Math.Round(distance*1000)} м.";
        }

        public void UpdateStates(IFilter filter)
        {
            var visible = CalculateDistance(filter.CurrentLocation, filter.MaxDistance);
            visible = visible && CheckType(filter.PossibleTypes);
            visible = visible && CheckName(filter.SearchQuery, filter.CheckAlternativeName);
            IsVisible = visible;
        }

        private bool CheckName(Regex searchQuery, bool checkAlternativeName)
        {
            if (searchQuery == null)
                return true;
            var result = searchQuery.IsMatch(_street.Name);

            if (!result && checkAlternativeName && _street.AltType != null)
                result = searchQuery.IsMatch(_street.AltType);

            return result;
        }

        private bool CheckType(IEnumerable<string> possibleTypes)
        {
            if (possibleTypes == null || !possibleTypes.Any())
                return true;

            return possibleTypes.Contains(_street.Type) || (_street.AltType != null && possibleTypes.Contains(_street.AltType));
        }

        private bool CalculateDistance(GeoPoint currentLocation, double maxDistance)
        {
            if (currentLocation.IsEmpty)
            {
                _distance = 0;
                DistanceRepr = string.Empty;
                return true;
            }

            _distance = currentLocation.DistanceTo(_centerStreetPoint);
            DistanceRepr = GetDistanceRepresentation(_distance);

            return !(maxDistance > 0) || _distance <= maxDistance;
        }
    }
}