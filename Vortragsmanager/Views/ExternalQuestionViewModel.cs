using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class ExternalQuestionViewModel : ViewModelBase
    {
        public ExternalQuestionViewModel()
        {
            Redner = new ObservableCollection<Speaker>(Core.DataContainer.Redner.Where(X => X.Versammlung == Core.DataContainer.MeineVersammlung));
            Versammlungen = Core.DataContainer.Versammlungen;
            SelectedRednerTalks = new ObservableCollection<Outside>();
            SelectedDatumTalks = new ObservableCollection<Outside>();
            SelectedDatum = DateTime.Today;
            Speichern = new DelegateCommand<bool>(AnfrageSpeichern);
        }

        public DelegateCommand<bool> Speichern { get; private set; }

        private void AnfrageSpeichern(bool annehmen)
        {
            //ToDo: Anfrage annehmen -> MailText für Redner
            //ToDo: Anfrage ablehnen -> MailText für Koordinator
            var i = new Outside
            {
                Ältester = SelectedRedner,
                Versammlung = SelectedVersammlung,
                Datum = SelectedDatum,
                Reason = OutsideReason.Talk,
                Vortrag = SelectedVortrag
            };
            var v = new AnfrageBestätigenViewModel(i, annehmen, annehmen);
            var w = new AnfrageBestätigenDialog
            {
                DataContext = v
            };

            w.ShowDialog();
            var data = (AnfrageBestätigenViewModel)w.DataContext;
            if (data.Speichern)
            {
                Core.DataContainer.ExternerPlan.Add(i);
            }
        }

        public ObservableCollection<Speaker> Redner { get; }

        public ObservableCollection<Conregation> Versammlungen { get; }

        public Conregation SelectedVersammlung
        {
            get { return GetProperty(() => SelectedVersammlung); }
            set { SetProperty(() => SelectedVersammlung, value); }
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
            SelectedDatumTalks = new ObservableCollection<Outside>(Core.DataContainer.ExternerPlan.Where(x => x.Datum == SelectedDatum));
        }

        public Speaker SelectedRedner
        {
            get { return GetProperty(() => SelectedRedner); }
            set { SetProperty(() => SelectedRedner, value, SelectedRednerChanged); }
        }

        public Talk SelectedVortrag
        {
            get { return GetProperty(() => SelectedVortrag); }
            set { SetProperty(() => SelectedVortrag, value); }
        }

        public Invitation MeineVersammlung
        {
            get { return GetProperty(() => MeineVersammlung); }
            set { SetProperty(() => MeineVersammlung, value); }
        }

        private void SelectedRednerChanged()
        {
            SelectedRednerTalks = new ObservableCollection<Outside>(Core.DataContainer.ExternerPlan.Where(x => x.Ältester == SelectedRedner && x.Datum >= DateTime.Today).OrderBy(x => x.Datum));
            RaisePropertyChanged(nameof(SelectedRednerTalks));
        }

        public ObservableCollection<Outside> SelectedRednerTalks { get; private set; }

        public ObservableCollection<Outside> SelectedDatumTalks { get; private set; }
    }
}