using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP.Models
{
    class Face : TrackObject
    {
        public Rectangle Position { get; set; }
        public ColorObject Color { get; set; }
    }
}
