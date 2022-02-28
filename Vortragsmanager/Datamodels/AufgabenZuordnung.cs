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

        //public int SortOrder 
        //{
        //    get
        //    {
        //        if (Id <= 0) return 2;
        //        return Häufigkeit == 1 ? 1 : 0;
        //    }
        //}

        //Wert zwischen 1 - 5
        public int Häufigkeit { get; set; } = 3;

        public int LetzterEinsatz { get; set; }

        public override string ToString()
        {
            return PersonName;
        }
    }
}
