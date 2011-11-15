#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ProjectPropertiesEditViewModel : ViewModelBase
    {
        readonly ProjectPropertiesEdit model;

        public ObservableCollection<ReferenceEditViewModel> References { get; private set; }
        public ObservableCollection<ProjectReferenceEditViewModel> ProjectReferences { get; private set; }

        public ICommand AddReferenceCommand { get; private set; }
        public ICommand AddProjectReferenceCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public ProjectPropertiesEditViewModel(ProjectPropertiesEdit model)
            : base(new Messenger())
        {
            this.model = model;

            References = new ObservableCollection<ReferenceEditViewModel>();
            foreach (var edit in model.References)
            {
                References.Add(new ReferenceEditViewModel(this, edit));
            }

            ProjectReferences = new ObservableCollection<ProjectReferenceEditViewModel>();
            foreach (var edit in model.ProjectReferences)
            {
                ProjectReferences.Add(new ProjectReferenceEditViewModel(this, edit));
            }

            InitializeCommands();
        }

        void InitializeCommands()
        {
            AddReferenceCommand = new RelayCommand(AddReference);
            AddProjectReferenceCommand = new RelayCommand(AddProjectReference);

            SaveCommand = new RelayCommand(Save);
        }

        public void DeleteReference(ReferenceEditViewModel reference)
        {
            References.Remove(reference);
            model.References.Remove(reference.Model);
        }

        public void DeleteProjectReference(ProjectReferenceEditViewModel projectReference)
        {
            ProjectReferences.Remove(projectReference);
            model.ProjectReferences.Remove(projectReference.Model);
        }

        void AddReference()
        {
            var reference = new ReferenceEditViewModel(this, new ReferenceEdit());
            var message = new EditReferenceMessage(reference);
            Messenger.Send(message);
            if (message.Result != true) return;

            model.References.Add(reference.Model);
            References.Add(reference);
        }

        void AddProjectReference()
        {
            var projectReference = new ProjectReferenceEditViewModel(this, new ProjectReferenceEdit());
            var message = new EditProjectReferenceMessage(projectReference);
            Messenger.Send(message);
            if (message.Result != true) return;

            model.ProjectReferences.Add(projectReference.Model);
            ProjectReferences.Add(projectReference);
        }

        void Save()
        {
            model.Save();

            Messenger.Send(new CloseWindowMessage(this));
        }
    }
}
