using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.MeineRedner
{
    /// <summary>
    /// Interaktionslogik für ExternalView.xaml
    /// </summary>
    public partial class MeineRednerPlan : UserControl
    {
        public MeineRednerPlan()
        {
            InitializeComponent();
        }

        private void DataControlBase_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.NewItem is Outside eintrag))
            {
                return;
            }

            var redner = eintrag.Ältester;
            if (Calendar != null)
                Calendar.Person = redner;
        }
    }
}