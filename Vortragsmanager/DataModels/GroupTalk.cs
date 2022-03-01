using Vortragsmanager.Datamodels;
using Vortragsmanager.Helper;

namespace Vortragsmanager.DataModels
{
    /// <summary>
    /// Liste der Vorträge eines Redners
    /// </summary>
    public class GroupTalk
    {
        public Talk Vortrag { get; set; }

        public int AnzahlGehört { get; set; }

        //public bool InZukunft { get; set; }

        //public bool InVergangenheit { get; set; }

        public string Name => Vortrag.ToString();

        public string ZuletztGehalten
        {
            get
            {
                if (Vortrag.ZuletztGehalten == -1)
                    return "nicht gehalten";

                var datum = DateCalcuation.CalculateWeek(Vortrag.ZuletztGehalten);
                return $"{datum.ToString("dd.MM.yyyy", Helper.Helper.German)}";
            }
        }

        public override string ToString()
        {
            return $"{Vortrag.Nummer} {Vortrag.Thema} ({AnzahlGehört}*, zuletzt am {ZuletztGehalten})";
        }
    }
}