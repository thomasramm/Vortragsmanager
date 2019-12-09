using System;
using System.Collections.Generic;

namespace Vortragsmanager.Models
{

    public class Speaker
    {
        public string Name { get; set; }

        public Conregation Versammlung { get; set; }

        public List<Talk> Vorträge { get; } = new List<Talk>();

        public string Mail { get; set; }

        public string Telefon { get; set; }

        public string Mobil { get; set; }

        public bool Ältester { get; set; } = true;

        public bool Aktiv { get; set; } = true;

        public string InfoPrivate { get; set; }

        public string InfoPublic { get; set; }

        public override string ToString() => $"{Name}";

    }

}