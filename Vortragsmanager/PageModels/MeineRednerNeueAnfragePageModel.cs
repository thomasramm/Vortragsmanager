using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.Interface;
using Vortragsmanager.Windows;

namespace Vortragsmanager.PageModels
{
    public class MeineRednerNeueAnfragePageModel : ViewModelBase
    {
        public MeineRednerNeueAnfragePageModel()
        {
            Redner = new ObservableCollection<Speaker>(DataContainer.Redner.Where(x => x.Versammlung == DataContainer.MeineVersammlung));
            Versammlungen = DataContainer.Versammlungen;
            SelectedRednerTalks = new ObservableCollection<string>();
            SelectedDatumTalks = new ObservableCollection<Outside>();
            SelectedDatum = DateTime.Today;
            Speichern = new DelegateCommand<bool>(AnfrageSpeichern);
        }

        public DelegateCommand<bool> Speichern { get; }

        private readonly int _startDatum = DateCalcuation.CalculateWeek(DateTime.Today.AddMonths(-1));

        private void AnfrageSpeichern(bool annehmen)
        {
            //Dialog vorbereiten
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            var buchung = new Outside
            {
                Ältester = SelectedRedner,
                Versammlung = SelectedVersammlung,
                Kw = _kalenderwoche,
                Reason = OutsideReason.Talk,
                Vortrag = SelectedVortrag
            };

            var doppelbuchung = DataContainer.ExternerPlan.Any(x => x.Ältester == SelectedRedner && x.Kw == _kalenderwoche)
                || DataContainer.MeinPlan.Where(x => x.Kw == _kalenderwoche && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Any(x => x.Ältester == SelectedRedner);

            if (doppelbuchung)
                if (ThemedMessageBox.Show("Warnung", "Für diesen Redner gibt es an dem Datum schon eine Buchung. Trotzdem Buchung speichern?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.No)
                    return;

            //Anfrage akzeptieren
            if (annehmen)
            {
                data.Titel = "Buchung bestätigen";
                data.MailTextKoordinator = Templates.GetMailTextAnnehmenKoordinator(buchung);
                data.MailTextRedner = Templates.GetMailTextAnnehmenRedner(buchung);
                w.ShowDialog();

                if (data.Speichern)
                {
                    DataContainer.ExternerPlan.Add(buchung);
                    ActivityAddItem.Outside(buchung, data.MailTextKoordinator, data.MailTextRedner, true);
                }
            }
            //Anfrage ablehnen
            else
            {
                data.Titel = "Anfrage ablehnen";
                data.MailTextKoordinator = Templates.GetMailTextAblehnenKoordinator(buchung);
                data.MailTextRedner = null;
                w.ShowDialog();

                if (data.Speichern)
                {
                    ActivityAddItem.Outside(buchung, data.MailTextKoordinator, null, false);
                }
            }
        }

        public ObservableCollection<Speaker> Redner { get; }

        public ObservableCollection<Conregation> Versammlungen { get; }

        public Conregation SelectedVersammlung
        {
            get { return GetProperty(() => SelectedVersammlung); }
            set { SetProperty(() => SelectedVersammlung, value, ParameterValidieren); }
        }

        public DateTime SelectedDatum
        {
            get { return GetProperty(() => SelectedDatum); }
            set { SetProperty(() => SelectedDatum, value, CorrectDate); }
        }

        //wird beim setzen von SelectedDatum über CorrectDate aktualisiert
        private int _kalenderwoche = -1;

        private void CorrectDate()
        {
            if ((int)SelectedDatum.DayOfWeek != (int)DateCalcuation.Wochentag)
            {
                SelectedDatum = DateCalcuation.GetConregationDay(SelectedDatum);
                return;
            }
            _kalenderwoche = DateCalcuation.CalculateWeek(SelectedDatum);

            MeineVersammlung = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == _kalenderwoche);

            SelectedDatumTalks.Clear();
            foreach (var x in DataContainer.ExternerPlan.Where(x => x.Kw == _kalenderwoche))
                SelectedDatumTalks.Add(x);

            ParameterValidieren();
        }

        private bool IsAbwesend()
        {
            return DataContainer.Abwesenheiten.Any(x => x.Redner == SelectedRedner && x.Kw == _kalenderwoche);
        }

        private bool HatAufgabe()
        {
            return DataContainer.AufgabenPersonKalender.Any(x => x.Kw == _kalenderwoche && (x.Vorsitz.VerknüpftePerson == SelectedRedner || x.Leser.VerknüpftePerson == SelectedRedner));
        }

        public Speaker SelectedRedner
        {
            get { return GetProperty(() => SelectedRedner); }
            set { SetProperty(() => SelectedRedner, value, SelectedRednerChanged); }
        }

        public TalkSong SelectedVortrag
        {
            get { return GetProperty(() => SelectedVortrag); }
            set { SetProperty(() => SelectedVortrag, value, ParameterValidieren); }
        }

        public string SelectedVersammlungZeit =>
            SelectedVersammlung != null 
                ? SelectedVersammlung.Zeit.Get(SelectedDatum.Year).ToString() 
                : string.Empty;

        private void ParameterValidieren()
        {
            Hinweis = string.Empty;
            _hinweisLevel = 0;
            RaisePropertyChanged(nameof(SelectedVersammlungZeit));

            //Hinweis zum gewählten Redner
            ParameterValidiert = SelectedRedner != null &&
                                 SelectedVortrag != null &&
                                 SelectedVersammlung != null;

            if (!_parameterValidiert)
                return;

            if (MeineVersammlung?.Status == EventStatus.Zugesagt)
            {
                if (MeineVersammlung is Invitation s && s.Ältester == SelectedRedner)
                    if (SelectedRedner != null)
                        Hinweis += $"{SelectedRedner.Name} hält bereits einen Vortrag in unserer Versammlung!" +
                                   Environment.NewLine;
                _hinweisLevel = 10;
            }
            else if (MeineVersammlung?.Status == EventStatus.Ereignis)
            {
                Hinweis += $"{MeineVersammlung.Anzeigetext}" + Environment.NewLine;
                _hinweisLevel = 5;
            }
            if (SelectedDatumTalks.Any(x => x.Ältester == SelectedRedner))
            {
                if (SelectedRedner != null)
                    Hinweis += $"{SelectedRedner.Name} ist bereits in einer anderen Versammlung eingeladen!" +
                               Environment.NewLine;
                _hinweisLevel = 10;
            }
            if (HatAufgabe())
            {
                Hinweis += $"{SelectedRedner?.Name} hat an dem Datum bereits eine Zuteilung in der Versammlung!" + Environment.NewLine;
                _hinweisLevel = 10;
            }
            if (IsAbwesend())
            {
                Hinweis += $"{SelectedRedner.Name} ist als Abwesend gekennzeichnet" + Environment.NewLine;
                _hinweisLevel = 10;
            }

            //1 Vortrag pro Monat
            var vorher = _kalenderwoche - SelectedRedner.Abstand;
            var nachher = _kalenderwoche + SelectedRedner.Abstand;
            foreach (var zuDicht in _selectedRednerAllTalks.Where(x => x.Kalenderwoche >= vorher && x.Kalenderwoche <= nachher))
            {
                Hinweis += $"Angefragtes Datum ist zu dicht an Vortrag vom {zuDicht.Datum:dd.MM.yyyy}" + Environment.NewLine;
                _hinweisLevel = 5;
            }

            if (SelectedDatumTalks.Count >= 2)
            {
                Hinweis += Environment.NewLine + $"Es sind bereits {SelectedDatumTalks.Count} Redner in anderen Versammlungen" + Environment.NewLine;
                _hinweisLevel = 5;
            }
            RaisePropertyChanged(nameof(HinweisLevelAsFontColor));
        }

        private int _hinweisLevel;

        public Brush HinweisLevelAsFontColor
        {
            get
            {
                switch (_hinweisLevel)
                {
                    case 5:
                        return Helper.Helper.StyleIsDark ? Brushes.Orange : Brushes.OrangeRed;
                    case 10:
                        return Helper.Helper.StyleIsDark ? Brushes.OrangeRed : Brushes.Red;
                    default:
                        return Helper.Helper.StyleIsDark ? Brushes.White : Brushes.Black;
                }
            }
        }

        private bool _parameterValidiert;

        public bool ParameterValidiert
        {
            get => _parameterValidiert;
            set
            {
                _parameterValidiert = value;
                RaisePropertyChanged();
            }
        }

        public IEvent MeineVersammlung
        {
            get { return GetProperty(() => MeineVersammlung); }
            set { SetProperty(() => MeineVersammlung, value); }
        }

        public string Hinweis
        {
            get { return GetProperty(() => Hinweis); }
            set
            {
                if (SetProperty(() => Hinweis, value))
                    RaisePropertyChanged(() => Hinweis);
            }
        }

        private void SelectedRednerChanged()
        {
            //var vorträge = DataContainer.ExternerPlan.Where(x => x.Ältester == SelectedRedner && x.Datum >= DateTime.Today).Select(x => new Core.DataHelper.DateWithConregation(x.Datum, x.Versammlung.Name, x.Vortrag?.Vortrag?.Nummer));
            _selectedRednerAllTalks = DataContainer.SpeakerGetActivities(SelectedRedner);
            LoadRednerTalksZumAngefragtenDatum();
            RaisePropertyChanged(nameof(SelectedRednerTalks));
            ParameterValidieren();
        }

        private void LoadRednerTalksZumAngefragtenDatum()
        {
            SelectedRednerTalks.Clear();
            foreach (var einladung in _selectedRednerAllTalks?
                         .Where(x => x.Kalenderwoche >= _startDatum)?
                         .OrderByDescending(x => x.Kalenderwoche)
                         .Select(x => x.ToString()))
            {
                SelectedRednerTalks.Add(einladung);
            }
        }

        private IEnumerable<DateWithConregation> _selectedRednerAllTalks;

        public ObservableCollection<string> SelectedRednerTalks { get; private set; }

        public ObservableCollection<Outside> SelectedDatumTalks { get; }
    }
}