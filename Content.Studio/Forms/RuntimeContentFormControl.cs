#region Using

using Microsoft.Xna.Framework.Content;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Xna.Framework.Content.Build;
using Willcraftia.Win.Xna.Framework.Forms;

#endregion

namespace Willcraftia.Content.Studio.Forms
{
    public class RuntimeContentFormControl : GraphicsDeviceControl
    {
        public RuntimeContentViewModel RuntimeContent { get; set; }

        ContentManager contentManager;
        public ContentManager ContentManager
        {
            get
            {
                if (contentManager == null)
                {
                    contentManager = Workspace.Current.CreateContentManager(this);
                }
                return contentManager;
            }
        }

        public virtual void LoadContent() { }
        
        public virtual void UnloadContent()
        {
            if (contentManager != null)
            {
                contentManager.Unload();
            }
        }
    }
}
