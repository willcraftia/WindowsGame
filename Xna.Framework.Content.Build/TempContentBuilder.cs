#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    /// <summary>
    /// このクラスは、実行時に動的に XNA Framework コンテンツをビルドするために必要な 
    /// MSBuild 機能をラップします。メモリー内に一時 MSBuild プロジェクトを作成し、
    /// 任意で選択したコンテンツ ファイルをこのプロジェクトに追加します。
    /// 続いてプロジェクトをビルドします。これにより一時ディレクトリにコンパイル済みの .xnb 
    /// コンテンツ ファイルが作成されます。ビルドが終了した後、一般的な ContentManager 
    /// を使用して、通常どおりにこれらの一時 .xnb ファイルを読み込めます。
    /// </summary>
    public sealed class TempContentBuilder : IContentBuilder, IDisposable
    {
        #region Fields and Properties

        // コンテンツ ビルドで使用される一時ディレクトリ。
        string tempDirectory;
        string processDirectory;
        string baseDirectory;

        // 複数の ContentBuilder がある場合に、一意のディレクトリ名を生成します。
        static int directorySalt;

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public TempContentBuilder()
        {
            CreateTempDirectory();
            project = ContentProject.Create(Path.Combine(tempDirectory, "Content.contentproj"));
        }

        #endregion

        #region Temp Directories

        /// <summary>
        /// コンテンツをビルドする一時ディレクトリを作成します。
        /// </summary>
        void CreateTempDirectory()
        {
            // ディレクトリの名前の基底は次のとおりです。
            //
            //  %temp%\WinFormsContentLoading.ContentBuilder
            baseDirectory = Path.Combine(Path.GetTempPath(), GetType().FullName);

            // 同時に実行するプログラムのコピーが複数ある場合は、
            // 次のようにプロセス ID を含めます。
            //
            //  %temp%\WinFormsContentLoading.ContentBuilder\<ProcessId>
            int processId = Process.GetCurrentProcess().Id;
            processDirectory = Path.Combine(baseDirectory, processId.ToString());

            // プログラムで複数の ContentBuilder インスタンスが作成される場合は、
            // 次のようにソルト値を含めます。
            //
            //  %temp%\WinFormsContentLoading.ContentBuilder\<ProcessId>\<Salt>
            directorySalt++;
            tempDirectory = Path.Combine(processDirectory, directorySalt.ToString());

            // 一時ディレクトリを作成します。
            Directory.CreateDirectory(tempDirectory);

            PurgeStaleTempDirectories();
        }

        /// <summary>
        /// 一時ディレクトリが必要なくなった場合、それを削除します。
        /// </summary>
        void DeleteTempDirectory()
        {
            Directory.Delete(tempDirectory, true);

            // 各自の一時ディレクトリをまだ使用している ContentBuilder のインスタンスが
            // ほかにない場合は、プロセス ディレクトリも削除できます。
            if (Directory.GetDirectories(processDirectory).Length == 0)
            {
                Directory.Delete(processDirectory);

                // 各自の一時ディレクトリをまだ使用しているプログラムのコピーが
                // ほかにない場合は、基本ディレクトリも削除できます。
                if (Directory.GetDirectories(baseDirectory).Length == 0)
                {
                    Directory.Delete(baseDirectory);
                }
            }
        }

        /// <summary>
        /// 使用する必要がなくなったときに一時ディレクトリを削除できることが、理想的です。
        /// DeleteTempDirectory メソッド (Dispose またはデコンストラクターのうち最初に発生する
        /// ものによって呼び出されます) がまさしくこれを行います。問題は、これらのクリーンアップ 
        /// メソッドがまったく実行されない場合があることです。たとえば、プログラムが
        /// クラッシュしたり、デバッガーで停止された場合、削除を行う機会が得られません。
        /// 次回に起動すると、このメソッドは、以前の実行で正しくシャットダウンできなかったために
        /// 残された一時ディレクトリをすべて調べます。これにより、これら孤立したディレクトリは、
        /// 永久に散乱したまま残されることがなくなります。
        /// </summary>
        void PurgeStaleTempDirectories()
        {
            // 基底の場所のサブディレクトリをすべて調べます。
            foreach (var directory in Directory.GetDirectories(baseDirectory))
            {
                // サブディレクトリ名は、作成したプロセスの ID になります。
                int processId;

                if (int.TryParse(Path.GetFileName(directory), out processId))
                {
                    try
                    {
                        // クリエーター プロセスはまだ実行していますか。
                        Process.GetProcessById(processId);
                    }
                    catch (ArgumentException)
                    {
                        // プロセスが存在しない場合、その一時ディレクトリを削除できます。
                        Directory.Delete(directory, true);
                    }
                }
            }
        }

        #endregion

        #region IContentBuilder

        ContentProject project;
        public ContentProject Project
        {
            get { return project; }
        }

        #endregion

        #region IDisposable

        bool disposed;

        /// <summary>
        /// デコンストラクタ。
        /// </summary>
        ~TempContentBuilder()
        {
            Dispose(false);
        }

        /// <summary>
        /// 必要がなくなったときにインスタンスを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 標準の .NET IDisposable パターンを実装します。
        /// </summary>
        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                DeleteTempDirectory();
            }
        }

        #endregion
    }
}
