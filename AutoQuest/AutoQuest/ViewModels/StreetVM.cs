﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoQuest.ViewModels
{
    public class StreetVM
    {
        private readonly Street _street;
        private double _distance;
        private readonly GeoPoint _centerStreetPoint;

        public StreetVM(Street street)
        {
            _street = street;
            _centerStreetPoint = new GeoPoint(_street.Lat, _street.Long);
            Name = street.ToString();
        }

        public async Task UpdateStates(IFilter filter)
        {
            await Task.Yield();
            var visible = CalculateDistance(filter.CurrentLocation, filter.MaxDistance);
            visible = visible && CheckType(filter.PossibleTypes);
            visible = visible && CheckName(filter.SearchQuery,filter.CheckAlternativeName);
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
            if (!possibleTypes.Any())
                return true;

            return possibleTypes.Contains(_street.Type) || (_street.AltType != null && possibleTypes.Contains(_street.AltType));
        }

        private bool CalculateDistance(GeoPoint currentLocation, double maxDistance)
        {
            if (currentLocation.IsEmpty)
            {
                _distance = 0;
                Distance = string.Empty;
                return true;
            }

            _distance = currentLocation.DistanceTo(_centerStreetPoint);
            Distance = _distance >= 1000.0 ? $"{_distance / 1000:F2} км." : $"{_distance} м.";

            return !(maxDistance > 0) || _distance <= maxDistance;
        }

        public bool IsVisible { get; private set; } = true;

        public string Distance { get; private set; } = string.Empty;

        public string Name { get; private set; }

    }
}
