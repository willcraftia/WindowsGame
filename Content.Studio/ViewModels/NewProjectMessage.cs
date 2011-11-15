#region Using

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class NewProjectMessage
    {
        public bool? Result { get; set; }
        public string ProjectName { get; set; }
        public string DirectoryPath { get; set; }
    }
}
