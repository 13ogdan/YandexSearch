// <copyright>☺ Raccoon corporation ☻</copyright>

using System;
using AutoQuest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoQuestTest
{
    [TestClass]
    public class GeoPointTests
    {
        public const string VDNHLatitude = "50°22′56″N";
        public const string VDNHLongitude = "30°28′38″E";
        public const double DistanceByYandex = 915;
        public static GeoPoint VDNHLocation = new GeoPoint(50.382092, 30.477134);
        public static GeoPoint HouseLocation = new GeoPoint(50.385768, 30.465608);

        [TestMethod]
        public void ShouldDistanceBeSame()
        {
            var distance = VDNHLocation.DistanceTo(HouseLocation);
            var distance2 = HouseLocation.DistanceTo(VDNHLocation);
            Assert.AreEqual(distance, distance2, GeoPoint.Epsilon);
        }

        [TestMethod]
        public void ShouldDistanceBeCorrect()
        {
            var distance = VDNHLocation.DistanceTo(HouseLocation);
            var distanceInMeter = Math.Ceiling(distance*1000);
            Assert.AreEqual(DistanceByYandex, distanceInMeter);
        }

        [TestMethod]
        public void ShouldBePossibleToGetStringInTwoFormats()
        {
            var expectedSimple = "50.382092, 30.477134";
            var expectedInDegrees = $"{VDNHLatitude}, {VDNHLongitude}";
            Assert.AreEqual(expectedSimple, VDNHLocation.ToString());
            Assert.AreEqual(expectedInDegrees, VDNHLocation.ToString("D"));
        }
    }
}