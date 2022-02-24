using System;
using System.Linq;
using System.Windows.Media;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.UserControls;
using Vortragsmanager.Views;

namespace Vortragsmanager.PageModels
{
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
                ActivityLog.ActivityAddItem.EreignisBearbeiten(ev, neu ? ActivityTypes.EreignisAnlegen : ActivityTypes.EreignisBearbeiten);
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
            ActivityLog.ActivityAddItem.RednerEintragen(i);

            Monat.GetWeeks(Jahr);
        }

        public void BuchungLöschen()
        {
            if (Zuteilung.Status == EventStatus.Ereignis)
            {

                DataContainer.MeinPlanRemove(Zuteilung);
                var ereignis = (Zuteilung as SpecialEvent);
                ActivityLog.ActivityAddItem.EreignisBearbeiten(ereignis, ActivityTypes.EreignisLöschen);
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
            ActivityLog.ActivityAddItem.BuchungLöschen(zuteilung, mailtext);
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

            ActivityLog.ActivityAddItem.EinladungBearbeiten(Einladung, data.SelectedRedner, data.SelectedVortrag);

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
            ActivityLog.ActivityAddItem.RednerErinnern(Zuteilung as Invitation, data.MailTextKoordinator);
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
                Tag = DateCalcuation.CalculateWeek(value);
            }
        }

        public DateTime Tag { get; private set; }

        public SolidColorBrush Background
        {
            get
            {
                var color = Helper.Helper.StyleIsDark ? Color.FromRgb(51, 51, 51) : Colors.White;
                if (Zuteilung == null)
                    color = Colors.Tomato;
                else if (Zuteilung.Status == EventStatus.Anfrage)
                    color = Colors.Orange;
                else if (Zuteilung.Status == EventStatus.Ereignis)
                    color = Helper.Helper.StyleIsDark ? Colors.SlateGray : (Color)ColorConverter.ConvertFromString("#2a8dd4");
                return new SolidColorBrush(color);
            }
        }

        public SolidColorBrush FontColor
        {
            get
            {
                var c = Helper.Helper.StyleIsDark ? Colors.White : Colors.Black;

                if (Einladung?.Ältester?.Versammlung == DataContainer.MeineVersammlung)
                    c = Helper.Helper.StyleIsDark ? Colors.LightGreen : Colors.LimeGreen;

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