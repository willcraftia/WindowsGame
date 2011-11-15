using System;
using System.Collections.Generic;
using System.Text;

namespace Willcraftia.Xna.Foundation.Debugs
{
    public delegate void DebugCommandExecute(IDebugCommandHost host, string name, IList<string> arguments);

    public interface IDebugCommandHost : IDebugEchoListner, IDebugCommandExecutor
    {
        /// <summary>
        /// デバッグ コマンドを登録します。
        /// </summary>
        /// <param name="name">コマンド文字列。</param>
        /// <param name="description">コマンドの説明。</param>
        /// <param name="callback">コマンドの入力に対するコールバック。</param>
        void RegisterDebugCommand(string name, string description, DebugCommandExecute callback);

        /// <summary>
        /// デバッグ コマンドの登録を解除します。
        /// </summary>
        /// <param name="name">コマンド文字列。</param>
        void UnregisterDebugCommand(string name);

        /// <summary>
        /// 文字列をコンソールに表示します。
        /// </summary>
        /// <param name="text"></param>
        void Echo(string text);

        /// <summary>
        /// 文字列を警告メッセージとしてコンソールに表示します。
        /// </summary>
        /// <param name="text"></param>
        void EchoWarning(string text);

        /// <summary>
        /// 文字列をエラー メッセージとしてコンソールに表示します。
        /// </summary>
        /// <param name="text"></param>
        void EchoError(string text);

        void RegisterDebugEchoListner(IDebugEchoListner listner);
        void UnregisterDebugEchoListner(IDebugEchoListner listner);

        void PushDebugCommandExecutor(IDebugCommandExecutor executioner);
        void PopDebugCommandExecutor();
    }
}
