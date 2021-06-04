namespace ObjectDetectionAn
{
    public class ObjectBox
    {
        #region Private Members
        private float _X;
        private float _Y;
        private float _H;
        private float _W;
        private double _distance;
        private string _personIndex;

        private float _sX;
        private float _sY;

        private double _sXcm;
        private double _sYcm;
        #endregion Private Attributes

        #region Constructors
        public ObjectBox() { }
        public ObjectBox(float x, float y, float h, float w, string personIndex)
        {
            _X = x;
            _Y = y;
            _H = h;
            _W = w;
            _personIndex = personIndex;

            _sX = _X + (_W / 2);
            _sY = _Y + (_H / 2);
        }
        #endregion Constructors

        #region Public Attributes
        public float X
        {
            get { return _X; }
            set { _X = value; }
        }
        public float Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        public float H
        {
            get { return _H; }
            set { _H = value; }
        }
        public float W
        {
            get { return _W; }
            set { _W = value; }
        }
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        public string PersonIndex
        {
            get { return _personIndex; }
            set { _personIndex = value; }
        }

        public double SXcm
        {
            get { return _sXcm; }
            set { _sXcm = value; }
        }

        public double SYcm
        {
            get { return _sYcm; }
            set { _sYcm = value; }
        }

        public float sX
        {
            get { return _sX; }
            set { _sX = value; }
        }
        public float sY
        {
            get { return _sY; }
            set { _sY = value; }
        }
        #endregion Public Attributes
    }
}
