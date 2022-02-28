using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.LayoutControl;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace Vortragsmanager.UserControls
{
    public class MyGroupBox : GroupBox
    {
        protected FrameworkElement TitleElement { get; set; }
        public override void OnApplyTemplate()
        {
            if (TitleElement != null)
                TitleElement.MouseLeftButtonUp -= OnTitleElementMouseLeftButtonUp;
            base.OnApplyTemplate();
            TitleElement = LayoutHelper.FindParentObject<LayoutGroup>(GetTemplateChild("MaximizeElement"));
            if (TitleElement != null)
            {
                TitleElement.MouseLeftButtonUp += OnTitleElementMouseLeftButtonUp;
            }
        }
        void OnTitleElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            State = State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
            e.Handled = true;
        }
    }
}
