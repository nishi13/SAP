using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
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
using System.Windows.Media;
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
        private Image<Bgr, byte> model;
        private Thread thread;
        Point startingPoint;
        Point endingPoint;
        Point startDisplay;
        Point endDisplay;
        Mat firstFrame;
        System.Drawing.Point modelPoint;

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
            capture = new Capture("DSCN3998.MOV");
            firstFrame = capture.QuerySmallFrame();
            display.Source = ToBitmapSource(firstFrame);
        }

        private void GetPic()
        {
            var queryframe = capture.QuerySmallFrame();
            if (queryframe != null)
            {
                try
                {
                    bool success;
                    var detected = MatchUtil.Track(queryframe.ToImage<Bgr, byte>(), model, ref modelPoint, out success);
                    if (success)
                    {
                        model = MatchUtil.GetModel(queryframe, modelPoint, model.Size);
                    }
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        display.Source = ToBitmapSource(detected);
                    }));
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                GetPic();
            }
        }

        bool isPressed = false;

        private void display_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startingPoint = e.GetPosition(Grid);
                startDisplay = e.GetPosition(display);
                isPressed = true;
                if (thread != null)
                {
                    thread.Abort();
                    thread = null;
                }
            }

        }

        private void display_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isPressed = false;
            rectangle.Width = 0;
            rectangle.Height = 0;
            float x = (float)((float)startDisplay.X < endDisplay.X ? startDisplay.X : endDisplay.X) / (float)display.ActualWidth;
            float y = (float)((float)startDisplay.Y < endDisplay.Y ? startDisplay.Y : endDisplay.Y) / (float)display.ActualHeight;
            float width = ((float)Math.Abs(startDisplay.X - endDisplay.X)) / (float)display.ActualWidth;
            float height = ((float)Math.Abs(startDisplay.Y - endDisplay.Y)) / (float)display.ActualHeight;
            model = MatchUtil.GetModel(firstFrame, x, y, width, height);
            thread = new Thread(new ThreadStart(GetPic));
            thread.Start();
        }

        private void display_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPressed)
            {
                endingPoint = e.GetPosition(Grid);
                endDisplay = e.GetPosition(display);
                var x = startingPoint.X < endingPoint.X ? startingPoint.X : endingPoint.X;
                var y = startingPoint.Y < endingPoint.Y ? startingPoint.Y : endingPoint.Y;
                rectangle.Width = Math.Abs(startingPoint.X - endingPoint.X);
                rectangle.Height = Math.Abs(startingPoint.Y - endingPoint.Y);
                rectangle.Margin = new Thickness(x, y, 0, 0);
            }
        }
    }
}
