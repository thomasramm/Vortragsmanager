using Vortragsmanager.Enums;

namespace Vortragsmanager.DataModels
{
    public class Zusammenkunftszeit
    {
        public Zusammenkunftszeit(int jahr)
        {
            Jahr = jahr;
            Tag = Wochentag.Sonntag;
            Zeit = "10:00 Uhr";
        }

        public Zusammenkunftszeit(int jahr, Wochentag tag, string zeit)
        {
            Jahr = jahr;
            Tag = tag;
            Zeit = zeit;
        }

        public int Jahr { get; set; }

        public Wochentag Tag { get; set; }

        public string Zeit { get; set; }

        public override string ToString()
        {
            return $"{Tag} {Zeit}";
        }
    }
}