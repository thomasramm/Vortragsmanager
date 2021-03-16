using DevExpress.Mvvm;
using System;
using System.Linq;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Views
{
    public class KalendereintragVerschiebenView : ViewModelBase
    {
        public KalendereintragVerschiebenView()
        {
            SaveCommand = new DelegateCommand<ICloseable>(Save);

            CloseCommand = new DelegateCommand<ICloseable>(Close);
        }

        public DelegateCommand<ICloseable> SaveCommand { get; private set; }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public void LadeStartDatum(IEvent ereignis)
        {
            if (ereignis is null)
                return;

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

            ZielDatum = Helper.CalculateWeek(ereignis.Kw);
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
            get
            {
                return _zielVersammlung;
            }
            set
            {
                _zielVersammlung = value;
                RaisePropertyChanged();
            }
        }

        private string _zielName;

        public string ZielName
        {
            get
            {
                return _zielName;
            }
            set
            {
                _zielName = value;
                RaisePropertyChanged();
            }
        }

        private string _zielVortrag;

        public string ZielVortrag
        {
            get
            {
                return _zielVortrag;
            }
            set
            {
                _zielVortrag = value;
                RaisePropertyChanged();
            }
        }

        private string _zielTyp = "Versammlung";

        public string ZielTyp
        {
            get
            {
                return _zielTyp;
            }
            set
            {
                _zielTyp = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _zielDatum;

        public DateTime ZielDatum
        {
            get
            {
                return _zielDatum;
            }
            set
            {
                if (value == null)
                    return;
                _zielDatum = Helper.GetConregationDay(value);
                RaisePropertyChanged();
                LadeZielBuchung();
            }
        }

        private bool _zielBuchungBelegt;

        public bool ZielBuchungBelegt
        {
            get
            {
                return _zielBuchungBelegt;
            }
            set
            {
                if (_zielBuchungBelegt != value)
                {
                    if (value)
                        WindowHeight += 200;
                    else
                        WindowHeight -= 200;
                }
                _zielBuchungBelegt = value;
                RaisePropertyChanged();
            }
        }

        private int _windowHeight = 320;

        public int WindowHeight
        {
            get
            {
                return _windowHeight;
            }
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

        private static void LadeExterneZielBuchung()
        {
        }

        private void LadeInterneZielBuchung()
        {
            var zielKw = Helper.CalculateWeek(ZielDatum);
            var woche = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == zielKw);

            ZielBuchungBelegt = (woche != null);

            if (ZielBuchungBelegt)
            {
                if (woche.Status == EventStatus.Zugesagt)
                {
                    var invitation = (woche as Invitation);
                    ZielTyp = "Versammlung";
                    ZielVersammlung = invitation.Ältester.Versammlung.Name;
                    ZielName = invitation.Ältester.Name;
                    ZielVortrag = invitation.Vortrag.Vortrag.ToString();
                }

                if (woche.Status == EventStatus.Ereignis)
                {
                    var ereignis = (woche as SpecialEvent);
                    ZielTyp = "Ereignis";
                    ZielVersammlung = ereignis.Anzeigetext;
                    ZielName = ereignis.Vortragender;
                    if (string.IsNullOrEmpty(ZielName))
                        ZielName = ZielVersammlung == ereignis.Name ? "" : ereignis.Name;
                    if (string.IsNullOrWhiteSpace(ereignis.Vortrag?.Vortrag?.NumberTopicShort))
                        ZielVortrag = ereignis.Thema;
                    else
                        ZielVortrag = ereignis.Vortrag.Vortrag.ToString();
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

        public Kalenderart KalenderTyp { get; set; }

        public bool Speichern { get; set; }

        private bool _zielbuchungLöschenChecked;

        public bool ZielbuchungLöschenChecked
        {
            get
            {
                return _zielbuchungLöschenChecked;
            }
            set
            {
                _zielbuchungLöschenChecked = value;
                RaisePropertyChanged();
            }
        }

        public bool ZielbuchungTauschenChecked
        {
            get
            {
                return !_zielbuchungLöschenChecked;
            }
            set
            {
                _zielbuchungLöschenChecked = !value;
                RaisePropertyChanged();
            }
        }

        public void Save(ICloseable window)
        {
            var startKw = StartEvent.Kw;
            var startDatum = Core.Helper.CalculateWeek(StartEvent.Kw);
            var zielKw = Helper.CalculateWeek(ZielDatum);

            StartEvent.Kw = zielKw;
            var sendMail = false;

            var mails = new InfoAnRednerUndKoordinatorWindow();
            var mailsData = (InfoAnRednerUndKoordinatorViewModel)mails.DataContext;
            mailsData.DisableCancelButton();

            //MAIL WEGEN STARTBUCHUNG
            if (StartEvent.Status == EventStatus.Zugesagt)
            {
                var ev = (StartEvent as Invitation);
                if (ev.Ältester.Versammlung == DataContainer.MeineVersammlung)
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Redner";
                    mailsData.MailTextKoordinator = Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, startDatum, ZielDatum, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }
                else
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Koordinator";
                    mailsData.MailTextKoordinator = Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung, startDatum, ZielDatum, ev.Ältester.Name, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }

                sendMail = true;
            }
            string startBuchungInfo = string.Empty;
            //MAIL & TODO WEGEN ZIELBUCHUNG
            if (ZielBuchungBelegt)
            {
                var headerText = string.Empty;
                if (ZielbuchungTauschenChecked)
                {
                    if (ZielEvent.Status == EventStatus.Anfrage)
                    {
                        var ev = (ZielEvent as Inquiry);
                        ev.Kws.Remove(zielKw);
                        ev.Kws.Add(startKw);
                    }
                    else if (ZielEvent.Status == EventStatus.Ereignis)
                    {
                        ZielEvent.Kw = startKw;
                    }
                    else
                    {
                        ZielEvent.Kw = startKw;
                        var ev = (ZielEvent as Invitation);
                        if (ev.Ältester.Versammlung == DataContainer.MeineVersammlung)
                        {
                            mailsData.InfoAnRednerTitel = "Info an Redner";
                            mailsData.MailTextRedner = Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, ZielDatum, startDatum, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                        }
                        else
                        {
                            mailsData.InfoAnRednerTitel = "Info an Koordinator";
                            mailsData.MailTextRedner = Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung, ZielDatum, startDatum, ev.Ältester.Name, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                        }
                        sendMail = true;
                    }
                    startBuchungInfo = "Die Buchung am neuen Datum wurde mit dem bisherigen Datum getauscht.";
                    headerText = "Diese Buchung wurde verschoben";
                }
                else if (ZielbuchungLöschenChecked)
                {
                    switch (ZielEvent.Status)
                    {
                        case EventStatus.Anfrage:
                            var ev = (ZielEvent as Inquiry);
                            ev.Kws.Remove(zielKw);
                            if (ev.Kws.Count == 0)
                                DataContainer.OffeneAnfragen.Remove(ev);
                            //ToDo: Info an Versammlung über doppelbuchung der Anfrage??
                            break;

                        case EventStatus.Zugesagt:
                            var inv = (ZielEvent as Invitation);
                            if (inv.Ältester.Versammlung == DataContainer.MeineVersammlung)
                            {
                                mailsData.InfoAnRednerTitel = "Info an Redner";
                                mailsData.MailTextRedner = Templates.GetMailTextAblehnenRedner(inv);
                            }
                            else
                            {
                                mailsData.InfoAnRednerTitel = "Info an Koordinator";
                                mailsData.MailTextRedner = Templates.GetMailTextAblehnenKoordinator(inv);
                            }
                            DataContainer.Absagen.Add(new Cancelation(zielKw, inv.Ältester, EventStatus.Zugesagt));
                            DataContainer.MeinPlanRemove(inv);
                            sendMail = true;
                            break;

                        case EventStatus.Ereignis:
                            DataContainer.MeinPlanRemove(ZielEvent);
                            break;

                        default:
                            break;
                    }
                    startBuchungInfo = "Die Buchung am neuen Datum wurde gelöscht.";
                    headerText = "Diese Buchung wurde gelöscht";
                }
                ActivityLog.AddActivity.BuchungVerschieben(ZielEvent, mailsData.MailTextRedner, ZielDatum, "Eine andere Buchung wurde auf das bisherige Datum verschoben.", headerText);
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

            ActivityLog.AddActivity.BuchungVerschieben(StartEvent, mailsData.MailTextKoordinator, startDatum, startBuchungInfo, "Buchung wurde verschoben"); // Event1
            //Event2
            Speichern = true;
            window?.Close();
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