using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoQuest;
using AutoQuest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoQuestTest
{
    [TestClass]
    public class FilteredStreetsTest
    {
        private SteetsViewModel _streets;

        [TestInitialize]
        public void TestInitialize()
        {
            var sl = new StreetLoader();
            var streets = sl.LoadStreets();
            _streets = new SteetsViewModel(streets, null);
        }

        [TestMethod]
        public async Task ShouldFindVDNHLocation()
        {
            var testFilter = new TestFilter() { SearchQuery = new Regex("Глушкова") };
            await _streets.OnFilterChanged(testFilter);
            var count = _streets.OrderedStreets.Count();
            var count2 = _streets.OrderedStreets.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task ShouldOrderByDistance()
        {
            var testFilter = new TestFilter() { CurrentLocation = GeoPointTests.HouseLocation };
            await _streets.OnFilterChanged(testFilter);
            var count = _streets.OrderedStreets.Count();

            var streets = _streets.OrderedStreets.ToArray();
            for (int index = 0; index < streets.Length - 1; index++)
            {
                var street = streets[index];
                var nextStreet = streets[index + 1];
                Assert.IsTrue(street.Distance <= nextStreet.Distance);
            }
            Assert.AreEqual(1800, count);
        }

        [TestMethod]
        public async Task ShouldHideFarStreets()
        {
            var testFilter = new TestFilter() { CurrentLocation = GeoPointTests.HouseLocation , MaxDistance = 0.5};
            await _streets.OnFilterChanged(testFilter);
            var count = _streets.OrderedStreets.Count();

            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public async Task ShouldCancelRecalculating()
        {
            var testFilter1 = new TestFilter() { CurrentLocation = GeoPointTests.HouseLocation };
            var testFilter2 = new TestFilter() { CurrentLocation = GeoPointTests.HouseLocation, MaxDistance = 1 };
            var testFilter3 = new TestFilter() { CurrentLocation = GeoPointTests.HouseLocation, MaxDistance = 0.5 };
            var notificationCount = 0;
            _streets.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OrderedStreets")
                    notificationCount++;
            };
            await _streets.OnFilterChanged(testFilter1);
            var canceledRequest = _streets.OnFilterChanged(testFilter2);
            var finalRequest = _streets.OnFilterChanged(testFilter3);
            await canceledRequest;
            await finalRequest;
            var afterfinalRequestFinished = _streets.OrderedStreets.Count();

            Assert.AreEqual(4, afterfinalRequestFinished);
            Assert.AreEqual(2, notificationCount);
        }
    }
}
