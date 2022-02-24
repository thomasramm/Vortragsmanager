using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.LayoutControl;
using Vortragsmanager.Interface;
using Vortragsmanager.MeineVerwaltung;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace Vortragsmanager.Pages
{
    /// <summary>
    /// Interaction logic for VorlagenView.xaml
    /// </summary>
    public partial class VerwaltungVersammlungPage
    {
        public VerwaltungVersammlungPage(INavigation parentModel)
        {
            InitializeComponent();
            DataContext = new ConregationsViewModelCollection(parentModel);
        }

        public void SelectConregation(Datamodels.Conregation versammlung)
        {
            var boxList = lc.Children;
            foreach (var box in boxList)
            {
                var gBox = (box as GroupBox);
                if (gBox is null)
                    continue;
                var data = (Views.ConregationViewModel)gBox.DataContext;
                if (data.Versammlung == versammlung)
                {
                    lc.MaximizedElement = gBox;
                }
            }
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
            AddNewConregation.Visibility = editMode ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
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
            var filter = e.NewValue?.ToString().ToLower(Helper.Helper.German) ?? null;
            foreach (var box in lc.Children)
            {
                var gBox = (box as GroupBox);
                if (gBox is null)
                    continue;
                var data = (Views.ConregationViewModel)gBox.DataContext;

                var filterValue = $"{data.Versammlung.Kreis} {data.Versammlung.Name} {data.Versammlung.Koordinator}".ToLower(Helper.Helper.German);
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