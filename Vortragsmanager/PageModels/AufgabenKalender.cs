using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.PageModels
{
    public class AufgabenKalender : ViewModelBase
    {
        private AufgabenZuordnung _vorsitz;
        private AufgabenZuordnung _leser;

        public AufgabenKalender(int kw)
        {
            Kw = kw;
        }

        public AufgabenKalender(int kw, AufgabenZuordnung vorsitz, AufgabenZuordnung leser)
        {
            Kw = kw;
            Vorsitz = vorsitz;
            Leser = leser;
        }

        public AufgabenZuordnung Vorsitz
        {
            get => _vorsitz; 
            set
            {
                _vorsitz = value;
                RaisePropertyChanged(nameof(Vorsitz));
            }
        }

        public AufgabenZuordnung Leser
        {
            get => _leser;
            set
            {
                _leser = value;
                RaisePropertyChanged(nameof(Leser));
            }
        }

        public int Kw { get; set; }
    }
}