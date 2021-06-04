using System;
using System.Windows.Input;

namespace ObjectDetectionAn.DataViewModel
{
    public class ViewModelCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _executeAction;

        public ViewModelCommand(Action<object> executeAction,
                        Predicate<object> canExecute)
        {
            if (executeAction == null)
                throw new ArgumentNullException("executeAction");
            _executeAction = executeAction;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
