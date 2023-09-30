using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.Interface;
using Vortragsmanager.Module;
using Vortragsmanager.PageModels;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.DataModels
{
    public static class DataContainer
    {
        private static Conregation _meineVersammlung;

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

        public static AufgabenZuordnung AufgabenZuordnungAdd()
        {
            var id = AufgabenPersonZuordnung.Count > 0 ? AufgabenPersonZuordnung.Select(x => x.Id).Max() + 1 : 1;
            var az = new AufgabenZuordnung(id);
            AufgabenPersonZuordnung.Add(az);
            return az;
        }

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

        public static Conregation ConregationGetUnknown() { return ConregationFindOrAdd("Unbekannt"); }

        public static bool ConregationRemove(Conregation versammlung)
        {
            var redner = Redner.Where(x => x.Versammlung == versammlung).OrderBy(x => x.Name).ToList();
            var unbekannteVersammlung = ConregationGetUnknown();
            var unbekannterRedner = SpeakerGetUnknown();

            if (versammlung == unbekannteVersammlung)
            {
                ThemedMessageBox.Show(
                    "Achtung",
                    "Die gewählte Versammlung kann nicht gelöscht werden. Die Versammlung 'Unbekannt' ist für das Programm notwendig!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }

            //Einladungen umschreiben (vergangene) oder löschen (zukünftige)
            var einladungen = MeinPlan
                .Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.AnfrageVersammlung == versammlung)
                .ToList();
            foreach (var einladung in einladungen)
            {
                if (einladung.Kw <= DateCalcuation.CurrentWeek)
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
            var anfragen = OffeneAnfragen.Where(x => x.Versammlung == versammlung).ToList();
            foreach (var anfrage in anfragen)
            {
                OffeneAnfragen.Remove(anfrage);
            }

            //Externe Vorträge in dieser Versammlung
            var externeE = ExternerPlan.Where(x => x.Versammlung == versammlung).ToList();
            foreach (var outside in externeE)
            {
                if (outside.Kw <= DateCalcuation.CurrentWeek)
                    outside.Versammlung = unbekannteVersammlung;
                else
                    ExternerPlan.Remove(outside);
            }

            foreach (var r in redner)
            {
                SpeakerRemove(r);
            }

            Versammlungen.Remove(versammlung);

            return true;
        }

        public static (string, string, string, string) GetRednerAuswärts(int kw)
        {
            Log.Info(nameof(GetRednerAuswärts), kw);
            var e = DataContainer.ExternerPlan.Where(x => x.Kw == kw).ToList();
            if (e.Count == 0)
                return ("", "", "", "");

            var ausgabe = "Redner Auswärts: ";
            var name = string.Empty;
            var ort = string.Empty;
            var ausgabeNr = ausgabe;
            foreach (var r in e)
            {
                ausgabe += $"{r.Ältester.Name} in {r.Versammlung.Name}, ";
                ausgabeNr += $"{r.Ältester.Name} in {r.Versammlung.Name} ({r.Vortrag.Vortrag.Nummer}), ";
                name += r.Ältester.Name + ", ";
                ort += r.Versammlung.Name + ", ";
            }

            ausgabe = ausgabe.Length > 2 ? ausgabe.Substring(0, ausgabe.Length - 2) : ausgabe;
            ausgabeNr = ausgabeNr.Length > 2 ? ausgabeNr.Substring(0, ausgabeNr.Length - 2) : ausgabeNr;
            name = name.Length > 2 ? name.Substring(0, name.Length - 2) : name;
            ort = ort.Length > 2 ? ort.Substring(0, ort.Length - 2) : ort;

            return (ausgabe, name, ort, ausgabeNr);
        }

        public static void MeinPlanAdd(IEvent newEvent)
        {
            if (MeinPlan.Any(x => x.Kw == newEvent.Kw))
            {
                var alt = MeinPlan.First(x => x.Kw == newEvent.Kw);
                MeinPlan.Remove(alt);
            }
            MeinPlan.Add(newEvent);
            if (newEvent?.Vortrag?.Vortrag != null && newEvent.Vortrag.Vortrag.ZuletztGehalten < newEvent.Kw)
                newEvent.Vortrag.Vortrag.ZuletztGehalten = newEvent.Kw;
        }

        public static void MeinPlanRemove(IEvent oldEvent)
        {
            MeinPlan.Remove(oldEvent);
            UpdateTalkDate(oldEvent?.Vortrag?.Vortrag);
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

        public static IEnumerable<DateWithConregation> SpeakerGetActivities(Speaker redner)
        {
            if (redner == null)
                return null;

            var list1 = MeinPlan.Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.Ältester == redner && x.Kw >= 200001);
            IEnumerable<DateWithConregation> erg = list1.Select(
                x => new DateWithConregation(
                    DateCalcuation.CalculateWeek(x.Kw),
                    MeineVersammlung.Name,
                    x.Vortrag?.Vortrag?.Nummer));

            if (redner.Versammlung == MeineVersammlung)
                erg = erg.Union(
                    ExternerPlan.Where(x => x.Ältester == redner)
                        .Select(x => new DateWithConregation(x.Datum, x.Versammlung.Name, x.Vortrag?.Vortrag?.Nummer)));

            return erg;
        }

        public static Speaker SpeakerGetUnknown()
        {
            var unbCon = ConregationGetUnknown();
            var s = SpeakerFind("Unbekannt", unbCon);

            if (s == null)
            {
                Log.Info(nameof(SpeakerFindOrAdd), $"add=Unbekannt to conregation={unbCon?.Id}, {unbCon?.Name}");
                s = new Speaker
                {
                    Name = "Unbekannt",
                    Versammlung = unbCon,
                    Id = Redner.Count > 0 ? Redner.Select(x => x.Id).Max() + 1 : 1,
                    Aktiv = false,
                    Einladen = false
                };
                Redner.Add(s);
            }
            return s;
        }

        public static bool SpeakerRemove(Speaker redner)
        {
            if (redner is null)
                return false;

            var unbekannteVersammlung = ConregationGetUnknown();
            var unbekannterRedner = SpeakerGetUnknown();

            if (redner == unbekannterRedner)
            {
                ThemedMessageBox.Show(
                    "Achtung",
                    "Der gewählte Redner kann nicht gelöscht werden. Der Redner 'Unbekannt' ist für das Programm notwendig!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }

            //Einladungen
            var einladungen = MeinPlan
                .Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.Ältester == redner)
                .ToList();
            foreach (var einladung in einladungen)
            {
                if (einladung.Kw < DateCalcuation.CurrentWeek)
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
                .Where(x => x.Ältester == redner)
                .ToList();
            foreach (var absage in absagen)
                Absagen.Remove(absage);

            Redner.Remove(redner);

            return true;
        }

        public static void UpdateTalkDate(Talk talk)
        {
            if (talk == null)
                return;

            Log.Info(nameof(UpdateTalkDate), talk.Nummer);
            var gehaltene = MeinPlan.Where(x => x.Vortrag?.Vortrag == talk).DefaultIfEmpty(null).Max(x => x?.Kw) ?? -1;
            talk.ZuletztGehalten = gehaltene;
        }

        public static ObservableCollection<Cancelation> Absagen { get; } = new ObservableCollection<Cancelation>();

        public static List<Busy> Abwesenheiten { get; } = new List<Busy>();

        public static ObservableCollection<ActivityItemViewModel> Aktivitäten
        {
            get;
        } = new ObservableCollection<ActivityItemViewModel>();

        public static ObservableCollection<AufgabenKalender> AufgabenPersonKalender
        {
            get;
        } = new ObservableCollection<AufgabenKalender>();

        public static ObservableCollection<AufgabenZuordnung> AufgabenPersonZuordnung
        {
            get;
        } = new ObservableCollection<AufgabenZuordnung>();

        public static ObservableCollection<Outside> ExternerPlan { get; } = new ObservableCollection<Outside>();

        public static bool IsInitialized { get; set; }

        public static bool IsDemo {  get; set; }

        public static Conregation MeineVersammlung
        {
            get => _meineVersammlung;
            set
            {
                _meineVersammlung = value;
                if (value != null)
                    //Default Wochentag setzen:
                    DateCalcuation.Wochentag = MeineVersammlung.Zeit.Get(DateTime.Today.Year).Tag;
            }
        }

        public static ObservableCollection<IEvent> MeinPlan { get; } = new ObservableCollection<IEvent>();

        public static ObservableCollection<Inquiry> OffeneAnfragen { get; } = new ObservableCollection<Inquiry>();

        public static ObservableCollection<Speaker> Redner { get; } = new ObservableCollection<Speaker>();

        public static ObservableCollection<Conregation> Versammlungen
        {
            get;
        } = new ObservableCollection<Conregation>();

        public static int Version { get; set; }
    }

    public static class TalkList
    {
        private static List<Talk> Vorträge { get; } = new List<Talk>();

        /// <summary>
        /// Fügt einen neuen Vortrag ohne Prüfung zur Liste hinzu.
        /// </summary>
        /// <param name="talk"></param>
        public static void Add(Talk talk) { Vorträge.Add(talk); }

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
        /// Löscht alle Vorträge aus der Liste
        /// </summary>
        public static void Clear() { Vorträge.Clear(); }

        /// <summary>
        /// Sucht nach der übergebenen Vortragsnummer und gibt den entsprechenden Vortrag zurück. Es werden zuerst die
        /// gültigen Vorträge durchsucht, danach die nicht mehr gültigen.
        /// </summary>
        /// <param name="nummer">Nummer des gesuchten Vortrags</param>
        /// <returns>Den Vortrag zur übergebenen Nummer.</returns>
        public static Talk Find(int nummer)
        {
            Log.Info(nameof(TalkList.Find), $"Nummer={nummer}");
            foreach (var t in Vorträge.Where(x => x.Gültig))
            {
                if (t.Nummer == nummer)
                    return t;
            }
            foreach (var t in Vorträge.Where(x => !x.Gültig))
            {
                if (t.Nummer == nummer)
                    return t;
            }
            return Find(-1);
        }

        /// <summary>
        /// Listet alle Vorträge auf, zuerst Gültige, dann Ungültige, sortiert nach der Vortragsnummer
        /// </summary>
        /// <returns>Liste der Vorträge</returns>
        public static IOrderedEnumerable<Talk> Get()
        { return Vorträge.OrderByDescending(x => x.Gültig).ThenBy(x => x.Nummer); }

        /// <summary>
        /// Listet alle gültigen Vorträge auf, sortiert nach der Vortragsnummer
        /// </summary>
        /// <returns>Liste der Vorträge</returns>
        public static IOrderedEnumerable<Talk> GetValid()
        { return Vorträge.Where(x => x.Gültig).OrderBy(x => x.Nummer); }

        public static void Reset()
        {
            var defaultTalks = Initialize.LoadDefaultTalks();
            var gültigeVortragsNr = new List<int>(200);

            //Vortragsnummern die in der default-Liste vorkommen auf default zurücksetzen
            foreach (var defaultTalk in defaultTalks)
            {
                gültigeVortragsNr.Add(defaultTalk.Nummer);
                var lokalerTalk = Vorträge.FirstOrDefault(x => x.Nummer == defaultTalk.Nummer);
                if (lokalerTalk == null)
                {
                    lokalerTalk = defaultTalk;
                    Add(defaultTalk);
                }
                lokalerTalk.Thema = defaultTalk.Thema;
                lokalerTalk.Gültig = defaultTalk.Gültig;
            }

            //Vortragsnummern die NICHT in der default-Liste vorkommen
            foreach (var lokalTalk in Vorträge.Where(x => !gültigeVortragsNr.Contains(x.Nummer)))
            {
                lokalTalk.Gültig = false;
            }
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
    }
}