using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Datamodels
{
    public class Busy
    {
        public Busy(Speaker person, DateTime tag)
        {
            Redner = person;
            Datum = tag;
        }

        public Speaker Redner { get; set; }
        public DateTime Datum { get; set; }
    }
}
