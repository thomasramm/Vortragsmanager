using Vortragsmanager.Datamodels;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VerwaltungRednerPage
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

        //Vorschlagsliste aktualisieren
        private void DropDownVersammlung_ConregationChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Conregation> e)
        {
            _dataModel.SelectedConregation = e.NewValue;
            RednerSelect.SelectedVersammlung = e.NewValue;
        }

        private void DropDownRedner_OnSpeakerChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Speaker> e)
        {
            _dataModel.Redner = e.NewValue;
            Calendar.Person = e.NewValue;
        }

        private void DropDownVortrag_OnSelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Talk> e)
        {
            _dataModel.NeuerVortrag = e.NewValue;
        }

        private void ComboBoxEdit_PopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Cancel)
                return;
            _dataModel.NewConregation = e.EditValue as Conregation;
        }
    }
}