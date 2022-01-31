using System;
using System.ComponentModel;
using System.Windows.Controls;
using Vortragsmanager.Core;
using System.Collections.ObjectModel;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für ZeitItem.xaml
    /// </summary>
    public partial class ZeitItem : UserControl, INotifyPropertyChanged
    {
        private Core.DataHelper.Zusammenkunftszeit _zeit;
        private ObservableCollection<ZeitItem> _parentList;
        private Core.DataHelper.Zusammenkunftszeiten _parentObject;
        private bool _myConregation;

        public ZeitItem()
        {
            _zeit = new Core.DataHelper.Zusammenkunftszeit(DateTime.Today.Year, DayOfWeeks.Sonntag, "10:00 Uhr");
            Initialize();
        }

        public ZeitItem(Core.DataHelper.Zusammenkunftszeit zeit, ObservableCollection<ZeitItem> parentList, Core.DataHelper.Zusammenkunftszeiten parentObject, bool myConregation)
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

        public DayOfWeeks Wochentag
        {
            get
            {
                return _zeit.Tag;
            }
            set
            {
                _zeit.Tag = value;
                RaisePropertyChanged(nameof(Wochentag));
                if (_myConregation)
                    Helper.Wochentag = Datamodels.DataContainer.MeineVersammlung.Zeit.Get(DateTime.Today.Year).Tag;
            }
        }

        public string Zeit
        {
            get
            {
                return _zeit.Zeit;
            }
            set
            {
                _zeit.Zeit = value;
                RaisePropertyChanged(nameof(Zeit));
            }
        }

        public Core.DataHelper.Zusammenkunftszeit Zusammenkunfzszeit
        {
            get
            {
                return _zeit;
            }
        }

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
