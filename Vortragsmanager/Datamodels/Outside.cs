using System;
using Vortragsmanager.Core;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;

namespace Vortragsmanager.Datamodels
{
    public class Outside
    {
        public Speaker Ältester { get; set; }

        public Conregation Versammlung { get; set; }

        public int Kw { get; set; }

        public DateTime Datum => DateCalcuation.CalculateWeek(Kw, Versammlung);

        public OutsideReason Reason { get; set; } = OutsideReason.Talk;

        public TalkSong Vortrag { get; set; }

        private int Jahr => Kw / 100;

        private Wochentag Wochentag => Zeit.Tag;

        public Core.DataHelper.Zusammenkunftszeit Zeit => Versammlung.Zeit.Get(Jahr);
    }
}