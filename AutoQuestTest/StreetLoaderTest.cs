using System;
using System.Linq;
using AutoQuest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoQuestTest
{
    [TestClass]
    public class StreetLoaderTest
    {
        [TestMethod]
        public void ShouldLoadStreets()
        {
            var sl = new StreetLoader();
            var streets = sl.LoadStreets();
            Assert.AreEqual(1800, streets.Count());
        }
    }
}
