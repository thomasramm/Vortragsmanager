using System;
using System.Collections.Generic;

namespace Vortragsmanager.Models
{

    class Ältester
    {
        public string Name { get; set; }

        public Conregation Versammlung { get; set; }

        public List<Talk> Vorträge { get; set; } = new List<Talk>();
    }

}