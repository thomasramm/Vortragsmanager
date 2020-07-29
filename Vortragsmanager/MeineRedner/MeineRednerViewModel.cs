﻿using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.UserControls;
using Vortragsmanager.Views;

namespace Vortragsmanager.MeineRedner
{
    public class MeineRednerViewModel : ViewModelBase
    {
        public MeineRednerViewModel()
        {
            Messenger.Default.Register<int>(this, Messages.DisplayYearChanged, OnMessage);
            ChangeYear = new DelegateCommand<int>(ChangeCurrentYear);
            ChangeView = new DelegateCommand<RednerViewType>(ChangeCurrentView);
            ListeSenden = new DelegateCommand(ListeVersenden);
            VortragAbsagen = new DelegateCommand(Absagen);

            var z = DataContainer.Redner.Where(x => x.Versammlung == DataContainer.MeineVersammlung);
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
            Helper.DisplayedYear += step;
            RaisePropertyChanged(nameof(CurrentYear));
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            //Jahr
            List<Outside> list = DataContainer.ExternerPlan.Where(x => x.Datum.Year == CurrentYear).ToList();
            //ToDo2: interne sind auch externe!!!
            var listIntern = DataContainer.MeinPlan.Where(x => x.Datum.Year == CurrentYear && x.Status == EventStatus.Zugesagt).Cast<Invitation>().Where(x => x.Ältester.Versammlung == DataContainer.MeineVersammlung);

            if (!History)
            {
                list = list.Where(x => x.Datum >= DateTime.Today).ToList();
                listIntern = listIntern.Where(x => x.Datum >= DateTime.Today);
            }

            foreach (var item in listIntern)
            {
                list.Add(new Outside()
                {
                    Ältester = item.Ältester,
                    Versammlung = DataContainer.MeineVersammlung,
                    Datum = item.Datum,
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
                    (li, fi) => new { Redner = li, fi.IsChecked }).Where(x => x.IsChecked == true).Select(x => x.Redner).OrderBy(x => x.Datum);
                Talks = new ObservableCollection<Outside>(list2);
            }
            else
            {
                Talks = new ObservableCollection<Outside>(list.OrderBy(x => x.Datum));
            }
            RaisePropertyChanged(nameof(Talks));
        }

        public DelegateCommand<RednerViewType> ChangeView { get; private set; }

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

                default:
                    break;
            }
            RaisePropertiesChanged(new[] { "ViewStateYear", "ViewStateAgenda" });
        }

        public bool ViewStateYear { get; set; }

        public bool ViewStateAgenda { get; set; }

        public int CurrentYear => Helper.DisplayedYear;

        private void OnMessage(int year)
        {
            RaisePropertyChanged(nameof(CurrentYear));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822")]
        private void LoadAgendaView()
        {
            //ToDo: Alternative Ansicht öffnen
        }

        public DelegateCommand ListeSenden { get; private set; }

        private string GetListeMailText(List<Speaker> listeRedner)
        {
            var mt = Templates.GetTemplate(Templates.TemplateName.RednerTermineMailText).Inhalt;

            var mails = "";
            var termine = "";

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
                    termine += $"\tVortrag:\t{einladung.Vortrag.Vortrag}" + Environment.NewLine;
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

            var listeRedner = new List<Speaker>();
            foreach (var einladung in Talks)
            {
                if (!listeRedner.Contains(einladung.Ältester))
                    listeRedner.Add(einladung.Ältester);
            }
            data.MailTextRedner = GetListeMailText(listeRedner);
            data.DisableCancelButton();

            w.ShowDialog();

            var einRedner = listeRedner.Count() == 1 ? listeRedner[0] : null;
            ActivityViewModel.AddActivityOutsideSendList(einRedner, data.MailTextRedner);
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
                DataContainer.ExternerPlan.Remove(SelectedTalk);
                Talks.Remove(SelectedTalk);
                ActivityViewModel.AddActivityOutside(SelectedTalk, data.MailTextKoordinator, data.MailTextRedner, false);
            }
        }
    }

    public enum RednerViewType
    {
        Year,
        Agenda,
    }
}