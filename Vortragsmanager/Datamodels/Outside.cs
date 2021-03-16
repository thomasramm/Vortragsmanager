using System;
using Vortragsmanager.Core;

namespace Vortragsmanager.Datamodels
{
    public class Outside
    {
        public Speaker Ältester { get; set; }

        public Conregation Versammlung { get; set; }

        public int Kw { get; set; }

        public DateTime Datum => Helper.CalculateWeek(Kw, Versammlung);

        public OutsideReason Reason { get; set; } = OutsideReason.Talk;

        public TalkSong Vortrag { get; set; }

        private int Jahr => Kw / 100;

        private DayOfWeeks Wochentag => Zeit.Tag;

        public Core.DataHelper.Zusammenkunftszeit Zeit => Versammlung.Zeit.Get(Jahr);
    }

    public enum OutsideReason
    {
        Talk,
        NotAvailable,
    }
}