#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// コマンド実行用情報格納用のクラス
    /// </summary>
    public sealed class DebugCommandInfo
    {
        // コマンド名
        public string Name { get; private set; }

        // コマンド詳細
        public string Description { get; private set; }

        // コマンド実行用のデリゲート
        public DebugCommandExecute Callback { get; private set; }

        public DebugCommandInfo(string name, string description, DebugCommandExecute callback)
        {
            Name = name;
            Description = description;
            Callback = callback;
        }
    }
}
