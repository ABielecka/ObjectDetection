using ObjectDetectionAn.DataViewModel;
using ObjectDetectionAn.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ObjectDetectionAn
{
    public class FrameModel : ViewModel
    {
        #region Private Members
        private long _idFrame;
        private byte[] _image;
        private DateTime _dtime;
        private bool _status = false;
        private Bitmap _bimage;
        private Bitmap _bimageOrig;
        private IList<FrameObject> _frameObjects;
        #endregion Private Members

        #region Constructors
        public FrameModel() 
        {
            _frameObjects = new List<FrameObject>();
        }

        public FrameModel(long idFrame, byte[] image, DateTime dtime)
        {
            _idFrame = idFrame;
            _image = image;
            _dtime = dtime;
        }
        #endregion Constructors

        #region Public Attributes
        public long IdFrame
        {
            get { return _idFrame; }
            set { _idFrame = value; NotifyPropertyChanged("IdFrame"); }
        }

        public byte[] Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        public DateTime Dtime
        {
            get { return _dtime; }
            set { _dtime = value; NotifyPropertyChanged("DTime"); }
        }

        public IList<FrameObject> FrameObjects
        {
            get { return _frameObjects; }
            set { _frameObjects = value; NotifyPropertyChanged("FrameObjects"); }
        }

        [NotMapped]
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [NotMapped]
        public Bitmap Bimage
        {
            get { return _bimage; }
            set { _bimage = value; }
        }

        [NotMapped]
        public Bitmap BimageOrig
        {
            get { return _bimageOrig; }
            set { _bimageOrig = value; }
        }
        #endregion Public Attributes
    }
}
