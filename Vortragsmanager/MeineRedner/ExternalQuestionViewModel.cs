﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.Core.DataHelper;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;

namespace Vortragsmanager.MeineRedner
{
    public class ExternalQuestionViewModel : ViewModelBase
    {
        public ExternalQuestionViewModel()
        {
            Redner = new ObservableCollection<Speaker>(DataContainer.Redner.Where(X => X.Versammlung == DataContainer.MeineVersammlung));
            Versammlungen = DataContainer.Versammlungen;
            SelectedRednerTalks = new ObservableCollection<string>();
            SelectedDatumTalks = new ObservableCollection<Outside>();
            SelectedDatum = DateTime.Today;
            Speichern = new DelegateCommand<bool>(AnfrageSpeichern);
        }

        public DelegateCommand<bool> Speichern { get; private set; }

        private void AnfrageSpeichern(bool annehmen)
        {
            //Dialog vorbereiten
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            var buchung = new Outside
            {
                Ältester = SelectedRedner,
                Versammlung = SelectedVersammlung,
                Kw = Kalenderwoche,
                Reason = OutsideReason.Talk,
                Vortrag = SelectedVortrag
            };

            var doppelbuchung = DataContainer.ExternerPlan.Any(x => x.Ältester == SelectedRedner && x.Kw == Kalenderwoche)
                || DataContainer.MeinPlan.Where(x => x.Kw == Kalenderwoche && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Any(x => x.Ältester == SelectedRedner);

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
                    ActivityLog.AddActivity.Outside(buchung, data.MailTextKoordinator, data.MailTextRedner, true);
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
                    ActivityLog.AddActivity.Outside(buchung, data.MailTextKoordinator, null, false);
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
        private int Kalenderwoche = -1;

        private void CorrectDate()
        {
            if ((int)SelectedDatum.DayOfWeek != (int)Core.Helper.Wochentag)
            {
                SelectedDatum = Core.Helper.GetConregationDay(SelectedDatum);
                return;
            }
            Kalenderwoche = Core.Helper.CalculateWeek(SelectedDatum);

            MeineVersammlung = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == Kalenderwoche);

            SelectedDatumTalks.Clear();
            foreach (var x in DataContainer.ExternerPlan.Where(x => x.Kw == Kalenderwoche))
                SelectedDatumTalks.Add(x);

            LoadRednerTalksZumAngefragtenDatum();

            ParameterValidieren();
        }

        private bool IsAbwesend()
        {
            return DataContainer.Abwesenheiten.Any(x => x.Redner == SelectedRedner && x.Kw == Kalenderwoche);
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

        public string SelectedVersammlungZeit
        {
            get
            {
                if (SelectedDatum != null && SelectedVersammlung != null)
                    return SelectedVersammlung.Zeit.Get(SelectedDatum.Year).ToString();
                else
                    return string.Empty;
            }
        }

        private void ParameterValidieren()
        {
            Hinweis = string.Empty;
            RaisePropertyChanged(nameof(SelectedVersammlungZeit));

            //Hinweis zum gewählten Redner
            ParameterValidiert = SelectedRedner != null && SelectedDatum != null;
            if (!_parameterValidiert)
                return;

            if (MeineVersammlung?.Status == EventStatus.Zugesagt)
            {
                var s = MeineVersammlung as Invitation;
                if (s.Ältester == SelectedRedner)
                    Hinweis += $"{SelectedRedner.Name} hält bereits einen Vortrag in unserer Versammlung" + Environment.NewLine;
            }
            if (SelectedDatumTalks.Any(x => x.Ältester == SelectedRedner))
                Hinweis += $"{SelectedRedner.Name} ist bereits in einer anderen Versammlung eingeladen" + Environment.NewLine;

            if (IsAbwesend())
                Hinweis += $"{SelectedRedner.Name} ist als Abwesend gekennzeichnet" + Environment.NewLine;

            //1 Vortrag pro Monat
            var vorher = Kalenderwoche - 4;
            var nachher = Kalenderwoche + 4;
            foreach(var zuDicht in _selectedRednerAllTalks.Where(x => x.Kalenderwoche >= vorher && x.Kalenderwoche <= nachher))
            {
                Hinweis += $"Angefragtes Datum ist zu dicht an Vortrag vom {zuDicht.Datum:dd.MM.yyyy}" + Environment.NewLine;
            }

            if (SelectedDatumTalks.Count >= 2)
                Hinweis += Environment.NewLine + $"Es sind bereits {SelectedDatumTalks.Count} Redner in anderen Versammlungen" + Environment.NewLine;

        }

        private bool _parameterValidiert;

        public bool ParameterValidiert
        {
            get
            {
                return _parameterValidiert;
            }
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
            set {
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
            if (_selectedRednerAllTalks != null)
                SelectedRednerTalks = new ObservableCollection<string>(_selectedRednerAllTalks?.Where(x => x.Kalenderwoche >= Kalenderwoche - 4)?.OrderByDescending(x => x.Kalenderwoche).Select(x => x.ToString()));
            else
                SelectedRednerTalks.Clear();
        }

        private IEnumerable<DateWithConregation> _selectedRednerAllTalks;

        public ObservableCollection<string> SelectedRednerTalks { get; private set; }

        public ObservableCollection<Outside> SelectedDatumTalks { get; private set; }
    }
}