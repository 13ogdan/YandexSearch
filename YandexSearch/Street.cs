// <copyright>3Shape A/S</copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Yandex;

namespace YandexSearch
{
    [DataContract]
    public class Street
    {
        private const char Separator = '\t';
        private GeoPoint _coordinatesCalculated;
        private List<HouseNumber> _houses = new List<HouseNumber>();

        public Street(string line)
        {
            const string country = "Украина,";
            const string city = "Киев,";

            // Fully clarified line
            if (line.StartsWith(country))
            {
                line = line.Remove(0, country.Length).Trim();
                if (line.StartsWith(city))
                    line = line.Remove(0, city.Length).Trim();
                else
                {
                    IsValid = false;
                    return;
                }
            }

            var lines = line.Split(Separator);
            ParseName(lines[0]);

            if (lines.Length > 1)
                ParseNumbers(lines[1]);
        }

        [DataMember(IsRequired = true, Order = 1)]
        public string Name { get; private set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string Type { get; private set; }

        [DataMember(IsRequired = false, Order = 3, EmitDefaultValue = false)]
        public string District { get; private set; }

        [DataMember(IsRequired = true, Order = 4)]
        public double Long { get { return _coordinatesCalculated.Long; } set { _coordinatesCalculated.Long = value; } }

        [DataMember(IsRequired = true, Order = 5)]
        public double Lat { get { return _coordinatesCalculated.Lat; } set { _coordinatesCalculated.Lat = value; } }

        [DataMember(IsRequired = false, Order = 6, EmitDefaultValue = false)]
        public string AltName { get; private set; }

        [DataMember(IsRequired = false, Order = 7, EmitDefaultValue = false)]
        public string AltType { get; private set; }

        public bool CoordinatesCalculated => (
            !Equals(_coordinatesCalculated, default(GeoPoint))
            );

        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var representation = District != null ? $"{Name} {Type}, {District}" : $"{Name} {Type}";
            if (AltName != null || AltType != null)
                representation += $"({AltName ?? Name}, {AltType ?? Type})";
            return representation;
        }

        public Tuple<int, int> GetMaxMinNumber()
        {
            return null;
        }

        public void WriteToConsole()
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Name);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($" {Type}");
            if (_houses.Any())
            {
                Console.WriteLine();
                foreach (var houseNumber in _houses)
                {
                    Console.Write($"{houseNumber.Name}({houseNumber.Number})");
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = oldColor;
        }

        public void UpdateLocation(GeoObjectCollection geoObjectCollection)
        {
            foreach (var geo in geoObjectCollection)
            {
                if (CheckLocation(geo.GeocoderMetaData.Text))
                {
                    _coordinatesCalculated = geo.Point;
                    continue;
                }

                var formattableString = $"{Program.KievPrefix}{this}\n{geo.GeocoderMetaData.Text}\n\n";
                File.AppendAllText("WrongStreet.txt", formattableString);
            }
        }

        private void ParseName(string nameAndType)
        {
            var input = nameAndType;
            // Remove district if exists
            if (nameAndType.Contains(','))
            {
                var lastIndexOf = nameAndType.LastIndexOf(',');
                District = nameAndType.Substring(0, lastIndexOf).Trim();
                nameAndType = nameAndType.Remove(0, lastIndexOf + 1).Trim();
            }
            // find word which started not from Capital or Number
            var typeIndex = 0;
            var canBeFirst = true;
            for (var index = 0; index < nameAndType.Length; index++)
            {
                var c = nameAndType[index];
                if (c == ' ')
                {
                    canBeFirst = true;
                    continue;
                }
                if (!canBeFirst)
                    continue;
                canBeFirst = false;
                if (char.IsLetter(c) && char.IsLower(c))
                {
                    typeIndex = index;
                    break;
                }
            }
            var end = nameAndType.IndexOf(' ', typeIndex);
            if (end == -1)
            {
                Type = nameAndType.Substring(typeIndex).Trim();
                Name = nameAndType.Remove(typeIndex).Trim();
            }
            else
            {
                Type = nameAndType.Substring(typeIndex, end - typeIndex + 1).Trim();
                Name = nameAndType.Remove(typeIndex, end - typeIndex + 1).Trim();
            }

            if (IsValid)
                IsValid = !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Type));
        }

        private void ParseNumbers(string numbersLine)
        {
            if (numbersLine == "null" || string.IsNullOrWhiteSpace(numbersLine))
                return;
            var numbersArray = numbersLine.Split(',');
            foreach (var s in numbersArray)
            {
                _houses.Add(new HouseNumber(s));
            }
            _houses = new List<HouseNumber>(_houses.OrderBy(number => number.Number));
        }

        private bool CheckLocation(string text)
        {
            var test = new Street(text);
            if (!test.IsValid)
                return false;

            if (test.Name != Name)
            {
                AltName = Name;
                Name = test.Name;
            }

            if (test.Type != Type)
            {
                AltType = Type;
                Type = test.Type;
            }

            if (test.District == null)
                return true;

            if (District == null)
                District = test.District;

            return true;
        }
    }
}