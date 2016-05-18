using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ObservableObject : INotifyPropertyChanged
    {
        // Event to notify UI changes
        public event PropertyChangedEventHandler PropertyChanged;
        // Raise Event for the specific property
        protected void RaisepropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //  reference to Backing Field of the property
        // new value for the property
        // Property name (Default with CallerMemberName)
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {

            // if old value is the same no property change needed
            if (EqualityComparer<T>.Default.Equals(field, value)) 
                return false;

            field = value;
            RaisepropertyChanged(propertyName);
            return true;
        }


    }
}
