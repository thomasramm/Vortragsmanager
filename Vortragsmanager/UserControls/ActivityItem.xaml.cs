using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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