using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace fileChanger.ViewModels.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var handlers = PropertyChanged;
            if (handlers is null) return;
            var invocation_list = handlers.GetInvocationList();
            var arg = new PropertyChangedEventArgs(propertyName);
            foreach(var action in  invocation_list)
            {
                if (action.Target is DispatcherObject target)
                    target.Dispatcher.Invoke(action, this, arg);
                else
                    action.DynamicInvoke(this, arg);
            }
        }
        protected virtual bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if(oldValue?.Equals(newValue) == true) return false;
            oldValue = newValue;
            OnPropertyChanged(propertyName); return true;
        }
    }
}
