using System;
using System.Globalization;
using DevExpress.Mvvm;

namespace Vortragsmanager.PageModels
{
    /// <summary>
    /// Freie Termine meiner Planung
    /// </summary>
    public class Termin : ViewModelBase
    {
        private bool _isChecked = true;

        public Termin(DateTime datum, int kw)
        {
            Datum = datum;
            Kalenderwoche = kw;
        }

        public DateTime Datum { get; set; }

        public int Kalenderwoche { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFirstDateOfMonth { get; set; }

        public string Titel => Datum.ToString("dd.MM.yyyy", new CultureInfo("de-DE"));

        public bool Aktiv
        {
            get => IsChecked;
            set
            {
                IsChecked = value;
                RaisePropertyChanged();
                RaisePropertiesChanged("Aktiv", "IsChecked");
            }
        }
    }
}