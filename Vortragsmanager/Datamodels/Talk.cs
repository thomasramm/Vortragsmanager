using System;

namespace Vortragsmanager.Datamodels
{
    public class Talk
    {
        public Talk(int Number, string Title, bool Aktiv = true)
        {
            Nummer = Number;
            Thema = Title;
            Gültig = Aktiv;
        }

        public Talk(int Number, string Title, bool Valid, int LastPresented)
        {
            Nummer = Number;
            Thema = Title;
            Gültig = Valid;
            ZuletztGehalten = LastPresented;
        }

        public int Nummer { get; set; }

        public string Thema { get; set; }

        public bool Gültig { get; set; } = true;

        public int ZuletztGehalten { get; set; }

        public override string ToString() => $"({Nummer}) {Thema}";

        public string NumberTopicShort => $"{Nummer} {Thema}";

        public string NumberTopicDate => $"{Nummer} {Thema} | " + ZuletztGehaltenDatum;
    
        public string ZuletztGehaltenDatum => ((ZuletztGehalten <= 0) ? "nie gehalten" : Core.Helper.CalculateWeek(ZuletztGehalten).ToShortDateString());
    }

    public class TalkSong : IComparable<TalkSong>
    {
        public TalkSong(Talk vortrag, int? lied, int? ersatz)
        {
            Vortrag = vortrag;
            Lied = lied;
            LiedErsatz = ersatz;
        }

        public TalkSong(Talk vortrag)
        {
            Vortrag = vortrag;
        }

        public Talk Vortrag { get; set; }

        public int? Lied { get; set; }

        public int? LiedErsatz { get; set; }

        public string VortragMitNummerUndLied => Vortrag + AddSong();

        public string VortragMitLied => Vortrag.Thema + AddSong();

        public string AddSong()
        {
            var erg = string.Empty;
            if (Lied > 0)
            {
                erg += $" (♪ {Lied}";
                if (LiedErsatz > 0)
                    erg += $"/{LiedErsatz}";
                erg += ")";
            }
            else if (LiedErsatz > 0)
                erg += $" (♪ {LiedErsatz})";

            return erg;
        }

        public int CompareTo(TalkSong other)
        {
            return Vortrag.Nummer.CompareTo(other?.Vortrag.Nummer);
        }
    }
}