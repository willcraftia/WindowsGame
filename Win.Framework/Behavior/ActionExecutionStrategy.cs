#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

#endregion

namespace Willcraftia.Win.Framework.Behavior
{

    /// <summary>
    /// executes a delegate
    /// </summary>
    public class ActionExecutionStrategy : IExecutionStrategy
    {

        #region IExecutionStrategy Members

        /// <summary>
        /// Gets or sets the Behavior that we execute this strategy
        /// </summary>
        public CommandBehaviorBinding Behavior { get; set; }

        /// <summary>
        /// Executes an Action delegate
        /// </summary>
        /// <param name="parameter">The parameter to pass to the Action</param>
        public void Execute(object parameter)
        {
            Behavior.Action(parameter);
        }

        #endregion
    }
}
