using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Models
{
    public interface IEvent
    {
        DateTime Datum { get; set; }

        InvitationStatus Status { get; }

        string Anzeigetext { get; }

        Talk Vortrag { get; set; }
    }
}
