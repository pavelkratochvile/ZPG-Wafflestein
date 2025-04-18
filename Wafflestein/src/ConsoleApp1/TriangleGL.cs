using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TriangleGL
    {
        public int i1, i2, i3;

        public static int SizeOf()
        {
            return Marshal.SizeOf(typeof(TriangleGL));
        }
    }
}
