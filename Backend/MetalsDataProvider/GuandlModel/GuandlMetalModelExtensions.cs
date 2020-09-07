﻿using MetalsDataProvider.ReadModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalsDataProvider.GuandlModel
{
    internal static partial class GuandlMetalModelExtensions
    {
        internal static GuandlMetalModel Deserialize(this string json)
        {
            var metalDataJson = ExtractDailyMetalPricesFromExternalJson(json);
            var metalData = JsonConvert.DeserializeObject<GuandlMetalModel>(metalDataJson);

            return metalData;
        }

        internal static MetalPrices Map(this GuandlMetalModel guandlMetalDataModel)
        {
            return new MetalPrices
            {
                Prices = GetDailyGoldPricesFromExternalData(guandlMetalDataModel.Data)
                .Select(d => new MetalPriceDateTime
                {
                    DateTime = d.Key,
                    Price = d.Value
                }).ToList()
            };
        }

        private static Dictionary<DateTime, double> GetDailyGoldPricesFromExternalData(List<List<object>> data)
        {
            var dailyGoldPrices = new Dictionary<DateTime, double>();

            foreach (var dayData in data)
            {
                if (!DateTime.TryParse(dayData.First().ToString(), out DateTime key) ||
                    !double.TryParse(dayData.Last().ToString(), out double value)) continue;

                dailyGoldPrices.Add(key, value);
            }

            return dailyGoldPrices;
        }

        private static string ExtractDailyMetalPricesFromExternalJson(string json)
        {
            var allChildren = AllChildren(JObject.Parse(json));

            var metalDataJsonata = allChildren
                .First(c => c.Path.Contains("dataset"))
                .Children<JObject>()
                .First()
                .ToString();

            return metalDataJsonata;
        }

        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;

                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }
    }
}
