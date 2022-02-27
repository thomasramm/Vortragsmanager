using DevExpress.Mvvm;
using System;
using System.Linq;
using System.Windows.Media;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Views
{
    public class SpeakerViewModel : ViewModelBase
    {
        private readonly INavigation _parentModel;

        public SpeakerViewModel(INavigation parentModel, Speaker redner)
        {
            _parentModel = parentModel;
            Redner = redner ?? throw new NullReferenceException();
        }     

        public void NavigateToEditor()
        {
            _parentModel.NavigateTo(NavigationPage.VerwaltungLandingPage, Redner);
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

        public Speaker Redner { get; }
    }
}