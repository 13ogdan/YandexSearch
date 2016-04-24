using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoQuest;
using AutoQuest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoQuestTest
{
    public class TestFilter : IFilter
    {
        /// <summary>Gets the search query for creating RegEx.</summary>
        public Regex SearchQuery { get; }

        /// <summary>Gets the maximum distance. Zero if we should skip this parameter.</summary>
        public double MaxDistance { get; }

        /// <summary>Gets a value indicating whether check RegEx for alternative name.</summary>
        public bool CheckAlternativeName { get; }

        /// <summary>Gets the possible types. Empty if all types available.</summary>
        public IEnumerable<string> PossibleTypes { get; }

        /// <summary>Gets the current location.</summary>
        public GeoPoint CurrentLocation { get; set; }
    }

    /// <summary>
    /// Summary description for StreetVMTest
    /// </summary>
    [TestClass]
    public class StreetVMTest
    {
        [TestMethod]
        public async Task ShouldDistanceBeRepresentedInMeters()
        {
            var street = new Street
            {
                Name = "Глушкова",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new StreetVM(street);

            var mock = new TestFilter {CurrentLocation = GeoPointTests.HouseLocation};
            await streetVm.UpdateStates(mock);
            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
            Assert.AreEqual($"{GeoPointTests.DistanceByYandex} м.", streetVm.Distance);
        }
    }
}
