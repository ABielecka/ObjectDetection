using System.Windows;
using System.Windows.Controls;

namespace ObjectDetectionAn.Views
{
    public partial class History : UserControl
    {
        public History()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App._oFrameHistory;
        }
    }
}
