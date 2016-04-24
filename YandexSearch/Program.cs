using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Yandex;

namespace YandexSearch
{
    class Program
    {
        public const string KievPrefix = "Украина, Киев, ";

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("streets.txt");
            var list = lines.Select(line => new Street(line)).ToList();
            list = new List<Street>(list.OrderBy(street => street.Name));
            var dic = FindSameStreet(list).Where(pair => pair.Value.Count > 1);
            Console.WriteLine("Start read coordinate");
            Console.ReadKey(true);
            using (var progresBar = new ConsoleProgress("Update Yandex coordinate"))
            {
                progresBar.SetRange(0, list.Count);
                for (int index = 0; index < list.Count; index++)
                {
                    var street = list[index];
                    var location = $"{KievPrefix}{street}";
                    var geoObjectCollection = YandexGeocoder.Geocode(location, 1, LangType.ru_RU);
                    street.UpdateLocation(geoObjectCollection);
                    progresBar.SetProgress(index);
                }
            }
           
            var validStreets = list.Where(street => street.CoordinatesCalculated).ToArray();
            var invalidStreets = list.Where(street => !street.CoordinatesCalculated).ToArray();
            Console.WriteLine($"Found {invalidStreets.Length} not valid streets. You can find it in Invalid.json");
            SerializeStreets(invalidStreets, "Invalid");
            Console.WriteLine("Write file name for output");
            var fileName = Console.ReadLine();
            SerializeStreets(validStreets, fileName);
        }

        private static void SerializeStreets(Street[] streets, string fileName)
        {
            var contractJsonSerializer = new DataContractJsonSerializer(typeof(Street));
            using (var progresBar = new ConsoleProgress($"Serialize into {fileName}."))
            {
                progresBar.SetRange(0, streets.Length);
                using (var sw = new FileStream(Path.ChangeExtension(fileName, ".json"), FileMode.Create))
                {
                    for (int index = 0; index < streets.Length; index++)
                    {
                        var validStreet = streets[index];
                        contractJsonSerializer.WriteObject(sw, validStreet);
                        progresBar.SetProgress(index);
                    }
                }
            }
        }

        private static Dictionary<string, List<Street>> FindSameStreet(List<Street> list)
        {
            var dictionary = new Dictionary<string, List<Street>>();
            foreach (var street in list)
            {
                var key = street.ToString();
                if (dictionary.ContainsKey(key))
                    continue;
                dictionary[key] = new List<Street>();
                dictionary[key].Add(street);
                foreach (var street2 in list)
                {
                    if (street2 == street)
                        continue;
                    if (street2.ToString() == key)
                        dictionary[key].Add(street2);
                }
            }
            int totalCount = 0;
            foreach (var name in dictionary)
            {
                var count = name.Value.Count;
                totalCount += count;
                if (count <= 1)
                    continue;

                Console.WriteLine($"{name.Key} {count}");
                foreach (var street in name.Value)
                {
                    street.WriteToConsole();
                }
            }

            if (list.Count > totalCount)
                throw new Exception("wrong parsing");

            return dictionary;
        }
    }
}
