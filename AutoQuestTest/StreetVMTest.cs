using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoQuest;
using AutoQuest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Street = AutoQuest.ViewModels.Street;

namespace AutoQuestTest
{
    public class TestFilter : IFilter
    {
        /// <summary>Gets the search query for creating RegEx.</summary>
        public Regex SearchQuery { get; set; }

        /// <summary>Gets the maximum distance. Zero if we should skip this parameter.</summary>
        public double MaxDistance { get; set; }

        /// <summary>Gets a value indicating whether check RegEx for alternative name.</summary>
        public bool CheckAlternativeName { get; set; }

        /// <summary>Gets the possible types. Empty if all types available.</summary>
        public IEnumerable<string> PossibleTypes { get; set; }

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
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);

            var mock = new TestFilter { CurrentLocation = GeoPointTests.HouseLocation };
            streetVm.UpdateStates(mock);
            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
            Assert.AreEqual($"{GeoPointTests.DistanceByYandex} м.", streetVm.DistanceRepr);
        }

        [TestMethod]
        public async Task ShouldStreetBeInvisibleBecauseOfMaxDistance()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);

            var mock = new TestFilter
            {
                CurrentLocation = GeoPointTests.HouseLocation,
                MaxDistance = 0.8
            };
            streetVm.UpdateStates(mock);
            Assert.IsFalse(streetVm.IsVisible, "streetVm.IsVisible");
        }

        [TestMethod]
        public async Task ShouldHideStreetBecauseOfStreetType()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Type = "проспект",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);
            var mock = new TestFilter { CurrentLocation = GeoPointTests.HouseLocation, PossibleTypes = new[] { "улица" } };
            streetVm.UpdateStates(mock);
            var isVisibleWithoutCorrectType = streetVm.IsVisible;
            mock.PossibleTypes = new[] { "улица", "проспект" };
            streetVm.UpdateStates(mock);

            Assert.IsFalse(isVisibleWithoutCorrectType, "streetVm.IsVisible");
            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
        }

        [TestMethod]
        public async Task ShouldFindStreetByFirstLetters()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Type = "проспект",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);
            var mock = new TestFilter
            {
                CurrentLocation = GeoPointTests.HouseLocation,
                PossibleTypes = new List<string>() {"проспект"},
                SearchQuery = new Regex("^глу",RegexOptions.IgnoreCase)
            };
            streetVm.UpdateStates(mock);

            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
        }

        [TestMethod]
        public async Task ShouldFindStreetByFirstLetters2()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Type = "проспект",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);
            var mock = new TestFilter
            {
                CurrentLocation = GeoPointTests.HouseLocation,
                PossibleTypes = new List<string>() { "проспект" },
                SearchQuery = new Regex("глу", RegexOptions.IgnoreCase)
            };
            streetVm.UpdateStates(mock);

            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
        }

        [TestMethod]
        public async Task ShouldFindStreetByLastLetters()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Type = "проспект",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);
            var mock = new TestFilter
            {
                CurrentLocation = GeoPointTests.HouseLocation,
                PossibleTypes = new List<string>() { "проспект" },
                SearchQuery = new Regex("ва", RegexOptions.IgnoreCase)
            };
            streetVm.UpdateStates(mock);

            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
        }

        [TestMethod]
        public async Task ShouldFindStreetMatchInTheMiddle()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Type = "проспект",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);
            var mock = new TestFilter
            {
                CurrentLocation = GeoPointTests.HouseLocation,
                PossibleTypes = new List<string>() { "проспект" },
                SearchQuery = new Regex("ш.о", RegexOptions.IgnoreCase)
            };
            streetVm.UpdateStates(mock);

            Assert.IsTrue(streetVm.IsVisible, "streetVm.IsVisible");
        }

        [TestMethod]
        public async Task ShouldNotFindStreetMatchInTheMiddle()
        {
            var street = new AutoQuest.Street
            {
                Name = "Глушкова",
                Type = "проспект",
                Lat = GeoPointTests.VDNHLocation.Lat,
                Long = GeoPointTests.VDNHLocation.Long
            };
            var streetVm = new Street(street);
            var mock = new TestFilter
            {
                CurrentLocation = GeoPointTests.HouseLocation,
                PossibleTypes = new List<string>() { "проспект" },
                SearchQuery = new Regex("ш..о", RegexOptions.IgnoreCase)
            };
            streetVm.UpdateStates(mock);

            Assert.IsFalse(streetVm.IsVisible, "streetVm.IsVisible");
        }
    }

}
