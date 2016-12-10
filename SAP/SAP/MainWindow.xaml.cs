using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SAP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Capture capture;
        private Thread thread;
        Mat firstFrame;
        public ColorObject colorObj { get; set; } = new ColorObject() { MinHsv = new Hsv(0, 0, 0), MaxHsv = new Hsv(255, 255, 255) };
        List<Hand> hands = new List<Hand>();
        List<Face> faces = new List<Face>();

        private List<Brush> colorList = new List<Brush> { Brushes.Yellow, Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Orange, Brushes.Purple };

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (var stream = new MemoryStream())
            {
                // My way to display frame 
                image.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(stream.ToArray());
                bitmap.EndInit();
                return bitmap;
            };
        }

        public MainWindow()
        {
            InitializeComponent();
            capture = new Capture();
            //capture = new Capture("DSCN3998.MOV");
            firstFrame = capture.QuerySmallFrame();
            prevImage = firstFrame.ToImage<Bgr, byte>();
            display.Source = ToBitmapSource(firstFrame);
        }

        private void processingThread()
        {
            while (true)
            {
                bool hasNext = true;
                while (hasNext)
                {
                    hasNext = GetPic();
                }

                capture = new Capture();
                //capture = new Capture("DSCN3998.MOV");
                firstFrame = capture.QueryFrame();
                prevImage = firstFrame.ToImage<Bgr, byte>();
            }
        }

        private Image<Bgr, byte> prevImage;

        private bool GetPic()
        {
            var queryframe = capture.QuerySmallFrame();
            if (queryframe != null)
            {
                try
                {
                    prevImage = queryframe.ToImage<Bgr, byte>();
                    faces = Tracking.Face(prevImage, faces);
                    hands = Tracking.Hands(prevImage, hands);
                    var image = Tracking.DrawRectangles(prevImage.Mat, faces, hands);
                    //colorObj.MinHsv = new Hsv(MinHue.Value, MinSatuation.Value, MinValue.Value);
                    //colorObj.MaxHsv = new Hsv(MaxHue.Value, MaxSatuation.Value, MaxValue.Value);
                    var tracked = Tracking.DrawRaw(prevImage.Mat);
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        display.Source = ToBitmapSource(image);
                        display2.Source = ToBitmapSource(tracked);
                    }));
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                return true;
            }
            return false;
        }

        bool isPressed = false;
        
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            thread = new Thread(new ThreadStart(processingThread));
            thread.Start();
        }
    }
}
