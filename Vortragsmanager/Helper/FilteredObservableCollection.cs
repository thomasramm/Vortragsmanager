using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Helper
{
    public class FilteredObservableCollection<T> : ObservableCollection<T>
    {
        public void RefreshFilter(T changedobject)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, changedobject, changedobject));
        }
    }
}
