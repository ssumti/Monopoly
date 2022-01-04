using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Models
{
    public class MapTiles
    {
        public string Type { get; set; }
        public RealEstate realEstate { get; set; }
        public Tax tax { get; set; }
        public Chance chance { get; set; }
        public string Owner { get; set; }
        public int OwnerPos { get; set; }
    }
}
