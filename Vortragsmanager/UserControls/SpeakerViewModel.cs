using DevExpress.Mvvm;
using System;
using System.Linq;
using System.Windows.Media;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Views
{
    public class SpeakerViewModel : ViewModelBase
    {
        public SpeakerViewModel(Speaker redner)
        {
            if (redner is null)
                throw new NullReferenceException();

            Redner = redner;
        }     

        public void NavigateToEditor()
        {
            //ToDo: Navigation auf Rednereinstellungen von VersammlungenView
            //Navigation.NavigationView.Frame.Navigate("RednerView", Redner);
        }

        #region XAML Eigenschaften (Neben Redner.Name)
        public SolidColorBrush AktivBrush => Redner.Aktiv && Redner.Einladen ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

        public string AktivText => Redner.Aktiv && Redner.Einladen ? "Aktiv" : "Inaktiv";

        public SolidColorBrush ÄltesterBrush => Redner.Ältester ? new SolidColorBrush(Colors.DodgerBlue) : new SolidColorBrush(Colors.Brown);

        public string ÄltesterText => Redner.Ältester ? "Ältester" : "DAG";

        public string Overview
        {
            get
            {
                string o = "Vorträge: ";
                foreach (var v in Redner.Vorträge.OrderBy(x => x.Vortrag.Nummer))
                    o += v.Vortrag.Nummer + ", ";

                return o.TrimEnd(' ').TrimEnd(',');
            }
        }
        #endregion

        public Speaker Redner { get; private set; }
    }
}