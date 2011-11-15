#region Using

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ReferFileMessage
    {
        public bool AddExtension { get; set; }
        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; }
        public string DefaultExt { get; set; }
        public bool DereferenceLinks { get; set; }
        public string FileName { get; set; }
        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string InitialDirectory { get; set; }
        public string Title { get; set; }
        public bool ValidateNames { get; set; }

        public bool Multiselect { get; set; }
        public bool ReadOnlyChecked { get; set; }
        public bool ShowReadOnly { get; set; }

        public bool? Result { get; set; }

        public ReferFileMessage()
        {
            AddExtension = true;
            CheckFileExists = false;
            CheckPathExists = true;
            DefaultExt = string.Empty;
            DereferenceLinks = false;
            FileName = string.Empty;
            Filter = string.Empty;
            FilterIndex = 1;
            InitialDirectory = string.Empty;
            Title = string.Empty;
            ValidateNames = false;
            Multiselect = false;
            ReadOnlyChecked = false;
            ShowReadOnly = false;
        }
    }
}
