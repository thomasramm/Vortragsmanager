using DevExpress.Xpf.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows;
using Vortragsmanager.Models;

namespace Vortragsmanager.Core
{
    public static class GeoApi
    {
        private const string url = "https://maps.googleapis.com/maps/api/directions/json?origin={START}&destination={ZIEL}&key={KEY}";

        public static int? GetDistance(string startAdress, string endAdress)
        {
            Log.Info(nameof(GetDistance), $"start={startAdress}, end={endAdress}");

            if (string.IsNullOrWhiteSpace(startAdress))
                return 0;
            if (string.IsNullOrWhiteSpace(endAdress))
                return 0;

            startAdress = startAdress.Replace(" ", "+");
            endAdress = endAdress.Replace(" ", "+");
            var myUrl = url
                .Replace("{START}", startAdress)
                .Replace("{ZIEL}", endAdress)
                .Replace("{KEY}", Security.GoogleApiKey);
            var gString = string.Empty;

            using (WebClient client = new WebClient())
            {
                gString = client.DownloadString(myUrl);
            }

            var json = JObject.Parse(gString);

            if (json["status"].Value<string>() == "OK")
            {
                var routes = json["routes"];
                var legs = routes[0]["legs"];
                var distance = legs[0]["distance"];
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
            Log.Info(nameof(GetDistance), $"start={start.Name}, end={end.Name}");

            if (start == null || end == null)
                return null;

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