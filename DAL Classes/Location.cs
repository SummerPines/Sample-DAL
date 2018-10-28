using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_Classes {
    class Location {
        public int ID { get; set; }
        public City City { get; set; }
        public string  Description { get; set; }
    }
}
