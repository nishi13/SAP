using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP.Models
{
    class Hand : TrackObject
    {
        public Rectangle Position { get; set; }
        public ColorObject Color { get; set; }
        public int number { get; set; }
        public bool IsNew { get; set; } = true;
    }
}
