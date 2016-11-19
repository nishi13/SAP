using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP
{
    class ColorObject
    {
        public Hsv MinHsv { get; set; }
        public Hsv MaxHsv { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
