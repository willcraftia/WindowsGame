#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace Willcraftia.Win.Framework
{
    public class DynamicViewModel<T> : DynamicObject/*, INotifyPropertyChanged*/, IDisposable where T : class
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        T model;
        public T Model
        {
            get { return model; }
        }

        public DynamicViewModel(T model)
        {
            this.model = model;
        }

        public void Dispose()
        {
            this.model = null;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = Model.GetType().GetProperty(binder.Name);
            if (property == null || !property.CanRead)
            {
                result = null;
                return false;
            }

            result = property.GetValue(Model, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = Model.GetType().GetProperty(binder.Name);
            if (property == null || !property.CanWrite)
            {
                return false;
            }

            property.SetValue(Model, value, null);
            //RaisePropertyChanged(binder.Name);
            return true;
        }

        //protected void RaisePropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
