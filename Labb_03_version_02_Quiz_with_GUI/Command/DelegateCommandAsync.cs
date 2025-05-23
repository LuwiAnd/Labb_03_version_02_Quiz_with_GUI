using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

//using System.Windows.Input;

namespace Labb_03_version_02_Quiz_with_GUI.Command
{
    public class DelegateCommandAsync : ICommand
    {
        // execute är en Action i DelegateCommand, men jag vill
        // returnera en Task när jag tar emot asynkrona execute.
        private readonly Func<object, Task> execute; 
        private readonly Func<object?, bool> canExecute;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommandAsync(
            Func<object, Task> execute, 
            Func<object?, bool> canExecute = null
            )
        {
            ArgumentNullException.ThrowIfNull(execute);
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged() => 
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object? parameter) => 
            canExecute is null ? true : canExecute(parameter);
        public async void Execute(object? parameter)
        {
            try
            {
                await execute(parameter);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DelegateCommandAsync] Error: {ex.Message}");
            }
        }
        
    }
}
