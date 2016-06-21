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
    class HogUtil
    {
        public static Image<Bgr, byte> Track (Mat observedImage, float x, float y, float width, float height) {
            int iX = (int)Math.Floor(x * observedImage.Width);
            int iY = (int)Math.Floor(y * observedImage.Height);
            int iWidth = (int)Math.Ceiling(width * observedImage.Width);
            int iHeight = (int)Math.Ceiling(height * observedImage.Height);
            var rect = new Rectangle(iX, iY, iWidth, iHeight);
            var model = observedImage.ToImage<Bgr, byte>();
            model.ROI = rect;
            HOGDescriptor hog = new HOGDescriptor();
            hog.SetSVMDetector(GetVector(model));

            var regions = hog.DetectMultiScale(observedImage);

            var result = observedImage.ToImage<Bgr, byte>();

            foreach (var detected in regions)
            {
                result.Draw(detected.Rect, new Bgr(Color.Red), 1);
            }
            return result;
        }
        public static float[] GetVector(Image<Bgr, Byte> imageOfInterest)
        {
            HOGDescriptor hog = new HOGDescriptor(); 
            Point[] p = new Point[imageOfInterest.Width * imageOfInterest.Height];
            int k = 0;
            for (int i = 0; i < imageOfInterest.Width; i++)
            {
                for (int j = 0; j < imageOfInterest.Height; j++)
                {
                    Point p1 = new Point(i, j);
                    p[k++] = p1;
                }
            }

            return hog.Compute(imageOfInterest, new Size(imageOfInterest.Width, imageOfInterest.Height), new Size(0, 0), p);
        }
    }
}
