using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Vortragsmanager.Models;
using Vortragsmanager.Views;

namespace Vortragsmanager.Core
{
    public static class DataContainer
    {
        private static int displayedYear = DateTime.Now.Year;

        public static CultureInfo German { get; } = new CultureInfo("de-DE");

        public static bool IsInitialized { get; set; }

        public static int Version { get; set; }

        public static ObservableCollection<Talk> Vorträge { get; } = new ObservableCollection<Talk>();

        public static Conregation MeineVersammlung { get; set; }

        public static ObservableCollection<Conregation> Versammlungen { get; } = new ObservableCollection<Conregation>();

        public static ObservableCollection<Speaker> Redner { get; } = new ObservableCollection<Speaker>();

        public static ObservableCollection<IEvent> MeinPlan { get; } = new ObservableCollection<IEvent>();

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

        public static Conregation FindOrAddConregation(string name)
        {
            var c = FindConregation(name);

            if (c == null)
            {
                c = new Conregation();
                c.Name = name;
                c.Kreis = MeineVersammlung.Kreis;
                Versammlungen.Add(c);
            }

            return c;
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

        public static Speaker FindOrAddSpeaker(string name, Conregation versammlung)
        {
            var s = FindSpeaker(name, versammlung);

            if (s == null)
            {

                s = new Speaker
                {
                    Name = name,
                    Versammlung = versammlung
                };
                Redner.Add(s);
            }
            return s;
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