using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Timer timer;
        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer(GetPic, null, 0, 33);
        }

        private void GetPic(object state)
        {
            capture = new Capture();
            var queryframe = capture.QueryFrame();
            if (queryframe != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    queryframe.Bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    display.Source = bitmapimage;
                }
            }
        }
    }
}
