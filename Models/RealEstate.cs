using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Models
{
    public class RealEstate
    {
        public string Name { get; set; }
        public double BuyPrice { get; set; }
        public ObservableCollection<double> GuestRented { get; set; }
        public int Level { get; set; }
    }
}
