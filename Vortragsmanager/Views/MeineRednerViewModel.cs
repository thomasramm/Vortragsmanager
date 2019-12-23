using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class MeineRednerViewModel : ViewModelBase
    {
        public MeineRednerViewModel()
        {
            Messenger.Default.Register<Messages>(this, OnMessage);
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            ChangeView = new DelegateCommand<View>(ChangeCurrentView);
            ListeSenden = new DelegateCommand(ListeVersenden);

            var z = Core.DataContainer.Redner.Where(x => x.Versammlung == Core.DataContainer.MeineVersammlung);
            Redner = new List<CheckBox>(z.Count());
            var box = new CheckBox() { Content = "Alle", IsChecked = true };
            box.Checked += CheckAll;
            box.Unchecked += CheckAll;
            box.Margin = new System.Windows.Thickness(0, 5, 5, 10);
            Redner.Add(box);
            foreach (var r in z)
            {
                box = new CheckBox() { Content = r.Name, IsChecked = true };
                box.Checked += Box_Checked;
                box.Unchecked += Box_Checked;
                box.Margin = new System.Windows.Thickness(15, 5, 5, 5);
                Redner.Add(box);
            }
            //Talks = Core.DataContainer.ExternerPlan;//.Where(x => x.Datum.Year == CurrentYear);
            ChangeCurrentYear(0);
        }

        private void CheckAll(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (CheckBox)sender;
            var isChecked = b.IsChecked;
            for (int i = 1; i < Redner.Count; i++)
            {
                Redner[i].IsChecked = isChecked;
            }
        }

        private void Box_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplyFilter();
        }

        public DelegateCommand<int> ChangeYear { get; private set; }

        public void ChangeCurrentYear(int step)
        {
            Core.DataContainer.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            //Jahr
            var list = Core.DataContainer.ExternerPlan.Where(x => x.Datum.Year == CurrentYear);

            if (!History)
                list = list.Where(x => x.Datum >= DateTime.Today);

            //Person
            if (Redner.Any(x => x.IsChecked == false))
            {
                var list2 = list.Join(Redner,
                    li => li.Ältester.Name,
                    fi => fi.Content.ToString(),
                    (li, fi) => new { Redner = li, fi.IsChecked }).Where(x => x.IsChecked == true).Select(x => x.Redner);
                Talks = new ObservableCollection<Outside>(list2);
            }
            else
                Talks = new ObservableCollection<Outside>(list);
            RaisePropertyChanged(nameof(Talks));
        }

        public DelegateCommand<View> ChangeView { get; private set; }

        public void ChangeCurrentView(View view)
        {
            switch (view)
            {
                case View.Year:
                    ViewStateYear = true;
                    ViewStateAgenda = false;
                    break;

                case View.Agenda:
                    ViewStateYear = false;
                    ViewStateAgenda = true;
                    LoadAgendaView();
                    break;

                default:
                    break;
            }
            RaisePropertiesChanged(new[] { "ViewStateYear", "ViewStateAgenda" });
        }

        public bool ViewStateYear { get; set; }
        public bool ViewStateAgenda { get; set; }

        public int CurrentYear
        {
            get
            {
                return Core.DataContainer.DisplayedYear;
            }
        }

        private void OnMessage(Messages message)
        {
            switch (message)
            {
                case Messages.DisplayYearChanged:
                    RaisePropertyChanged(nameof(CurrentYear));
                    break;

                default:
                    break;
            }
        }

        public List<CheckBox> Redner { get; private set; }

        public ObservableCollection<Outside> Talks { get; private set; }

        public bool History
        {
            get { return GetProperty(() => History); }
            set { SetProperty(() => History, value, ApplyFilter); }
        }

        private void LoadAgendaView()
        {
            //ToDo: Alternative Ansicht öffnen
        }

        public DelegateCommand ListeSenden { get; private set; }

        public void ListeVersenden()
        {
            var w = new ListeRednerTermineSendenDialog
            {
                DataContext = new ListeRednerTermineSendenViewModel(Talks)
            };
            w.ShowDialog();
        }
    }

    public enum View
    {
        Year,
        Agenda,
    }
}