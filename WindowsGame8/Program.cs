#region Using

using System;
using Willcraftia.Xna.Foundation;

#endregion

namespace WindowsGame8
{
    static class Program
    {
        /// <summary>
        /// The main entry vector for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

