using ObjectDetectionAn.DataViewModel.FrameListViewModel;
using ObjectDetectionAn.Model;
using ObjectDetectionAn.Views;
using System.Windows;

namespace ObjectDetectionAn
{
    public partial class App : Application
    {
        private static Context _context;

        public static string _conString = "insert_info";

        public static AnalyzerViewModel _oFrameAnalyze = new AnalyzerViewModel();
        public static HistoryViewModel _oFrameHistory = new HistoryViewModel();
        public static MediaAnalyzer _vAnalyze = new MediaAnalyzer();
        public static History _vHistory = new History();

        public static Context Ctx
        {
            get
            {
                if (_context == null)
                {
                    _context = new Context();
                }
                return _context;
            }
        }
    }
}
