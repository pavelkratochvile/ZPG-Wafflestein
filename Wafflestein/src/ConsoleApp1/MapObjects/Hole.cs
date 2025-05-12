using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Hole: Model
    {
        public int id;
        public bool Changed = false;

        public int TILE_SIZE = 2;
        public Hole(int id)
        {
            this.id = id;
        }
    }
}
