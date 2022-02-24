using Vortragsmanager.Enums;

namespace Vortragsmanager.Datamodels
{
    public class Cancelation
    {
        public Cancelation()
        {
        }

        public Cancelation(int kw, Speaker person, EventStatus status)
        {
            Kw = kw;
            Ältester = person;
            LetzterStatus = status;
        }

        public int Kw { get; set; }

        public Speaker Ältester { get; set; }

        public EventStatus LetzterStatus { get; set; }
    }
}