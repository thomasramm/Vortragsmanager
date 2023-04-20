using Vortragsmanager.Enums;
using Vortragsmanager.Interface;

namespace Vortragsmanager.DataModels
{
    public class SpecialEvent : IEvent
    {
        #region Interface

        public int Kw { get; set; }

        public EventStatus Status { get; } = EventStatus.Ereignis;

        public string Anzeigetext => string.IsNullOrEmpty(Name) ? Typ.ToString() : Name;

        #endregion Interface

        public SpecialEventTyp Typ { get; set; }

        public string Name { get; set; }

        public string Thema { get; set; }

        public string Vortragender { get; set; }

        public TalkSong Vortrag { get; set; }

        public SpecialEvent Clone()
        {
            SpecialEvent s = new SpecialEvent
            {
                Typ = Typ,
                Name = Name,
                Thema = Thema,
                Vortragender = Vortragender,
                Kw = Kw,
                Vortrag = Vortrag,
            };
            return s;
        }
    }
}