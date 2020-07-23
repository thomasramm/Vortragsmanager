using System;

namespace Vortragsmanager.Datamodels
{
    public class Outside
    {
        public Speaker Ältester { get; set; }

        public Conregation Versammlung { get; set; }

        public DateTime Datum { get; set; }

        public OutsideReason Reason { get; set; } = OutsideReason.Talk;

        public TalkSong Vortrag { get; set; }
    }

    public enum OutsideReason
    {
        Talk,
        NotAvailable,
    }
}