using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SbTranslationHelper
{
    /// <summary>
    /// Generic command
    /// </summary>
    public class RelayCommand<T> : ObservableObject, ICommand
    {
        private Action<T> _Execute = null;
        private Func<T, bool> _CanExecute = null;

        /// <summary>
        /// Create a new command
        /// </summary>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Create a new command
        /// </summary>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _Execute = execute;
            _CanExecute = canExecute;
        }

        /// <summary>
        /// Raise the CanExecuteChanged event
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public virtual void Execute(object parameter)
        {
            _Execute((T)parameter);
        }

        /// <summary>
        /// Indicates if the command can be executed
        /// </summary>
        public virtual bool CanExecute(object parameter)
        {
            return _CanExecute != null ? _CanExecute((T)parameter) : true;
        }

        /// <summary>
        /// Event raised when the CanExecute state changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_CanExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_CanExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
    }
}
