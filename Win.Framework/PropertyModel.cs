#region Using

using System.Reflection;

#endregion

namespace Willcraftia.Win.Framework
{
    public sealed class PropertyModel<T> where T : class
    {
        object owner;
        PropertyInfo property;

        public PropertyModel(object owner, string propertyName)
        {
            this.owner = owner;
            this.property = owner.GetType().GetProperty(propertyName);
        }

        public T Value
        {
            get { return property.GetValue(owner, null) as T; }
            set { property.SetValue(owner, value, null); }
        }
    }
}
