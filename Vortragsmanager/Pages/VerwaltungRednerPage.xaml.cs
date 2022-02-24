using System.Windows.Controls;
using Vortragsmanager.Datamodels;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VerwaltungRednerPage : UserControl
    {
        private readonly VerwaltungRednerPageModel _dataModel;

        public VerwaltungRednerPage()
        {
            InitializeComponent();
            _dataModel = (VerwaltungRednerPageModel)DataContext;
        }

        public VerwaltungRednerPage(Speaker redner)
        {
            InitializeComponent();
            _dataModel = (VerwaltungRednerPageModel)DataContext;
            RednerSelect.SelectedName = redner?.Name;
            RednerSelect.SelectedItem = redner;
        }


        /// <summary>
        /// Ich habe die view bereits verlassen.
        /// </summary>
        /// <param name="e">Parameter.</param>
        //public void NavigatedFrom(NavigationEventArgs e)
        //{
        //    if (e.Source.ToString() == "VersammlungSettingsView")
        //    {
        //        var vers = DataModel.Redner?.Versammlung;
        //        if (vers == null)
        //            vers = DataModel.SelectedConregation;
        //        if (vers == null)
        //            return;

        //        var versView = (e.Content as VerwaltungVersammlungPage);
        //        versView.SelectConregation(vers);
        //        //e.Parameter = DataModel.Redner;
        //    }

        //}

        //Vorschlagsliste aktualisieren


        private void DropDownVersammlung_ConregationChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Conregation> e)
        {
            _dataModel.SelectedConregation = e.NewValue;
            RednerSelect.SelectedVersammlung = e.NewValue;
        }

        private void DropDownRedner_OnSpeakerChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Speaker> e)
        {
            _dataModel.Redner = e.NewValue;
            calendar.Person = e.NewValue;
        }

        private void DropDownVortrag_OnSelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Datamodels.Talk> e)
        {
            _dataModel.NeuerVortrag = e.NewValue;
        }

        private void ComboBoxEdit_PopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Cancel)
                return;
            _dataModel.NewConregation = e.EditValue as Datamodels.Conregation;
        }
    }
}