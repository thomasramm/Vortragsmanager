using System.Windows.Controls;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VerwaltungVorträgePage
    {
        public VerwaltungVorträgePage()
        {
            InitializeComponent();
        }

        private void TableView_GetIsEditorActivationAction(object sender, DevExpress.Xpf.Grid.GetIsEditorActivationActionEventArgs e)
        {
            if (e.ActivationAction == DevExpress.Xpf.Editors.ActivationAction.MouseLeftButtonDown)
                e.IsActivationAction = true;
        }

        private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                if (e.NewSize.Width > 1000)
                {
                    if (MasterLayoutControl.Orientation == Orientation.Vertical)
                        MasterLayoutControl.Orientation = Orientation.Horizontal;
                        
                }
                else 
                {
                    if (MasterLayoutControl.Orientation == Orientation.Horizontal)
                        MasterLayoutControl.Orientation = Orientation.Vertical;
                }
            }
        }

        private void numericEditor_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            if (e.Value is null)
                return;

            var eingabe = e.Value.ToString();
            if (string.IsNullOrWhiteSpace(eingabe))
            {
                e.IsValid = false;
                return;
            }
            
            if (!int.TryParse(eingabe, out int nummer))
            {
                e.IsValid = false;
                return;
            }

            if (Datamodels.TalkList.Find(nummer).Nummer != -1)
            {
                e.IsValid = false;
                AddButton.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                e.IsValid = true;
                AddButton.Foreground = System.Windows.Media.Brushes.Green;
            }
        }
    }
}