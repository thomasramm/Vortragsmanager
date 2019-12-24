using System;

namespace Vortragsmanager.Models
{
    public class SpecialEvent : IEvent
    {
        #region Interface

        public DateTime Datum { get; set; }

        public InvitationStatus Status { get; } = InvitationStatus.Ereignis;

        public string Anzeigetext
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? Typ.ToString() : Name;
            }
        }

        #endregion Interface

        public EventTyp Typ { get; set; }

        public string Name { get; set; }

        public string Thema { get; set; }

        public string Vortragender { get; set; }

        public Talk Vortrag { get; set; }

        public SpecialEvent Clone()
        {
            SpecialEvent s = new SpecialEvent
            {
                Typ = Typ,
                Name = Name,
                Thema = Thema,
                Vortragender = Vortragender,
                Datum = Datum
            };
            return s;
        }
    }

    public enum EventTyp
    {
        Dienstwoche = 1,
        RegionalerKongress = 2,
        Kreiskongress = 3,
        Streaming = 4,
        Sonstiges = 5,
    }
}