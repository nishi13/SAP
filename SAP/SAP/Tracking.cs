using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP
{
    class Tracking
    {
        private static int HsvThreshold = 300;
        public static Image<Gray, byte> Motion(Mat prevImage, Mat curImage)
        {
            var greyImage = curImage.ToImage<Gray, byte>();
            var image = greyImage.AbsDiff(prevImage.ToImage<Gray, byte>());
            CvInvoke.Threshold(image, image, 20, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
            return Blur(image);
        }

        public static Image<Gray, byte> Color(Mat prevImage, Mat curImage, ColorObject obj)
        {
            var image = curImage.ToImage<Hsv, byte>().InRange(obj.MinHsv, obj.MaxHsv);
            return Blur(image);
        }

        public static ColorObject GetColorObject(Mat image, Rectangle rect)
        {
            var obj = new ColorObject();
            obj.X = rect.X + rect.Width / 2;
            obj.Y = rect.Y + rect.Height / 2;

            var hsvMax = new List<Hsv>();
            var hsvMin = new List<Hsv>();
            var hsvMean = new List<Hsv>();
            var hsvCount = new List<int>();

            var hsvImage = image.ToImage<Hsv, byte>();
            for (var i = rect.Left; i < rect.Right; i++)
            {
                for (var j = rect.Top; j < rect.Bottom; j++)
                {
                    var color = hsvImage[j,i];
                    var found = false;
                    for (var n = 0; n < hsvMean.Count; n++)
                    {
                        var mean = hsvMean[n];
                        double diff = 0;
                        diff = diff + 2 * (color.Hue - mean.Hue) * (color.Hue - mean.Hue);
                        diff = diff + (color.Satuation - mean.Satuation) * (color.Satuation - mean.Satuation);
                        diff = diff + (color.Value - mean.Value) * (color.Value - mean.Value);
                        if (diff <= HsvThreshold)
                        {
                            var max = hsvMax[n];
                            max.Hue = max.Hue >= color.Hue ? max.Hue : color.Hue;
                            max.Satuation = max.Satuation >= color.Satuation ? max.Satuation : color.Satuation;
                            max.Value = max.Value >= color.Value ? max.Value : color.Value;
                            hsvMax[n] = max;

                            var min = hsvMin[n];
                            min.Hue = min.Hue <= color.Hue ? min.Hue : color.Hue;
                            min.Satuation = min.Satuation <= color.Satuation ? min.Satuation : color.Satuation;
                            min.Value = min.Value <= color.Value ? min.Value : color.Value;
                            hsvMin[n] = min;

                            var count = hsvCount[n];
                            mean.Hue = (mean.Hue * count + color.Hue) / (count + 1);
                            mean.Satuation = (mean.Satuation * count + color.Satuation) / (count + 1);
                            mean.Value = (mean.Value * count + color.Value) / (count + 1);
                            hsvMean[n] = mean;

                            hsvCount[n]++;

                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        hsvMax.Add(color);
                        hsvMin.Add(color);
                        hsvMean.Add(color);
                        hsvCount.Add(1);
                    }
                }
            }

            int index = 0;
            int total = 0;
            for (var n = 0; n < hsvCount.Count; n++)
            {
                if (total < hsvCount[n])
                {
                    index = n;
                    total = hsvCount[n];
                }
            }
            if (index < hsvCount.Count)
            {
                obj.MinHsv = hsvMin[index];
                obj.MaxHsv = hsvMax[index];
            }

            return obj;
        }

        private static Image<Gray, byte> Blur (Image<Gray, byte> image)
        {
            CvInvoke.Blur(image, image, new Size(10, 10), new Point(-1, -1));
            CvInvoke.Threshold(image, image, 25, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
            return image;
        }

        private static Image<Gray, byte> Filter(Image<Gray, byte> image)
        {
            Mat erodeElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(2, 2), new Point(-1, -1));
            Mat dialateElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(6, 6), new Point(-1, -1));
            CvInvoke.Erode(image, image, erodeElement, new Point(-1, -1), 4, BorderType.Default, new MCvScalar(1));
            CvInvoke.Dilate(image, image, dialateElement, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(1));
            return image;
        }
    }
}
