﻿using Vortragsmanager.Helper;

namespace Vortragsmanager.DataModels
{
    public class Talk
    {
        public Talk(int number, string title, bool aktiv = true)
        {
            Nummer = number;
            Thema = title;
            Gültig = aktiv;
        }

        public Talk(int number, string title, bool valid, int lastPresented)
        {
            Nummer = number;
            Thema = title;
            Gültig = valid;
            ZuletztGehalten = lastPresented;
        }

        public int Nummer { get; set; }

        public string Thema { get; set; }

        public bool Gültig { get; set; }

        public int ZuletztGehalten { get; set; }

        public override string ToString() => $"({Nummer}) {Thema}";

        public string NumberTopicShort => $"{Nummer} {Thema}";


        // ReSharper disable once UnusedMember.Global MeinPlanRednerSuchenPAge.xaml Displaymember der DropdownBox VortragListe
        public string NumberTopicDate => $"{Nummer} {Thema} | " + ZuletztGehaltenDatum;
    
        public string ZuletztGehaltenDatum => ((ZuletztGehalten <= 0) ? "nie gehalten" : DateCalcuation.CalculateWeek(ZuletztGehalten).ToShortDateString());
    }
}