using System;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    public class KalendereintragVerschiebenView : ViewModelBase
    {
        public KalendereintragVerschiebenView()
        {
            SaveCommand = new DelegateCommand<ICloseable>(Save);

            CloseCommand = new DelegateCommand<ICloseable>(Close);
        }

        public DelegateCommand<ICloseable> SaveCommand { get; }

        public DelegateCommand<ICloseable> CloseCommand { get; }

        public void LadeStartDatum(IEvent ereignis)
        {
            if (ereignis is null)
                return;

            KalenderTyp = Kalenderart.Intern;

            StartEvent = ereignis;

            if (ereignis.Status == EventStatus.Zugesagt)
            {
                var invitation = (ereignis as Invitation);
                if (invitation is null) return;
                StartTyp = "Versammlung";
                StartName = invitation.Ältester.Name;
                StartVortrag = invitation.Vortrag.Vortrag.ToString();
                StartVersammlung = invitation.Ältester.Versammlung.Name;
            }

            if (ereignis.Status == EventStatus.Ereignis)
            {
                var special = (ereignis as SpecialEvent);
                if (special is null) return;
                StartTyp = "Ereignis";
                StartVersammlung = special.Anzeigetext;
                StartName = special.Vortragender;
                if (string.IsNullOrEmpty(StartName))
                    StartName = StartVersammlung == special.Name ? "" : special.Name;
                StartVortrag = special.Vortrag?.Vortrag?.NumberTopicShort;
                if (string.IsNullOrWhiteSpace(StartVortrag))
                    StartVortrag = special.Thema;
            }

            ZielDatum = DateCalcuation.CalculateWeek(ereignis.Kw);
            StartDatum = ZielDatum.ToShortDateString();
        }

        public void LadeStartDatum(Outside ereignis)
        {
            if (ereignis is null)
                return;

            KalenderTyp = Kalenderart.Extern;

            StartEvent = null;
            StartBuchung = ereignis;

            StartTyp = "Versammlung";
            StartName = ereignis.Ältester.Name;
            StartVortrag = ereignis.Vortrag.Vortrag.ToString();
            StartVersammlung = ereignis.Versammlung.Name;

            ZielDatum = DateCalcuation.CalculateWeek(ereignis.Kw);
            StartDatum = ZielDatum.ToShortDateString();
        }

        public string StartTyp { get; set; }

        public string StartVersammlung { get; set; }

        public string StartName { get; set; }

        public string StartVortrag { get; set; }

        public string StartDatum { get; set; }

        private string _zielVersammlung;

        public string ZielVersammlung
        {
            get => _zielVersammlung;
            set
            {
                _zielVersammlung = value;
                RaisePropertyChanged();
            }
        }

        private string _zielName;

        public string ZielName
        {
            get => _zielName;
            set
            {
                _zielName = value;
                RaisePropertyChanged();
            }
        }

        private string _zielVortrag;

        public string ZielVortrag
        {
            get => _zielVortrag;
            set
            {
                _zielVortrag = value;
                RaisePropertyChanged();
            }
        }

        private string _zielTyp = "Versammlung";

        public string ZielTyp
        {
            get => _zielTyp;
            set
            {
                _zielTyp = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _zielDatum;

        public DateTime ZielDatum
        {
            get => _zielDatum;
            set
            {
                _zielDatum = DateCalcuation.GetConregationDay(value);
                RaisePropertyChanged();
                LadeZielBuchung();
            }
        }

        private bool _zielBuchungBelegt;

        public bool ZielBuchungBelegt
        {
            get => _zielBuchungBelegt;
            set
            {
                if (_zielBuchungBelegt != value)
                {
                    if (value)
                        WindowHeight += 220;
                    else
                        WindowHeight -= 220;
                }
                _zielBuchungBelegt = value;
                RaisePropertyChanged();
            }
        }

        private int _windowHeight = 320;

        public int WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                RaisePropertyChanged();
            }
        }

        private void LadeZielBuchung()
        {
            switch (KalenderTyp)
            {
                case Kalenderart.Extern:
                    LadeExterneZielBuchung();
                    break;

                case Kalenderart.Intern:
                    LadeInterneZielBuchung();
                    break;
            }
        }

        private void LadeExterneZielBuchung()
        {
            var zielKw = DateCalcuation.CalculateWeek(ZielDatum);
            var woche = DataContainer.ExternerPlan.FirstOrDefault(x => x.Kw == zielKw);

            ZielBuchungBelegt = (woche != null && zielKw != StartBuchung.Kw);
            ZielBuchung = woche;

            if (woche == null)
                return;

            ZielTyp = "Versammlung";
            ZielVersammlung = woche.Versammlung.Name;
            ZielName = woche.Ältester.Name;
            ZielVortrag = woche.Vortrag.Vortrag.ToString();
        }

        private void LadeInterneZielBuchung()
        {
            var zielKw = DateCalcuation.CalculateWeek(ZielDatum);
            var woche = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == zielKw);

            ZielBuchungBelegt = (woche != null);

            if (ZielBuchungBelegt)
            {
                if (woche != null && woche.Status == EventStatus.Zugesagt)
                {
                    var invitation = (woche as Invitation);
                    ZielTyp = "Versammlung";
                    if (invitation != null)
                    {
                        ZielVersammlung = invitation.Ältester.Versammlung.Name;
                        ZielName = invitation.Ältester.Name;
                        ZielVortrag = invitation.Vortrag.Vortrag.ToString();
                    }
                }

                if (woche != null && woche.Status == EventStatus.Ereignis)
                {
                    var ereignis = (woche as SpecialEvent);
                    ZielTyp = "Ereignis";
                    if (ereignis != null)
                    {
                        ZielVersammlung = ereignis.Anzeigetext;
                        ZielName = ereignis.Vortragender;
                        if (string.IsNullOrEmpty(ZielName))
                            ZielName = ZielVersammlung == ereignis.Name ? "" : ereignis.Name;
                        ZielVortrag = string.IsNullOrWhiteSpace(ereignis.Vortrag?.Vortrag?.NumberTopicShort) 
                            ? ereignis.Thema 
                            : ereignis.Vortrag.Vortrag.ToString();
                    }
                }
                ZielEvent = woche;
                return;
            }

            var anfrage = DataContainer.OffeneAnfragen.FirstOrDefault(x => x.Kw == zielKw);
            if (anfrage == null)
                return;

            ZielVersammlung = anfrage.Versammlung.Name;
            ZielName = anfrage.RednerVortrag.Keys.First().Name + " ...";
            ZielVortrag = "OFFENE ANFRAGE";
            ZielEvent = anfrage;
            ZielBuchungBelegt = true;
        }

        private IEvent ZielEvent { get; set; }

        private IEvent StartEvent { get; set; }

        private Outside StartBuchung { get; set; }

        public Outside ZielBuchung { get; set; }

        public Kalenderart KalenderTyp { get; set; }

        public bool Speichern { get; set; }

        private int _zielbuchungOptionChecked;

        public bool ZielbuchungLöschenChecked
        {
            get => _zielbuchungOptionChecked == 0;
            set
            {
                _zielbuchungOptionChecked = value ? 0 : -1;
                RaisePropertyChanged();
            }
        }

        public bool ZielbuchungTauschenChecked
        {
            get => _zielbuchungOptionChecked == 1;
            set
            {
                _zielbuchungOptionChecked = value ? 1 : -1;
                RaisePropertyChanged();
            }
        }
        
        public bool ZielbuchungBeideChecked
        {
            get => _zielbuchungOptionChecked == 2;
            set
            {
                _zielbuchungOptionChecked = value ? 2 : -1;
                RaisePropertyChanged();
            }
        }

        public bool ZielBuchungBeideCheckboxVisible => KalenderTyp == Kalenderart.Extern;

        public void Save(ICloseable window)
        {
            switch (KalenderTyp)
            {
                case Kalenderart.Intern:
                    SaveIntern();
                    break;
                case Kalenderart.Extern:
                    SaveExtern();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(KalenderTyp.ToString());
            }
            
            //Event2
            Speichern = true;
            window?.Close();
        }

        private void SaveIntern()
        {
            var startKw = StartEvent.Kw;
            var startDatum = DateCalcuation.CalculateWeek(StartEvent.Kw);
            var zielKw = DateCalcuation.CalculateWeek(ZielDatum);

            StartEvent.Kw = zielKw;
            var sendMail = false;

            var mails = new InfoAnRednerUndKoordinatorWindow();
            var mailsData = (InfoAnRednerUndKoordinatorViewModel)mails.DataContext;
            mailsData.DisableCancelButton();

            //MAIL WEGEN STARTBUCHUNG
            if (StartEvent.Status == EventStatus.Zugesagt)
            {
                var ev = (StartEvent as Invitation);
                if (ev != null && ev.Ältester.Versammlung == DataContainer.MeineVersammlung)
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Redner";
                    mailsData.MailTextKoordinator = Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, startDatum, ZielDatum, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }
                else
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Koordinator";
                    if (ev != null)
                        mailsData.MailTextKoordinator = Templates.GetMailTextEreignisTauschenAnKoordinator(
                            ev.Ältester.Versammlung, startDatum, ZielDatum, ev.Ältester.Name,
                            ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }

                sendMail = true;
            }
            var startBuchungInfo = string.Empty;

            //MAIL & AKTIVITÄT WEGEN ZIELBUCHUNG
            if (ZielBuchungBelegt)
            {
                var headerText = string.Empty;
                if (ZielbuchungTauschenChecked)
                {
                    switch (ZielEvent.Status)
                    {
                        case EventStatus.Anfrage:
                        {
                            if (ZielEvent is Inquiry ev)
                            {
                                ev.Kws.Remove(zielKw);
                                ev.Kws.Add(startKw);
                            }

                            break;
                        }
                        case EventStatus.Ereignis:
                            ZielEvent.Kw = startKw;
                            break;
                        default:
                        {
                            ZielEvent.Kw = startKw;
                            var ev = (ZielEvent as Invitation);
                            if (ev != null && ev.Ältester.Versammlung == DataContainer.MeineVersammlung)
                            {
                                mailsData.InfoAnRednerTitel = "Info an Redner";
                                mailsData.MailTextRedner = Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, ZielDatum, startDatum, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                            }
                            else
                            {
                                mailsData.InfoAnRednerTitel = "Info an Koordinator";
                                if (ev != null)
                                    mailsData.MailTextRedner =
                                        Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung,
                                            ZielDatum, startDatum, ev.Ältester.Name, ev.Vortrag.Vortrag.ToString(),
                                            ev.Ältester.Versammlung.Name);
                            }
                            sendMail = true;
                            break;
                        }
                    }
                    startBuchungInfo = "Die Buchung am neuen Datum wurde mit dem bisherigen Datum getauscht.";
                    headerText = "Diese Buchung wurde verschoben";
                }
                else if (ZielbuchungLöschenChecked)
                {
                    switch (ZielEvent.Status)
                    {
                        case EventStatus.Anfrage:
                            if (ZielEvent is Inquiry ev)
                            {
                                ev.Kws.Remove(zielKw);
                                if (ev.Kws.Count == 0)
                                    DataContainer.OffeneAnfragen.Remove(ev);
                            }

                            //ToDo: Info an Versammlung über doppelbuchung der Anfrage??
                            break;

                        case EventStatus.Zugesagt:
                            var inv = (ZielEvent as Invitation);
                            if (inv != null && inv.Ältester.Versammlung == DataContainer.MeineVersammlung)
                            {
                                mailsData.InfoAnRednerTitel = "Info an Redner";
                                mailsData.MailTextRedner = Templates.GetMailTextAblehnenRedner(inv);
                            }
                            else
                            {
                                mailsData.InfoAnRednerTitel = "Info an Koordinator";
                                mailsData.MailTextRedner = Templates.GetMailTextAblehnenKoordinator(inv);
                            }

                            if (inv != null)
                            {
                                DataContainer.Absagen.Add(new Cancelation(zielKw, inv.Ältester, EventStatus.Zugesagt));
                                DataContainer.MeinPlanRemove(inv);
                            }

                            sendMail = true;
                            break;

                        case EventStatus.Ereignis:
                            DataContainer.MeinPlanRemove(ZielEvent);
                            break;
                    }
                    startBuchungInfo = "Die Buchung am neuen Datum wurde gelöscht.";
                    headerText = "Diese Buchung wurde gelöscht";
                }
                ActivityAddItem.BuchungVerschiebenIntern(ZielEvent, mailsData.MailTextRedner, ZielDatum, "Eine andere Buchung wurde auf das bisherige Datum verschoben.", headerText);
            }
            else
            {
                startBuchungInfo = "Das neue Datum war in der Planung offen.";
            }

            if (sendMail)
            {
                mails.ShowDialog();
            }

            DataContainer.UpdateTalkDate(StartEvent?.Vortrag?.Vortrag);
            DataContainer.UpdateTalkDate(ZielEvent?.Vortrag?.Vortrag);

            ActivityAddItem.BuchungVerschiebenIntern(StartEvent, mailsData.MailTextKoordinator, startDatum, startBuchungInfo, "Buchung wurde verschoben"); // Event1
        }

        private void SaveExtern()
        {
            var startKw = StartBuchung.Kw;
            var startDatum = DateCalcuation.CalculateWeek(StartBuchung.Kw);
            var zielKw = DateCalcuation.CalculateWeek(ZielDatum);

            if  (StartBuchung.Versammlung == DataContainer.MeineVersammlung)
            {
                ThemedMessageBox.Show("Achtung","Bei diese Buchung handelt es sich um eine Buchung in deiner eigenen Versammlung! Bitte benutze zum Ändern deiner eigenen Buchungen den Kalender unter 'Mein Plan'.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            StartBuchung.Kw = zielKw;

            var mails = new InfoAnRednerUndKoordinatorWindow();
            var mailsData = (InfoAnRednerUndKoordinatorViewModel)mails.DataContext;
            mailsData.DisableCancelButton();

            //MAIL WEGEN STARTBUCHUNG
            if (StartBuchung != null)
            {
                mailsData.InfoAnKoordinatorTitel = "Info an Redner & Koordinator";
                mailsData.MailTextKoordinator = Templates.GetMailTextEreignisTauschenAnRednerUndKoordinator(StartBuchung.Versammlung,StartBuchung.Ältester, startDatum,
                    ZielDatum, StartBuchung.Vortrag.Vortrag.ToString(), StartBuchung.Versammlung.Name);
            }

            var startBuchungInfo = string.Empty;

            //MAIL & TODO WEGEN ZIELBUCHUNG
            if (ZielBuchungBelegt)
            {
                if (ZielbuchungTauschenChecked)
                {

                    ZielBuchung.Kw = startKw;
                    if (ZielBuchung != null)
                    {
                        if (ZielBuchung.Versammlung == DataContainer.MeineVersammlung)
                        {
                            ThemedMessageBox.Show("Achtung", "Bei diese Buchung handelt es sich um eine Buchung in deiner eigenen Versammlung! Bitte benutze zum Ändern deiner eigenen Buchungen den Kalender unter 'Mein Plan'.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }

                        mailsData.InfoAnRednerTitel = "Info an Redner & Koordinator";
                        mailsData.MailTextRedner = Templates.GetMailTextEreignisTauschenAnRednerUndKoordinator(ZielBuchung.Versammlung,ZielBuchung.Ältester,
                            ZielDatum, startDatum, ZielBuchung.Vortrag.Vortrag.ToString(), ZielBuchung.Versammlung.Name);
                    }

                    startBuchungInfo = "Die Buchung am neuen Datum wurde mit dem bisherigen Datum getauscht.";
                    const string headerText = "Diese Buchung wurde verschoben";
                    ActivityAddItem.BuchungVerschiebenExtern(ZielBuchung, mailsData.MailTextRedner, ZielDatum, "Eine andere Buchung wurde auf das bisherige Datum verschoben.", headerText);
                }
                else if (ZielbuchungLöschenChecked)
                {
                    if (ZielBuchung != null)
                    {
                        if (ZielBuchung.Versammlung == DataContainer.MeineVersammlung)
                        {
                            ThemedMessageBox.Show("Achtung", "Bei diese Buchung handelt es sich um eine Buchung in deiner eigenen Versammlung! Bitte benutze zum Ändern deiner eigenen Buchungen den Kalender unter 'Mein Plan'.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }

                        mailsData.InfoAnRednerTitel = "Info an Redner & Koordinator";
                        mailsData.MailTextRedner = Templates.GetMailTextAblehnenRednerUndKoordinator(ZielBuchung);

                        ActivityAddItem.Outside(ZielBuchung, mailsData.MailTextKoordinator, mailsData.MailTextRedner, false);
                        DataContainer.ExternerPlan.Remove(ZielBuchung);
                    }

                    startBuchungInfo = "Die Buchung am neuen Datum wurde gelöscht.";
                    
                }
            }
            else
            {
                startBuchungInfo = "Das neue Datum war in der Planung offen.";
            }

            mails.ShowDialog();

            ActivityAddItem.BuchungVerschiebenExtern(StartBuchung, mailsData.MailTextKoordinator, startDatum, startBuchungInfo, "Buchung wurde verschoben"); // Event1
        }

        public void Close(ICloseable window)
        {
            Speichern = false;
            window?.Close();
        }
    }

    public enum Kalenderart
    {
        Intern,
        Extern,
    }
}