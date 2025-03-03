using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.Properties;
using Vortragsmanager.Windows;

namespace Vortragsmanager.PageModels
{
    public class MeineRednerKalenderPageModel : ViewModelBase
    {
        public MeineRednerKalenderPageModel()
        {
            Messenger.Default.Register<int>(this, Messages.DisplayYearChanged, OnMessage);
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            ChangeView = new DelegateCommand<RednerViewType>(ChangeCurrentView);
            ListeSenden = new DelegateCommand(ListeVersenden);
            VortragAbsagen = new DelegateCommand(Absagen);
            VortragBearbeiten = new DelegateCommand(Bearbeiten);

            var z = DataContainer.Redner.Where(x => x.Versammlung == DataContainer.MeineVersammlung);
            var enumerable = z.ToList();
            Redner = new List<CheckBox>(enumerable.Count);
            var box = new CheckBox() { Content = "Alle", IsChecked = true };
            box.Checked += CheckAll;
            box.Unchecked += CheckAll;
            Redner.Add(box);
            foreach (var r in enumerable)
            {
                box = new CheckBox() { Content = r.Name, IsChecked = true };
                box.Checked += Box_Checked;
                box.Unchecked += Box_Checked;
                box.Margin = new System.Windows.Thickness(15, 1, 0, 1);
                Redner.Add(box);
            }
            //Talks = Core.DataContainer.ExternerPlan;//.Where(x => x.Datum.Year == CurrentYear);
            //Laden der Optionen
            
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

        public DelegateCommand<int> ChangeYear { get; }

        public DelegateCommand VortragAbsagen { get; }

        public DelegateCommand VortragBearbeiten { get; }

        public void ChangeCurrentYear(int step)
        {
            Helper.Helper.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            var firstWeekOfYear = DateCalcuation.GetFirstWeekOfYear(CurrentYear);
            //Jahr
            List<Outside> list = DataContainer.ExternerPlan.Where(x => x.Kw >= firstWeekOfYear).ToList();
            //ToDo2: interne sind auch externe!!!
            var listIntern = DataContainer.MeinPlan.Where(x => x.Kw >= firstWeekOfYear && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Where(x => x.Ältester.Versammlung == DataContainer.MeineVersammlung);

            if (!History)
            {
                list = list.Where(x => x.Kw >= DateCalcuation.CurrentWeek).ToList();
                listIntern = listIntern.Where(x => x.Kw >= DateCalcuation.CurrentWeek);
            }

            if (OneYearOnly)
            {
                list = list.Where(x => x.Datum.Year == CurrentYear).ToList();
                listIntern = listIntern.Where(x => x.Kw < (CurrentYear+1)*100).ToList();
            }

            foreach (var item in listIntern)
            {
                list.Add(new Outside()
                {
                    Ältester = item.Ältester,
                    Versammlung = DataContainer.MeineVersammlung,
                    Kw = item.Kw,
                    Reason = OutsideReason.Talk,
                    Vortrag = item.Vortrag
                });
            }

            //Person
            if (Redner.Any(x => x.IsChecked == false))
            {
                var list2 = list.Join(Redner,
                    li => li.Ältester.Name,
                    fi => fi.Content.ToString(),
                    (li, fi) => new { Redner = li, fi.IsChecked }).Where(x => x.IsChecked == true).Select(x => x.Redner).OrderBy(x => x.Kw);
                Talks = new ObservableCollection<Outside>(list2);
            }
            else
            {
                Talks = new ObservableCollection<Outside>(list.OrderBy(x => x.Kw));
            }
            RaisePropertyChanged(nameof(Talks));
        }

        public DelegateCommand<RednerViewType> ChangeView { get; }

        public void ChangeCurrentView(RednerViewType view)
        {
            switch (view)
            {
                case RednerViewType.Year:
                    ViewStateYear = true;
                    ViewStateAgenda = false;
                    break;

                case RednerViewType.Agenda:
                    ViewStateYear = false;
                    ViewStateAgenda = true;
                    LoadAgendaView();
                    break;
            }

            RaisePropertiesChanged("ViewStateYear", "ViewStateAgenda");
        }

        public bool ViewStateYear { get; set; }

        public bool ViewStateAgenda { get; set; }

        public int CurrentYear => Helper.Helper.DisplayedYear;

        private void OnMessage(int year)
        {
            RaisePropertyChanged(nameof(CurrentYear));
        }

        public List<CheckBox> Redner { get; }

        public ObservableCollection<Outside> Talks { get; private set; }

        private Outside _selectedTalk;

        public Outside SelectedTalk
        {
            get => _selectedTalk;
            set
            {
                _selectedTalk = value;
                RaisePropertyChanged();
            }
        }

        public bool History
        {
            get => Helper.Helper.GlobalSettings.MeineRednerHistory;
            set
            {
                Helper.Helper.GlobalSettings.MeineRednerHistory = value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertiesChanged();
                ApplyFilter();
            }
        }

        public bool OneYearOnly
        {
            get => Helper.Helper.GlobalSettings.MeineRednerOneYearOnly;
            set
            {
                Helper.Helper.GlobalSettings.MeineRednerOneYearOnly = value;
                Helper.Helper.GlobalSettings.Save();
                RaisePropertiesChanged();
                ApplyFilter();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")]
        private void LoadAgendaView()
        {
            //ToDo: Alternative Ansicht öffnen
        }

        public DelegateCommand ListeSenden { get; }

        public void ListeVersenden()
        {
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            data.Titel = "Liste der Vortragseinladungen versenden";

            var listeRedner = new List<Speaker>();
            foreach (var einladung in Talks)
            {
                if (!listeRedner.Contains(einladung.Ältester))
                    listeRedner.Add(einladung.Ältester);
            }
            data.MailTextRedner = Templates.GetRednerlisteMailText(listeRedner, Talks);
            data.DisableCancelButton();

            w.ShowDialog();

            var einRedner = listeRedner.Count == 1 ? listeRedner[0] : null;
            ActivityAddItem.OutsideSendList(einRedner, data.MailTextRedner);
        }

        public void Absagen()
        {
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            data.Titel = "Vortrag absagen";
            data.MailTextKoordinator = Templates.GetMailTextAblehnenKoordinator(SelectedTalk);
            data.MailTextRedner = Templates.GetMailTextAblehnenRedner(SelectedTalk);
            w.ShowDialog();

            if (data.Speichern)
            {
                ActivityAddItem.Outside(SelectedTalk, data.MailTextKoordinator, data.MailTextRedner, false);
                DataContainer.ExternerPlan.Remove(SelectedTalk);
                Talks.Remove(SelectedTalk);
            }
        }

        public void Bearbeiten()
        {
            if (SelectedTalk == null)
                return;

            var dlg1 = new KalendereintragVerschieben();
            var data = (KalendereintragVerschiebenView)dlg1.DataContext;
            var origKw = SelectedTalk.Kw;
            data.LadeStartDatum(SelectedTalk);
            dlg1.ShowDialog();


            if (!data.Speichern) 
                return;
            
            var origDataBaseItem = DataContainer.ExternerPlan.FirstOrDefault(x =>
                x.Kw == origKw && x.Ältester.Name == data.StartName &&
                x.Versammlung.Name == data.StartVersammlung);

            if (origDataBaseItem != null)
            {
                origDataBaseItem.Kw = SelectedTalk.Kw;
            }

            if (data.ZielbuchungLöschenChecked && data.ZielBuchung != null)
            {
                Talks.Remove(data.ZielBuchung);
            }

            ApplyFilter();
        }
    }
}