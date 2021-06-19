using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.MeineVerwaltung;
using Vortragsmanager.UserControls;
using System.Linq;
using Vortragsmanager.Core.DataHelper;

namespace Vortragsmanager.Views
{
    public class ConregationViewModel : ViewModelBase
    {
        private bool _myConregation;
        ConregationsViewModelCollection _all;

        public ConregationViewModel(Conregation versammlung, ConregationsViewModelCollection all)
        {
            _all = all;
            Versammlung = versammlung;
            _myConregation = versammlung == DataContainer.MeineVersammlung;
            RednerListe = new SpeakersViewModelCollection(versammlung);
            DeleteCommand = new DelegateCommand<object>(Delete);
            NewPersonCommand = new DelegateCommand(NewPerson);
            CalculateDistanceCommand = new DelegateCommand(CalculateDistance);
            AddZeitCommand = new DelegateCommand(AddZusammenkunftszeit);

            _zusammenkunftszeitenItems = new ObservableCollection<ZeitItem>();
            var zeitItems = new List<Zusammenkunftszeit>(1)
            {
                Versammlung.Zeit.Get(DateTime.Today.Year)
            };
            zeitItems.AddRange(Versammlung.Zeit.Items.Where(x => x.Jahr >= DateTime.Today.Year && x != zeitItems[0]));
            if (zeitItems.Count == 0)
                zeitItems.Add(Versammlung.Zeit.GetLastItem());

            foreach (var eintrag in zeitItems)
            {
                var uiItem = new ZeitItem(eintrag, _zusammenkunftszeitenItems, Versammlung.Zeit, _myConregation);
                _zusammenkunftszeitenItems.Add(uiItem);
            }
        }

        public DelegateCommand<object> DeleteCommand { get; private set; }

        public DelegateCommand NewPersonCommand { get; private set; }

        public DelegateCommand CalculateDistanceCommand { get; private set; }

        public DelegateCommand AddZeitCommand { get; private set; }

        private bool _deleted;

        public void Delete(object lca)
        {
            var vld = new VersammlungLöschenDialog();
            var data = (VersammlungLöschenDialogView)vld.DataContext;
            data.Versammlung = Versammlung;

            vld.ShowDialog();

            if (!data.Abbrechen)
            {
                foreach(var x in _all)
                {
                    x.EditMode = false;
                }

                _deleted = true;
                RefreshVisibility();
                //Sichtbarkeit = Visibility.Collapsed;
                //RaisePropertyChanged(nameof(Sichtbarkeit));
                
            }
        }

        public void NewPerson()
        {
            var redner = DataContainer.SpeakerFindOrAdd("Neuer Redner", Versammlung);
            var rednerModel = new SpeakerViewModel(redner);
            RednerListe.Add(rednerModel);
            rednerModel.Select();
        }

        public void CalculateDistance()
        {
            var start = DataContainer.MeineVersammlung;
            var end = Versammlung;
            Entfernung = GeoApi.GetDistance(start, end);
        }

        public int? Entfernung
        {
            get
            {
                return Versammlung.Entfernung;
            }
            set
            {
                if (value != null)
                {
                    Versammlung.Entfernung = (int)value;
                }
                RaisePropertyChanged();
            }
        }

        private readonly ObservableCollection<ZeitItem> _zusammenkunftszeitenItems;
        public ObservableCollection<ZeitItem> ZusammenkunftszeitenItems
        {
            get
            {
                return _zusammenkunftszeitenItems;
            }
        }

        public void AddZusammenkunftszeit()
        {
            var lastItem = Versammlung.Zeit.GetLastItem();
            var nextYear = Math.Max(lastItem.Jahr+1, DateTime.Today.Year);
            
            // In beide Listen eintragen.
            var newItem = Versammlung.Zeit.Add(nextYear, lastItem.Tag, lastItem.Zeit);
            _zusammenkunftszeitenItems.Add(new ZeitItem(newItem, _zusammenkunftszeitenItems, Versammlung.Zeit, _myConregation));
        }

        public bool EigeneVersammlung
        {
            get
            {
                return (DataContainer.MeineVersammlung == Versammlung);
            }
            set
            {
                if ((value == true) && (ThemedMessageBox.Show(
                    Properties.Resources.Achtung,
                    "Willst du diese Versammlung wirklich als deine eigene Versammlung setzen?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes))
                    DataContainer.MeineVersammlung = Versammlung;
            }
        }

        public Brush EigeneVersammlungTextInOrange
        {
            get
            {
                return (EigeneVersammlung) ? Brushes.Orange : Brushes.White;
            }
        }

        public Conregation Versammlung { get; private set; }

        public SpeakersViewModelCollection RednerListe { get; private set; }

        public DevExpress.Xpf.LayoutControl.GroupBoxState IsSelected { get; set; }

        public void Select(bool isSelected)
        {
            if (isSelected)
            {
                IsSelected = DevExpress.Xpf.LayoutControl.GroupBoxState.Maximized;
            }
            else
            {
                IsSelected = DevExpress.Xpf.LayoutControl.GroupBoxState.Normal;
            }
            RaisePropertyChanged(nameof(IsSelected));
            RefreshVisibility();
        }

        private bool _matchFilter = true;

        public bool MatchFilter
        {
            get
            {
                return _matchFilter;
            }
            set
            {
                _matchFilter = value;
                RaisePropertyChanged();
                RefreshVisibility();
            }
        }

        private void RefreshVisibility()
        {
            if (_deleted)
                Sichtbarkeit = Visibility.Collapsed;
            else if (IsSelected == DevExpress.Xpf.LayoutControl.GroupBoxState.Maximized)
                Sichtbarkeit = Visibility.Visible;
            else if (EditMode)
                Sichtbarkeit = Visibility.Collapsed;
            else if (MatchFilter)
                Sichtbarkeit = Visibility.Visible;
            else
                Sichtbarkeit = Visibility.Collapsed;
        }

        private Visibility _sichtbarkeit = Visibility.Visible;

        public Visibility Sichtbarkeit
        {
            get
            {
                return _sichtbarkeit;
            }
            set
            {
                _sichtbarkeit = value;
                RaisePropertyChanged();
            }
        }

        private bool _editMode;

        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                RaisePropertyChanged();
                RefreshVisibility();
            }
        }
    }
}