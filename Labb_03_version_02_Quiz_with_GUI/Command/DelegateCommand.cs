using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Labb_03_version_02_Quiz_with_GUI.Command
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object?, bool> canExecute;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<object> execute, Func<object?, bool> canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object? parameter) => canExecute is null ? true : canExecute(parameter);
        /* Ovanstående är samma som nedanstående.
        public bool CanExecute(object? parameter)
        {
            return canExecute is null ? true : canExecute(parameter);
        }
        */

        public void Execute(object? parameter) => execute(parameter);
        /* Ovanstående är samma som nedanstående.
        public void Execute(object? parameter) 
        {
            execute(parameter);
        }
        */
    }
}
