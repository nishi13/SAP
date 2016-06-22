using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP
{
    class MatchUtil
    {
        public static Image<Bgr, byte> Track(Image<Bgr, byte> observedImage, Image<Bgr, byte> model, ref Point p, out bool success)
        {
            var matches = observedImage.MatchTemplate(model, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

            double[] minValues, maxValues;
            Point[] minLocations, maxLocations;
            matches.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

            var result = observedImage;
            if (maxLocations.Length > 0)
            {
                var match = new Rectangle(maxLocations[0], model.Size);
                result.Draw(match, new Bgr(Color.Red), 1);
                p = maxLocations[0];
                success = true;
            }
            else
            {
                success = false;
            }
            return result;
        }

        public static Image<Bgr, byte> GetModel(Mat observedImage, float x, float y, float width, float height)
        {
            int iX = (int)Math.Floor(x * observedImage.Width);
            int iY = (int)Math.Floor(y * observedImage.Height);
            int iWidth = (int)Math.Ceiling(width * observedImage.Width);
            int iHeight = (int)Math.Ceiling(height * observedImage.Height);
            var rect = new Rectangle(iX, iY, iWidth, iHeight);
            var model = observedImage.ToImage<Bgr, byte>();
            model.ROI = rect;
            return model;
        }

        public static Image<Bgr, byte> GetModel(Mat observedImage, Point p, Size s)
        {
            var rect = new Rectangle(p, s);
            var model = observedImage.ToImage<Bgr, byte>();
            model.ROI = rect;
            return model;
        }
    }
}