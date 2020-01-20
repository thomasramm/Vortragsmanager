using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void LadeStartDatum(Models.IEvent ereignis)
        {
            if (ereignis is null)
                return;

            StartEvent = ereignis;

            if (ereignis.Status == Models.EventStatus.Zugesagt)
            {
                var invitation = (ereignis as Models.Invitation);
                if (invitation is null) return;
                StartName = invitation.Ältester.Name;
                StartVortrag = invitation.Vortrag.ToString();
                StartVersammlung = invitation.Ältester.Versammlung.Name;
            }

            if (ereignis.Status == Models.EventStatus.Ereignis)
            {
                var special = (ereignis as Models.SpecialEvent);
                if (special is null) return;
                StartName = special.Name;
                StartVortrag = special.Vortrag?.Thema;
                StartVersammlung = special.Thema;
            }
            StartDatum = ereignis.Datum.ToShortDateString();
            ZielDatum = ereignis.Datum;
        }

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
                _zielDatum = Core.Helper.GetSunday(value);
                RaisePropertyChanged();
                LadeZielBuchung();
            }
        }

        private bool _zielBuchungBelegt = false;

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
            var woche = Core.DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == ZielDatum);

            ZielBuchungBelegt = (woche != null);

            if (woche != null)
            {
                if (woche.Status == Models.EventStatus.Zugesagt)
                {
                    var invitation = (woche as Models.Invitation);
                    ZielName = invitation.Ältester.Name;
                    ZielVortrag = invitation.Vortrag.ToString();
                    ZielVersammlung = invitation.Ältester.Versammlung.Name;
                }

                if (woche.Status == Models.EventStatus.Ereignis)
                {
                    var ereignis = (woche as Models.SpecialEvent);
                    ZielName = ereignis.Name;
                    ZielVortrag = ereignis.Vortrag?.Thema;
                    ZielVersammlung = ereignis.Thema;
                }
                ZielEvent = woche;
                return;
            }

            var anfrage = Core.DataContainer.OffeneAnfragen.FirstOrDefault(x => x.Datum == ZielDatum);
            if (anfrage == null)
                return;

            ZielVersammlung = anfrage.Versammlung.Name;
            ZielName = anfrage.RednerVortrag.Keys.First().Name + " ...";
            ZielVortrag = "OFFENE ANFRAGE";
            ZielEvent = anfrage;
        }

        private Models.IEvent ZielEvent { get; set; }

        private Models.IEvent StartEvent { get; set; }

        public Kalenderart KalenderTyp { get; set; }

        public bool Speichern { get; set; }

        private bool _zielbuchungLöschenChecked = false;

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
            var mails = new InfoAnRednerUndKoordinatorWindow();
            var mailsData = (InfoAnRednerUndKoordinatorViewModel)mails.DataContext;
            mailsData.DisableCancelButton();

            var startDatum = StartEvent.Datum;

            //MAIL & TODO WEGEN STARTBUCHUNG
            StartEvent.Datum = ZielDatum;

            if (StartEvent.Status == Models.EventStatus.Zugesagt)
            {
                var ev = (StartEvent as Models.Invitation);
                if (ev.Ältester.Versammlung == Core.DataContainer.MeineVersammlung)
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Redner";
                    mailsData.MailTextKoordinator = Core.Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, startDatum, ZielDatum, ev.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }
                else
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Koordinator";
                    mailsData.MailTextKoordinator = Core.Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung, startDatum, ZielDatum, ev.Ältester.Name, ev.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }
            }

            //MAIL & TODO WEGEN ZIELBUCHUNG
            if (ZielBuchungBelegt)
            {
                if (ZielbuchungTauschenChecked)
                {
                    if (ZielEvent.Status == Models.EventStatus.Anfrage)
                    {
                        var ev = (ZielEvent as Models.Inquiry);
                        ev.Wochen.Remove(ZielDatum);
                        ev.Wochen.Add(startDatum);
                    }
                    else if (ZielEvent.Status == Models.EventStatus.Ereignis)
                    {
                        ZielEvent.Datum = startDatum;
                    }
                    else
                    {
                        ZielEvent.Datum = startDatum;
                        var ev = (ZielEvent as Models.Invitation);
                        if (ev.Ältester.Versammlung == Core.DataContainer.MeineVersammlung)
                        {
                            mailsData.InfoAnRednerTitel = "Info an Redner";
                            mailsData.MailTextRedner = Core.Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, ZielDatum, startDatum, ev.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                        }
                        else
                        {
                            mailsData.InfoAnRednerTitel = "Info an Koordinator";
                            mailsData.MailTextRedner = Core.Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung, ZielDatum, startDatum, ev.Ältester.Name, ev.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                        }
                    }
                }
                else if (ZielbuchungLöschenChecked)
                {
                    switch (ZielEvent.Status)
                    {
                        case Models.EventStatus.Anfrage:
                            var ev = (ZielEvent as Models.Inquiry);
                            ev.Wochen.Remove(ZielDatum);
                            if (ev.Wochen.Count == 0)
                                Core.DataContainer.OffeneAnfragen.Remove(ev);
                            //ToDo: Info an Versammlung über doppelbuchung der Anfrage??
                            break;

                        case Models.EventStatus.Zugesagt:
                            var inv = (ZielEvent as Models.Invitation);
                            if (inv.Ältester.Versammlung == Core.DataContainer.MeineVersammlung)
                            {
                                mailsData.InfoAnRednerTitel = "Info an Redner";
                                mailsData.MailTextRedner = Core.Templates.GetMailTextAblehnenRedner(inv);
                            }
                            else
                            {
                                mailsData.InfoAnRednerTitel = "Info an Koordinator";
                                mailsData.MailTextRedner = Core.Templates.GetMailTextAblehnenKoordinator(inv);
                            }

                            Core.DataContainer.MeinPlan.Remove(inv);
                            break;

                        case Models.EventStatus.Ereignis:
                            Core.DataContainer.MeinPlan.Remove(ZielEvent);
                            break;

                        case Models.EventStatus.Abgesagt:
                            break;

                        default:
                            break;
                    }
                }
            }
            mails.ShowDialog();

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