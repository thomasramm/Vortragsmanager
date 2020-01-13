﻿using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class ExternalQuestionViewModel : ViewModelBase
    {
        public ExternalQuestionViewModel()
        {
            Redner = new ObservableCollection<Speaker>(Core.DataContainer.Redner.Where(X => X.Versammlung == Core.DataContainer.MeineVersammlung));
            Versammlungen = Core.DataContainer.Versammlungen;
            SelectedRednerTalks = new ObservableCollection<DateTime>();
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

            //Anfrage akzeptieren
            if (annehmen)
            {
                data.Titel = "Buchung bestätigen";
                data.MailTextKoordinator = Core.Templates.GetMailTextAnnehmenKoordinator(buchung);
                data.MailTextRedner = Core.Templates.GetMailTextAnnehmenRedner(buchung);
                w.ShowDialog();

                if (data.Speichern)
                {
                    Core.DataContainer.ExternerPlan.Add(buchung);
                }
            }
            //Anfrage ablehnen
            else
            {
                data.Titel = "Anfrage ablehnen";
                data.MailTextKoordinator = Core.Templates.GetMailTextAblehnenKoordinator(buchung);
                data.MailTextRedner = null;
                w.ShowDialog();
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

            MeineVersammlung = Core.DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == SelectedDatum);

            SelectedDatumTalks.Clear();
            foreach (var x in Core.DataContainer.ExternerPlan.Where(x => x.Datum == SelectedDatum))
                SelectedDatumTalks.Add(x);

            ParameterValidieren();
        }

        public Speaker SelectedRedner
        {
            get { return GetProperty(() => SelectedRedner); }
            set { SetProperty(() => SelectedRedner, value, SelectedRednerChanged); }
        }

        public Talk SelectedVortrag
        {
            get { return GetProperty(() => SelectedVortrag); }
            set { SetProperty(() => SelectedVortrag, value, ParameterValidieren); }
        }

        private void ParameterValidieren()
        {
            ParameterValidiert = SelectedVortrag != null
                && SelectedRedner != null
                && SelectedDatum != null
                && SelectedVersammlung != null;

            Hinweis = string.Empty;

            if (!_parameterValidiert)
                return;

            if (MeineVersammlung?.Status == EventStatus.Zugesagt)
            {
                var s = (MeineVersammlung as Invitation);
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
            foreach (var t in SelectedRednerTalks)
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
            var vorträge = Core.DataContainer.ExternerPlan.Where(x => x.Ältester == SelectedRedner && x.Datum >= DateTime.Today).Select(x => x.Datum);
            vorträge = vorträge.Union(Core.DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Zugesagt && x.Datum >= DateTime.Today).Cast<Invitation>().Where(x => x.Ältester == SelectedRedner).Select(x => x.Datum));
            SelectedRednerTalks = new ObservableCollection<DateTime>(vorträge.OrderBy(x => x));
            RaisePropertyChanged(nameof(SelectedRednerTalks));
            ParameterValidieren();
        }

        public ObservableCollection<DateTime> SelectedRednerTalks { get; private set; }

        public ObservableCollection<Outside> SelectedDatumTalks { get; private set; }
    }
}