// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutoQuest.ViewModels
{
    public class Street : BaseViewModel
    {
        private readonly GeoPoint _centerStreetPoint;
        private readonly AutoQuest.Street _street;
        private double _distance;
        private string _distance1 = string.Empty;
        private bool _isVisible = true;
        private string _name;

        public Street(AutoQuest.Street street)
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
            get { return _distance1; }
            private set
            {
                if (value == _distance1)
                    return;
                _distance1 = value;
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
            DistanceRepr = _distance >= 1 ? $"{_distance:F2} км." : $"{Math.Round(_distance * 1000)} м.";

            return !(maxDistance > 0) || _distance <= maxDistance;
        }
    }
}