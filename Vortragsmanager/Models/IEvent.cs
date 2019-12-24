using System;

namespace Vortragsmanager.Models
{
    public interface IEvent
    {
        DateTime Datum { get; set; }

        InvitationStatus Status { get; }

        string Anzeigetext { get; }

        Talk Vortrag { get; set; }
    }
}