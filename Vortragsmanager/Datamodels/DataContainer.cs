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

        public static Conregation MeineVersammlung { get; set; }

        public static ObservableCollection<Conregation> Versammlungen { get; } = new ObservableCollection<Conregation>();

        public static ObservableCollection<Speaker> Redner { get; } = new ObservableCollection<Speaker>();

        public static ObservableCollection<IEvent> MeinPlan { get; } = new ObservableCollection<IEvent>();

        public static List<Busy> Abwesenheiten { get; } = new List<Busy>();

        public static void MeinPlanAdd(IEvent newEvent)
        {
            MeinPlan.Add(newEvent);
            if (newEvent?.Vortrag?.Vortrag != null && newEvent.Vortrag.Vortrag.ZuletztGehalten < newEvent.Kw)
                newEvent.Vortrag.Vortrag.ZuletztGehalten = newEvent.Kw;
        }

        public static void MeinPlanRemove(IEvent oldEvent)
        {
            MeinPlan.Remove(oldEvent);
            UpdateTalkDate(oldEvent?.Vortrag?.Vortrag);
        }

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
                if (einladung.Kw <= Helper.CurrentWeek)
                {
                    einladung.AnfrageVersammlung = unbekannteVersammlung;
                    einladung.Ältester = unbekannterRedner;
                }
                else
                {
                    MeinPlanRemove(einladung);
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
                if (outside.Kw <= Helper.CurrentWeek)
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
                if (einladung.Kw < Helper.CurrentWeek)
                {
                    einladung.AnfrageVersammlung = unbekannteVersammlung;
                    einladung.Ältester = unbekannterRedner;
                }
                else
                    MeinPlanRemove(einladung);
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

        public static void UpdateTalkDate(Talk talk)
        {
            if (talk == null)
                return;

            Log.Info(nameof(UpdateTalkDate), talk.Nummer);
            var gehaltene = MeinPlan.Where(x => x.Vortrag?.Vortrag == talk).DefaultIfEmpty(null).Max(x => x?.Kw) ?? -1;
            talk.ZuletztGehalten = gehaltene;
        }

        public static IEnumerable<Core.DataHelper.DateWithConregation> SpeakerGetActivities(Speaker redner)
        {
            if (redner == null)
                return null;

            IEnumerable<Core.DataHelper.DateWithConregation> erg;

            erg = DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Zugesagt).Cast<Invitation>().Where(x => x.Ältester == redner).Select(x => new Core.DataHelper.DateWithConregation(x.Kw, DataContainer.MeineVersammlung.Name, x.Vortrag?.Vortrag?.Nummer));
            
            if (redner.Versammlung == DataContainer.MeineVersammlung)
                erg = erg.Union(DataContainer.ExternerPlan.Where(x => x.Ältester == redner).Select(x => new Core.DataHelper.DateWithConregation(x.Kw, x.Versammlung.Name, x.Vortrag?.Vortrag?.Nummer)));
                
            return erg;
        }

        public static AufgabenZuordnung AufgabenZuordnungAdd()
        {
            var id = AufgabenPersonZuordnung.Count > 0 ? AufgabenPersonZuordnung.Select(x => x.Id).Max() + 1 : 1;
            var az = new AufgabenZuordnung(id);
            AufgabenPersonZuordnung.Add(az);
            return az;
        }

        public static ObservableCollection<AufgabenZuordnung> AufgabenPersonZuordnung { get; private set; } = new ObservableCollection<AufgabenZuordnung>();

        public static ObservableCollection<AufgabenKalender> AufgabenPersonKalender { get; private set; } = new ObservableCollection<AufgabenKalender>();

        public static AufgabenKalender AufgabenPersonKalenderFindOrAdd(int kw)
        {
            var ergebnis = AufgabenPersonKalender.FirstOrDefault(x => x.Kw == kw);
            if (ergebnis == null)
            {
                ergebnis = new AufgabenKalender(kw);
                AufgabenPersonKalender.Add(ergebnis);
            }
            return ergebnis;
        }

        public static string GetRednerAuswärts(int kw)
        {
            Log.Info(nameof(GetRednerAuswärts), kw);
            var e = DataContainer.ExternerPlan.Where(x => x.Kw == kw).ToList();
            if (e.Count == 0)
                return "";

            var ausgabe = "Redner Auswärts: ";
            foreach (var r in e)
            {
                ausgabe += $"{r.Ältester.Name} in {r.Versammlung.Name}, ";
            }

            return ausgabe.Substring(0, ausgabe.Length - 2);
        }
    }

    public static class TalkList
    {
        private static List<Talk> Vorträge { get; } = new List<Talk>();

        /// <summary>
        /// Sucht nach der übergebenen Vortragsnummer und gibt den entsprechenden Vortrag zurück.
        /// Es werden zuerst die gültigen Vorträge durchsucht, danach die nicht mehr gültigen.
        /// </summary>
        /// <param name="Nummer">Nummer des gesuchten Vortrags</param>
        /// <returns>Den Vortrag zur übergebenen Nummer.</returns>
        public static Talk Find(int Nummer)
        {
            Log.Info(nameof(TalkList.Find), $"Nummer={Nummer}");
            foreach (var t in Vorträge.Where(x => x.Gültig))
            {
                if (t.Nummer == Nummer)
                    return t;
            }
            foreach (var t in Vorträge.Where(x => !x.Gültig))
            {
                if (t.Nummer == Nummer)
                    return t;
            }
            return Find(-1);
        }

        /// <summary>
        /// Prüft ob die Vortragsnummer bereits existiert und fügt den Vortrag zur Liste hinzu.
        /// </summary>
        /// <param name="nummer">Vortrags Nummer</param>
        /// <param name="thema">Vortrags Thema</param>
        /// <param name="gültig"></param>
        /// <returns>True wenn der Vortrag hinzugefügt wurde, False wenn der Vortrag bereits in der Liste existiert.</returns>
        public static bool Add(int nummer, string thema, bool gültig = true)
        {
            if (Vorträge.Any(x => x.Nummer == nummer))
                return false;
            Vorträge.Add(new Talk(nummer, thema) { Gültig = gültig });
            return true;
        }

        /// <summary>
        /// Fügt einen neuen Vortrag ohne Prüfung zur Liste hinzu.
        /// </summary>
        /// <param name="talk"></param>
        public static void Add(Talk talk)
        {
            Vorträge.Add(talk);
        }

        /// <summary>
        /// Listet alle Vorträge auf, zuerst Gültige, dann Ungültige, sortiert nach der Vortragsnummer
        /// </summary>
        /// <returns>Liste der Vorträge</returns>
        public static IOrderedEnumerable<Talk> Get()
        {
            return Vorträge.OrderByDescending(x => x.Gültig).ThenBy(x => x.Nummer);
        }

        /// <summary>
        /// Listet alle gültigen Vorträge auf, sortiert nach der Vortragsnummer
        /// </summary>
        /// <returns>Liste der Vorträge</returns>
        public static IOrderedEnumerable<Talk> GetValid()
        {
            return Vorträge.Where(x => x.Gültig).OrderBy(x => x.Nummer);
        }

        /// <summary>
        /// Aktualisiert das Datum wann der Vortrag das letzte mal gehalten wurde.
        /// </summary>
        public static void UpdateDate()
        {
            Log.Info(nameof(UpdateDate), "");
            Vorträge.ForEach(x => x.ZuletztGehalten = -1);
            foreach (var evt in DataContainer.MeinPlan)
            {
                if (evt.Vortrag is null || evt.Vortrag.Vortrag is null)
                    continue;
                if (evt.Kw > evt.Vortrag.Vortrag.ZuletztGehalten || evt.Vortrag.Vortrag.ZuletztGehalten == -1)
                    evt.Vortrag.Vortrag.ZuletztGehalten = evt.Kw;
            }
        }

        /// <summary>
        /// Löscht alle Vorträge aus der Liste
        /// </summary>
        public static void Clear()
        {
            Vorträge.Clear();
        }
    }
}