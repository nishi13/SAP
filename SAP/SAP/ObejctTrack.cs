using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP
{
    class ObejctTrack
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int InitialHeight { get; set; }
        public int InitialWidth { get; set; }
        public List<object> ImageList { get; set; }
        public Color  Color { get; set; }

        public ObejctTrack (int x, int y, int h, int w, object image, Color c)
        {
            X = x;
            Y = y;
            Height = h;
            InitialHeight = h;
            Width = w;
            InitialWidth = w;
            Color = c;
            ImageList = new List<object> { image };
        }

        public void AddImage(object image, int max = 0)
        {
            ImageList.Add(image);
            if (max > 0 && ImageList.Count > max)
            {
                ImageList.RemoveAt(1);
            }
        }
    }
}
