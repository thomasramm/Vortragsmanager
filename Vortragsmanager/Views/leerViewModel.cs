using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class LeerViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Outside> _talks;

        public LeerViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
        }

        public LeerViewModel(ObservableCollection<Outside> Talks) : this()
        {
            _talks = Talks;
            GetMailText();
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }

        private void GetMailText()
        {
            var mt = Core.Templates.GetTemplate(Core.Templates.TemplateName.RednerTermineMailText).Inhalt;
            var listeRedner = new List<Speaker>();
            var mails = "";
            var termine = "";

            foreach (var einladung in _talks)
            {
                if (!listeRedner.Contains(einladung.Ältester))
                    listeRedner.Add(einladung.Ältester);
            }

            foreach (var ä in listeRedner)
            {
                mails += $"{ä.Mail}; ";
                termine += "-----------------------------------------------------" + Environment.NewLine;
                termine += ä.Name + Environment.NewLine;

                foreach (var einladung in _talks)
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

            MailText = mt;
            Clipboard.SetText(mt);
        }

        public string MailText
        {
            get { return GetProperty(() => MailText); }
            set { SetProperty(() => MailText, value); }
        }
    }
}