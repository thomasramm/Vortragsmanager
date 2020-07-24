using DevExpress.Xpf.LayoutControl;
using System.Windows.Controls;
using System.Windows.Input;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace Vortragsmanager.MeineVerwaltung
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VersammlungSettingsView : UserControl
    {
        public VersammlungSettingsView()
        {
            InitializeComponent();
        }

        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var groupBox = (GroupBox)sender;
            if (groupBox.State == GroupBoxState.Normal)
                groupBox.State = GroupBoxState.Maximized;
            //else if (groupBox.State == GroupBoxState.Maximized)
            //    groupBox.State = GroupBoxState.Normal;
        }

        //wenn eine Versammlung maximiert wird, dann alle anderen ausblenden.
        private void Lc_MaximizedElementChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<System.Windows.FrameworkElement> e)
        {
            var boxList = ((FlowLayoutControl)sender).Children;
            var editMode = (e.NewValue != null); // ?  System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            var maximizedBox = (e.NewValue as GroupBox);
            foreach (var box in boxList)
            {
                var gBox = (box as GroupBox);
                if (gBox is null)
                    continue;
                var data = (Views.ConregationViewModel)gBox.DataContext;
                data.EditMode = editMode;
                data.Select(gBox == maximizedBox);
            }
        }

        private void SearchBox_ValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var filter = e.NewValue?.ToString().ToLower(Core.Helper.German) ?? null;
            foreach (var box in lc.Children)
            {
                var gBox = (box as GroupBox);
                if (gBox is null)
                    continue;
                var data = (Views.ConregationViewModel)gBox.DataContext;

                var filterValue = $"{data.Versammlung.Kreis} {data.Versammlung.Name} {data.Versammlung.Koordinator}".ToLower(Core.Helper.German);
                if (string.IsNullOrEmpty(filter))
                    data.MatchFilter = true;
                else if (filterValue.Contains(filter))
                    data.MatchFilter = true;
                else
                    data.MatchFilter = false;
            }
        }
    }
}