using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JobViewer.Base
{
    public class NotifyPropertyChangedClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
