using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Vortragsmanager.DataModels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für DropDownVersammlung.xaml
    /// </summary>
    public partial class DropDownRedner : INotifyPropertyChanged
    {
        public DropDownRedner()
        {
            var items = DataContainer.Redner.OrderBy(x => x.Name);
            foreach (var item in items)
            {
                ListeAlle.Add(item);
                ListeFilteredItems.Add(item);
            }
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
  nameof(SelectedItem),
  typeof(Speaker),
  typeof(DropDownRedner),
  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SpeakerChanged));

        public Speaker SelectedItem
        {
            get => (Speaker)GetValue(SelectedItemProperty);
            set 
            {
                SetValue(SelectedItemProperty, value);
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(SelectedName));
            }
        }

        private static void SpeakerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sh = (DropDownRedner)d;
            Speaker oldC = (Speaker)e.OldValue;
            Speaker newC = (Speaker)e.NewValue;
            sh.SpeakerChanged(oldC, newC);
        }


        public ObservableCollection<Speaker> ListeAlle { get; } = new ObservableCollection<Speaker>();

        public ObservableCollection<Speaker> ListeFilteredItems { get; private set; } = new ObservableCollection<Speaker>();

        private void AutoSuggestEdit_QuerySubmitted(object sender, DevExpress.Xpf.Editors.AutoSuggestEditQuerySubmittedEventArgs e)
        {
            SetFilter(e.Text);
        }

        private void SetFilter(string filter = null)
        {
            ListeFilteredItems.Clear();
            var items = ListeAlle.Count;
            var newCount = 0;
            for (int i = 0; i < items; i++)
            {
                if (SelectedVersammlung == null || ListeAlle[i].Versammlung == SelectedVersammlung)
                {
                    if (string.IsNullOrEmpty(filter) || (Regex.IsMatch(ListeAlle[i].Name, Regex.Escape(filter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
                    {
                        ListeFilteredItems.Add(ListeAlle[i]);
                        newCount++;
                        //if (newCount == 10)
                        //    break;
                    }
                }
            }
            if (SelectedItem == null || SelectedVersammlung == null || SelectedItem.Versammlung == SelectedVersammlung)
                return;

            SelectedItem = null;
            RaisePropertyChanged(nameof(SelectedName));
        }

        public string SelectedName
        {
            get => SelectedItem?.Name;
            set
            {
                var vers = value != null ? DataContainer.SpeakerFind(value, SelectedVersammlung) : null;
                SelectedItem = vers;
                RaisePropertyChanged();
            }

        }

        public void RednerRemove(Speaker redner)
        {
            ListeAlle.Remove(redner);
            ListeFilteredItems.Remove(redner);
        }

        // RoutedEvent
        public static readonly RoutedEvent OnSpeakerChangedEvent = EventManager.RegisterRoutedEvent("OnSpeakerChanged", RoutingStrategy.Bubble,
              typeof(RoutedPropertyChangedEventHandler<Speaker>), typeof(DropDownRedner));

        public event RoutedPropertyChangedEventHandler<Speaker> OnSpeakerChanged
        {
            add => AddHandler(OnSpeakerChangedEvent, value);
            remove => RemoveHandler(OnSpeakerChangedEvent, value);
        }

        private void SpeakerChanged(Speaker oldValue, Speaker newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<Speaker>(oldValue, newValue)
                {
                    RoutedEvent = DropDownRedner.OnSpeakerChangedEvent
                };
            RaiseEvent(args);
        }

        public static readonly DependencyProperty SelectedVersammlungProperty = DependencyProperty.Register(
nameof(SelectedVersammlung),
typeof(Conregation),
typeof(DropDownRedner),
new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ConregationChanged));

        public Conregation SelectedVersammlung
        {
            get => (Conregation)GetValue(SelectedVersammlungProperty);
            set 
            { 
                SetValue(SelectedVersammlungProperty, value);
                SetFilter();
            }
        }

        private static void ConregationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;
            _ = (Conregation)e.NewValue;

            var sh = (DropDownRedner)d;
            sh.ConregationChanged();
        }

        private void ConregationChanged()
        {
            //Hier habe ich dauernd schon den korrekten wert
            //SelectedVersammlung = newValue;
            SetFilter();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty InfoProperty = DependencyProperty.Register(
nameof(Info),
typeof(object),
typeof(DropDownRedner),
new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnInfoChanged));

        public object Info
        {
            get => GetValue(InfoProperty);
            set => SetValue(InfoProperty, value);
        }

        private static void OnInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;

            var sh = (DropDownRedner)d;
            sh.OnInfoChanged(e.NewValue);
        }

        private void OnInfoChanged(object newValue)
        {
            //Hier habe ich dauernd schon den korrekten wert
            //SelectedVersammlung = newValue;
            Info = newValue;
        }
    }
}
