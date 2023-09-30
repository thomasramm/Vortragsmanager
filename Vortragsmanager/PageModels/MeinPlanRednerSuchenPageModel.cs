using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.PageModels
{
    //ToDo: Bei Suche in der eigenen Versammlung die Buchungszeiten berücksichtigen, analog zu "Redner Anfrage Extern"
    public class MeinPlanRednerSuchenPageModel : ViewModelBase
    {
        public MeinPlanRednerSuchenPageModel()
        {
            Messenger.Default.Register<GroupConregation>(this, Messages.DisplayModuleAskForSpeaker, LoadModul2);
            AnfrageSpeichernCommand = new DelegateCommand(AnfrageSpeichern);
            CopyToClipboardCommand = new DelegateCommand(CopyToClipboard);

            LoadModul1();
        }

        private void LoadModul1()
        {
            RednerSuchenCommand = new DelegateCommand(ReadData);
            TerminauswahlCollectionInitialize();

            Kreise = new ObservableCollection<int>(DataContainer.Versammlungen.Select(x => x.Kreis).Where(x => x > 0).Distinct());
            ReadTermine();
            SelectedTermine = 2;
            Modul1Visible = new GridLength(1, GridUnitType.Star);
            Modul2Visible = new GridLength(0);
            RednerCheckHistory = Helper.Helper.GlobalSettings.SearchSpeaker_RednerCheckHistory;
            RednerCheckFuture = Helper.Helper.GlobalSettings.SearchSpeaker_RednerCheckFuture;
            RednerCheckCancelation = Helper.Helper.GlobalSettings.SearchSpeaker_RednerCheckCancelation;
            VortragCheckFuture = Helper.Helper.GlobalSettings.SearchSpeaker_VortragCheckFuture;
            VortragCheckHistory = Helper.Helper.GlobalSettings.SearchSpeaker_VortragCheckHistory;
            MaxEntfernung = Helper.Helper.GlobalSettings.SearchSpeaker_MaxEntfernung;
            OffeneAnfrage = Helper.Helper.GlobalSettings.SearchSpeaker_OffeneAnfrage;
            ReadSelectedKreis();

            Modul1Visible = new GridLength(1, GridUnitType.Star);
            Modul2Visible = new GridLength(0);
        }

        public DelegateCommand RednerSuchenCommand { get; private set; }

        public void ReadSelectedKreis()
        {
            var selKreis = Helper.Helper.GlobalSettings.SearchSpeaker_Kreis;
            var kreis = selKreis.Split(';');
            if (!string.IsNullOrEmpty(selKreis))
            {
                List<object> kreisList = new List<object>(kreis.Length);
                foreach (var k in kreis)
                {
                    if (int.TryParse(k, out int i))
                        kreisList.Add(i);
                }
                SelectedKreise = kreisList;
            }
            else
                SelectedKreise = new List<object>() { DataContainer.MeineVersammlung.Kreis };

            FillVersammlungen();
        }

        public void SaveSelectedKreis()
        {
            var s = string.Empty;
            foreach (var k in (List<object>)SelectedKreise)
            {
                s += k + ";";
            }
            Helper.Helper.GlobalSettings.SearchSpeaker_Kreis = s.TrimEnd(';');
        }

        #region Freie Termine & Redner suchen

        public GridLength Modul1Visible
        {
            get { return GetProperty(() => Modul1Visible); }
            set { SetProperty(() => Modul1Visible, value); }
        }

        public ObservableCollection<int> Kreise { get; private set; }

        private List<object> _selectedKreise;

        public object SelectedKreise
        {
            get => _selectedKreise;
            set
            {
                _selectedKreise = (List<object>)value;
                FillVersammlungen();
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Conregation> Versammlungen { get; private set; }

        private void FillVersammlungen()
        {
            IEnumerable<Conregation> vers;
            if (_selectedKreise == null)
                vers = DataContainer.Versammlungen;
            else
                vers = DataContainer.Versammlungen
                    .Where(x => _selectedKreise.Contains(x.Kreis) && x.Entfernung <= MaxEntfernung);

            if (!OffeneAnfrage)
            {
                var filter = DataContainer.OffeneAnfragen.Where(x => x.Status == EventStatus.Anfrage).Select(x => x.Versammlung);
                vers = vers.Where(x => !filter.Contains(x));
            }
            Versammlungen = new ObservableCollection<Conregation>(vers.OrderBy(x => x.Name));
            RaisePropertyChanged(nameof(Versammlungen));
            SelectedVersammlungen = Versammlungen.Cast<object>().ToList();
        }

        private List<object> _selectedVersammlungen;

        public object SelectedVersammlungen
        {
            get => _selectedVersammlungen ?? (_selectedVersammlungen = Versammlungen.Cast<object>().ToList());
            set
            {
                _selectedVersammlungen = (List<object>)value;
                RaisePropertyChanged();
            }
        }

        public bool RednerCheckFuture
        {
            get { return GetProperty(() => RednerCheckFuture); }
            set { SetProperty(() => RednerCheckFuture, value); }
        }

        public bool RednerCheckHistory
        {
            get { return GetProperty(() => RednerCheckHistory); }
            set { SetProperty(() => RednerCheckHistory, value); }
        }

        private bool _vortragCheckFuture;

        public bool VortragCheckFuture
        {
            get => _vortragCheckFuture;

            set
            {
                _vortragCheckFuture = value;
                RaisePropertyChanged();
                ReadAvailableTalks();
            }
        }

        private bool _vortragCheckHistory;

        public bool VortragCheckHistory
        {
            get => _vortragCheckHistory;

            set
            {
                _vortragCheckHistory = value;
                RaisePropertyChanged();
                ReadAvailableTalks();
            }
        }

        private bool _vortragCheckOpenRequest;

        public bool VortragCheckOpenRequest
        {
            get => _vortragCheckOpenRequest;

            set
            {
                _vortragCheckOpenRequest = value;
                RaisePropertyChanged();
                ReadAvailableTalks();
            }
        }

        public bool RednerCheckCancelation
        {
            get { return GetProperty(() => RednerCheckCancelation); }
            set { SetProperty(() => RednerCheckCancelation, value); }
        }

        private int _maxEntfernung;

        public int MaxEntfernung
        {
            get => _maxEntfernung;
            set
            {
                _maxEntfernung = value;
                RaisePropertyChanged();
                FillVersammlungen();
            }
        }

        private bool _offeneAnfrage;

        public bool OffeneAnfrage
        {
            get => _offeneAnfrage;
            set
            {
                _offeneAnfrage = value;
                RaisePropertyChanged();
                FillVersammlungen();
            }
        }

        public int RednerSuchenAbstandAnzahlMonate => Helper.Helper.GlobalSettings.RednerSuchenAbstandAnzahlMonate;

        public List<GroupConregation> Redner { get; private set; }

        public ObservableCollection<Talk> VortragListe { get; } = new ObservableCollection<Talk>();

        private List<object> _test;

        public object VortragListeSelected
        {
            get =>
                //if (_test == null)
                //    _test = Versammlungen.Cast<object>().ToList();
                _test;
            set
            {
                _test = (List<object>)value;
                RaisePropertyChanged();
            }
        }

        public void ReadAvailableTalks()
        {
            var gewählt = new List<object>(150);
            VortragListe.Clear();
            var anfragen = new List<Talk>();
            if (VortragCheckOpenRequest)
            {
                foreach(var anfrage in DataContainer.OffeneAnfragen)
                {
                    foreach(var vortrag in anfrage.RednerVortrag.Values)
                    {
                        anfragen.Add(vortrag);
                    }
                }
            }

            foreach (var t in TalkList.GetValid())
            {
                if (VortragCheckFuture && t.ZuletztGehalten != -1 && t.ZuletztGehalten > DateCalcuation.CurrentWeek)
                    continue;

                if (VortragCheckHistory && t.ZuletztGehalten != -1 && t.ZuletztGehalten > DateCalcuation.CurrentWeek + 100)
                    continue;

                if (anfragen.Contains(t))
                    continue;

                VortragListe.Add(t);
                gewählt.Add(t.Nummer);
            }
            VortragListeSelected = gewählt;
        }

        public void ReadData()
        {
            var list = new List<GroupConregation>();
            var vers = _selectedVersammlungen?.Cast<Conregation>();
            if (vers is null)
                return;
            var conregations = vers.ToList();
            foreach (var v in conregations)
            {
                var gC = new GroupConregation
                {
                    Versammlung = v
                };
                list.Add(gC);
            }
            var redner = DataContainer.Redner.Where(x => conregations.Contains(x.Versammlung) && x.Aktiv && x.Einladen).ToList();
            var einladungen = DataContainer.MeinPlan.Where(x => x.Status != EventStatus.Ereignis).Cast<Invitation>();
            var selektierteVorträge = _test.Cast<int>().ToList();
            var invitations = einladungen.ToList();

            foreach (var r in redner)
            {
                var gv = list.First(x => x.Versammlung == r.Versammlung);
                var gr = new GroupSpeaker(gv)
                {
                    Redner = r
                };
                var vorträge = invitations.Where(x => x.Ältester == r).ToList();

                gr.LetzteEinladungKw = vorträge.Count > 0 ? vorträge.Max(x => x.Kw) : -1;

                if (RednerCheckFuture 
                    && gr.LetzteEinladungKw > DateCalcuation.CurrentWeek)
                    continue;

                if (RednerCheckFuture 
                    && DataContainer.OffeneAnfragen.Any(x => x.RednerVortrag.ContainsKey(r)))
                    continue;

                if (RednerCheckHistory 
                    && vorträge.Any(x => DateTime.Today <=  DateCalcuation.CalculateWeek(x.Kw).AddMonths(RednerSuchenAbstandAnzahlMonate) 
                    ))
                    continue;

                if (RednerCheckCancelation 
                    && DataContainer.Absagen.Any(x => x.Ältester == r))
                    continue;

                var anzahlVorträge = 0;
                foreach (var t in r.Vorträge.Where(x => selektierteVorträge.Contains(x.Vortrag.Nummer)).OrderBy(x => x.Vortrag.ZuletztGehalten))
                {
                    var gt = new GroupTalk();
                    var gehalten = DataContainer.MeinPlan.Where(x => x.Vortrag?.Vortrag?.Nummer == t.Vortrag.Nummer).ToList();

                    gt.Vortrag = t.Vortrag;
                    gt.AnzahlGehört = gehalten.Count;
                    gr.Vorträge.Add(gt);
                    anzahlVorträge++;
                }

                if (anzahlVorträge > 0)
                    gv.Redner.Add(gr);
            }

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Redner.Count == 0)
                    list.RemoveAt(i);
            }

            Redner = list;
            RaisePropertyChanged(nameof(Redner));
        }

        public ObservableCollection<Termin> FreieTermine { get; } = new ObservableCollection<Termin>();

        public void ReadTermine()
        {
            FreieTermine.Clear();

            var start = DateCalcuation.GetConregationDay(DateTime.Today);
            var ende = start.AddMonths(Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate).AddDays(7);
            var datum = start;
            var month = datum.AddMonths(-1).Month;
            while (datum < ende)
            {
                var kw = DateCalcuation.CalculateWeek(datum);
                if (DataContainer.MeinPlan.All(x => x.Kw != kw) &&
                    !DataContainer.OffeneAnfragen.Any(x => x.Kws.Contains(kw)))
                {
                    var t = new Termin(datum, kw) { IsFirstDateOfMonth = datum.Month != month };
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

        public string SelectedTerminName
        {
            get { return GetProperty(() => SelectedTerminName); }
            set { SetProperty(() => SelectedTerminName, value, ChangeSelectedTermine); }
        }
        /*<ComboBoxItem Content="nächster freier Termin" />
                                <ComboBoxItem Content="1 Monat" />
                                <ComboBoxItem Content="3 Monate" />
                                <ComboBoxItem Content="6 Monate" />
                                <ComboBoxItem Content="12 Monate" />
                                <ComboBoxItem Content="24 Monate" />
                                <ComboBoxItem Content="Alle Termine" />*/
        private void TerminauswahlCollectionInitialize()
        {
            TerminauswahlCollection = new List<string>
            {
                "nächster freier Termin",
                "1 Monat"
            };
            if (Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate >= 3)
                TerminauswahlCollection.Add("3 Monate");
            if (Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate >= 6)
                TerminauswahlCollection.Add("6 Monate");
            if (Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate >= 12)
                TerminauswahlCollection.Add("12 Monate");
            if (Helper.Helper.GlobalSettings.RednerSuchenAnzahlMonate >= 24)
                TerminauswahlCollection.Add("24 Monate");
            TerminauswahlCollection.Add("Alle Termine");
        }

        public List<string> TerminauswahlCollection { get; private set; }

        private void ChangeSelectedTermine()
        {
            //0 = nächster
            //1 = 1 Monat
            //2 = 3 Monate
            //3 = 6 Monate
            //4 = 12 Monate
            //5 = 24 Monate
            //6 = Alle
            var maxTermin = DateCalcuation.GetConregationDay(DateTime.Today);
            switch (SelectedTerminName)
            {
                case "nächster":
                    maxTermin = FreieTermine.Min(x => x.Datum);
                    break;
                case "1 Monat":
                    maxTermin = maxTermin.AddMonths(1);
                    break;

                case "3 Monate":
                    maxTermin = maxTermin.AddMonths(3);
                    break;

                case "6 Monate":
                    maxTermin = maxTermin.AddMonths(6);
                    break;
                case "12 Monate":
                    maxTermin = maxTermin.AddMonths(12);
                    break;
                case "24 Monate":
                    maxTermin = maxTermin.AddMonths(24);
                    break;
                case "Alle Termine":
                    maxTermin = FreieTermine.Max(x => x.Datum);
                    break;
            }
            foreach (var t in FreieTermine)
            {
                //t.Aktiv = (t.Datum <= maxTermin);
                t.IsChecked = t.Datum <= maxTermin;
                if (SelectedTermine == 0)
                    maxTermin = DateTime.Today;
            }
        }

        #endregion Freie Termine & Redner suchen

        #region Mail versenden

        public GroupConregation AktuelleAnfrage { get; set; }

        public DelegateCommand AnfrageSpeichernCommand { get; }

        public DelegateCommand CopyToClipboardCommand { get; }

        private void AnfrageSpeichern()
        {
            var anfrage = new Inquiry
            {
                AnfrageDatum = DateTime.Today,
                Versammlung = AktuelleAnfrage.Versammlung,
                Id = DataContainer.OffeneAnfragen.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1
            };

            var kommentar = $"Anfrage an Versammlung {AktuelleAnfrage.Versammlung.Name} am {DateTime.Today:dd.MM.yyyy}";

            foreach (var r in AktuelleAnfrage.Redner.Where(x => x.Gewählt))
            {
                var v = r.Vorträge[r.SelectedIndex].Vortrag;
                anfrage.RednerVortrag.Add(r.Redner, v);
                kommentar += $"\t{r.Name}, Vortrag Nr. {v.Nummer} ({v.Thema})" + Environment.NewLine;
            }
            anfrage.Kws.Clear();
            foreach (var d in FreieTermine.Where(x => x.Aktiv).Select(x => x.Kalenderwoche))
            {
                anfrage.Kws.Add(d);
            }
            anfrage.Kommentar = kommentar;
            anfrage.Mailtext = MailText;
            DataContainer.OffeneAnfragen.Add(anfrage);

            ActivityAddItem.RednerAnfragen(anfrage);

            LoadModul1();
        }

        public string MailText
        {
            get { return GetProperty(() => MailText); }
            set { SetProperty(() => MailText, value, CopyToClipboard); }
        }

        private void LoadModul2(GroupConregation inhalt)
        {
            Modul1Visible = new GridLength(0);
            Helper.Helper.GlobalSettings.SearchSpeaker_RednerCheckHistory = RednerCheckHistory;
            Helper.Helper.GlobalSettings.SearchSpeaker_RednerCheckFuture = RednerCheckFuture;
            Helper.Helper.GlobalSettings.SearchSpeaker_VortragCheckFuture = VortragCheckFuture;
            Helper.Helper.GlobalSettings.SearchSpeaker_VortragCheckHistory = VortragCheckHistory;
            Helper.Helper.GlobalSettings.SearchSpeaker_VortragCheckOpenRequest = VortragCheckOpenRequest;
            Helper.Helper.GlobalSettings.SearchSpeaker_RednerCheckCancelation = RednerCheckCancelation;
            Helper.Helper.GlobalSettings.SearchSpeaker_MaxEntfernung = MaxEntfernung;
            Helper.Helper.GlobalSettings.SearchSpeaker_OffeneAnfrage = OffeneAnfrage;
            SaveSelectedKreis();

            Modul2Visible = new GridLength(1, GridUnitType.Star);

            AktuelleAnfrage = inhalt;

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

            MailText = Templates.GetMailTextRednerAnfragen(inhalt.Versammlung, stringRedner, stringFreieTermine);
        }

        public void CopyToClipboard()
        {
            Clipboard.SetText(MailText);
        }

        public GridLength Modul2Visible
        {
            get { return GetProperty(() => Modul2Visible); }
            set { SetProperty(() => Modul2Visible, value); }
        }

        #endregion Mail versenden
    }
}