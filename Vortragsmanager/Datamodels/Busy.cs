using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Datamodels
{
    public class Busy
    {
        public Busy(Speaker person, int woche)
        {
            Redner = person;
            Kw = woche;
        }

        public Speaker Redner { get; set; }
        public int Kw { get; set; }
    }
}
