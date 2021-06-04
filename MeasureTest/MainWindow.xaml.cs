using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MeasureTest
{

    public partial class MainWindow : System.Windows.Window
    {
        ComputerVisionClient client;
        VideoCapture _capture;
        Mat _image = new Mat();
        string filePath = "C:\\Users\\anna_\\Desktop\\TEST\\xd4.png";
        Bitmap _btm;

        static string _subscriptionKey = "insert_key";
        static string _endpoint = "insert_endpoint";

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public MainWindow()
        {
            InitializeComponent();
            client = Authenticate(_endpoint, _subscriptionKey);

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _image.SaveImage(filePath);
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Title = "Select a picture";
            //dlg.Filter = "All supported image files(*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png;";
            //if (dlg.ShowDialog() == true)
            //{
            //    imgCam.Source = new BitmapImage(new Uri(dlg.FileName));
            //    _btm = new Bitmap(dlg.FileName);
            //}
            _capture = new VideoCapture(1);
            if (_capture.Read(_image))
            {
                _btm = _image.ToBitmap();
                imgCam.Source = BitmapToImageSource(_btm);
            }
        }
    }
}
