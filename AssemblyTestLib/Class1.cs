using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace AssemblyTestLib
{
    public class Class1 /*: MarshalByRefObject*/
    {
        public Class1()
        {
            var v = new Vector3();
            Console.WriteLine("v=" + v);
        }
    }
}
