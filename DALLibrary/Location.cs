using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALLibrary {
    public class Location {
        public int LocationID { get; set; }
        public City City { get; set; }
        public string Description { get; set; }
    }
}
