using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Models
{
    public class InGamePlayer
    {
        public string Name { get; set; }
        public bool IsPlayer { get; set; }
        public string Avatar { get; set; }
        public int CurrentPosition { get; set; }
        public double TopMap { get; set; }
        public double LeftMap { get; set; }
        public double Property { get; set; }
        public int NumberOfHotel { get; set; }
        public bool OnGame { get; set; }
        public int ListPosition { get; set; }
        public int NumberOfStation { get; set; }
        public bool IsInJail { get; set; }
    }
}
