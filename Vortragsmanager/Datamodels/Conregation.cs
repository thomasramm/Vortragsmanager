using System;

namespace Vortragsmanager.DataModels
{
    public class Conregation
    {
        private string name;
        private string anschrift1;
        private string anschrift2;
        private string anreise;
        private int entfernung;
        private string zoom;
        private string telefon;
        private string koordinator;
        private string koordinatorTelefon;
        private string koordinatorMobil;
        private string koordinatorMail;
        private string koordinatorJw;

        public int Id { get; set; }

        public int Kreis { get; set; }

        public string Name 
        { 
            get => name;
            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;
                Aktualisierung = DateTime.Now;
            }
        }

        public string Anschrift1 { get => anschrift1; set => anschrift1 = value; }

        public string Anschrift2 { get => anschrift2; set => anschrift2 = value; }

        public string Anreise { get => anreise; set => anreise = value; }

        public int Entfernung { get => entfernung; set => entfernung = value; }

        public string Zoom { get => zoom; set => zoom = value; }

        public string Telefon { get => telefon; set => telefon = value; }

        public string Koordinator { get => koordinator; set => koordinator = value; }

        public string KoordinatorTelefon { get => koordinatorTelefon; set => koordinatorTelefon = value; }

        public string KoordinatorMobil { get => koordinatorMobil; set => koordinatorMobil = value; }

        public string KoordinatorMail { get => koordinatorMail; set => koordinatorMail = value; }

        public string KoordinatorJw { get => koordinatorJw; set => koordinatorJw = value; }

        public DateTime Aktualisierung { get; set; }

        public string NameMitKoordinator => $"{Name} ({Koordinator})";

        public override string ToString() => $"Versammlung {Name}";

        public Zusammenkunftszeiten Zeit { get; } = new Zusammenkunftszeiten();
    }
}