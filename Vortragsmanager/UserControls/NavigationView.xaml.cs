using DevExpress.Xpf.WindowsUI;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.Navigation
{
    /// <summary>
    /// Interaktionslogik für NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl
    {
        private ActivityViewModel DataModel;

        public NavigationView()
        {
            InitializeComponent();
            Frame = frame;
            DataModel = (ActivityViewModel)DataContext;
        }

        public static NavigationFrame Frame { get; set; }

        private void VersammlungenFilter_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            DataModel.SetVersammlungfilter(e.Text);
        }
    }

    public class FrameAnimationSelector : AnimationSelector
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

        protected override Storyboard SelectStoryboard(FrameAnimation animation)
        {
            if (animation is null)
                throw new NullReferenceException();
            return animation.Direction == AnimationDirection.Forward ? ForwardStoryboard : BackStoryboard;
        }
    }
}