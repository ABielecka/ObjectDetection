using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ObjectDetectionAn.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ObjectDetectionAn.DataViewModel.FrameListViewModel
{
    public class FrameGrab
    {
        private FrameModel _data;
        private VideoCapture _capture;
        private ComputerVisionClient client;
        private FrameModel _frameModel;
        private int _focal;
        private int _distanceValue;

        public Mat _image = new Mat();

        private string _fileType;
        private string _fileName;

        private string _subscriptionKey;
        private string _endpoint;

        private double real_height = 175 / 2.54; //in inch
        private double distance = 420 / 2.54; //in inch
        private double focalLenght = 633.6; //in pix

        private List<VisualFeatureTypes> _features = new List<VisualFeatureTypes>() { VisualFeatureTypes.Objects };

        public FrameModel Data { get { return _data; } set { _data = value; } }
        public VideoCapture Capture { get { return _capture; } set { _capture = value; } }
        public ComputerVisionClient Client { get { return client; } set { client = value; } }
        public string FileType { set { _fileType = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public string SubscriptionKey { get { return _subscriptionKey; } set { _subscriptionKey = value; } }
        public string Endpoint { get { return _endpoint; } set { _endpoint = value; } }
        public int DistanceValue { get { return _distanceValue; } set { _distanceValue = value; } }
        public int Focal { get { return _focal; } set { _focal = value; } }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public delegate void FrameGet(FrameModel frameProperties);
        public event FrameGet OnFrameGet;

        public void Run()
        {
            try
            {
                client = Authenticate(Endpoint, SubscriptionKey);
                _frameModel = new FrameModel();

                if (_fileType.Equals("i"))
                {
                    ImageCapture();
                }
                else if (_fileType.Equals("v"))
                {
                    VideoCapture();
                }
                else if (_fileType.Equals("c"))
                {
                    while (true)
                    {
                        CameraCapture();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "1", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImageCapture()
        {
            try
            {
                _frameModel.Dtime = DateTime.Now;
                AnalyzeImageFile(_fileName).Wait();
                OnFrameGet(_frameModel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "2", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void VideoCapture()
        {
            try
            {
                double frameCount = _capture.Get(CaptureProperty.FrameCount);

                for (int i = 0; i < frameCount; i += 40)
                {
                    _capture.Set(CaptureProperty.PosFrames, i);
                    //_capture.Get(CaptureProperty.PosMsec);        //returns how many seconds have elapsed since the start of the video
                    if (_capture.Read(_image))
                    {
                        _frameModel = new FrameModel();
                        _frameModel.Dtime = DateTime.Now;
                        AnalyzeImageStream(_image).Wait();
                        Thread.Sleep(6000);
                        OnFrameGet(_frameModel);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "3", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CameraCapture()
        {
            try
            {
                if (_capture.Read(_image))
                {
                    _frameModel = new FrameModel();
                    _frameModel.Dtime = DateTime.Now;
                    AnalyzeImageStream(_image).Wait();
                    OnFrameGet(_frameModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "4", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Font drawFont = new Font("Arial", 12, System.Drawing.FontStyle.Bold);
        public SolidBrush drawBrush = new SolidBrush(Color.WhiteSmoke);
        public SolidBrush drawBrushAlarm = new SolidBrush(Color.Red);
        public StringFormat drawFormat = new StringFormat();
        public System.Drawing.Pen linePen = new System.Drawing.Pen(Color.Cyan, 3);
        public System.Drawing.Pen linePenAlarm = new System.Drawing.Pen(Color.Red, 3);

        private async Task AnalyzeImageStream(Mat image)
        {
            int indexO = 1;
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            List<ObjectBox> _frameBoxes = new List<ObjectBox>();
            Bitmap _frameBitmap;

            _frameBitmap = image.ToBitmap();
            _frameModel.BimageOrig = _frameBitmap;
            ImageAnalysis result = await client.AnalyzeImageInStreamAsync(image.ToMemoryStream(), _features);

            foreach (var obj in result.Objects.Where(p => p.Confidence > 0.5 && p.ObjectProperty.Equals("person")))
            {
                ObjectBox _frameBox = new ObjectBox(obj.Rectangle.X, obj.Rectangle.Y, obj.Rectangle.H, obj.Rectangle.W, "PR" + indexO.ToString());
                _frameBoxes.Add(_frameBox);
                indexO++;
            }
            AnalyzeImage(_frameBitmap, _frameBoxes);
        }

        public async Task AnalyzeImageFile(string fileName)
        {
            int indexO = 1;
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            List<ObjectBox> _objectBoxes = new List<ObjectBox>();
            Bitmap _frameBitmap;

            _frameBitmap = new Bitmap(fileName);
            _frameModel.BimageOrig = _frameBitmap;

            using (System.IO.Stream analyzeImageStream = File.OpenRead(fileName))
            {
                int start, stop;
                start = Environment.TickCount & Int32.MaxValue;
                ImageAnalysis result = await client.AnalyzeImageInStreamAsync(analyzeImageStream, _features);
                stop = Environment.TickCount & Int32.MaxValue;
                int czas_pracy = stop - start;
                
                Console.WriteLine("Czas pracy: " + (czas_pracy));

                
                foreach (var obj in result.Objects.Where(p => p.Confidence > 0.5 && p.ObjectProperty.Equals("person")))
                {
                    ObjectBox _objectBox = new ObjectBox(obj.Rectangle.X, obj.Rectangle.Y, obj.Rectangle.H, obj.Rectangle.W, "PR" + indexO.ToString());
                    _objectBoxes.Add(_objectBox);
                    indexO++;
                }
            }
            AnalyzeImage(_frameBitmap, _objectBoxes);
        }

        private void AnalyzeImage(Bitmap frameBitmap, List<ObjectBox> frameBoxes)
        {
            List<ObjectBox> objectBoxesSupp;
            FrameObject frameObject;
            double distance;

            try
            {
                using (Graphics g = Graphics.FromImage(frameBitmap))
                {
                    foreach (var obj in frameBoxes)
                    {
                        g.DrawRectangle(linePen, obj.X, obj.Y, obj.W, obj.H);
                        String drawString = obj.PersonIndex;
                        g.DrawString(drawString, drawFont, drawBrush, obj.X - 5, obj.Y - 5);
                    }
                }
                _frameModel.Bimage = frameBitmap;

                foreach (var obj in frameBoxes)
                {
                    obj.Distance = CalculateDistanceFromCamera(obj);
                    obj.SXcm = (obj.sX * obj.Distance) / focalLenght;
                    obj.SYcm = (obj.sY * obj.Distance) / focalLenght;
                }

                if (frameBoxes.Count > 1)
                {
                    objectBoxesSupp = new List<ObjectBox>();
                    foreach (var obj in frameBoxes)
                    {
                        objectBoxesSupp.Add(obj);
                    }

                    foreach (var obj1 in frameBoxes)
                    {
                        objectBoxesSupp.Remove(obj1);

                        foreach (var obj2 in objectBoxesSupp)
                        {
                            distance = Math.Sqrt(Math.Pow(obj1.SXcm - obj2.SXcm, 2) + Math.Pow(obj1.SYcm - obj2.SYcm, 2) + Math.Pow(obj1.Distance - obj2.Distance, 2));
                            frameObject = new FrameObject();
                            frameObject.Person1 = obj1.PersonIndex;
                            frameObject.Person2 = obj2.PersonIndex;
                            frameObject.Distance = distance;
                            _frameModel.FrameObjects.Add(frameObject);

                            if (distance < DistanceValue)
                            {
                                _frameModel.Status = true;
                                using (Graphics g = Graphics.FromImage(frameBitmap))
                                {
                                    g.DrawRectangle(linePenAlarm, obj1.X, obj1.Y, obj1.W, obj1.H);
                                    g.DrawRectangle(linePenAlarm, obj2.X, obj2.Y, obj2.W, obj2.H);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "5", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public double DetermineFocalLenght(ObjectBox _detectedFrame)
        {
            double focalLenght;

            focalLenght = (_detectedFrame.H * (distance)) / (real_height);
            return focalLenght; //in pix
        }

        public double CalculateDistanceFromCamera(ObjectBox _detectedFrame)
        {
            double distanceNew = 0;
            if (Focal != 0)
            {
                distanceNew = ((real_height) * Focal) / _detectedFrame.H;
            }
            distanceNew *= 2.54;
            return distanceNew; //in cm???
        }

    }
}