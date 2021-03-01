using System;

namespace Vortragsmanager.Datamodels
{
    public class Outside
    {
        public Speaker Ältester { get; set; }

        public Conregation Versammlung { get; set; }

        public int Kw { get; set; }

        public DateTime Datum => Core.Helper.CalculateWeek(Kw);

        public OutsideReason Reason { get; set; } = OutsideReason.Talk;

        public TalkSong Vortrag { get; set; }
    }

    public enum OutsideReason
    {
        Talk,
        NotAvailable,
    }
}