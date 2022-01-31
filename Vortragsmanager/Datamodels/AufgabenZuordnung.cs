using DevExpress.Mvvm;

namespace Vortragsmanager.Datamodels
{
    public class AufgabenZuordnung
    {
        public AufgabenZuordnung(int id)
        {
            Id = id;
        }

        public AufgabenZuordnung()
        {
            Id = -1;
        }

        public int Id { get; set; }

        public string PersonName { get; set; } = "Unbekannt";

        public bool IsVorsitz { get; set; }

        public bool IsLeser { get; set; }

        public Speaker VerknüpftePerson { get; set; }

        public int SortOrder 
        {
            get
            {
                if (Id <= 0) return 2;
                if (Häufigkeit == 1) return 1;
                return 0;
            }
        }

        //Wert zwischen 1 - 5
        public int Häufigkeit { get; set; } = 3;

        public int LetzterEinsatz { get; set; }

        public override string ToString()
        {
            return PersonName;
        }
    }

    public class AufgabenKalender : ViewModelBase
    {
        private AufgabenZuordnung vorsitz;
        private AufgabenZuordnung leser;

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
            get => vorsitz; 
            set
            {
                vorsitz = value;
                RaisePropertyChanged(nameof(Vorsitz));
            }
        }

        public AufgabenZuordnung Leser
        {
            get => leser;
            set
            {
                leser = value;
                RaisePropertyChanged(nameof(Leser));
            }
        }

        public int Kw { get; set; }
    }
}
