using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ObjectC : Model
    {
        public bool Changed = false;

        public int TILE_SIZE = 2;

        public ObjectC(string objFileName)
        {
            loadObj(objFileName);
            countNormals();
        }
        public void loadObj(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts[0] == "v")
                {
                    Vertices.Add(new Vertex(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture) ,
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture) 
                    )));
                }
                if (parts[0] == "f")
                {
                    int[] vertexIndices = new int[3];

                    for (int i = 0; i < 3; i++)
                    {
                        string[] indices = parts[i + 1].Split('/');
                        vertexIndices[i] = int.Parse(indices[0]) - 1; // v index, převedeno na 0-based
                    }

                    Triangles.Add(new Triangle(vertexIndices[0], vertexIndices[1], vertexIndices[2]));
                }

            }
        }
        public void SimpleNormals()
        {
            for (int i = 0; i < Triangles.Count; i += 3)
            {
                int i1 = Triangles[i].I1;
                int i2 = Triangles[i].I2;
                int i3 = Triangles[i].I3;

                Vector3 v1 = Vertices[i1].Position;
                Vector3 v2 = Vertices[i2].Position;
                Vector3 v3 = Vertices[i3].Position;

                Vector3 u = v2 - v1;
                Vector3 v = v3 - v1;

                Vector3 norm = Vector3.Cross(u, v).Normalized();

                Vertices[i1].normal += norm;
                Vertices[i2].normal += norm;
                Vertices[i3].normal += norm;
            }

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].normal.Normalize();
            }
        }
        public void countNormals()
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].normal = new Vector3(0, 0, 0);
            }

            for (int i = 0; i < Triangles.Count; i++)
            {

                int i1 = Triangles[i].I1;
                int i2 = Triangles[i].I2;
                int i3 = Triangles[i].I3;

                Vector3 v1 = Vertices[i1].Position;
                Vector3 v2 = Vertices[i2].Position;
                Vector3 v3 = Vertices[i3].Position;

                Vector3 u = v2 - v1;
                Vector3 v = v3 - v1;
                Vector3 normal = Vector3.Cross(u, v);
                normal = normal.Normalized();

                Vertices[i1].normal = normal;
                Vertices[i2].normal = normal;
                Vertices[i3].normal = normal;
            }

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].normal = Vertices[i].normal.Normalized();
            }
        }

    }
}
