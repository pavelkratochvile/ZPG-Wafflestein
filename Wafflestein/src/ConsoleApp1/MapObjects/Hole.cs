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
        public bool isOpening = false;
        public bool isClosing = false;
        public Vector3 targetPosition = new Vector3(0, 0, 0);
        public Vector3 defaultposition = new Vector3(0, 0, 0);
        public bool isOpened = false;

        public Hole(int id)
        {
            this.id = id;   

            float outerMin = -1f;
            float outerMax = 1f;
            float innerMin = -0.8f;
            float innerMax = 0.8f;
            float y = -1f;

            // Spodní pás
            Vertices.Add(new Vertex(new Vector3(outerMin, y, outerMin))); // 0
            Vertices.Add(new Vertex(new Vector3(outerMax, y, outerMin))); // 1
            Vertices.Add(new Vertex(new Vector3(innerMax, y, innerMin))); // 2
            Vertices.Add(new Vertex(new Vector3(innerMin, y, innerMin))); // 3
            Triangles.Add(new Triangle(0, 1, 2));
            Triangles.Add(new Triangle(0, 2, 3));

            // Horní pás
            Vertices.Add(new Vertex(new Vector3(outerMin, y, outerMax))); // 4
            Vertices.Add(new Vertex(new Vector3(outerMax, y, outerMax))); // 5
            Vertices.Add(new Vertex(new Vector3(innerMax, y, innerMax))); // 6
            Vertices.Add(new Vertex(new Vector3(innerMin, y, innerMax))); // 7
            Triangles.Add(new Triangle(7, 6, 5));
            Triangles.Add(new Triangle(7, 5, 4));

            // Levý pás
            Vertices.Add(new Vertex(new Vector3(outerMin, y, outerMin))); // 8
            Vertices.Add(new Vertex(new Vector3(outerMin, y, outerMax))); // 9
            Vertices.Add(new Vertex(new Vector3(innerMin, y, innerMax))); // 10
            Vertices.Add(new Vertex(new Vector3(innerMin, y, innerMin))); // 11
            Triangles.Add(new Triangle(8, 9, 10));
            Triangles.Add(new Triangle(8, 10, 11));

            // Pravý pás
            Vertices.Add(new Vertex(new Vector3(outerMax, y, outerMin))); // 12
            Vertices.Add(new Vertex(new Vector3(outerMax, y, outerMax))); // 13
            Vertices.Add(new Vertex(new Vector3(innerMax, y, innerMax))); // 14
            Vertices.Add(new Vertex(new Vector3(innerMax, y, innerMin))); // 15
            Triangles.Add(new Triangle(14, 13, 12));
            Triangles.Add(new Triangle(14, 12, 15));
        }
    }
}
