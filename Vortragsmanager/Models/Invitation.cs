using System;

namespace Vortragsmanager.Models
{
    public class Invitation : IEvent
    {
        #region Interface

        public DateTime Datum { get; set; }

        public InvitationStatus Status { get; set; } = InvitationStatus.Zugesagt;

        public string Anzeigetext
        {
            get
            {
                if (Status == InvitationStatus.Anfrage)
                    return AnfrageVersammlung.Name;
                if (Status == InvitationStatus.Zugesagt)
                    return Ältester?.Name ?? "unbekannt";
                return "FEHLER";
            }
        }

        #endregion

        public Speaker Ältester { get; set; }

        public Talk Vortrag { get; set; }
               
        public Conregation AnfrageVersammlung { get; set; }

        public DateTime LetzteAktion { get; set; }

        public string Kommentar { get; set; }

        public override string ToString() => $"{Ältester}\n{Vortrag}";
    }

    public enum InvitationStatus
    {
        Anfrage,
        Zugesagt,
        Abgesagt,
        Ereignis
    }
}