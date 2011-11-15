#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

#endregion

namespace Willcraftia.Xna.Framework.Content
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IContent : ICloneable
    {
        ICollection<string> GetDependentAssetNames();
    }
}
