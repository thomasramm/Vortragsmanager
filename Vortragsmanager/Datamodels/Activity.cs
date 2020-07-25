using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Datamodels
{
    public class Activity
    {
        public int Id { get; set; }

        public DateTime Datum { get; set; }

        public ActivityType Type { get; set; }

        public string Objekt { get; set; }

        public string Kommentar { get; set; }
    }

    public enum ActivityType
    {
        VortragAnfragen,
        VortragsanfrageBestätigen,
        VortragLöschen,
        VortragTauschen,
        MailSenden,
        ExterneAnfrageAblehnen,
        ExterneAnfrageBestätigen,
    }
}