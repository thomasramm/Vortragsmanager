using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Helper;
using Vortragsmanager.UserControls;
using Vortragsmanager.Windows;

namespace Vortragsmanager.DataModels
{
    public class Anfrage
    {
        public Inquiry BaseAnfrage { get; set; }

        public Anfrage(Inquiry inquiry)
        {
            BaseAnfrage = inquiry;
            if (inquiry is null)
                return;

            MailtextAnzeigenCommand = new DelegateCommand(MailtextAnzeigen);

            foreach (var x in BaseAnfrage.RednerVortrag)
            {
                Redner.Add(new AnfrageDetail(this, x.Key, x.Value));
            }
        }

        public DelegateCommand MailtextAnzeigenCommand { get; }

        public void MailtextAnzeigen()
        {
            var w = new InfoAnRednerUndKoordinatorWindow();
            var data = (InfoAnRednerUndKoordinatorViewModel)w.DataContext;
            data.Titel = $"Original Mailtext vom {AnfrageDatum}";
            data.MailTextKoordinator = string.IsNullOrWhiteSpace(BaseAnfrage.Mailtext) ? "kein Mailtext vorhanden!" + Environment.NewLine + BaseAnfrage.Kommentar : BaseAnfrage.Mailtext;
            data.DisableCancelButton();

            w.ShowDialog();
        }

        private readonly ObservableCollection<DateTime> _wochen = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> Wochen
        {
            get
            {
                _wochen.Clear();
                foreach(var w in BaseAnfrage.Kws)
                {
                    _wochen.Add(DateCalcuation.CalculateWeek(w));
                }

                return _wochen;
            }
        }

        public string Versammlung => BaseAnfrage.Versammlung.Name;

        public string AnfrageDatum => BaseAnfrage.AnfrageDatum.ToString("dd.MM.yyyy", Helper.Helper.German);

        public ObservableCollection<AnfrageDetail> Redner { get; } = new ObservableCollection<AnfrageDetail>();
    }
}