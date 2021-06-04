using ObjectDetectionAn.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ObjectDetectionAn.DataViewModel.FrameListViewModel
{
    public class HistoryViewModel : ViewModel
    {
        #region Private Members
        private ObservableCollection<FrameModel> _dataList = new ObservableCollection<FrameModel>();
        private ObservableCollection<FrameObject> _dataListObjects = new ObservableCollection<FrameObject>();

        private HistoryDataService _dataService = new HistoryDataService();
        private ImageConverter _convert = new ImageConverter();

        private DateTime _dateStart = DateTime.Today;
        private DateTime _dateStop = DateTime.Now;
        private BitmapImage _imageConvert;

        private FrameModel _selected;
        private ViewModelCommand _search = null;
        #endregion Private Members


        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; NotifyPropertyChanged("DateStart"); Search.OnCanExecuteChanged(); }
        }

        public DateTime DateStop
        {
            get { return _dateStop; }
            set { _dateStop = value; NotifyPropertyChanged("DataStop"); Search.OnCanExecuteChanged(); }
        }

        public BitmapImage ImageConvert
        {
            get { return _imageConvert; }
            set { _imageConvert = value; NotifyPropertyChanged("ImageConvert"); }
        }

        #region Constructors
        public HistoryViewModel()
        {
            App._oFrameAnalyze.OnFrameSaved += _oFrameAnalyze_OnFrameSaved;
        }
        #endregion Constructors

        private void _oFrameAnalyze_OnFrameSaved(FrameModel frameModel)
        {
            _dataService.Save(frameModel);
        }

        public ObservableCollection<FrameModel> DataList
        {
            get { return _dataList; }
            set { _dataList = value; NotifyPropertyChanged("DataList"); }
        }
        public ObservableCollection<FrameObject> DataListObjects
        {
            get { return _dataListObjects; }
            set { _dataListObjects = value; NotifyPropertyChanged("DataListObject"); }
        }

        public HistoryDataService DataService
        {
            get { return _dataService; }
            set { _dataService = value; }
        }

        public FrameModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                ConvertToBitmapImage();
                DataListObjects.Clear();
                foreach (var obj in Selected.FrameObjects)
                {
                    DataListObjects.Add(obj);
                }
                NotifyPropertyChanged("Selected");
            }
        }

        private void ConvertToBitmapImage()
        {
            BitmapImage img = new BitmapImage();
            try
            {
                if (Selected != null && Selected.Image.Length > 0)
                {
                    img.BeginInit();
                    MemoryStream memoryStream = new MemoryStream();
                    ((System.Drawing.Image)_convert.ConvertFrom(Selected.Image)).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    img.StreamSource = memoryStream;
                    img.EndInit();
                    ImageConvert = img;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Load()
        {
            if (_dataService.AreDataLoaded)
            {
                _dataList = new ObservableCollection<FrameModel>();
                ShapeAndLoad(_dataService.DataList);
            }
            else
            {
                _dataList = new ObservableCollection<FrameModel>();
                _dataService.DataLoaded += (s, e) =>
                {
                    ShapeAndLoad(_dataService.DataList);
                };
                _dataService.LoadData(_dateStart, _dateStop);
            }
        }

        private void ShapeAndLoad(List<FrameModel> list)
        {
            var shaped = new ObservableCollection<FrameModel>();
            foreach (FrameModel obj in list)
            {
                shaped.Add(obj);
            }
            DataList = shaped;
        }

        private bool CheckDate()
        {
            if (DateStart > DateStop) return false;
            return true;
        }

        #region Command

        public ViewModelCommand Search
        {
            get
            {
                if (_search == null)
                {
                    _search = new ViewModelCommand(k => search(), k => CheckDate());
                }
                return _search;
            }
        }

        private void search()
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "XI", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion Command
    }
}