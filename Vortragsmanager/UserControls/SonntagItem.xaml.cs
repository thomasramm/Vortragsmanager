using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for SonntagItem.xaml
    /// </summary>
    public partial class SonntagItem : UserControl
    {
        private SonntagItemViewModel _model;
        public SonntagItem()
        {
            InitializeComponent();
        }

        public SonntagItem(DateTime datum)
        {
            InitializeComponent();
            var kw = Core.Helper.CalculateWeek(datum);
            _model = new SonntagItemViewModel(kw);
            DataContext = _model;
        }

        public DateTime Datum => Core.Helper.CalculateWeek(_model.Kalenderwoche);

        public AufgabenZuordnung SelectedLeser
        {
            get => _model.SelectedLeser;
            set => _model.SelectedLeser = value;
        }

        public AufgabenZuordnung SelectedVorsitz
        {
            get => _model.SelectedVorsitz;
            set => _model.SelectedVorsitz = value;
        }

        public IEnumerable<AufgabenZuordnung> Leser => _model.Leser;

        public IEnumerable<AufgabenZuordnung> Vorsitz => _model.Vorsitz;
    }
}
