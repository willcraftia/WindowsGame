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
using System.Windows.Shapes;
using System.Windows.Threading;
using Willcraftia.Win.Framework.Diagnostics;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    /// <summary>
    /// ConsoleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConsoleWindow : AvalonDock.DockableContent
    {
        // 永続化される設定。
        ConsoleWindowSettings settings = new ConsoleWindowSettings();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ConsoleWindow()
        {
            InitializeComponent();

            Console.MaxCharacters = settings.MaxCharacters;
            settings.PropertyChanged += new PropertyChangedEventHandler(OnSettingsPropertyChanged);
        }

        void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaxCharacters")
            {
                Console.MaxCharacters = settings.MaxCharacters;
            }
        }

        void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Console.ClearMessages();
        }
    }
}
