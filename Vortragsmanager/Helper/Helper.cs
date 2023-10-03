using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;

namespace Vortragsmanager.Helper
{
    internal class Helper
    {
        public static MyGloabalSettings GlobalSettings { get; set; }

        private static int _displayedYear = DateTime.Now.Year;

        public static int DisplayedYear
        {
            get => _displayedYear;
            set
            {
                _displayedYear = value;
                Messenger.Default.Send(_displayedYear, Messages.DisplayYearChanged);
            }
        }

        public static CultureInfo German { get; } = new CultureInfo("de-DE");

        public static string TemplateFolder => AppDomain.CurrentDomain.BaseDirectory + @"Templates\";

        public static bool CheckNegativListe(string input, string[] liste)
        {
            var u = input.ToUpperInvariant();
            return liste.All(x => x != u);
        }

        public static bool StyleIsDark => Helper.GlobalSettings.ThemeIsDark;

        public static string AppFolderPath
        {
            get
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + @"\Vortragsmanager DeLuxe\";
                Directory.CreateDirectory(folder);
                return folder;
            }
        }
    }
}
