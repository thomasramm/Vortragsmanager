using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für ZeitItem.xaml
    /// </summary>
    public partial class ZeitItem : INotifyPropertyChanged
    {
        private readonly Zusammenkunftszeit _zeit;
        private readonly ObservableCollection<ZeitItem> _parentList;
        private readonly Zusammenkunftszeiten _parentObject;
        private readonly bool _myConregation;

        public ZeitItem()
        {
            _zeit = new Zusammenkunftszeit(DateTime.Today.Year, Wochentag.Sonntag, "10:00 Uhr");
            Initialize();
        }

        public ZeitItem(Zusammenkunftszeit zeit, ObservableCollection<ZeitItem> parentList, Zusammenkunftszeiten parentObject, bool myConregation)
        {
            _parentList = parentList;
            _parentObject = parentObject;
            _zeit = zeit;
            _myConregation = myConregation;
            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();
            RaisePropertyChanged(nameof(Zeit));
            RaisePropertyChanged(nameof(Jahr));
            RaisePropertyChanged(nameof(Wochentag));
        }

        public int Jahr
        {
            get => _zeit.Jahr;
            set
            {
                _zeit.Jahr = value;
                RaisePropertyChanged(nameof(Jahr));
    }
        }

        public Wochentag Wochentag
        {
            get => _zeit.Tag;
            set
            {
                _zeit.Tag = value;
                RaisePropertyChanged(nameof(Wochentag));
                if (_myConregation)
                    DateCalcuation.Wochentag = Datamodels.DataContainer.MeineVersammlung.Zeit.Get(DateTime.Today.Year).Tag;
            }
        }

        public string Zeit
        {
            get => _zeit.Zeit;
            set
            {
                _zeit.Zeit = value;
                RaisePropertyChanged(nameof(Zeit));
            }
        }

        public Zusammenkunftszeit Zusammenkunfzszeit => _zeit;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DeleteButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_parentList.Count > 1)
            {
                _parentObject.Remove(_zeit);
                _parentList.Remove(this);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Der letzte Eintrag kann nicht gelöscht werden");
            }
        }
    }
}
