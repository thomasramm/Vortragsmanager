using System;

namespace Vortragsmanager.Models
{
    public class Outside
    {
        public Speaker Ältester { get; set; }

        public Conregation Versammlung { get; set; }

        public DateTime Datum { get; set; }

        public OutsideReason Reason { get; set; } = OutsideReason.Talk;

        public Talk Vortrag { get; set; }
    }

    public enum OutsideReason
    {
        Talk,
        NotAvailable,
    }
}