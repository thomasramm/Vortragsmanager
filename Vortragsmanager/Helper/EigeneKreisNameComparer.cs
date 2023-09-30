using System;
using System.Collections.Generic;
using Vortragsmanager.DataModels;

namespace Vortragsmanager.Helper
{
    public class EigeneKreisNameComparer : IComparer<Conregation>
    {
        public int Compare(Conregation x, Conregation y)
        {
            var eigene = DataContainer.MeineVersammlung;
            var eigenerKreis = eigene.Kreis;
            var value1 = (x != null && x.Kreis == eigenerKreis ? "0" : "1") + (x == eigene ? "0" : "1") + x?.Kreis + x?.Name;
            var value2 = (y != null && y.Kreis == eigenerKreis ? "0" : "1") + (y == eigene ? "0" : "1") + y?.Kreis + y?.Name;
            return string.Compare(value1, value2, StringComparison.InvariantCulture);
        }
    }
}
