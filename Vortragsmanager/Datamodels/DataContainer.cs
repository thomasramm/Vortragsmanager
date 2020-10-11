using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Vortragsmanager.Core;

namespace Vortragsmanager.Datamodels
{
    public class MyGloabalSettings : ViewModelBase
    {
        public MyGloabalSettings()
        {
            Titel = "vdl";
        }

        public string Titel { get; set; }

        public void RefreshTitle()
        {
            var fi = new FileInfo(Properties.Settings.Default.sqlite);
            Titel = "Vortragsmanager DeLuxe | " + fi.Name;

            RaisePropertyChanged(Titel);
        }
    }

    public static class DataContainer
    {
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

        public static ObservableCollection<ActivityLog.Activity> Aktivitäten { get; } = new ObservableCollection<ActivityLog.Activity>();

        public static Conregation ConregationFind(string name)
        {
            Log.Info(nameof(ConregationFind), $"name={name}");
            foreach (var c in Versammlungen)
            {
                if (c.Name == name)
                    return c;
            }

            return null;
        }

        public static Conregation ConregationFind(string name, int kreis)
        {
            Log.Info(nameof(ConregationFind), $"name={name}, kreis={kreis}");
            foreach (var c in Versammlungen)
            {
                if (c.Name == name && c.Kreis == kreis)
                    return c;
            }

            return null;
        }

        public static Conregation ConregationFindOrAdd(string name)
        {
            var c = ConregationFind(name);

            if (c == null)
            {
                Log.Info(nameof(ConregationFindOrAdd), $"add={name}");
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

        public static Conregation ConregationFindOrAdd(string name, int kreis)
        {
            var c = ConregationFind(name, kreis);

            if (c == null)
            {
                Log.Info(nameof(ConregationFindOrAdd), $"add={name}, {kreis}");
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

        public static Conregation ConregationGetUnknown()
        {
            return ConregationFindOrAdd("Unbekannt");
        }

        public static void ConregationRemove(Conregation Versammlung)
        {
            var redner = Redner.Where(x => x.Versammlung == Versammlung).OrderBy(x => x.Name).ToList();
            var unbekannteVersammlung = ConregationGetUnknown();
            var unbekannterRedner = SpeakerGetUnknown();

            //Einladungen umschreiben (vergangene) oder löschen (zukünftige)
            var einladungen = MeinPlan
                .Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.AnfrageVersammlung == Versammlung)
                .ToList();
            foreach (var einladung in einladungen)
            {
                if (einladung.Datum <= DateTime.Today)
                {
                    einladung.AnfrageVersammlung = unbekannteVersammlung;
                    einladung.Ältester = unbekannterRedner;
                }
                else
                {
                    MeinPlan.Remove(einladung);
                }
            }

            //Anfragen löschen
            var anfragen = OffeneAnfragen.Where(x => x.Versammlung == Versammlung).ToList();
            foreach (var anfrage in anfragen)
            {
                OffeneAnfragen.Remove(anfrage);
            }

            //Externe Vorträge in dieser Versammlung
            var externeE = ExternerPlan.Where(x => x.Versammlung == Versammlung).ToList();
            foreach (var outside in externeE)
            {
                if (outside.Datum <= DateTime.Today)
                    outside.Versammlung = unbekannteVersammlung;
                else
                    ExternerPlan.Remove(outside);
            }

            foreach (var r in redner)
            {
                SpeakerRemove(r);
            }

            Versammlungen.Remove(Versammlung);
        }

        public static Talk TalkFind(int Nummer)
        {
            Log.Info(nameof(TalkFind), $"Nummer={Nummer}");
            foreach (var t in Vorträge)
            {
                if (t.Nummer == Nummer)
                    return t;
            }
            return TalkFind(-1);
        }

        public static Speaker SpeakerFind(string name, Conregation versammlung)
        {
            Log.Info(nameof(SpeakerFind), $"name={name}, conregation={versammlung?.Id}, {versammlung?.Name}");
            foreach (var s in Redner)
            {
                if (s.Name == name && (s.Versammlung == versammlung || versammlung == null))
                    return s;
            }

            return null;
        }

        public static Speaker SpeakerFindOrAdd(string name, Conregation versammlung)
        {
            var s = SpeakerFind(name, versammlung);

            if (s == null)
            {
                Log.Info(nameof(SpeakerFindOrAdd), $"add={name} to conregation={versammlung?.Id}, {versammlung?.Name}");
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

        public static Speaker SpeakerGetUnknown()
        {
            return SpeakerFindOrAdd("Unbekannt", ConregationGetUnknown());
        }

        public static void SpeakerRemove(Speaker redner)
        {
            if (redner is null)
                return;

            var unbekannteVersammlung = ConregationGetUnknown();
            var unbekannterRedner = SpeakerGetUnknown();

            //Einladungen
            var einladungen = MeinPlan
                .Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.Ältester == redner)
                .ToList();
            foreach (var einladung in einladungen)
            {
                if (einladung.Datum < DateTime.Today)
                {
                    einladung.AnfrageVersammlung = unbekannteVersammlung;
                    einladung.Ältester = unbekannterRedner;
                }
                else
                    MeinPlan.Remove(einladung);
            }
            //Offene Anfragen
            var anfragen = OffeneAnfragen
                .Where(x => x.RednerVortrag.ContainsKey(redner))
                .ToList();
            foreach (var anfrage in anfragen)
            {
                anfrage.RednerVortrag.Remove(redner);
                if (anfrage.RednerVortrag.Count == 0)
                    OffeneAnfragen.Remove(anfrage);
            }

            //Externe Einladungen
            if (redner.Versammlung == MeineVersammlung)
            {
                var externeE = ExternerPlan.Where(x => x.Ältester == redner).ToList();
                foreach (var einladung in externeE)
                    ExternerPlan.Remove(einladung);
            }

            //Absagen
            var absagen = Absagen
                .Where(x => x.Ältester == redner).ToList();
            foreach (var absage in absagen)
                Absagen.Remove(absage);

            Redner.Remove(redner);
        }

        public static void UpdateTalkDate()
        {
            Log.Info(nameof(UpdateTalkDate), "");
            Vorträge.ForEach(x => x.ZuletztGehalten = null);
            foreach (var evt in MeinPlan)
            {
                if (evt.Vortrag is null || evt.Vortrag.Vortrag is null)
                    continue;
                if (evt.Datum > evt.Vortrag.Vortrag.ZuletztGehalten || evt.Vortrag.Vortrag.ZuletztGehalten == null)
                    evt.Vortrag.Vortrag.ZuletztGehalten = evt.Datum;
            }
        }

        public static IEnumerable<Core.DataHelper.DateWithConregation> SpeakerGetActivities(Speaker redner, int anzahl)
        {
            if (redner == null)
                return null;

            IEnumerable<Core.DataHelper.DateWithConregation> erg;

            erg = DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Zugesagt).Cast<Invitation>().Where(x => x.Ältester == redner).Select(x => new Core.DataHelper.DateWithConregation(x.Datum, DataContainer.MeineVersammlung.Name, x.Vortrag?.Vortrag?.Nummer));
            
            if (redner.Versammlung == DataContainer.MeineVersammlung)
                erg = erg.Union(DataContainer.ExternerPlan.Where(x => x.Ältester == redner).Select(x => new Core.DataHelper.DateWithConregation(x.Datum, x.Versammlung.Name, x.Vortrag?.Vortrag?.Nummer)));
                
            return erg.OrderByDescending(x => x.Datum).Take(anzahl);
        }
    }
}