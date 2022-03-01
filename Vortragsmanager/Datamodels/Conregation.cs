using Vortragsmanager.DataModels;

namespace Vortragsmanager.Datamodels
{
    public class Conregation
    {
        public int Id { get; set; }

        public int Kreis { get; set; }

        public string Name { get; set; }

        public string Anschrift1 { get; set; }

        public string Anschrift2 { get; set; }

        public string Anreise { get; set; }

        public int Entfernung { get; set; }

        public string Zoom { get; set; }

        public string Telefon { get; set; }

        public string Koordinator { get; set; }

        public string KoordinatorTelefon { get; set; }

        public string KoordinatorMobil { get; set; }

        public string KoordinatorMail { get; set; }

        public string KoordinatorJw { get; set; }

        public string NameMitKoordinator => $"{Name} ({Koordinator})";

        public override string ToString() => $"Versammlung {Name}";

        public Zusammenkunftszeiten Zeit { get; } = new Zusammenkunftszeiten();
    }
}