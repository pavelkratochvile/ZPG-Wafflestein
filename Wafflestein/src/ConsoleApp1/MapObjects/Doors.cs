using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Doors : Block
    {
        public int TILE_SIZE = 2;
        public Doors() : base(true)
        {
            float shrink = 0.999f;

            Vertices.Add(new Vertex(new Vector3(1 * shrink, -1 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, -1 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, 2 * shrink, -1 * shrink)));
            Triangles.Add(new Triangle(0, 1, 2));

            Vertices.Add(new Vertex(new Vector3(1 * shrink, -1 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, 2 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, 2 * shrink, 1 * shrink)));
            Triangles.Add(new Triangle(3, 4, 5));

            Vertices.Add(new Vertex(new Vector3(1 * shrink, -1 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, 2 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, -1 * shrink, 1 * shrink)));
            Triangles.Add(new Triangle(6, 7, 8));

            Vertices.Add(new Vertex(new Vector3(1 * shrink, 2 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, -1 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, 2 * shrink, 1 * shrink)));
            Triangles.Add(new Triangle(9, 11, 10));

            Vertices.Add(new Vertex(new Vector3(-1 * shrink, -1 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, 2 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, -1 * shrink, -1 * shrink)));
            Triangles.Add(new Triangle(12, 13, 14));

            Vertices.Add(new Vertex(new Vector3(-1 * shrink, 2 * shrink, 1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, 2 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, -1 * shrink, -1 * shrink)));
            Triangles.Add(new Triangle(15, 16, 17));

            Vertices.Add(new Vertex(new Vector3(-1 * shrink, -1 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(-1 * shrink, 2 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, -1 * shrink, -1 * shrink)));
            Triangles.Add(new Triangle(18, 19, 20));

            Vertices.Add(new Vertex(new Vector3(-1 * shrink, 2 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, -1 * shrink, -1 * shrink)));
            Vertices.Add(new Vertex(new Vector3(1 * shrink, 2 * shrink, -1 * shrink)));
            Triangles.Add(new Triangle(21, 23, 22));
        }

    }
}
