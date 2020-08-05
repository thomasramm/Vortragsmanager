﻿using DevExpress.Mvvm;
using System;
using System.Linq;
using Vortragsmanager.Core;

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

        public void LadeStartDatum(Datamodels.IEvent ereignis)
        {
            if (ereignis is null)
                return;

            StartEvent = ereignis;

            if (ereignis.Status == Datamodels.EventStatus.Zugesagt)
            {
                var invitation = (ereignis as Datamodels.Invitation);
                if (invitation is null) return;
                StartTyp = "Versammlung";
                StartName = invitation.Ältester.Name;
                StartVortrag = invitation.Vortrag.Vortrag.ToString();
                StartVersammlung = invitation.Ältester.Versammlung.Name;
            }

            if (ereignis.Status == Datamodels.EventStatus.Ereignis)
            {
                var special = (ereignis as Datamodels.SpecialEvent);
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

            StartDatum = ereignis.Datum.ToShortDateString();
            ZielDatum = ereignis.Datum;
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
                _zielDatum = Helper.GetSunday(value);
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
            var woche = Datamodels.DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == ZielDatum);

            ZielBuchungBelegt = (woche != null);

            if (ZielBuchungBelegt)
            {
                if (woche.Status == Datamodels.EventStatus.Zugesagt)
                {
                    var invitation = (woche as Datamodels.Invitation);
                    ZielTyp = "Versammlung";
                    ZielVersammlung = invitation.Ältester.Versammlung.Name;
                    ZielName = invitation.Ältester.Name;
                    ZielVortrag = invitation.Vortrag.Vortrag.ToString();
                }

                if (woche.Status == Datamodels.EventStatus.Ereignis)
                {
                    var ereignis = (woche as Datamodels.SpecialEvent);
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

            var anfrage = Datamodels.DataContainer.OffeneAnfragen.FirstOrDefault(x => x.Datum == ZielDatum);
            if (anfrage == null)
                return;

            ZielVersammlung = anfrage.Versammlung.Name;
            ZielName = anfrage.RednerVortrag.Keys.First().Name + " ...";
            ZielVortrag = "OFFENE ANFRAGE";
            ZielEvent = anfrage;
            ZielBuchungBelegt = true;
        }

        private Datamodels.IEvent ZielEvent { get; set; }

        private Datamodels.IEvent StartEvent { get; set; }

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
            var startDatum = StartEvent.Datum;
            StartEvent.Datum = ZielDatum;
            var sendMail = false;

            var mails = new InfoAnRednerUndKoordinatorWindow();
            var mailsData = (InfoAnRednerUndKoordinatorViewModel)mails.DataContext;
            mailsData.DisableCancelButton();

            //MAIL WEGEN STARTBUCHUNG
            if (StartEvent.Status == Datamodels.EventStatus.Zugesagt)
            {
                var ev = (StartEvent as Datamodels.Invitation);
                if (ev.Ältester.Versammlung == Datamodels.DataContainer.MeineVersammlung)
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Redner";
                    mailsData.MailTextKoordinator = Datamodels.Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, startDatum, ZielDatum, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }
                else
                {
                    mailsData.InfoAnKoordinatorTitel = "Info an Koordinator";
                    mailsData.MailTextKoordinator = Datamodels.Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung, startDatum, ZielDatum, ev.Ältester.Name, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                }

                sendMail = true;
            }

            //MAIL & TODO WEGEN ZIELBUCHUNG
            if (ZielBuchungBelegt)
            {
                if (ZielbuchungTauschenChecked)
                {
                    if (ZielEvent.Status == Datamodels.EventStatus.Anfrage)
                    {
                        var ev = (ZielEvent as Datamodels.Inquiry);
                        ev.Wochen.Remove(ZielDatum);
                        ev.Wochen.Add(startDatum);
                    }
                    else if (ZielEvent.Status == Datamodels.EventStatus.Ereignis)
                    {
                        ZielEvent.Datum = startDatum;
                    }
                    else
                    {
                        ZielEvent.Datum = startDatum;
                        var ev = (ZielEvent as Datamodels.Invitation);
                        if (ev.Ältester.Versammlung == Datamodels.DataContainer.MeineVersammlung)
                        {
                            mailsData.InfoAnRednerTitel = "Info an Redner";
                            mailsData.MailTextRedner = Datamodels.Templates.GetMailTextEreignisTauschenAnRedner(ev.Ältester, ZielDatum, startDatum, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                        }
                        else
                        {
                            mailsData.InfoAnRednerTitel = "Info an Koordinator";
                            mailsData.MailTextRedner = Datamodels.Templates.GetMailTextEreignisTauschenAnKoordinator(ev.Ältester.Versammlung, ZielDatum, startDatum, ev.Ältester.Name, ev.Vortrag.Vortrag.ToString(), ev.Ältester.Versammlung.Name);
                        }
                        sendMail = true;
                    }
                }
                else if (ZielbuchungLöschenChecked)
                {
                    switch (ZielEvent.Status)
                    {
                        case Datamodels.EventStatus.Anfrage:
                            var ev = (ZielEvent as Datamodels.Inquiry);
                            ev.Wochen.Remove(ZielDatum);
                            if (ev.Wochen.Count == 0)
                                Datamodels.DataContainer.OffeneAnfragen.Remove(ev);
                            //ToDo: Info an Versammlung über doppelbuchung der Anfrage??
                            break;

                        case Datamodels.EventStatus.Zugesagt:
                            var inv = (ZielEvent as Datamodels.Invitation);
                            if (inv.Ältester.Versammlung == Datamodels.DataContainer.MeineVersammlung)
                            {
                                mailsData.InfoAnRednerTitel = "Info an Redner";
                                mailsData.MailTextRedner = Datamodels.Templates.GetMailTextAblehnenRedner(inv);
                            }
                            else
                            {
                                mailsData.InfoAnRednerTitel = "Info an Koordinator";
                                mailsData.MailTextRedner = Datamodels.Templates.GetMailTextAblehnenKoordinator(inv);
                            }
                            Datamodels.DataContainer.Absagen.Add(new Datamodels.Cancelation(ZielDatum, inv.Ältester, Datamodels.EventStatus.Zugesagt));
                            Datamodels.DataContainer.MeinPlan.Remove(inv);
                            sendMail = true;
                            break;

                        case Datamodels.EventStatus.Ereignis:
                            Datamodels.DataContainer.MeinPlan.Remove(ZielEvent);
                            break;

                        default:
                            break;
                    }
                }
            }
            if (sendMail)
            {
                mails.ShowDialog();
            }

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