using System;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;

namespace Vortragsmanager.DataModels
{
    public class Invitation : IEvent
    {
        #region Interface

        public int Kw { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Zugesagt;

        public string Anzeigetext
        {
            get
            {
                if (Status == EventStatus.Anfrage)
                    return AnfrageVersammlung.Name;
                if (Status == EventStatus.Zugesagt)
                    return Ältester?.Name ?? "unbekannt";
                return "FEHLER";
            }
        }

        #endregion Interface

        public Speaker Ältester { get; set; }

        public TalkSong Vortrag { get; set; }

        public Conregation AnfrageVersammlung { get; set; }

        public DateTime LetzteAktion { get; set; }

        public bool ErinnerungsMailGesendet { get; set; }

        public string Kommentar { get; set; }

        public override string ToString() => $"{Ältester}\n{Vortrag}";
    }
}