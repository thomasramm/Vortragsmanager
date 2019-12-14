using System;

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

        public DateTime? zuletztGehalten { get; set; }

        public override string ToString() => $"({Nummer}) {Thema}";
    }
}