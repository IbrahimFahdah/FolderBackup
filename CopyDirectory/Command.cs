using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CopyDirectory.Commands
{
    public class Command : ICommand
    {
        public Command(Action action, Func<bool> canAction = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _canAction = canAction;
            this.action = action;
        }

        private readonly Action action;
        private readonly Func<bool> _canAction;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            action?.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            if (_canAction == null)
                return true;
            return _canAction.Invoke();
        }
    }
}
