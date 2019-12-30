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
using System.Windows.Shapes;

namespace Vortragsmanager.Views
{
    /// <summary>
    /// Interaction logic for AntwortEintragenView.xaml
    /// </summary>
    public partial class AntwortEintragenView : UserControl
    {
        public AntwortEintragenView()
        {
            InitializeComponent();

            var data = (AntwortEintragenViewModel)AntwortEintragen.DataContext;
            data.LoadData();
        }
    }
}
