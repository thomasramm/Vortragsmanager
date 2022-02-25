using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für DropDownVersammlung.xaml
    /// </summary>
    public partial class DropDownVersammlung : UserControl
    {
        public DropDownVersammlung()
        {
            var items = DataContainer.Versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer());
            foreach (var item in items)
            {
                ListeAlle.Add(item);
                ListeFilteredItems.Add(item);
            }
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
  nameof(SelectedItem),
  typeof(Conregation),
  typeof(DropDownVersammlung),
  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnConregationChanged));

        public Conregation SelectedItem
        {
            get { return (Conregation)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnConregationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sh = (DropDownVersammlung)d;
            //sh.SelectedName = ((Conregation)e.NewValue).Name;
            Conregation oldC = (Conregation)e.OldValue;
            Conregation newC = (Conregation)e.NewValue;
            sh.OnConregationChanged(oldC, newC);
        }


        public ObservableCollection<Conregation> ListeAlle { get; private set; } = new ObservableCollection<Conregation>();

        public ObservableCollection<Conregation> ListeFilteredItems { get; private set; } = new ObservableCollection<Conregation>();

        private void AutoSuggestEdit_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            SetFilter(e.Text);
        }

        public void SetFilter(string filter = null)
        {
            ListeFilteredItems.Clear();
            var items = ListeAlle.Count;
            var newCount = 0;
            var maxCount = (filter == null) ? items : 10;
            for (int i = 0; i < items; i++)
            {
                if (string.IsNullOrEmpty(filter) || (Regex.IsMatch(ListeAlle[i].NameMitKoordinator, Regex.Escape(filter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                {
                    ListeFilteredItems.Add(ListeAlle[i]);
                    newCount++;
                    if (newCount == maxCount)
                        break;
                }
            }
        }

        public string SelectedName
        {
            get => SelectedItem?.Name;
            set
            {
                var vers = value != null ? DataContainer.ConregationFind(value) : null;
                SelectedItem = vers;
            }
            
        }

        // RoutedEvent
        public static readonly RoutedEvent ConregationChangedEvent = EventManager.RegisterRoutedEvent("ConregationChanged", RoutingStrategy.Bubble,
              typeof(RoutedPropertyChangedEventHandler<Conregation>), typeof(DropDownVersammlung));

        public event RoutedPropertyChangedEventHandler<Conregation> ConregationChanged
        {
            add { AddHandler(ConregationChangedEvent, value); }
            remove { RemoveHandler(ConregationChangedEvent, value); }
        }

        private void OnConregationChanged(Conregation oldValue, Conregation newValue)
        {
            RoutedPropertyChangedEventArgs<Conregation> args = new RoutedPropertyChangedEventArgs<Conregation>(oldValue, newValue);
            args.RoutedEvent = DropDownVersammlung.ConregationChangedEvent;
            RaiseEvent(args);
        }
    }
}
