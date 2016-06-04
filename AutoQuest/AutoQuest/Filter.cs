// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoQuest
{
    public interface IFilter
    {
        /// <summary>Gets the search query for creating RegEx.</summary>
        Regex SearchQuery { get; }

        /// <summary>Gets the maximum distance. Zero if we should skip this parameter.</summary>
        double MaxDistance { get; }

        /// <summary>Gets a value indicating whether check RegEx for alternative name.</summary>
        bool CheckAlternativeName { get; }

        /// <summary>Gets the possible types. Empty if all types available.</summary>
        IEnumerable<string> PossibleTypes { get; }

        /// <summary>Gets the current location.</summary>
        GeoPoint CurrentLocation { get; }
    }
}