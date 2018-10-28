using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Drawing;

namespace DALClasses {
    class Photo {

        public int ID { get; set; }
        public int PhotographerID { get; set; }
        public Location LocationID { get; set; }
        public string Subject { get; set; }
        public Image image { get; set; }
    }
}
