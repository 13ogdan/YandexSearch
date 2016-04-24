// <copyright>☺ Raccoon corporation ☻</copyright>

using System;
using System.Globalization;

namespace AutoQuest
{
    public struct GeoPoint : IFormattable
    {
        private const double Radian = Math.PI/180;
        private readonly double _long;
        private readonly double _lat;
        private readonly string _degreeFormat;
        private double _l;
        public const double Epsilon = 0.0001;
        private const double EarthRadius = 6378.1;

        public GeoPoint(double lat, double @long) : this()
        {
            _long = @long;
            _lat = lat;

            _degreeFormat = $"{ConvertToDegree(_lat)}N, {ConvertToDegree(_long)}E";
        }

        private string ConvertToDegree(double degreeInDecimal)
        {
            var hours = Math.Floor(degreeInDecimal);
            var mantissa = degreeInDecimal - hours;
            var minutes = Math.Floor(60*mantissa);
            mantissa = 60*mantissa - minutes;
            var seconds = Math.Round(60*mantissa);
            return $"{hours:00}°{minutes:00}′{seconds:00}″";
        }

        public bool IsEmpty => _long == default(double) && _lat == default(double);

        public double Lat => _lat;

        public double Long => _long;

        public double DistanceTo(GeoPoint p)
        {
            var dlong = (p._long - _long)*Radian;
            var a = Math.Pow(Math.Cos(p._lat*Radian)*Math.Sin(dlong), 2);
            var b = Math.Cos(_lat*Radian)*Math.Sin(p._lat*Radian) - Math.Sin(_lat*Radian)*Math.Cos(p._lat*Radian)*Math.Cos(dlong);
            var c = Math.Sin(_lat*Radian)*Math.Sin(p._lat*Radian) + Math.Cos(_lat*Radian)*Math.Cos(p._lat*Radian)*Math.Cos(dlong);
            var d = Math.Atan(Math.Sqrt(a + b*b)/c)*EarthRadius;
            return d;
        }

        //public double DistanceTo(GeoPoint p)
        //{
        //    var dlong = (p._long - _long) * radian;
        //    var dlat = (p._lat - _lat) * radian;
        //    var a = Pow(Sin(dlat / 2.0), 2) + Cos(_lat * radian) * Cos(p._lat * radian) * Pow(Sin(dlong / 2.0), 2);
        //    var c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
        //    var d = 6367 * c;
        //    return d;
        //}

        /// <summary>
        /// Возвращает полное имя типа этого экземпляра.
        /// </summary>
        /// <returns>
        /// Объект типа <see cref = "T:System.String"/>, содержащий полное имя типа.
        /// </returns>
        public override string ToString()
        {
            return ToString(string.Empty);
        }

        /// <summary>
        /// Форматирует значение текущего экземпляра с использованием заданного формата.
        /// </summary>
        /// <returns>
        /// Значение текущего экземпляра в указанном формате.
        /// </returns>
        /// <param name = "format">
        /// Используемый формат. -или-  Пустая ссылка (Nothing в Visual Basic) для использования формата по умолчанию, определенного для типа
        /// реализации<see cref = "T:System.IFormattable"/>.
        /// </param>
        /// <param name = "formatProvider">
        /// Поставщик, используемый для форматирования значения. -или-  Пустая ссылка (Nothing в Visual Basic) для получения сведений о формате чисел
        /// из текущего установленного в операционной системе языкового стандарта.
        /// </param>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            switch (format.ToUpper())
            {
                case "D":
                    return _degreeFormat;
                default:
                    return string.Format(formatProvider ?? CultureInfo.InvariantCulture, "{0:00.000000}, {1:00.000000}", _lat, _long);
            }
        }
    }
}