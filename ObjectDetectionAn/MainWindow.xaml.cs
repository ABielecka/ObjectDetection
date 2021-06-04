using System;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace ObjectDetectionAn
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //var ctx = App.Ctx.FrameModels.ToList<FrameModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "1", "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void tviAnalyze_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (LayoutDocument l in PaneLayoutDocument.Children.Where(p => p.Title.Equals("Analyze Media")))
            {
                l.IsActive = true;
                return;
            }

            LayoutDocument ld = new LayoutDocument();
            ld.Title = "Analyze Media";

            ld.Content = App._vAnalyze;
            PaneLayoutDocument.Children.Add(ld);
        }

        private void tviHistory_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (LayoutDocument l in PaneLayoutDocument.Children.Where(p => p.Title.Equals("History")))
            {
                l.IsActive = true;
                return;
            }

            LayoutDocument ld = new LayoutDocument();
            ld.Title = "History";

            ld.Content = App._vHistory;
            PaneLayoutDocument.Children.Add(ld);
        }
    }
}