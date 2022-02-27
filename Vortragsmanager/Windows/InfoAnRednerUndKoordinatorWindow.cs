using System.Windows;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    /// <summary>
    /// Interaktionslogik für BuchungLöschenCommandDialog.xaml
    /// </summary>
    public partial class InfoAnRednerUndKoordinatorWindow : Window, ICloseable
    {
        public InfoAnRednerUndKoordinatorWindow()
        {
            InitializeComponent();
        }
    }
}