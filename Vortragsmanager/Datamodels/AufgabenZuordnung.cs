using DevExpress.Mvvm;
using System;

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

        public int Id { get; private set; }

        public string PersonName { get; set; } = "Unbekannt";

        public bool IsVorsitz { get; set; }

        public bool IsLeser { get; set; }

        public Speaker VerknüpftePerson { get; set; }

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

        public AufgabenKalender(DateTime datum)
        {
            Datum = datum;
        }

        public AufgabenKalender(DateTime datum, AufgabenZuordnung vorsitz, AufgabenZuordnung leser)
        {
            Datum = datum;
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

        public DateTime Datum { get; set; }
    }
}
