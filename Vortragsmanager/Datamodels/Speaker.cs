﻿using System.Collections.Generic;

namespace Vortragsmanager.Datamodels
{
    public class Speaker
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Conregation Versammlung { get; set; }

        public List<TalkSong> Vorträge { get; } = new List<TalkSong>();

        public string Mail { get; set; }

        public string JwMail { get; set; }

        public string Telefon { get; set; }

        public string Mobil { get; set; }

        public bool Ältester { get; set; } = true;

        public bool Aktiv { get; set; } = true;

        public bool Einladen { get; set; } = true;

        public string InfoPrivate { get; set; }

        public string InfoPublic { get; set; }

        public override string ToString() => $"{Name}";
    }
}