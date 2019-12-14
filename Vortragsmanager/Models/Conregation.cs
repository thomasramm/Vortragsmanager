﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Vortragsmanager.Models
{
    public class Conregation
    {
        public int Id { get; set; }

        public int Kreis { get; set; }

        public string Name { get; set; }

        public string Anschrift1 { get; set; }

        public string Anschrift2 { get; set; }

        public string Anreise { get; set; }

        public string Telefon { get; set; }

        public string Koordinator { get; set; }

        public string KoordinatorTelefon { get; set; }

        public string KoordinatorMobil { get; set; }

        public string KoordinatorMail { get; set; }

        public string KoordinatorJw { get; set; }

        public string GetZusammenkunftszeit(int Jahr)
        {
            var letztesJahr = Zusammenkunftszeiten.Where(x => x.Key <= Jahr).Max(y => y.Key);
            return letztesJahr == null ? "unbekannt" : Zusammenkunftszeiten[letztesJahr];
        }
        public string GetZusammenkunftszeit(DateTime Datum)
        {
            return GetZusammenkunftszeit(Datum.Year);
        }
        public void SetZusammenkunftszeit(int Jahr, string Zeit)
        {
            if (Zusammenkunftszeiten.ContainsKey(Jahr))
                Zusammenkunftszeiten[Jahr] = Zeit;
            else
                Zusammenkunftszeiten.Add(Jahr, Zeit);
        }

        public override string ToString() => $"Versammlung {Name}";

        public readonly Dictionary<int, string> Zusammenkunftszeiten = new Dictionary<int, string>(1);
    }
}
