using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JobViewer.Base
{
    public class NotifiedClass<T> : NotifyPropertyChangedClass
    {
        private Action onCurrentChanged;

        private ObservableCollection<T> collection;
        public ObservableCollection<T> Collection
        {
            get => collection;
            set
            {
                collection = value;
                NotifyPropertyChanged(nameof(Collection));
            }
        }

        private T current;
        public T Current
        {
            get => current;
            set
            {
                current = value;
                NotifyPropertyChanged(nameof(Current));
                onCurrentChanged();
            }
        }

        public NotifiedClass(Action a)
        {
            onCurrentChanged = a;
        }
    }
}
