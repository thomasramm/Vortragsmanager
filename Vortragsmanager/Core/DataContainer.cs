using System;
using Vortragsmanager.Models;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Views;
using System.Globalization;

namespace Vortragsmanager.Core
{
    public static class DataContainer
    {
        private static int displayedYear = DateTime.Now.Year;

        public static CultureInfo German { get; } = new CultureInfo("de-DE");

        public static bool IsInitialized { get; set; }

        public static ObservableCollection<Talk> Vorträge { get; } = new ObservableCollection<Talk>();

        public static Conregation MeineVersammlung { get; set; }

        public static ObservableCollection<Conregation> Versammlungen { get; } = new ObservableCollection<Conregation>();

        public static ObservableCollection<Speaker> Redner { get; } = new ObservableCollection<Speaker>();

        public static ObservableCollection<Invitation> MeinPlan { get; } = new ObservableCollection<Invitation>();

        public static ObservableCollection<Outside> ExternerPlan { get; } = new ObservableCollection<Outside>();
                       
        public static Conregation FindConregation(string name)
        {
            foreach (var c in Versammlungen)
            {
                if (c.Name == name)
                    return c;
            }

            return null;
        }

        public static Talk FindTalk(int Nummer)
        {
            foreach (var t in Vorträge)
            {
                if (t.Nummer == Nummer)
                    return t;
            }

            return null;
        }

        public static Speaker FindSpeaker(string name, Conregation versammlung)
        {
            foreach (var s in Redner)
            {
                if (s.Name == name && s.Versammlung == versammlung)
                    return s;
            }

            return null;
        }

        public static int DisplayedYear 
        {
            get => displayedYear;
            set 
            { 
                displayedYear = value;
                Messenger.Default.Send(Messages.DisplayYearChanged);
            }
        }
    }
}
