using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Vortragsmanager.Datamodels;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for VersammlungRednerMaximizedControl.xaml
    /// </summary>
    public partial class VersammlungRednerMaximizedControl : UserControl
    {
        private SpeakerViewModel _model;

        public VersammlungRednerMaximizedControl()
        {
            InitializeComponent();
            this.SetBinding(BoundDataContextProperty, new Binding());
            
        }

        public static readonly DependencyProperty BoundDataContextProperty = DependencyProperty.Register(
    "BoundDataContext",
    typeof(object),
    typeof(VersammlungRednerMaximizedControl),
    new PropertyMetadata(null, OnBoundDataContextChanged));

        private static void OnBoundDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var x = e;
            var y = d;
            if (e.NewValue != null)
            {
                var model = (e.NewValue as SpeakerViewModel);
                if (model != null)
                {
                    (d as VersammlungRednerMaximizedControl).SetRednerForCalendar(model.Redner);
                }
            }
            // e.NewValue is your new DataContext
            // d is your UserControl
        }

        public void SetRednerForCalendar(Speaker person)
        {
            calendar.Person = person;
        }
    }
}