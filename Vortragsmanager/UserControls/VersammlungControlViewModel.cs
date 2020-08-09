using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.MeineVerwaltung;

namespace Vortragsmanager.Views
{
    public class ConregationViewModel : ViewModelBase
    {
        public ConregationViewModel(Conregation versammlung)
        {
            Versammlung = versammlung;
            RednerListe = new SpeakersViewModelCollection(versammlung);
            DeleteCommand = new DelegateCommand<object>(Delete);
            NewPersonCommand = new DelegateCommand(NewPerson);
            CalculateDistanceCommand = new DelegateCommand(CalculateDistance);
        }

        public DelegateCommand<object> DeleteCommand { get; private set; }

        public DelegateCommand NewPersonCommand { get; private set; }

        public DelegateCommand CalculateDistanceCommand { get; private set; }

        private bool _deleted = false;

        public void Delete(object lca)
        {
            var vld = new VersammlungLöschenDialog();
            var data = (VersammlungLöschenDialogView)vld.DataContext;
            data.Versammlung = Versammlung;

            vld.ShowDialog();

            if (!data.Abbrechen)
            {
                Sichtbarkeit = Visibility.Collapsed;
                _deleted = true;
                RaisePropertyChanged(nameof(Sichtbarkeit));
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

        public int Jahr1 { get; } = DateTime.Today.Year;

        public int Jahr2 => Jahr1 + 1;

        public int Jahr3 => Jahr1 + 2;

        public string ZusammenkunftszeitJahr1
        {
            get
            {
                return Versammlung.GetZusammenkunftszeit(Jahr1);
            }
            set
            {
                if (value != ZusammenkunftszeitJahr1)
                    Versammlung.SetZusammenkunftszeit(Jahr1, value);
                RaisePropertyChanged(ZusammenkunftszeitJahr1);
            }
        }

        public string ZusammenkunftszeitJahr2
        {
            get
            {
                return Versammlung.GetZusammenkunftszeit(Jahr2);
            }
            set
            {
                if ((value != ZusammenkunftszeitJahr1) && (value != Versammlung.GetZusammenkunftszeit(Jahr2)))
                    Versammlung.SetZusammenkunftszeit(Jahr2, value);
                RaisePropertyChanged(ZusammenkunftszeitJahr2);
            }
        }

        public string ZusammenkunftszeitJahr3
        {
            get
            {
                return Versammlung.GetZusammenkunftszeit(Jahr3);
            }
            set
            {
                if ((value != ZusammenkunftszeitJahr2) && (value != Versammlung.GetZusammenkunftszeit(Jahr3)))
                    Versammlung.SetZusammenkunftszeit(Jahr3, value);
                if (value != ZusammenkunftszeitJahr3 && value == ZusammenkunftszeitJahr2)
                    Versammlung.Zusammenkunftszeiten.Remove(Jahr3);
                RaisePropertyChanged(ZusammenkunftszeitJahr3);
            }
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