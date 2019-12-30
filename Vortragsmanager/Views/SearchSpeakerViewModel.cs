using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Vortragsmanager.Models;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Views
{
    public class SearchSpeakerViewModel : ViewModelBase
    {
        public SearchSpeakerViewModel()
        {
            Messenger.Default.Register<GroupConregation>(this, LoadModul2);
            AnfrageSpeichernCommand = new DelegateCommand(AnfrageSpeichern);

            LoadModul1();

        }

        private void LoadModul1()
        {
            Kreise = new ObservableCollection<int>(Core.DataContainer.Versammlungen.Select(x => x.Kreis).Where(x => x > 0).Distinct());
            ReadTermine();
            SelectedTermine = 2;
            Modul1Visible = new GridLength(1, GridUnitType.Star);
            Modul2Visible = new GridLength(0);
            RednerCheckHistory = Settings.Default.SearchSpeaker_RednerCheckHistory;
            RednerCheckFuture = Settings.Default.SearchSpeaker_RednerCheckFuture;
            VortragCheckFuture = Settings.Default.SearchSpeaker_VortragCheckFuture;
            VortragCheckHistory = Settings.Default.SearchSpeaker_VortragCheckHistory;
        }

        #region Freie Termine & Redner suchen

        public GridLength Modul1Visible
        {
            get { return GetProperty(() => Modul1Visible); }
            set { SetProperty(() => Modul1Visible, value); }
        }

        public ObservableCollection<int> Kreise { get; private set; }

        private List<object> selectedKreise;

        public object SelectedKreise
        {
            get
            {
                if (selectedKreise == null)
                {
                    selectedKreise = FillMyKreis();
                    FillVersammlungen();
                }
                return selectedKreise;
            }
            set
            {
                selectedKreise = (List<object>)value;
                FillVersammlungen();
                RaisePropertyChanged();
            }
        }

        private static List<object> FillMyKreis()
        {
            return new List<object>() {
                Core.DataContainer.MeineVersammlung.Kreis
            };
        }

        public ObservableCollection<Conregation> Versammlungen { get; private set; }

        private void FillVersammlungen()
        {
            if (selectedKreise == null)
                Versammlungen = new ObservableCollection<Conregation>();
            Versammlungen = new ObservableCollection<Conregation>(Core.DataContainer.Versammlungen.Where(x => selectedKreise.Contains(x.Kreis)));
            RaisePropertyChanged(nameof(Versammlungen));
            SelectedVersammlungen = Versammlungen.Cast<object>().ToList();
        }

        private List<object> selectedVersammlungen;

        public object SelectedVersammlungen
        {
            get
            {
                if (selectedVersammlungen == null)
                    selectedVersammlungen = Versammlungen.Cast<object>().ToList();
                return selectedVersammlungen;
            }
            set
            {
                selectedVersammlungen = (List<object>)value;
                RaisePropertyChanged();
                ReadData();
            }
        }

        public bool RednerCheckFuture
        {
            get { return GetProperty(() => RednerCheckFuture); }
            set { SetProperty(() => RednerCheckFuture, value, ReadData); }
        }

        public bool RednerCheckHistory
        {
            get { return GetProperty(() => RednerCheckHistory); }
            set { SetProperty(() => RednerCheckHistory, value, ReadData); }
        }

        public bool VortragCheckFuture
        {
            get { return GetProperty(() => VortragCheckFuture); }
            set { SetProperty(() => VortragCheckFuture, value, ReadData); }
        }

        public bool VortragCheckHistory
        {
            get { return GetProperty(() => VortragCheckHistory); }
            set { SetProperty(() => VortragCheckHistory, value, ReadData); }
        }

        public List<GroupConregation> Redner { get; private set; }

        public void ReadData()
        {
            var list = new List<GroupConregation>();
            var vers = selectedVersammlungen.Cast<Conregation>();
            foreach (var v in vers)
            {
                var gC = new GroupConregation
                {
                    Versammlung = v
                };
                list.Add(gC);
            }
            var redner = Core.DataContainer.Redner.Where(x => vers.Contains(x.Versammlung)).ToList();
            var einladungen = Core.DataContainer.MeinPlan.Where(x => x.Status != EventStatus.Ereignis).Cast<Invitation>();

            foreach (var r in redner)
            {
                var gv = list.First(x => x.Versammlung == r.Versammlung);
                var gr = new GroupSpeaker
                {
                    Redner = r
                };
                var vorträge = einladungen.Where(x => x.Ältester == r).ToList();
                if (vorträge.Count == 0)
                    continue;

                gr.LetzteEinladung = vorträge.Max(x => x.Datum);

                if (RednerCheckFuture && gr.LetzteEinladung > DateTime.Today)
                    continue;

                if (RednerCheckHistory && vorträge.Where(x => x.Datum >= DateTime.Today.AddYears(-1) && x.Datum <= DateTime.Today).Any())
                    continue;

                gv.Redner.Add(gr);

                foreach (var t in r.Vorträge.OrderBy(x => x.zuletztGehalten ?? DateTime.MinValue))
                {
                    var gt = new GroupTalk();
                    var gehalten = Core.DataContainer.MeinPlan.Where(x => x.Vortrag == t).ToList();

                    gt.Vortrag = t;
                    gt.AnzahlGehört = gehalten.Count;
                    gt.InZukunft = gehalten.Where(x => x.Datum > DateTime.Today).Any();
                    gt.InVergangenheit = gehalten.Where(x => x.Datum > DateTime.Today.AddYears(-1) && x.Datum <= DateTime.Today).Any();

                    if (VortragCheckFuture && gt.InZukunft)
                        continue;

                    if (VortragCheckHistory && gt.InVergangenheit)
                        continue;

                    gr.Vorträge.Add(gt);
                }
            }

            Redner = list;
            RaisePropertyChanged(nameof(Redner));
        }

        public ObservableCollection<Termin> FreieTermine { get; } = new ObservableCollection<Termin>();

        public void ReadTermine()
        {
            FreieTermine.Clear();

            var start = DateTime.Today;
            if (start.DayOfWeek != DayOfWeek.Sunday)
            {
                start = start.AddDays(7 - (int)start.DayOfWeek);
            }
            var ende = start.AddYears(1);
            var datum = start;
            var month = datum.AddMonths(-1).Month;
            while (datum < ende)
            {
                if (!Core.DataContainer.MeinPlan.Any(x => x.Datum == datum) &&
                    !Core.DataContainer.OffeneAnfragen.Any(x => x.Wochen.Contains(datum)))
                {
                    var t = new Termin(datum) { IsFirstDateOfMonth = (datum.Month != month) };
                    FreieTermine.Add(t);
                    month = datum.Month;
                }
                datum = datum.AddDays(7);
            }
        }

        public int SelectedTermine
        {
            get { return GetProperty(() => SelectedTermine); }
            set { SetProperty(() => SelectedTermine, value, ChangeSelectedTermine); }
        }

        private void ChangeSelectedTermine()
        {
            //0 = nächster
            //1 = 1 Monat
            //2 = 3 Monate
            //3 = 6 Monate
            //4 = 12 Monate
            var maxTermin = DateTime.Today;
            if (maxTermin.DayOfWeek != DayOfWeek.Sunday)
            {
                maxTermin = maxTermin.AddDays(7 - (int)maxTermin.DayOfWeek);
            }
            switch (SelectedTermine)
            {
                case 1:
                    maxTermin = maxTermin.AddMonths(1);
                    break;

                case 2:
                    maxTermin = maxTermin.AddMonths(3);
                    break;

                case 3:
                    maxTermin = maxTermin.AddMonths(6);
                    break;

                case 0:
                case 4:
                    maxTermin = maxTermin.AddMonths(12);
                    break;
            }
            foreach (var t in FreieTermine)
            {
                //t.Aktiv = (t.Datum <= maxTermin);
                t.IsChecked = (t.Datum <= maxTermin);
                if (SelectedTermine == 0)
                    maxTermin = DateTime.Today;
            }
        }

        #endregion Freie Termine & Redner suchen

        #region Mail versenden

        public GroupConregation AktuelleAnfrage { get; set; }

        public DelegateCommand AnfrageSpeichernCommand { get; private set; }

        private void AnfrageSpeichern()
        {
            var anfrage = new Inquiry();
            anfrage.AnfrageDatum = DateTime.Today;
            anfrage.Versammlung = AktuelleAnfrage.Versammlung;       
            anfrage.Id = Core.DataContainer.OffeneAnfragen.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
                       
            var Kommentar = $"Anfrage an Versammlung {AktuelleAnfrage.Versammlung.Name} am {DateTime.Today:dd.MM.yyyy}";

            foreach (var r in AktuelleAnfrage.Redner.Where(x => x.Gewählt))
            {
                var v = r.Vorträge[r.SelectedIndex].Vortrag;
                anfrage.RednerVortrag.Add(r.Redner, v);
                Kommentar += $"\t{r.Name}, Vortrag Nr. {v.Nummer} ({v.Thema})" + Environment.NewLine;
            }
            anfrage.Wochen.Clear();
            foreach(var d in FreieTermine.Where(x => x.Aktiv).Select(x => x.Datum))
            {
                anfrage.Wochen.Add(d);
            }
            anfrage.Kommentar = Kommentar;
            Core.DataContainer.OffeneAnfragen.Add(anfrage);
            LoadModul1();
        }

        public string MailText
        {
            get { return GetProperty(() => MailText); }
            set { SetProperty(() => MailText, value); }
        }

        private void LoadModul2(GroupConregation inhalt)
        {
            Modul1Visible = new GridLength(0);
            Settings.Default.SearchSpeaker_RednerCheckHistory = RednerCheckHistory;
            Settings.Default.SearchSpeaker_RednerCheckFuture = RednerCheckFuture;
            Settings.Default.SearchSpeaker_VortragCheckFuture = VortragCheckFuture;
            Settings.Default.SearchSpeaker_VortragCheckHistory = VortragCheckHistory;

            Modul2Visible = new GridLength(1, GridUnitType.Star);

            AktuelleAnfrage = inhalt;

            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.RednerAnfragenMailText).Inhalt;

            var stringFreieTermine = "\t";
            var anzahl = 1;
            foreach (var ft in FreieTermine.Where(x => x.Aktiv))
            {
                stringFreieTermine += $"{ft.Datum:dd.MM.yyyy}, ";
                if (anzahl == 4)
                {
                    anzahl = 0;
                    stringFreieTermine += Environment.NewLine + "\t";
                }
                anzahl++;
            }
            stringFreieTermine = stringFreieTermine.TrimEnd();

            var stringRedner = string.Empty;
            foreach (var r in inhalt.Redner.Where(x => x.Gewählt))
            {
                var v = r.Vorträge[r.SelectedIndex].Vortrag;
                stringRedner += $"\t{r.Name}, Vortrag Nr. {v.Nummer} ({v.Thema})" + Environment.NewLine;
            }

            mt = mt
                .Replace("{Freie Termine}", stringFreieTermine)
                .Replace("{Liste Redner}", stringRedner)
                .Replace("{Koordinator Mail}", $"{inhalt.Versammlung.KoordinatorJw}; {inhalt.Versammlung.KoordinatorMail}")
                .Replace("{Koordinator Name}", inhalt.Versammlung.Koordinator)
                .Replace("{Versammlung}", inhalt.Versammlung.Name);

            MailText = mt;
        }

        public GridLength Modul2Visible
        {
            get { return GetProperty(() => Modul2Visible); }
            set { SetProperty(() => Modul2Visible, value); }
        }

        #endregion Mail versenden
    }

    /// <summary>
    /// Freie Termine meiner Planung
    /// </summary>
    public class Termin : ViewModelBase
    {
        private bool isChecked = true;

        public Termin(DateTime datum)
        {
            Datum = datum;
        }

        public DateTime Datum { get; set; }

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFirstDateOfMonth { get; set; }

        public string Titel => Datum.ToString("dd.MM.yyyy", new CultureInfo("de-DE"));

        public bool Aktiv
        {
            get => IsChecked;
            set
            {
                IsChecked = value;
                RaisePropertyChanged();
                RaisePropertiesChanged(new string[] { "Aktiv", "IsChecked" });
            }
        }
    }

    /// <summary>
    /// Liste der Versammlungen
    /// </summary>
    public class GroupConregation : ViewModelBase
    {
        public GroupConregation()
        {
            AnfrageSendenCommand = new DelegateCommand(AskForSpeaker);
        }

        public DelegateCommand AnfrageSendenCommand { get; private set; }

        public void AskForSpeaker()
        {
            Messenger.Default.Send(this);
        }

        public Conregation Versammlung { get; set; }

        public List<GroupSpeaker> Redner { get; } = new List<GroupSpeaker>();

        public string Name => Versammlung.Name;
    }

    /// <summary>
    /// Liste der Redner einer Versammlung
    /// </summary>
    public class GroupSpeaker : ViewModelBase
    {
        public GroupSpeaker()
        {
            Gewählt = true;
        }

        public Speaker Redner { get; set; }

        public List<GroupTalk> Vorträge { get; } = new List<GroupTalk>();

        public int SelectedIndex { get; set; } = 0;

        public DateTime LetzteEinladung { get; set; }

        public Invitation LetzterVortrag { get; set; }

        public bool InZukunft { get; set; }

        public bool InVergangenheit { get; set; }

        public string Name => Redner?.Name;

        public bool Gewählt
        {
            get { return GetProperty(() => Gewählt); }
            set { SetProperty(() => Gewählt, value); }
        }
    }

    /// <summary>
    /// Liste der Vorträge eines Redners
    /// </summary>
    public class GroupTalk
    {
        public Talk Vortrag { get; set; }

        public int AnzahlGehört { get; set; }

        public bool InZukunft { get; set; }

        public bool InVergangenheit { get; set; }

        public string Name => Vortrag.ToString();

        public string ZuletztGehalten => (Vortrag.zuletztGehalten is null) ? "nicht gehalten" : $"{Vortrag.zuletztGehalten}";

        public override string ToString()
        {
            return $"{Vortrag.Nummer} {Vortrag.Thema} ({AnzahlGehört}*, zuletzt am {ZuletztGehalten})";
        }
    }
}