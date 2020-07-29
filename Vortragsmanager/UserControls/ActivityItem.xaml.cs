using System.Windows.Controls;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaction logic for ActivityItem.xaml
    /// </summary>
    public partial class ActivityItem : UserControl
    {
        private Activity _log;

        public ActivityItem()
        {
            InitializeComponent();
        }

        public ActivityItem(Activity log) : this()
        {
            _log = log;
            DataContext = _log;
        }

        //public int Id => _log.Id;

        //public DateTime Datum => _log.Datum;

        public string VersammlungName => _log.Versammlung.NameMitKoordinator;

        public ActivityType Typ => _log.Typ;

        public bool Aktiv
        {
            get
            {
                return _log.Sichtbar;
            }
            set
            {
                _log.Sichtbar = value;
            }
        }
    }
}