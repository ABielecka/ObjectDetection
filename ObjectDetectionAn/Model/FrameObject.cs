using ObjectDetectionAn.DataViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetectionAn.Model
{
    public class FrameObject : ViewModel
    {
        #region Private Members
        private long _id;
        private string _person1;
        private string _person2;
        private double _distance;
        #endregion Private Members

        #region Constructors
        public FrameObject() { }
        public FrameObject(long id, string person1, string person2, double distance)
        {
            _id = id;
            _person1 = person1;
            _person2 = person2;
            _distance = distance;
        }
        #endregion Constructors

        #region Public Attributes
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Person1
        {
            get { return _person1; }
            set { _person1 = value; }
        }

        public string Person2
        {
            get { return _person2; }
            set { _person2 = value; }
        }

        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        #endregion Public Attributes
    }
}
