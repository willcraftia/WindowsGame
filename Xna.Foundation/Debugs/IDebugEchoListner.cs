using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willcraftia.Xna.Foundation.Debugs
{
    public interface IDebugEchoListner
    {
        void Echo(DebugMessageLevels messageType, string text);
    }
}
