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
    /// Executes a command 
    /// </summary>
    public class CommandExecutionStrategy : IExecutionStrategy
    {
        #region IExecutionStrategy Members
        /// <summary>
        /// Gets or sets the Behavior that we execute this strategy
        /// </summary>
        public CommandBehaviorBinding Behavior { get; set; }

        /// <summary>
        /// Executes the Command that is stored in the CommandProperty of the CommandExecution
        /// </summary>
        /// <param name="parameter">The parameter for the command</param>
        public void Execute(object parameter)
        {
            if (Behavior == null)
            {
                throw new InvalidOperationException("Behavior property cannot be null when executing a strategy");
            }

            //
            // TODO:
            //
            // 暫定的な回避コード。
            // AvalonDock で Hide -> Show した時に WPF のイベントが先に発行される状況では、
            // Behavior.Command が null になってしまう。
            //
            // 現状、TreeView.IsSelected と ViewModel の IsSelected を同期しているが、
            // IsSelected の同期が Behavior の初期化前に実行され、イベントが先に発行されている？
            //
            // IsSelected の Binding をしなければ問題は発生しない。
            //
            if (Behavior.Command != null && Behavior.Command.CanExecute(Behavior.CommandParameter))
            {
                Behavior.Command.Execute(Behavior.CommandParameter);
            }
        }

        #endregion
    }
}
