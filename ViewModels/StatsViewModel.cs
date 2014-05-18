using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class StatsViewModel
    {
        public Edition Edition { get; set; }
        public bool isParticipating { get; set; }
        public bool hasPaid { get; set; }

    }
}
