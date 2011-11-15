#region Using

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework.Content;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Build;
using Willcraftia.Content.Studio.PluginFramework;
using Willcraftia.Content.Studio.Models;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class WorkspaceViewModel : ViewModelBase
    {
        public Workspace Model
        {
            get { return Workspace.Current; }
        }

        public ServiceContainer Services
        {
            get { return Model.Services; }
        }

        public ObservableCollection<ContentFolderViewModel> RootFolders { get; private set; }

        ContentFolderViewModel currentFolder;
        public ContentFolderViewModel CurrentFolder
        {
            get { return currentFolder; }
            private set
            {
                if (currentFolder == value) return;

                currentFolder = value;
                RaisePropertyChanged("CurrentFolder");
            }
        }

        string title = "Content Studio";
        public string Title
        {
            get { return title; }
            set
            {
                if (title == value) return;

                title = value;
                RaisePropertyChanged("Title");
            }
        }

        public string ProjectFilePath
        {
            get
            {
                if (Model.Project == null) return null;
                return Model.Project.FullPath;
            }
        }

        public ICommand EditPluginSettingsCommand { get; private set; }
        public ICommand CreateProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand CloseProjectCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand SaveProjectAsCommand { get; private set; }
        public ICommand BuildCommand { get; private set; }
        public ICommand EditProjectPropertiesCommand { get; private set; }
        public ICommand SelectFolderCommand { get; private set; }
        public ICommand RunGameCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public WorkspaceViewModel()
            : base(new Messenger())
        {
            RootFolders = new ObservableCollection<ContentFolderViewModel>();

            InitializeCommands();
        }

        void InitializeCommands()
        {
            EditPluginSettingsCommand = new RelayCommand(EditPluginSettings);
            CreateProjectCommand = new RelayCommand(CreateProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            CloseProjectCommand = new RelayCommand(CloseProjectAfterSaveIfNeeded, IsProjectOpened);
            SaveProjectCommand = new RelayCommand(SaveProject, IsProjectDirty);
            SaveProjectAsCommand = new RelayCommand(SaveProjectAs, IsProjectOpened);
            BuildCommand = new RelayCommand(Build, IsProjectOpened);
            EditProjectPropertiesCommand = new RelayCommand(EditProjectProperties, IsProjectOpened);
            SelectFolderCommand = new RelayCommand<object>(SelectFolder, p => IsProjectOpened());
            RunGameCommand = new RelayCommand(RunGame, IsProjectOpened);
        }

        public void OpenProject(string path)
        {

            // プロジェクトが既に開かれているならば、それを閉じます。
            // 同一プロジェクトを選択した場合にはロード状態の衝突が発生するため、
            // 選択したプロジェクトを開く前に既存のプロジェクトを閉じます。
            CloseProject();

            // プロジェクトを開きます。
            Model.OpenProject(path);

            CurrentFolder = new ContentFolderViewModel(Model.RootFolder, null, Messenger);
            RootFolders.Add(CurrentFolder);

            // 展開状態に設定します。
            CurrentFolder.IsExpanded = true;
            // 選択状態に設定します。
            CurrentFolder.IsSelected = true;

            // タイトルに選択したプロジェクト ファイル名を設定します。
            Title = Path.GetFileName(Model.Project.FullPath) + " - Content Studio";
        }

        public void CloseProjectAfterSaveIfNeeded()
        {
            PromptSaveProject();
            CloseProject();
        }

        bool IsProjectOpened()
        {
            return Model.Project != null;
        }

        bool IsProjectDirty()
        {
            return Model.Project != null && Model.Project.IsDirty;
        }

        void EditPluginSettings()
        {
            var messageContent = new PluginSettingsEditViewModel(Model.CreatePluginSettingsEdit());
            var message = new EditPluginSettingsMessage(messageContent);
            Messenger.Send(message);
        }

        void CreateProject()
        {
            // プロジェクトが既に開かれていて、変更があるならば、保存を促します。
            PromptSaveProject();

            var message = new NewProjectMessage();

            Messenger.Send(message);
            if (message.Result != true) return;

            var fileName = message.ProjectName + ".contentproj";
            var path = Path.Combine(message.DirectoryPath, fileName);

            // プロジェクトが既に開かれているならば、それを閉じます。
            CloseProject();

            Model.NewProject(path);

            CurrentFolder = new ContentFolderViewModel(Model.RootFolder, null, Messenger);
            RootFolders.Add(CurrentFolder);

            // 展開状態に設定します。
            CurrentFolder.IsExpanded = true;
            // 選択状態に設定します。
            CurrentFolder.IsSelected = true;

            // タイトルに選択したプロジェクト ファイル名を設定します。
            Title = Path.GetFileName(Model.Project.FullPath) + " - Content Studio";
        }

        void OpenProject()
        {
            // プロジェクトが既に開かれていて、変更があるならば、保存を促します。
            PromptSaveProject();

            var openFileDialogMessage = new ReferFileMessage();
            openFileDialogMessage.Title = Properties.Resources.TitleOpenProject;
            openFileDialogMessage.DefaultExt = ".contentproj";
            openFileDialogMessage.Filter = "Content projects (*.contentproj)|*.contentproj|All files (*.*)|*.*";

            // プロジェクトが既に開かれているならば、そのディレクトリを設定します。
            if (Model.Project != null)
            {
                openFileDialogMessage.InitialDirectory = Model.Project.DirectoryPath;
            }

            Messenger.Send(openFileDialogMessage);
            if (openFileDialogMessage.Result != true) return;

            OpenProject(openFileDialogMessage.FileName);
        }

        void PromptSaveProject()
        {
            if (Model.Project == null || !Model.Project.IsDirty) return;

            // プロジェクトが既に開かれていて、変更があるならば、保存を促します。
            var confirmationText = string.Format(Properties.Resources.ConfirmSaveProjectFile);
            if (ConfirmWithMessageBox(confirmationText))
            {
                Model.SaveProject();
            }
        }

        /// <summary>
        /// 現在開かれているプロジェクトを閉じます。
        /// </summary>
        void CloseProject()
        {
            if (Model.Project == null) return;

            Model.CloseProject();

            RootFolders.Clear();
            CurrentFolder = null;

            // タイトルをリセットします。
            Title = "Content Studio";

            // 開いているドキュメントを全て閉じるためにメッセージを送信します。
            Messenger.Send(CloseProjectMessage.Instance);
        }

        void SaveProject()
        {
            if (Model.Project == null) return;
            Model.SaveProject();
        }

        void SaveProjectAs()
        {
            if (Model.Project == null) return;

            var openFileDialogMessage = new ReferFileMessage();
            openFileDialogMessage.Title = Properties.Resources.TitleOpenProject;
            openFileDialogMessage.DefaultExt = ".contentproj";
            openFileDialogMessage.Filter = "Content projects (*.contentproj)|*.contentproj|All files (*.*)|*.*";
            openFileDialogMessage.InitialDirectory = Model.Project.DirectoryPath;
            openFileDialogMessage.FileName = Path.GetFileName(Model.Project.FullPath);

            Messenger.Send(openFileDialogMessage);
            if (openFileDialogMessage.Result != true) return;

            var path = openFileDialogMessage.FileName;

            if (File.Exists(path))
            {
                var confirmationText = string.Format(Properties.Resources.ConfirmOverrideFile, path);
                if (!ConfirmWithMessageBox(confirmationText)) return;
            }

            Model.SaveProjectAs(path);

            // タイトルに新しいプロジェクト ファイル名を設定します。
            Title = Path.GetFileName(Model.Project.FullPath) + " - Content Studio";
        }

        void Build()
        {
            if (Model.Project == null) return;

            // プロジェクトに変更があるならば、保存を促します。
            PromptSaveProject();

            //Cursor = Cursors.Wait;
            Model.Project.BuildAsync();
        }

        void EditProjectProperties()
        {
            if (Model.Project == null) return;

            var messageContent = new ProjectPropertiesEditViewModel(new ProjectPropertiesEdit(Model.Project));
            var message = new EditProjectPropertiesMessage(messageContent);

            Messenger.Send(message);
        }

        void SelectFolder(object folder)
        {
            if (CurrentFolder == folder) return;

            //
            // TODO:
            // ExplorerWindows.Hide() の際、folder パラメータが TreeViewItem になります。
            //
            CurrentFolder = folder as ContentFolderViewModel;
        }

        bool ConfirmWithMessageBox(string text)
        {
            bool confirmed = false;
            var message = DialogMessageHelper.CreateConfirmDialogBoxMessage(
                text,
                result => confirmed = result == MessageBoxResult.Yes);
            message.Caption = Properties.Resources.TitleConfirmation;

            Messenger.Send(message);
            return confirmed;
        }

        void RunGame()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                //var type = pluginManager.GameRegistry.GameTypes[0];
                //var game = Activator.CreateInstance(type) as Microsoft.Xna.Framework.Game;
                //Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //game.Run();

                //var setup = new AppDomainSetup();
                //setup.ApplicationBase = @"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\WindowsGame8\bin\x86\Debug";
                //setup.ApplicationName = "WindowsGame8";
                //setup.PrivateBinPath = @"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\WindowsGame8\bin\x86\Debug";
                //setup.ShadowCopyFiles = "false";

                //var evidence = new System.Security.Policy.Evidence(AppDomain.CurrentDomain.Evidence);

                //var gameDomain = AppDomain.CreateDomain("GameDomain", evidence, setup);
                //System.IO.Directory.SetCurrentDirectory(@"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\WindowsGame8\bin\x86\Debug");
                //gameDomain.ExecuteAssembly(@"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\WindowsGame8\bin\x86\Debug\WindowsGame8.exe");
            });
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);
    }
}
