using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ObjectDetectionAn.DataViewModel.FrameListViewModel
{
    public class AnalyzerViewModel : ViewModel
    {
        #region Private Members
        private ImageConverter _ic = new ImageConverter();
        private VideoCapture _capture;
        private FrameGrab _fG = new FrameGrab();
        private string _fileName;
        private HistoryDataService _dataService = new HistoryDataService();
        private ImageConverter _convert = new ImageConverter();
        private ObservableCollection<int> _cameraIndex = new ObservableCollection<int>();
        private int _selectedCamera;
        private int _numCamera = -1;
        private Thread _newThread = null;
        private System.Threading.Timer _tmr;

        private DispatcherTimer _timer;
        private TimeSpan _timeS;

        private Mail _ml = new Mail("smtp.wp.pl", 465, "insert_email", "insert_password");

        private FrameModel _selected;
        private ViewModelCommand _addmedia = null;
        private ViewModelCommand _stopmedia = null;
        private bool _savetoDB = false;
        private bool _sendAlert = false;

        private bool _radioImage;
        private bool _radioVideo;
        private bool _radioCamera;
        private bool _ableSend;

        private ImageSource _imageAnalyzed;

        private string _date;
        private string _time;
        private string _focalLenght;

        private string _subscriptionKey;
        private string _endpoint;
        private string _distanceValue;
        private string _emailName;
        private string _focal;
        #endregion Private Members

public delegate void FrameSet(BitmapImage btmImage);
        public event FrameSet OnAnalyzedFrameSet;

        public delegate void FrameSave(FrameModel frameModel);
        public event FrameSave OnFrameSaved;

        public AnalyzerViewModel()
        {
            _fG.OnFrameGet += _fG_OnFrameGet;
        }

        private void _fG_OnFrameGet(FrameModel frameModel)
        {
            try
            {
                BitmapImage btm = BitmapToImageSource(frameModel.Bimage);
                BitmapImage btmOrig = BitmapToImageSource(frameModel.BimageOrig);
                OnAnalyzedFrameSet(btm);
                if (frameModel.Status)
                {
                    if (SavetoDB)
                    {
                        frameModel.Image = (byte[])_ic.ConvertTo(frameModel.Bimage, typeof(byte[]));
                        OnFrameSaved(frameModel);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "5", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<int> CameraIndex
        {
            get { return _cameraIndex; }
            set { _cameraIndex = value; NotifyPropertyChanged("CameraIndex"); }
        }

        public int SelectedCamera
        {
            get { return _selectedCamera; }
            set { _selectedCamera = value; }
        }

        public VideoCapture Capture
        {
            get { return _capture; }
            set { _capture = value; }
        }

        public FrameGrab FG
        {
            get { return _fG; }
            set { _fG = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public FrameModel Selected
        {
            get { return _selected; }
            set { _selected = value; NotifyPropertyChanged("Selected"); }
        }

        public bool RadioImage
        {
            get { return _radioImage; }
            set { _radioImage = value; NotifyPropertyChanged("RadioImage"); AddMedia.OnCanExecuteChanged(); }
        }

        public bool RadioVideo
        {
            get { return _radioVideo; }
            set { _radioVideo = value; NotifyPropertyChanged("RadioVideo"); AddMedia.OnCanExecuteChanged(); }
        }

        public bool RadioCamera
        {
            get { return _radioCamera; }
            set
            {
                _radioCamera = value;
                if (value == true)
                {
                    CameraIndex.Clear();
                    for (int i = 0; i < GetNumberCamera(); i++)
                    {
                        CameraIndex.Add(i);
                    }
                }
                NotifyPropertyChanged("RadioCamera");
                AddMedia.OnCanExecuteChanged();
            }
        }
        public ImageSource ImageAnalyzed
        {
            get { return _imageAnalyzed; }
            set { _imageAnalyzed = value; NotifyPropertyChanged("ImageAnalyzed"); }
        }

        public string Date
        {
            get { return _date; }
            set { _date = value; NotifyPropertyChanged("Date"); }
        }

        public string Time
        {
            get { return _time; }
            set { _time = value; NotifyPropertyChanged("Time"); }
        }

        public string FocalLenght
        {
            get { return _focalLenght; }
            set { _focalLenght = value; NotifyPropertyChanged("FocalLenght"); }
        }

        public string SubscriptionKey
        {
            get { return _subscriptionKey; }
            set { _subscriptionKey = value; NotifyPropertyChanged("SubscriptionKey"); }
        }

        public string Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; NotifyPropertyChanged("Endpoint"); }
        }

        public string DistanceValue
        {
            get { return _distanceValue; }
            set { _distanceValue = value; NotifyPropertyChanged("DistanceValue"); }
        }
        public string EmailName
        {
            get { return _emailName; }
            set { _emailName = value; NotifyPropertyChanged("EmailName"); }
        }

        public string Focal
        {
            get { return _focal; }
            set { _focal = value; NotifyPropertyChanged("Focal"); }
        }

        public bool SavetoDB
        {
            get { return _savetoDB; }
            set
            {
                _savetoDB = value;
                SendAlert = _savetoDB;
                AbleSend = _savetoDB;
                NotifyPropertyChanged("SavetoDB");
            }
        }

        public bool SendAlert
        {
            get { return _sendAlert; }
            set { _sendAlert = value; NotifyPropertyChanged("SendAlert"); }
        }

        public bool AbleSend
        {
            get { return _ableSend; }
            set { _ableSend = value; NotifyPropertyChanged("AbleSend"); }
        }

        #region Command
        public ViewModelCommand AddMedia
        {
            get
            {
                if (_addmedia == null)
                {
                    _addmedia = new ViewModelCommand(k => addmedia(), k => (_radioCamera || _radioImage || _radioVideo));
                }
                return _addmedia;
            }
        }

        public ViewModelCommand StopMedia
        {
            get
            {
                if (_stopmedia == null)
                {
                    _stopmedia = new ViewModelCommand(k => stopmedia(), k => _newThread != null);
                }
                return _stopmedia;
            }
        }

        private async void addmedia()
        {
            try
            {
                _fG.SubscriptionKey = SubscriptionKey;
                _fG.Endpoint = Endpoint;
                _fG.DistanceValue = Int32.Parse(DistanceValue);
                _fG.Focal = Int32.Parse(Focal);

                ImageAnalyzed = null;

                _tmr = new Timer(Send, "", 420000, 420000);
                if (RadioImage == true)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Select a picture";
                    dlg.Filter = "All supported image files(*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png;";
                    if (dlg.ShowDialog() == true)
                    {
                        _fG.FileName = dlg.FileName;
                        _fG.FileType = "i";

                        _newThread = new Thread(new ThreadStart(_fG.Run));
                        _newThread.Start();
                    }
                }
                if (RadioVideo == true)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Select a picture";
                    dlg.Filter = "All supported files(*.mp4) | *.mp4;";
                    if (dlg.ShowDialog() == true)
                    {
                        _fileName = dlg.FileName;
                        _capture = new VideoCapture(_fileName);
                        _fG.Capture = _capture;
                        _fG.FileType = "v";
                        _newThread = new Thread(new ThreadStart(_fG.Run));
                        _newThread.Start();
                    }
                }
                if (RadioCamera == true)
                {
                    _capture = new VideoCapture(SelectedCamera);
                    _fG.Capture = _capture;
                    _fG.FileType = "c";
                    _newThread = new Thread(new ThreadStart(_fG.Run));
                    _newThread.Start();
                }
                StopMedia.OnCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "X", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void stopmedia()
        {
            try
            {
                _newThread.Abort();
                _newThread = null;
                StopMedia.OnCanExecuteChanged();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion Command

        public int GetNumberCamera()
        {
            if (_numCamera == -1)
            {
                _numCamera = 0;
                while (_numCamera < 100)
                {
                    using (var vc = VideoCapture.FromCamera(_numCamera))
                    {
                        if (vc.IsOpened())
                        {
                            _numCamera += 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return _numCamera;
        }

        private void Send(object state)
        {
            DateTime _dateStop = DateTime.Now;
            DateTime _dateStart;
            StringBuilder _sb = new StringBuilder();
            int _violationCount;

            try
            {
                if (SendAlert)
                {
                    _dateStart = _dateStop.AddSeconds(-420);
                    var data = App.Ctx.FrameModels.Include("FrameObjects").Where(k => k.Dtime >= _dateStart && k.Dtime <= _dateStop).OrderBy(k => k.Dtime).ToList<FrameModel>();

                    _sb.Append("<table BORDER=\"1\">");
                    _sb.Append("<TR><TH> DATE <TH> TIME <TH> VIOLATIONS</TR>");
                    foreach (var obj in data)
                    {
                        _violationCount = obj.FrameObjects.Where(k => k.Distance < 200).Count();
                        _sb.Append("<TR>");
                        _sb.Append("<TD ALIGN=\"center\">" + obj.Dtime.ToString("dd.MM.yyyy"));
                        _sb.Append("<TD ALIGN=\"center\">" + obj.Dtime.ToString("HH:mm:ss"));
                        _sb.Append("<TD ALIGN=\"right\">" + _violationCount.ToString());
                        _sb.Append("</TR>");
                    }
                    _sb.Append("</table>");

                    _ml.SendMail("insert_email_to_send_to", EmailName, "ObjectDetection system alarm", _sb.ToString());
                }
            }
            catch (Exception ex)
            {

            }
        }

        public BitmapImage BitmapToImageSource(Bitmap bitmap)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}