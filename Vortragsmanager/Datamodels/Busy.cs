namespace Vortragsmanager.DataModels
{
    public class Busy
    {
        public Busy(Speaker person, int woche)
        {
            Redner = person;
            Kw = woche;
        }

        public Speaker Redner { get; set; }

        public int Kw { get; set; }
    }
}
