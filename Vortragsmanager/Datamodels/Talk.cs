using Vortragsmanager.Helper;

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
    
        public string ZuletztGehaltenDatum => ((ZuletztGehalten <= 0) ? "nie gehalten" : DateCalcuation.CalculateWeek(ZuletztGehalten).ToShortDateString());
    }
}