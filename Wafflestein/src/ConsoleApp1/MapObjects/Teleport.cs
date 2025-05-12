using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Teleport : Block
    {
        public int tpIndex;
        public Teleport() : base(true)
        {
            float legWidth = 0.2f;
            float yBottom = -1f;
            float yTop = 1.99f;

            Vector2[] legPositions = new Vector2[]
            {
                new Vector2(-1f,  1f),  // Levý přední
                new Vector2( 1f,  1f),  // Pravý přední
                new Vector2(-1f, -1f),  // Levý zadní
                new Vector2( 1f, -1f)   // Pravý zadní
            };

            foreach (var pos in legPositions)
            {
                AddLeg((float)pos.X, (float)pos.Y, legWidth, yBottom, yTop);
            }

            countNormals();
        }
        private void AddLeg(float centerX, float centerZ, float width, float yBottom, float yTop)
        {
            float half = width / 2f;

            // Osm rohů kvádru
            Vector3 p0 = new Vector3(centerX - half, yBottom, centerZ - half);
            Vector3 p1 = new Vector3(centerX + half, yBottom, centerZ - half);
            Vector3 p2 = new Vector3(centerX + half, yBottom, centerZ + half);
            Vector3 p3 = new Vector3(centerX - half, yBottom, centerZ + half);

            Vector3 p4 = new Vector3(centerX - half, yTop, centerZ - half);
            Vector3 p5 = new Vector3(centerX + half, yTop, centerZ - half);
            Vector3 p6 = new Vector3(centerX + half, yTop, centerZ + half);
            Vector3 p7 = new Vector3(centerX - half, yTop, centerZ + half);

            // Každý trojúhelník má své vlastní vrcholy
            AddTriangle(p0, p1, p2); // spodní
            AddTriangle(p0, p2, p3);

            AddTriangle(p4, p6, p5); // horní
            AddTriangle(p4, p7, p6);

            AddTriangle(p0, p4, p1); // přední
            AddTriangle(p1, p4, p5);

            AddTriangle(p1, p5, p2); // pravá
            AddTriangle(p2, p5, p6);

            AddTriangle(p2, p6, p3); // zadní
            AddTriangle(p3, p6, p7);

            AddTriangle(p3, p7, p0); // levá
            AddTriangle(p0, p7, p4);
        }

        private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int i = Vertices.Count;
            Vertices.Add(new Vertex(v1));
            Vertices.Add(new Vertex(v2));
            Vertices.Add(new Vertex(v3));
            Triangles.Add(new Triangle(i, i + 1, i + 2));
        }
    }
}
