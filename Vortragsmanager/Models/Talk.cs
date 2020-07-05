﻿using System;

namespace Vortragsmanager.Models
{
    public class Talk
    {
        public Talk(int Number, string Title)
        {
            Nummer = Number;
            Thema = Title;
        }

        public int Nummer { get; set; }

        public string Thema { get; set; }

        public bool Gültig { get; set; } = true;

        public DateTime? ZuletztGehalten { get; set; }

        public override string ToString() => $"({Nummer}) {Thema}";

        public string NumberTopicShort => $"{Nummer} {Thema}";

        public string NumberTopicDate => $"{Nummer} {Thema} | " + ZuletztGehalten?.ToShortDateString() ?? "nie gehalten";
    }

    public class TalkSong
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
    }
}