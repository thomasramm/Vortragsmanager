using System;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;

namespace Vortragsmanager.UserControls
{
    public class ActivityItemViewModel : ViewModelBase
    {
        private DateTime _datum;
        private int _id;
        private Conregation _versammlung;
        private Speaker _redner;
        private Talk _vortrag;
        private int _calendarDate = -1;
        private ActivityTypes _typ;
        private string _objekt;
        private string _kommentar;
        private string _mails;

        public ActivityItemViewModel()
        {
            Datum = DateTime.Now;
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        public DateTime Datum
        {
            get => _datum;
            set
            {
                _datum = value;
                RaisePropertyChanged();
            }
        }

        public Conregation Versammlung
        {
            get => _versammlung;
            set
            {
                _versammlung = value;
                RaisePropertyChanged();
            }
        }

        public Speaker Redner
        {
            get => _redner;
            set
            {
                _redner = value;
                RaisePropertyChanged();
            }
        }

        public Talk Vortrag
        {
            get => _vortrag;
            set
            {
                _vortrag = value;
                RaisePropertyChanged();
            }
        }

        public int KalenderKw
        {
            get => _calendarDate;
            set
            {
                _calendarDate = value;
                RaisePropertyChanged();
            }
        }

        public ActivityTypes Typ
        {
            get => _typ;
            set
            {
                _typ = value;
                RaisePropertyChanged();
            }
        }

        public string Objekt
        {
            get => _objekt;
            set
            {
                _objekt = value;
                RaisePropertyChanged();
            }
        }

        public string Kommentar
        {
            get => _kommentar;
            set
            {
                _kommentar = value;
                RaisePropertyChanged();
            }
        }

        public string Mails
        {
            get => _mails;
            set
            {
                _mails = value;
                RaisePropertyChanged();
            }
        }
    }
}