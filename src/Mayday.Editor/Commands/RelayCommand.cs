using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Mayday.Editor.Commands
{
    public class RelayCommand : RelayCommand<object>{
        public RelayCommand(Action<object> execute) : base(execute)
        {}

        public RelayCommand(Action execute) : base(execute)
        {}

        public RelayCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {}

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        {}
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        { }

        public RelayCommand(Action execute)
            : this(o => execute())
        { }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(o => execute(), o => canExecute())
        { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion
    }
}