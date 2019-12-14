using System;

namespace Vortragsmanager.Models
{
    public class Invitation
    {
        public Speaker Ältester { get; set; }

        public Talk Vortrag { get; set; }

        public DateTime Datum { get; set; }

        public InvitationStatus Status { get; set; } = InvitationStatus.Zugesagt;

        public Conregation AnfrageVersammlung { get; set; }

        public DateTime LetzteAktion { get; set; }

        public string Kommentar { get; set; }

        public override string ToString() => $"{Ältester}\n{Vortrag}";
    }

    public enum InvitationStatus
    {
        Anfrage,
        Zugesagt,
        Abgesagt
    }
}