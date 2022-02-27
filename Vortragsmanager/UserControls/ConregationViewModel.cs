using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Datamodels;
using Vortragsmanager.UserControls;
using System.Linq;
using Vortragsmanager.DataModels;
using Vortragsmanager.Interface;
using Vortragsmanager.Module;
using Vortragsmanager.PageModels;
using Vortragsmanager.Windows;

namespace Vortragsmanager.Views
{
    public class ConregationViewModel : ViewModelBase
    {
        private readonly INavigation _parentModel;
        private readonly bool _myConregation;
        private readonly ConregationsViewModelCollection _all;

        public ConregationViewModel(INavigation parentModel, Conregation versammlung, ConregationsViewModelCollection all)
        {
            _parentModel = parentModel;
            _all = all;
            Versammlung = versammlung;
            _myConregation = versammlung == DataContainer.MeineVersammlung;
            RednerListe = new SpeakersViewModelCollection(parentModel, versammlung);
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

        public DelegateCommand<object> DeleteCommand { get; }

        public DelegateCommand NewPersonCommand { get; }

        public DelegateCommand CalculateDistanceCommand { get; }

        public DelegateCommand AddZeitCommand { get; }

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
            var rednerModel = new SpeakerViewModel(_parentModel, redner);
            RednerListe.Add(rednerModel);
            rednerModel.NavigateToEditor();
        }

        public void CalculateDistance()
        {
            var start = DataContainer.MeineVersammlung;
            var end = Versammlung;
            Entfernung = GeoApi.GetDistance(start, end);
        }

        public int? Entfernung
        {
            get => Versammlung.Entfernung;
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
        public ObservableCollection<ZeitItem> ZusammenkunftszeitenItems => _zusammenkunftszeitenItems;

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
            get => (DataContainer.MeineVersammlung == Versammlung);
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
                if (EigeneVersammlung)
                {
                    return Helper.Helper.StyleIsDark ? Brushes.Orange : Brushes.RoyalBlue;
                }
                return Helper.Helper.StyleIsDark ? Brushes.White : Brushes.Black;
            }
        }

        public Conregation Versammlung { get; }

        public string VersammlungName
        {
            get => Versammlung.Name;
            set
            {
                if (Versammlung.Name != "Unbekannt" && value != "Unbekannt")
                {
                    Versammlung.Name = value;
                }
            }
        }

        public SpeakersViewModelCollection RednerListe { get; }

        public DevExpress.Xpf.LayoutControl.GroupBoxState IsSelected { get; set; }

        public void Select(bool isSelected)
        {
            IsSelected = isSelected ? DevExpress.Xpf.LayoutControl.GroupBoxState.Maximized : DevExpress.Xpf.LayoutControl.GroupBoxState.Normal;
            RaisePropertyChanged(nameof(IsSelected));
            RefreshVisibility();
        }

        private bool _matchFilter = true;

        public bool MatchFilter
        {
            get => _matchFilter;
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
            get => _sichtbarkeit;
            set
            {
                _sichtbarkeit = value;
                RaisePropertyChanged();
            }
        }

        private bool _editMode;

        public bool EditMode
        {
            get => _editMode;
            set
            {
                _editMode = value;
                RaisePropertyChanged();
                RefreshVisibility();
            }
        }
    }
}