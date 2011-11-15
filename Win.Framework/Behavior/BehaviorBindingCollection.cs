#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

#endregion

namespace Willcraftia.Win.Framework.Behavior
{
    public class BehaviorBindingCollection : FreezableCollection<BehaviorBinding>
    {
        /// <summary>
        /// Gets or sets the Owner of the binding
        /// </summary>
        public DependencyObject Owner { get; set; }
    }
}
