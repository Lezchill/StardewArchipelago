﻿//using System.Collections.Generic;
//using System.IO;
//using Newtonsoft.Json.Linq;
//using StardewModdingAPI;

//namespace StardewArchipelago.Archipelago
//{
//    public class ObtainableArchipelagoLocation
//    {
//        public string Name { get; set; }
//        public long Id { get; set; }
//        public string Region { get; set; }

//        public ObtainableArchipelagoLocation(string name, long id, string region)
//        {
//            Name = name;
//            Id = id;
//            Region = region;
//        }

//        public static IEnumerable<ObtainableArchipelagoLocation> LoadLocations(IModHelper helper)
//        {
//            var pathToLocationTable = Path.Combine("IdTables", "stardew_valley_location_table.json");
//            var locationsTable = helper.Data.ReadJsonFile<Dictionary<string, JObject>>(pathToLocationTable);
//            var locations = locationsTable["locations"];
//            foreach (var (key, jEntry) in locations)
//            {
//                yield return LoadLocation(key, jEntry);
//            }
//        }

//        private static ObtainableArchipelagoLocation LoadLocation(string locationName, JToken locationJson)
//        {
//            var id = locationJson["code"].Value<long>();
//            var region = locationJson["region"].Value<string>();
//            var location = new ObtainableArchipelagoLocation(locationName, id, region);
//            return location;
//        }
//    }
//}

