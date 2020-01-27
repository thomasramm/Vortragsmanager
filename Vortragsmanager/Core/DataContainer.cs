using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
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

        public static ObservableCollection<Inquiry> OffeneAnfragen { get; } = new ObservableCollection<Inquiry>();

        public static ObservableCollection<Outside> ExternerPlan { get; } = new ObservableCollection<Outside>();

        public static ObservableCollection<Cancelation> Absagen { get; } = new ObservableCollection<Cancelation>();

        public static Conregation FindConregation(string name)
        {
            Log.Info(nameof(FindConregation), $"name={name}");
            foreach (var c in Versammlungen)
            {
                if (c.Name == name)
                    return c;
            }

            return null;
        }

        public static Conregation FindConregation(string name, int kreis)
        {
            Log.Info(nameof(FindConregation), $"name={name}, kreis={kreis}");
            foreach (var c in Versammlungen)
            {
                if (c.Name == name && c.Kreis == kreis)
                    return c;
            }

            return null;
        }

        public static Conregation FindOrAddConregation(string name)
        {
            var c = FindConregation(name);

            if (c == null)
            {
                Log.Info(nameof(FindOrAddConregation), $"add={name}");
                c = new Conregation
                {
                    Name = name,
                    Kreis = MeineVersammlung.Kreis,
                    Id = Versammlungen.Count > 0 ? Versammlungen.Select(x => x.Id).Max() + 1 : 1
                };
                Versammlungen.Add(c);
            }

            return c;
        }

        public static Conregation FindOrAddConregation(string name, int kreis)
        {
            var c = FindConregation(name, kreis);

            if (c == null)
            {
                Log.Info(nameof(FindOrAddConregation), $"add={name}, {kreis}");
                c = new Conregation
                {
                    Name = name,
                    Kreis = kreis,
                    Id = Versammlungen.Count > 0 ? Versammlungen.Select(x => x.Id).Max() + 1 : 1
                };
                Versammlungen.Add(c);
            }

            return c;
        }

        public static Talk FindTalk(int Nummer)
        {
            Log.Info(nameof(FindTalk), $"Nummer={Nummer}");
            foreach (var t in Vorträge)
            {
                if (t.Nummer == Nummer)
                    return t;
            }
            return FindTalk(-1);
        }

        public static Speaker FindSpeaker(string name, Conregation versammlung)
        {
            Log.Info(nameof(FindSpeaker), $"name={name}, conregation={versammlung?.Id}, {versammlung?.Name}");
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
                Log.Info(nameof(FindOrAddSpeaker), $"add={name} to conregation={versammlung?.Id}, {versammlung?.Name}");
                s = new Speaker
                {
                    Name = name,
                    Versammlung = versammlung,
                    Id = Redner.Count > 0 ? Redner.Select(x => x.Id).Max() + 1 : 1
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

        public static void UpdateTalkDate()
        {
            Log.Info(nameof(UpdateTalkDate), "");
            foreach (var evt in MeinPlan.Where(x => x.Status != EventStatus.Ereignis))
            {
                var m = (evt as Invitation);
                if (m.Vortrag is null)
                    continue;
                if (m.Datum > m.Vortrag.zuletztGehalten || m.Vortrag.zuletztGehalten == null)
                    m.Vortrag.zuletztGehalten = m.Datum;
            }
        }
    }
}