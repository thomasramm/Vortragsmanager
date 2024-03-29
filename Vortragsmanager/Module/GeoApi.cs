﻿using System;
using System.Net;
using System.Windows;
using DevExpress.Xpf.Core;
using Newtonsoft.Json.Linq;
using Vortragsmanager.DataModels;

namespace Vortragsmanager.Module
{
    public static class GeoApi
    {
        private const string Url = "https://maps.googleapis.com/maps/api/directions/json?origin={START}&destination={ZIEL}&key={KEY}";

        public static int? GetDistance(string startAdress, string endAdress)
        {
            Log.Info(nameof(GetDistance), $"start={startAdress}, end={endAdress}");

            if (string.IsNullOrWhiteSpace(startAdress))
                return 0;
            if (string.IsNullOrWhiteSpace(endAdress))
                return 0;

            startAdress = startAdress.Replace(" ", "+");
            endAdress = endAdress.Replace(" ", "+");
            var myUrl = Url
                .Replace("{START}", startAdress)
                .Replace("{ZIEL}", endAdress)
                .Replace("{KEY}", Security.GoogleApiKey);
            string gString;

            using (var client = new WebClient())
            {
                gString = client.DownloadString(myUrl);
            }

            var json = JObject.Parse(gString);

            if ((json["status"] ?? "Fehler").Value<string>() == "OK")
            {
                var routes = json["routes"];
                var legs = routes?[0]?["legs"];
                var distance = legs?[0]?["distance"];
                if (distance == null) 
                    return null;

                var value = distance["value"];
                var km = ((int)value + 500) / 1000;
                return km;
            }
            else
            {
                ThemedMessageBox.Show(
                    "Fehler bei der Routenberechnung",
                    "Es konnte keine Route berechnet werden. " + Environment.NewLine +
                    $"von: {startAdress}" + Environment.NewLine +
                    $"nach: {endAdress}" + Environment.NewLine +
                    "Bitte später nochmal versuchen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }
        }

        public static int? GetDistance(Conregation start, Conregation end)
        {
            if (start is null || end is null)
            {
                Log.Info(nameof(GetDistance), "start or end is null");
                return null;
            }
            Log.Info(nameof(GetDistance), $"start={start.Name}, end={end.Name}");

            var startAdress = start.Anschrift1 + " " + start.Anschrift2;
            if (string.IsNullOrWhiteSpace(startAdress))
                startAdress = start.Name;

            var endAdress = end.Anschrift1 + " " + end.Anschrift2;
            if (string.IsNullOrWhiteSpace(endAdress))
                endAdress = end.Name;

            return GetDistance(startAdress, endAdress);
        }
    }
}