using System;

namespace Vortragsmanager.DataModels
{
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