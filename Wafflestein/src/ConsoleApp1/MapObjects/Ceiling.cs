using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Ceiling : Model
    {
        private float offset = 1.001f;
        public Ceiling() 
        {
            Vertices.Add(new Vertex(new Vector3(-1, -1 * offset, -1)));
            Vertices.Add(new Vertex(new Vector3(-1, -1 * offset, 1)));
            Vertices.Add(new Vertex(new Vector3(1, -1 * offset, 1)));
            Triangles.Add(new Triangle(2, 1, 0));

            Vertices.Add(new Vertex(new Vector3(-1, -1 * offset, -1)));
            Vertices.Add(new Vertex(new Vector3(1, -1 * offset, 1)));
            Vertices.Add(new Vertex(new Vector3(1, -1 * offset, -1)));
            Triangles.Add(new Triangle(5, 4, 3));
        }
    }
}
