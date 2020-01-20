﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Models;

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

        public void Delete(object lc)
        {
            if (ThemedMessageBox.Show("Versammlung löschen",
                $"Soll die Versammlung {Versammlung.Name} mit allen {RednerListe.Count} Rednern gelöscht werden?" + Environment.NewLine +
                "Alle vergangenen Einladungen werden durch 'unbekannt' ersetzt," + Environment.NewLine +
                "alle zukünftigen Einladungen und Anfragen werden gelöscht!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            while (RednerListe.Count > 0)
            {
                RednerListe[0].RednerLöschen(true);
                RednerListe.RemoveAt(0);
            }
            //Einladungen
            var einladungen = Core.DataContainer.MeinPlan
                .Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.AnfrageVersammlung == Versammlung)
                .ToList();
            foreach (var einladung in einladungen)
                Core.DataContainer.MeinPlan.Remove(einladung);

            //Anfragen
            var anfragen = Core.DataContainer.OffeneAnfragen.Where(x => x.Versammlung == Versammlung).ToList();
            foreach (var anfrage in anfragen)
                Core.DataContainer.OffeneAnfragen.Remove(anfrage);

            //Externe Vorträge in dieser Versammlung
            var externeE = Core.DataContainer.ExternerPlan.Where(x => x.Versammlung == Versammlung);
            foreach (var outside in externeE)
            {
                outside.Versammlung = null;
            }

            Core.DataContainer.Versammlungen.Remove(Versammlung);
            Sichtbarkeit = Visibility.Collapsed;

            //var boxList = ((FlowLayoutControl)lc).Children;
            //for (int i = 0; i < boxList.Count; i++)
            //{
            //    var gBox = (boxList[i] as GroupBox);
            //    if (gBox is null)
            //        continue;
            //    var data = (ConregationViewModel)gBox.DataContext;
            //    data.EditMode = false;
            //    data.Select(false);
            //    if (data == this)
            //    {
            //        boxList.RemoveAt(i);
            //        i--;
            //    }
            //}

            RaisePropertyChanged(nameof(Sichtbarkeit));
        }

        public void NewPerson()
        {
            var redner = Core.DataContainer.FindOrAddSpeaker("Neuer Redner", Versammlung);
            var rednerModel = new SpeakerViewModel(redner);
            RednerListe.Add(rednerModel);
            rednerModel.Select();
        }

        public void CalculateDistance()
        {
            var start = Core.DataContainer.MeineVersammlung;
            var end = Versammlung;
            Entfernung = Core.GeoApi.GetDistance(start, end);
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
                return (Core.DataContainer.MeineVersammlung == Versammlung);
            }
            set
            {
                if ((value == true) && (ThemedMessageBox.Show(
                    Properties.Resources.Achtung,
                    "Willst du diese Versammlung wirklich als deine eigene Versammlung setzen?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes))
                    Core.DataContainer.MeineVersammlung = Versammlung;
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