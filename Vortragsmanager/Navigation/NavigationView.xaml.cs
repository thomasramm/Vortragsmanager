using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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