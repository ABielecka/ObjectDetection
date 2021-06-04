using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ObjectDetectionAn.Views
{
    public partial class MediaAnalyzer : UserControl
    {
        private FrameModel _data;

        public MediaAnalyzer()
        {
            InitializeComponent();
            App._oFrameAnalyze.OnAnalyzedFrameSet += _oFrameAnalyze_OnAnalyzedFrameSet;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App._oFrameAnalyze;
        }

        private void _oFrameAnalyze_OnAnalyzedFrameSet(BitmapImage btmImage)
        {
            btmImage.Freeze();
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                imgAn.Source = btmImage;
            }));
        }

        public FrameModel Data
        {
            get { return _data; }
            set { _data = value; }
        }


        //private void vidOrig_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    vidOrig.Position = TimeSpan.FromSeconds(0);
        //    vidOrig.Play();
        //}
    }
}
