using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            InitializeComponent();
        }
    }

    public class FrameAnimationSelector : DevExpress.Xpf.WindowsUI.AnimationSelector
    {
        private Storyboard _BackStoryboard;
        private Storyboard _ForwardStoryboard;
        public Storyboard ForwardStoryboard
        {
            get { return _ForwardStoryboard; }
            set { _ForwardStoryboard = value; }
        }
        public Storyboard BackStoryboard
        {
            get { return _BackStoryboard; }
            set { _BackStoryboard = value; }
        }
        protected override Storyboard SelectStoryboard(DevExpress.Xpf.WindowsUI.FrameAnimation animation)
        {
            if (animation is null)
                throw new NullReferenceException();
            return animation.Direction == DevExpress.Xpf.WindowsUI.AnimationDirection.Forward ? ForwardStoryboard : BackStoryboard;
        }
    }
}
