using System;
using Vortragsmanager.Helper;

namespace Vortragsmanager.DataModels
{
    /// <summary>
    /// Wird genutzt für Aktivitäten in meiner Versammlung (Redner)
    /// </summary>
    public class DateWithConregation
    {
        public DateWithConregation(DateTime datum, string versammlung, int? vortrag)
        {
            Datum = datum;
            Kalenderwoche = DateCalcuation.CalculateWeek(datum);
            Versammlung = versammlung;
            Vortrag = vortrag?.ToString(Helper.Helper.German) ?? "";
        }

        public int Kalenderwoche { get; set; }

        public DateTime Datum { get; set; }

        public string Versammlung { get; set; }

        public string Vortrag { get; set; }

        public override string ToString()
        {
            return $"{Datum :dd.MM.yyyy} {Versammlung} | {Vortrag}";
        }
    }
}