#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Willcraftia.Win.Framework.Diagnostics;

#endregion

namespace Willcraftia.Win.Framework.Controls
{
    /// <summary>
    /// Console.xaml の相互作用ロジック
    /// </summary>
    public partial class Console : UserControl
    {
        // メッセージのバッファ。
        StringBuilder stringBuilder = new StringBuilder();

        public int MaxCharacters
        {
            get { return stringBuilder.Capacity; }
            set
            {
                if (value < stringBuilder.Capacity)
                {
                    // 最大文字数が現在の設定よりも少なくなる場合は、先頭から少なくなる分の文字数を削除します。
                    int delta = stringBuilder.Capacity - value;
                    stringBuilder.Remove(0, delta);
                }
                stringBuilder.Capacity = value;
            }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public Console()
        {
            InitializeComponent();

            stringBuilder.Capacity = 3000;
        }

        /// <summary>
        /// 初期メッセージを設定します。
        /// </summary>
        /// <param name="message">メッセージ。</param>
        public void SetInitialMessage(string message)
        {
            AddMessageToTextBox(message);
        }

        /// <summary>
        /// コンソールにメッセージを出力します。
        /// </summary>
        /// <param name="message">メッセージ。</param>
        public void Write(string message)
        {
            // コンソールを管理するスレッドへディスパッチします。
            //
            // MEMO:
            //
            // 別スレッドからの書き込みもあるので、Invoke ではなく、BeginInvoke でディスパッチします。
            //
            //Dispatcher.Invoke(DispatcherPriority.Normal, new DelegateDraw(AddMessageToTextBox), message);
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DelegateDraw(AddMessageToTextBox), message);
        }

        /// <summary>
        /// TextBox にメッセージを追加します。
        /// </summary>
        /// <param name="newMessage">追加するメッセージ。</param>
        void AddMessageToTextBox(string newMessage)
        {
            var message = newMessage;

            if (stringBuilder.Capacity < stringBuilder.Length + message.Length)
            {
                if (stringBuilder.Capacity < message.Length)
                {
                    // 追加されたメッセージそのものが許容量を越える場合
                    message = message.Substring(message.Length - stringBuilder.Capacity, stringBuilder.Capacity);
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.Remove(0, message.Length);
                }
            }

            stringBuilder.Append(message);

            TextBox.Text = stringBuilder.ToString();
            TextBox.ScrollToEnd();
        }

        public void ClearMessages()
        {
            stringBuilder.Clear();
            TextBox.Text = null;
        }
    }
}
