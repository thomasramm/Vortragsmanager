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

namespace Vortragsmanager.UserControls
{
    /// <summary>
    /// Interaktionslogik für HamburgerMenu.xaml
    /// </summary>
    public partial class HamburgerMenu : UserControl
    {
        public HamburgerMenu()
        {
            InitializeComponent();
        }

        private MeineVerwaltung.ListCreateViewModel ListModel
        {
            get
            {
                return ListModel ?? new MeineVerwaltung.ListCreateViewModel();
            }
        }

        private void HamburgerSubMenuAushang_Click(object sender, RoutedEventArgs e)
        {
            ListModel.CreateAushang();
        }

        private void HamburgerSubMenuRednerliste_Click(object sender, RoutedEventArgs e)
        {
            ListModel.CreateExchangeRednerList();
        }

        private void HamburgerSubMenuKontaktliste_Click(object sender, RoutedEventArgs e)
        {
            ListModel.CreateContactList();
        }

        private void HamburgerSubMenuVortragsliste_Click(object sender, RoutedEventArgs e)
        {
            ListModel.CreateOverviewTalkCount();
        }

        private void HamburgerSubMenuKontaktdatenAlle_Click(object sender, RoutedEventArgs e)
        {
            ListModel.CreateSpeakerOverview();
        }
    }
}
