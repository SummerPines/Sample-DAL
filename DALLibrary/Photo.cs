using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Drawing;

namespace DALLibrary {
    public class Photo {

        public int PhotoID { get; set; }
        public int PhotographerID { get; set; }
        public int LocationID { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public Image PhotoImage { get; set; }
    }
}
