using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Datamodels
{
    public class AufgabenZuordnung
    {
        public AufgabenZuordnung(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public string PersonName { get; set; } = "Unbekannt";

        public bool IsVorsitz { get; set; }

        public bool IsLeser { get; set; }

        public Speaker VerknüpftePerson { get; set; }
    }

    public class AufgabenKalender
    {
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

        public AufgabenZuordnung Vorsitz { get; set; }

        public AufgabenZuordnung Leser { get; set; }

        public DateTime Datum { get; set; }
    }
}
