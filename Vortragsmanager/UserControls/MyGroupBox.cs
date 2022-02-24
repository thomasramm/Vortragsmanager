using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.LayoutControl;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;
using DevExpress.Xpf.Core.Native;

namespace Vortragsmanager.Core
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
                TitleElement.MouseLeftButtonUp += new MouseButtonEventHandler(OnTitleElementMouseLeftButtonUp);
            }
        }
        void OnTitleElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == GroupBoxState.Normal) State = GroupBoxState.Maximized;
            else State = GroupBoxState.Normal;
            e.Handled = true;
        }
    }
}
