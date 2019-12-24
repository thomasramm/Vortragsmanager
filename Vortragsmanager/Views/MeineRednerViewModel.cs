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
            VortragAbsagen = new DelegateCommand(Absagen);

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

        public DelegateCommand VortragAbsagen { get; private set; }

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

        private Outside _selectedTalk;

        public Outside SelectedTalk
        {
            get
            {
                return _selectedTalk;
            }
            set
            {
                _selectedTalk = value;
                RaisePropertyChanged();
            }
        }

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

        private string GetListeMailText()
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.RednerTermineMailText).Inhalt;
            var listeRedner = new List<Speaker>();
            var mails = "";
            var termine = "";

            foreach (var einladung in Talks)
            {
                if (!listeRedner.Contains(einladung.Ältester))
                    listeRedner.Add(einladung.Ältester);
            }

            foreach (var ä in listeRedner)
            {
                mails += $"{ä.Mail}; ";
                termine += "-----------------------------------------------------" + Environment.NewLine;
                termine += ä.Name + Environment.NewLine;

                foreach (var einladung in Talks)
                {
                    if (einladung.Ältester != ä)
                        continue;

                    termine += $"\tDatum:\t{einladung.Datum:dd.MM.yyyy}" + Environment.NewLine;
                    termine += $"\tVortrag:\t{einladung.Vortrag}" + Environment.NewLine;
                    termine += $"\tVersammlung:\t{einladung.Versammlung.Name}, {einladung.Versammlung.Anschrift1}, {einladung.Versammlung.Anschrift2}, Versammlungszeit: {einladung.Versammlung.GetZusammenkunftszeit(einladung.Datum.Year)}" + Environment.NewLine;
                    termine += Environment.NewLine;
                }
                termine += Environment.NewLine;
            }

            mails = mails.Substring(0, mails.Length - 2);

            mt = mt
                .Replace("{Redner Mail}", mails)
                .Replace("{Redner Termine}", termine);

            return mt;
        }

        public void ListeVersenden()
        {
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            data.Titel = "Liste der Vortragseinladungen versenden";
            data.MailTextRedner = GetListeMailText();

            w.ShowDialog();
        }

        public void Absagen()
        {
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            data.Titel = "Vortrag absagen";
            data.MailTextKoordinator = Core.Templates.GetMailTextAblehnenKoordinator(SelectedTalk);
            data.MailTextRedner = Core.Templates.GetMailTextAblehnenRedner(SelectedTalk);
            w.ShowDialog();

            if (data.Speichern)
            {
                Core.DataContainer.ExternerPlan.Remove(SelectedTalk);
                Talks.Remove(SelectedTalk);
            }
        }
    }

    public enum View
    {
        Year,
        Agenda,
    }
}