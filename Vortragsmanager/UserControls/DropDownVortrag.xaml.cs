using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für DropDownVersammlung.xaml
    /// </summary>
    public partial class DropDownVortrag
    {
        public DropDownVortrag()
        {
            ListeAlle = TalkList.Get().ToList();
            foreach (var item in ListeAlle)
            {
                ListeFilteredItems.Add(item);
            }
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
  nameof(SelectedItem),
  typeof(Talk),
  typeof(DropDownVortrag),
  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnItemChanged));

        public Talk SelectedItem
        {
            get => (Talk)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sh = (DropDownVortrag)d;
            Talk oldV = (Talk)e.OldValue;
            Talk newV = (Talk)e.NewValue;
            sh.SelectedItemChanged(oldV, newV);
        }


        public List<Talk> ListeAlle { get; }

        public ObservableCollection<Talk> ListeFilteredItems { get; private set; } = new ObservableCollection<Talk>();

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
                if (string.IsNullOrEmpty(filter) || (Regex.IsMatch(ListeAlle[i].NumberTopicShort, Regex.Escape(filter), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)))
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
            get => SelectedItem?.NumberTopicShort;
            set
            {
                if (value == null)
                {

                    return;
                }
                var trenner = value.IndexOf(' ');
                if (trenner == -1)
                    trenner = value.Length;
                var nr = value.Substring(0, trenner);
                if (int.TryParse(nr, out var inr))
                {
                    var t = TalkList.Find(inr);
                    SelectedItem = t;
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }

        // RoutedEvent
        public static readonly RoutedEvent OnSelectedItemChangedEvent = EventManager.RegisterRoutedEvent("OnSelectedItemChanged", RoutingStrategy.Bubble,
              typeof(RoutedPropertyChangedEventHandler<Talk>), typeof(DropDownVortrag));

        public event RoutedPropertyChangedEventHandler<Talk> OnSelectedItemChanged
        {
            add => AddHandler(OnSelectedItemChangedEvent, value);
            remove => RemoveHandler(OnSelectedItemChangedEvent, value);
        }

        private void SelectedItemChanged(Talk oldValue, Talk newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<Talk>(oldValue, newValue)
                {
                    RoutedEvent = DropDownVortrag.OnSelectedItemChangedEvent
                };
            RaiseEvent(args);
        }
    }
}
