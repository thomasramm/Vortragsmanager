﻿using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;

namespace Vortragsmanager.MeinPlan
{
    public class MeinPlanViewModel : ViewModelBase
    {
        public MeinPlanViewModel()
        {
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            Monate = new ObservableCollection<MonthViewModel>();
            Monate.Add(new MonthViewModel(1, "Januar", Monate));
            Monate.Add(new MonthViewModel(2, "Februar", Monate));
            Monate.Add(new MonthViewModel(3, "März", Monate));
            Monate.Add(new MonthViewModel(4, "April", Monate));
            Monate.Add(new MonthViewModel(5, "Mai", Monate));
            Monate.Add(new MonthViewModel(6, "Juni", Monate));
            Monate.Add(new MonthViewModel(7, "Juli", Monate));
            Monate.Add(new MonthViewModel(8, "August", Monate));
            Monate.Add(new MonthViewModel(9, "September", Monate));
            Monate.Add(new MonthViewModel(10, "Oktober", Monate));
            Monate.Add(new MonthViewModel(11, "November", Monate));
            Monate.Add(new MonthViewModel(12, "Dezember", Monate));

            Messenger.Default.Register<int>(this, Messages.DisplayYearChanged, OnMessage);
            UpdateMonate();
        }

        public ObservableCollection<MonthViewModel> Monate { get; private set; }

        public DelegateCommand<int> ChangeYear { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")]
        public int CurrentYear => Helper.DisplayedYear;

        public void UpdateMonate()
        {
            foreach (var m in Monate)
            {
                m.Wochen.Clear();
                m.GetWeeks(CurrentYear);
            }
        }

        private void OnMessage(int year)
        {
            RaisePropertyChanged(nameof(CurrentYear));
            UpdateMonate();
        }

        public void ChangeCurrentYear(int step)
        {
            Helper.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
        }
    }

    public class MonthViewModel : ViewModelBase
    {
        public MonthViewModel(int nr, string name, ObservableCollection<MonthViewModel> monate)
        {
            Nr = nr;
            Name = name;

            Wochen = new ObservableCollection<WeekViewModel>();
            Monate = monate;
        }

        public int Nr { get; set; }

        public string Name { get; set; }

        public ObservableCollection<WeekViewModel> Wochen { get; private set; }

        public ObservableCollection<MonthViewModel> Monate { get; private set; }

        public void GetWeeks(int jahr)
        {
            Wochen.Clear();
            var startDate = new DateTime(jahr, Nr, 1);
            var endDate = startDate.AddMonths(1);
            while (startDate < endDate)
            {
                if ((int)startDate.DayOfWeek == (int)Helper.Wochentag)
                {
                    var kw = Helper.CalculateWeek(startDate);
                    Wochen.Add(new WeekViewModel(jahr, this, kw));
                }
                startDate = startDate.AddDays(1);
            }
        }
    }

    //ToDo: Detailansicht bei Klick/Doppelklick zur Vortragsbuchung
    public class WeekViewModel : ViewModelBase
    {
        private int _kalenderwoche;

        public WeekViewModel(int jahr, MonthViewModel monat, int kw)
        {
            Jahr = jahr;
            Monat = monat;
            Kalenderwoche = kw;
            Zuteilung = DataContainer.MeinPlan?.FirstOrDefault(x => x.Kw == kw) ?? null;
            if (Zuteilung == null)
                Zuteilung = DataContainer.OffeneAnfragen?.FirstOrDefault(x => x.Kws.Contains(kw)) ?? null;

            AnzahlAuswärtigeRedner = DataContainer.ExternerPlan?.Count(x => x.Kw == kw) ?? 0;

            BuchungVerschiebenCommand = new DelegateCommand(BuchungVerschieben);
            BuchungLöschenCommand = new DelegateCommand(BuchungLöschen);
            RednerSuchenCommand = new DelegateCommand(RednerSuchen);
            RednerEintragenCommand = new DelegateCommand(RednerEintragen);
            EreignisEintragenCommand = new DelegateCommand(EreignisEintragen);
            AnfrageBearbeitenCommand = new DelegateCommand(AnfrageBearbeiten);
            BuchungBearbeitenCommand = new DelegateCommand(BuchungBearbeiten);
            BuchungErinnernCommand = new DelegateCommand(BuchungErinnern);
            ClickCommand = new DelegateCommand(OnClick);
            ClosePopupCommand = new DelegateCommand(ClosePopup);
        }

        private void OnClick()
        {
            if (IsAnfrage)
                AnfrageBearbeiten();
            else if (IsOffen)
                RednerSuchen();
            else if (IsEreignis)
                EreignisEintragen();
            else if (IsBuchung)
            {
                DetailView = true;
                RaisePropertyChanged(nameof(DetailView));
            }
        }

        private void ClosePopup()
        {
            DetailView = false;
            RaisePropertyChanged(nameof(DetailView));
        }

        public DelegateCommand ClosePopupCommand { get; private set; }

        public DelegateCommand ClickCommand { get; private set; }

        public DelegateCommand EreignisEintragenCommand { get; private set; }

        public DelegateCommand BuchungLöschenCommand { get; private set; }

        public DelegateCommand BuchungVerschiebenCommand { get; private set; }

        public DelegateCommand BuchungBearbeitenCommand { get; private set; }

        public DelegateCommand RednerSuchenCommand { get; private set; }

        public DelegateCommand RednerEintragenCommand { get; private set; }

        public DelegateCommand AnfrageBearbeitenCommand { get; private set; }

        public DelegateCommand BuchungErinnernCommand { get; private set; }

        private void EreignisEintragen()
        {
            var neu = false;
            if (!(Zuteilung is SpecialEvent ev))
            {
                ev = new SpecialEvent() { Kw = Kalenderwoche };
                neu = true;
            }
            var dialog = new EreignisEintragenCommandDialog();
            var data = (EreignisEintragenCommandDialogView)dialog.DataContext;
            data.Event = ev;

            dialog.ShowDialog();

            if (data.Speichern)
            {
                if (neu)
                    DataContainer.MeinPlanAdd(ev);
                ActivityLog.AddActivity.EreignisBearbeiten(ev, neu ? ActivityLog.Types.EreignisAnlegen : ActivityLog.Types.EreignisBearbeiten);
                Zuteilung = ev;
                Monat.GetWeeks(Jahr);
            }
        }

        private void RednerEintragen()
        {
            var dialog = new RednerEintragenDialog();
            var data = (RednerEintragenView)dialog.DataContext;
            dialog.ShowDialog();
            if (!data.Speichern)
                return;

            var i = new Invitation
            {
                Kw = Kalenderwoche,
                Status = EventStatus.Zugesagt,
                Ältester = data.SelectedRedner,
                Vortrag = data.SelectedVortrag
            };
            Zuteilung = i;
            DataContainer.MeinPlanAdd(i);
            ActivityLog.AddActivity.RednerEintragen(i);

            Monat.GetWeeks(Jahr);
        }

        public void BuchungLöschen()
        {
            if (Zuteilung.Status == EventStatus.Ereignis)
            {

                DataContainer.MeinPlanRemove(Zuteilung);
                var ereignis = (Zuteilung as SpecialEvent);
                ActivityLog.AddActivity.EreignisBearbeiten(ereignis, ActivityLog.Types.EreignisLöschen);
                Monat.GetWeeks(Jahr);
                return;
            }

            var zuteilung = Zuteilung as Invitation;

            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            string mailtext;
            if (zuteilung.Ältester.Versammlung == DataContainer.MeineVersammlung)
            {
                data.MailTextRedner = Templates.GetMailTextAblehnenRedner(zuteilung);
                mailtext = data.MailTextRedner;
            }
            else
            {
                data.MailTextKoordinator = Templates.GetMailTextAblehnenKoordinator(zuteilung);
                mailtext = data.MailTextKoordinator;
            }

            w.ShowDialog();
            if (!data.Speichern)
                return;

            DataContainer.MeinPlanRemove(Zuteilung);
            DataContainer.Absagen.Add(new Cancelation(zuteilung.Kw, zuteilung.Ältester, zuteilung.Status));
            ActivityLog.AddActivity.BuchungLöschen(zuteilung, mailtext);
            Monat.GetWeeks(Jahr);
        }

        public void BuchungVerschieben()
        {
            var verschieben = new KalendereintragVerschieben();
            var data = (KalendereintragVerschiebenView)verschieben.DataContext;
            data.KalenderTyp = Kalenderart.Intern;
            data.LadeStartDatum(Zuteilung);
            verschieben.ShowDialog();

            if (!data.Speichern)
                return;

            //StartBuchung aktualisieren
            Monat.GetWeeks(Jahr);
            //ZielBuchung aktualisieren
            var zielMonatNr = data.ZielDatum.Month;
            if (zielMonatNr != Monat.Nr)
                Monat.Monate.Single(x => x.Nr == zielMonatNr).GetWeeks(Jahr);
        }

        public void BuchungBearbeiten()
        {
            if (Zuteilung.Status == EventStatus.Ereignis)
            {
                EreignisEintragen();
                return;
            }
            var dialog = new RednerEintragenDialog();
            var data = (RednerEintragenView)dialog.DataContext;
            data.SelectedVersammlung = Einladung.Ältester.Versammlung;
            data.SelectedRedner = Einladung.Ältester;
            data.SelectedVortrag = Einladung.Vortrag;
            dialog.ShowDialog();
            if (!data.Speichern)
                return;

            ActivityLog.AddActivity.EinladungBearbeiten(Einladung, data.SelectedRedner, data.SelectedVortrag);

            Einladung.Ältester = data.SelectedRedner;
            if (Einladung.Vortrag?.Vortrag != data.SelectedVortrag?.Vortrag)
            {
                DataContainer.UpdateTalkDate(Einladung.Vortrag?.Vortrag);
                DataContainer.UpdateTalkDate(data.SelectedVortrag?.Vortrag);
            }
            Einladung.Vortrag = data.SelectedVortrag;
            Monat.GetWeeks(Jahr);
        }

        public void AnfrageBearbeiten()
        {
            var dev = new AntwortEintragenDialog();
            var data = (AntwortEintragenViewModel)dev.Control.DataContext;
            data.LoadData(Zuteilung as Inquiry);
            dev.ShowDialog();
            Messenger.Default.Send(0, Messages.DisplayYearChanged);
        }

        public void RednerSuchen()
        {
            Navigation.NavigationView.Frame.Navigate("SearchSpeaker", Tag);
        }

        public void BuchungErinnern()
        {
            var mail = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)mail.DataContext;
            data.MailTextKoordinator = Templates.GetMailTextRednerErinnerung(Zuteilung as Invitation);
            data.DisableCancelButton();
            mail.ShowDialog();
            ActivityLog.AddActivity.RednerErinnern(Zuteilung as Invitation, data.MailTextKoordinator);
            Einladung.ErinnerungsMailGesendet = true;
            RaisePropertyChanged(nameof(ErinnerungsMailSenden));
        }

        public IEvent Zuteilung { get; set; }

        public Invitation Einladung => Zuteilung as Invitation;

        public int Jahr { get; }

        public MonthViewModel Monat { get; }

        public int Kalenderwoche
        {
            get => _kalenderwoche;
            set
            {
                _kalenderwoche = value;
                Tag = Helper.CalculateWeek(value);
            }
        }

        public DateTime Tag { get; private set; }

        public SolidColorBrush Background
        {
            get
            {
                var color = Color.FromRgb(51, 51, 51);
                if (Zuteilung == null)
                    color = Colors.Tomato;
                else if (Zuteilung.Status == EventStatus.Anfrage)
                    color = Colors.Orange;
                else if (Zuteilung.Status == EventStatus.Ereignis)
                    color = Colors.SlateGray;
                return new SolidColorBrush(color);
            }
        }

        public SolidColorBrush FontColor
        {
            get
            {
                var c = Colors.White;

                if (Einladung?.Ältester?.Versammlung == DataContainer.MeineVersammlung)
                    c = Colors.LightGreen;

                return new SolidColorBrush(c);
            }
        }

        public int AnzahlAuswärtigeRedner
        {
            get { return GetProperty(() => AnzahlAuswärtigeRedner); }
            set { SetProperty(() => AnzahlAuswärtigeRedner, value); }
        }

        public string Anzeigetext
        {
            get
            {
                if (Zuteilung == null)
                    return "offen";
                return Zuteilung.Anzeigetext;
            }
        }

        public bool IsAnfrage => Zuteilung?.Status == EventStatus.Anfrage;

        public bool IsBuchung => Zuteilung?.Status == EventStatus.Zugesagt || Zuteilung?.Status == EventStatus.Ereignis;

        public bool IsEinladung => Zuteilung?.Status == EventStatus.Zugesagt;

        public bool IsEreignis => Zuteilung?.Status == EventStatus.Ereignis;

        public bool IsOffen => Zuteilung == null;

        public bool DetailView { get; set; }

        public bool ShowActivityButtons => Properties.Settings.Default.ShowActivityButtons;

        public bool ErinnerungsMailSenden => Einladung?.ErinnerungsMailGesendet ?? false;

        public string Woche => $"{Tag.Day:00}";

        public override string ToString()
        {
            return $"{Tag.Day:00}";
        }
    }
}