using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    internal class VersammlungLöschenDialogView : ViewModelBase
    {
        private Conregation _versammlung;

        public Conregation Versammlung
        {
            get => _versammlung;
            set
            {
                _versammlung = value;

                ListeAllerVersammlungen = new ObservableCollection<Conregation>(DataContainer.Versammlungen.Where(x => x != Versammlung).OrderBy(x => x, new Helper.EigeneKreisNameComparer()));
                ListeFilteredVersammlungen = new ObservableCollection<Conregation>(ListeAllerVersammlungen);

                RaisePropertyChanged(nameof(Merge1Koordinator));
                RaisePropertyChanged(nameof(Merge2Koordinator));
            }
        }

        public VersammlungLöschenDialogView()
        {
            Step1Height = new GridLength(1, GridUnitType.Star);
            Step2Height = new GridLength(0, GridUnitType.Pixel);

            ChoiseMergeCommand = new DelegateCommand(ChoiseMerge);
            KoordinatorSelectCommand = new DelegateCommand<int>(KoordinatorSelect);
            MergeCommand = new DelegateCommand<ICloseable>(Merge);
            CancelCommand = new DelegateCommand<ICloseable>(Cancel);
            DeleteCommand = new DelegateCommand<ICloseable>(Delete);
        }

        public DelegateCommand<int> KoordinatorSelectCommand { get; }

        public DelegateCommand ChoiseMergeCommand { get; }

        public DelegateCommand<ICloseable> MergeCommand { get; }

        public DelegateCommand<ICloseable> CancelCommand { get; }

        public DelegateCommand<ICloseable> DeleteCommand { get; }

        public ObservableCollection<Conregation> ListeAllerVersammlungen { get; private set; }

        public ObservableCollection<Conregation> ListeFilteredVersammlungen { get; private set; }

        public bool MergeEnabled { get; set; }

        public string MergeHint { get; set; }

        public bool Abbrechen { get; private set; } = true;

        private GridLength _step1Height;

        public GridLength Step1Height
        {
            get => _step1Height;
            set
            {
                _step1Height = value;
                RaisePropertyChanged();
            }
        }

        private GridLength _step2Height;

        public GridLength Step2Height
        {
            get => _step2Height;
            set
            {
                _step2Height = value;
                RaisePropertyChanged();
            }
        }

        private void ChoiseMerge()
        {
            Step1Height = new GridLength(0, GridUnitType.Pixel);
            Step2Height = new GridLength(1, GridUnitType.Star);
            CheckMergeStatus();
        }

        private void CheckMergeStatus()
        {
            if (_selectedConregation == null)
            {
                MergeEnabled = false;
                MergeHint = "Bitte wähle eine Zielversammlung aus.";
            }
            else if (_koordinatorSelect == 0)
            {
                MergeEnabled = false;
                MergeHint = "Bitte wähle den Koordinator der neuen Versammlung aus.";
            }
            else
            {
                var unb = DataContainer.ConregationGetUnknown();
                if (_selectedConregation == unb || Versammlung == unb)
                {
                    ThemedMessageBox.Show("Achtung", "Du kannst keine Versammlung mit 'Unbekannt' zusammenführen.", MessageBoxButton.OK, MessageBoxImage.Information);
                    MergeEnabled = false;
                    MergeHint = "'Unbekannt' ist in der Auswahl nicht erlaubt.";
                }
                else
                {
                    MergeEnabled = true;
                    MergeHint = "Beide Versammlungen zusammenführen.";
                }
            }
            RaisePropertyChanged(nameof(MergeEnabled));
            RaisePropertyChanged(nameof(MergeHint));
        }

        private string _selectedConregationName;

        private Conregation _selectedConregation;

        public string SelectedConregationName
        {
            get => _selectedConregationName;
            set
            {
                _selectedConregationName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    _selectedConregation = DataContainer.ConregationFind(value);
                }
                RaisePropertyChanged(nameof(Merge2Koordinator));
                CheckMergeStatus();
            }
        }

        public void SetVersammlungfilter(string versammlungFilter = null)
        {
            ListeFilteredVersammlungen.Clear();
            var items = ListeAllerVersammlungen.Count;
            var newCount = 0;
            var maxCount = (versammlungFilter == null) ? items : 10;
            for (int i = 0; i < items; i++)
            {
                if (string.IsNullOrEmpty(versammlungFilter) || (Regex.IsMatch(ListeAllerVersammlungen[i].Name, Regex.Escape(versammlungFilter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                {
                    ListeFilteredVersammlungen.Add(ListeAllerVersammlungen[i]);
                    newCount++;
                    if (newCount == maxCount)
                        break;
                }
            }
        }

        public string Merge1Koordinator => $"{Versammlung?.Koordinator} (Versammlung {Versammlung?.Name})";

        public string Merge2Koordinator => _selectedConregation == null
                    ? "Bitte zuerst Versammlung auswählen!"
                    : $"{_selectedConregation.Koordinator} (Versammlung {SelectedConregationName})";

        private int _koordinatorSelect;

        public void KoordinatorSelect(int select)
        {
            _koordinatorSelect = select;
            CheckMergeStatus();
        }

        private void Delete(ICloseable window)
        {
            if (ThemedMessageBox.Show("Versammlung löschen",
                $"Soll die Versammlung {Versammlung.Name} mit allen Rednern gelöscht werden?" + Environment.NewLine +
                "Alle vergangenen Einladungen werden durch 'unbekannt' ersetzt," + Environment.NewLine +
                "alle zukünftigen Einladungen und Anfragen werden gelöscht!",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            var gelöscht = DataContainer.ConregationRemove(Versammlung);

            CloseWindow(window, !gelöscht);
        }

        private void Merge(ICloseable window)
        {
            var redner = DataContainer.Redner.Where(x => x.Versammlung == Versammlung).OrderBy(x => x.Name).ToList();

            //Redner in die neue Versammlung verschieben
            foreach (var r in redner)
            {
                r.Versammlung = _selectedConregation;
            }

            //Fest geplante Einladungen (auch der Zukunft) auf neue Versammlung unbenennen
            var einladungen = DataContainer.MeinPlan
                .Where(x => x.Status == EventStatus.Zugesagt)
                .Cast<Invitation>()
                .Where(x => x.AnfrageVersammlung == Versammlung)
                .ToList();
            foreach (var einladung in einladungen)
            {
                einladung.AnfrageVersammlung = _selectedConregation;
            }

            //Anfragen
            var anfragen = DataContainer.OffeneAnfragen.Where(x => x.Versammlung == Versammlung).ToList();
            foreach (var anfrage in anfragen)
            {
                anfrage.Versammlung = _selectedConregation;
            }

            //Externe Vorträge in dieser Versammlung
            var externeE = DataContainer.ExternerPlan.Where(x => x.Versammlung == Versammlung);
            foreach (var outside in externeE)
            {
                outside.Versammlung = _selectedConregation;
            }

            //Koordinator der Zielversammlung
            if (_koordinatorSelect == 1)
            {
                _selectedConregation.Koordinator = Versammlung.Koordinator;
                _selectedConregation.KoordinatorJw = Versammlung.KoordinatorJw;
                _selectedConregation.KoordinatorMail = Versammlung.KoordinatorMail;
                _selectedConregation.KoordinatorMobil = Versammlung.KoordinatorMobil;
                _selectedConregation.KoordinatorTelefon = Versammlung.KoordinatorTelefon;
            }

            DataContainer.ConregationRemove(Versammlung);

            CloseWindow(window, false);
        }

        private void Cancel(ICloseable window)
        {
            CloseWindow(window, true);
        }

        private void CloseWindow(ICloseable window, bool cancel)
        {
            Abbrechen = cancel;
            if (window != null)
                window.Close();
        }
    }
}