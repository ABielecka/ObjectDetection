using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Windows;

namespace ObjectDetectionAn.DataViewModel.FrameListViewModel
{
    public class HistoryDataService
    {
        private List<FrameModel> _dataList = new List<FrameModel>();
        private bool _areDataLoaded = false;
        public event EventHandler DataLoaded;

        public List<FrameModel> DataList
        {
            get { return _dataList; }
            set { _dataList = value; }
        }
        public bool AreDataLoaded
        {
            get { return _areDataLoaded; }
            set { _areDataLoaded = value; }
        }

        protected void OnDataLoaded()
        {
            if (DataLoaded != null)
            {
                DataLoaded(null, EventArgs.Empty);
            }
        }

        public void LoadData(DateTime _dateStart, DateTime _dateStop)
        {
            try
            {
                _dataList.Clear();
                var data = App.Ctx.FrameModels.Include("FrameObjects").Where(k => k.Dtime >= _dateStart && k.Dtime <= _dateStop).OrderBy(k => k.Dtime).ToList<FrameModel>();
                foreach (FrameModel fr in data)
                {
                    _dataList.Add(fr);
                }
                OnDataLoaded();
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.InnerException != null) ? ex.Message + "\n\r\n\r" + ex.InnerException.Message : ex.Message, "Er", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool Save(FrameModel data)
        {
            try
            {
                App.Ctx.FrameModels.Add(data);
                App.Ctx.SaveChanges();
                _dataList.Add(data);

                return true;
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                        MessageBox.Show(error.PropertyName + "  -  " + error.ErrorMessage, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                return false;
            }

            catch (Exception ex)
            {

                MessageBox.Show((ex.InnerException != null) ? ex.Message + "\n\r\n\r" + ex.InnerException.Message : ex.Message, "Er", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
