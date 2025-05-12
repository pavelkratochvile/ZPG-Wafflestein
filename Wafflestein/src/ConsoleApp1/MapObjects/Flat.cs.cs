using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Flat : Model
    {
        public bool Changed = false;

        public int TILE_SIZE = 2;
        public Flat()
        {
            Vertices.Add(new Vertex(new Vector3(-1, -1, -1)));
            Vertices.Add(new Vertex(new Vector3(-1, -1, 1)));
            Vertices.Add(new Vertex(new Vector3(1, -1, 1))); 
            Triangles.Add(new Triangle(0, 1, 2));

            Vertices.Add(new Vertex(new Vector3(-1, -1, -1)));
            Vertices.Add(new Vertex(new Vector3(1, -1, 1))); 
            Vertices.Add(new Vertex(new Vector3(1, -1, -1)));
            Triangles.Add(new Triangle(3, 4, 5));
        }
    }
}
