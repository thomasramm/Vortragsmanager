﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                Datum = SelectedDatum,
                Reason = OutsideReason.Talk,
                Vortrag = SelectedVortrag
            };

            var doppelbuchung = DataContainer.ExternerPlan.Any(x => x.Ältester == SelectedRedner && x.Datum == SelectedDatum)
                || DataContainer.MeinPlan.Where(x => x.Datum == SelectedDatum && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Any(x => x.Ältester == SelectedRedner);

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

        private void CorrectDate()
        {
            if (SelectedDatum.DayOfWeek != DayOfWeek.Sunday)
            {
                SelectedDatum = SelectedDatum.AddDays(7 - (int)SelectedDatum.DayOfWeek);
                return;
            }

            MeineVersammlung = DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == SelectedDatum);

            SelectedDatumTalks.Clear();
            foreach (var x in DataContainer.ExternerPlan.Where(x => x.Datum == SelectedDatum))
                SelectedDatumTalks.Add(x);

            ParameterValidieren();
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

        private void ParameterValidieren()
        {
            ParameterValidiert = SelectedRedner != null && SelectedDatum != null;
            Hinweis = string.Empty;

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

            if (SelectedDatumTalks.Count >= 2)
                Hinweis += $"Es sind bereits {SelectedDatumTalks.Count} Redner in anderen Versammlungen" + Environment.NewLine;

            //1 Vortrag pro Monat
            var vorher = SelectedDatum.AddMonths(-1);
            var nachher = SelectedDatum.AddMonths(1);
            foreach (var t in _selectedRednerTalkDates)
            {
                if (t >= vorher && t <= nachher)
                {
                    Hinweis += $"Angefragtes Datum ist zu dicht an Vortrag vom {t:dd.MM.yyyy}" + Environment.NewLine;
                    break;
                }
            }
        }

        private bool _parameterValidiert = false;

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
            set { SetProperty(() => Hinweis, value); }
        }

        private void SelectedRednerChanged()
        {
            //var vorträge = DataContainer.ExternerPlan.Where(x => x.Ältester == SelectedRedner && x.Datum >= DateTime.Today).Select(x => new Core.DataHelper.DateWithConregation(x.Datum, x.Versammlung.Name, x.Vortrag?.Vortrag?.Nummer));
            var vorträge = DataContainer.SpeakerGetActivities(SelectedRedner,10);
            //vorträge = vorträge.Union(DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Zugesagt && x.Datum >= DateTime.Today).Cast<Invitation>().Where(x => x.Ältester == SelectedRedner).Select(x => new Core.DataHelper.DateWithConregation(x.Datum, DataContainer.MeineVersammlung.Name, x.Vortrag?.Vortrag?.Nummer)));
            _selectedRednerTalkDates = new ObservableCollection<DateTime>(vorträge.Select(x => x.Datum));
            SelectedRednerTalks = new ObservableCollection<string>(vorträge.Select(x => x.ToString()));
            RaisePropertyChanged(nameof(SelectedRednerTalks));
            ParameterValidieren();
        }

        private IEnumerable<DateTime> _selectedRednerTalkDates;

        public ObservableCollection<string> SelectedRednerTalks { get; private set; }

        public ObservableCollection<Outside> SelectedDatumTalks { get; private set; }
    }
}