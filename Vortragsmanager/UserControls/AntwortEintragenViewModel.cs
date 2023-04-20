using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Module;

namespace Vortragsmanager.UserControls
{
    public class AntwortEintragenViewModel : ViewModelBase
    {
        public void LoadData()
        {
            Log.Info(nameof(LoadData));
            _inquiryList = DataContainer.OffeneAnfragen;
            LoadInquiryUi();
        }

        public void LoadData(Inquiry anfrage)
        {
            Log.Info(nameof(LoadData), "Anfrage=" + anfrage?.Id);
            _inquiryList.Clear();
            _inquiryList.Add(anfrage);
            LoadInquiryUi();
        }

        private void LoadInquiryUi()
        {
            Log.Info(nameof(LoadInquiryUi));
            Anfragen.Clear();
            foreach (var a in _inquiryList)
            {
                Anfragen.Add(new Anfrage(a));
            }
        }

        private ObservableCollection<Inquiry> _inquiryList = new ObservableCollection<Inquiry>();

        public ObservableCollection<Anfrage> Anfragen { get; } = new ObservableCollection<Anfrage>();
    }
}