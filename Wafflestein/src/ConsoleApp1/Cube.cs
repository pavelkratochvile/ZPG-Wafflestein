using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Block : Model
    {
        public bool Changed = false;
        public bool isOpening = false;
        public bool isClosing = false;
        public Vector3 targetPosition = new Vector3(0, 0, 0);
        public Vector3 defaultposition = new Vector3(0, 0, 0);
        public bool isOpened = false;

        public int TILE_SIZE = 2;
        public Block()
        {
            Vertices.Add(new Vertex(new Vector3(1, -1, 1)));
            Vertices.Add(new Vertex(new Vector3(1, -1, -1)));
            Vertices.Add(new Vertex(new Vector3(1, 2, -1)));
            Triangles.Add(new Triangle(0, 1, 2));

            Vertices.Add(new Vertex(new Vector3(1, -1, 1)));
            Vertices.Add(new Vertex(new Vector3(1, 2, -1)));
            Vertices.Add(new Vertex(new Vector3(1, 2, 1)));
            Triangles.Add(new Triangle(3, 4, 5));

            Vertices.Add(new Vertex(new Vector3(1, -1, 1)));
            Vertices.Add(new Vertex(new Vector3(1, 2, 1)));
            Vertices.Add(new Vertex(new Vector3(-1, -1, 1)));
            Triangles.Add(new Triangle(6, 7, 8));

            Vertices.Add(new Vertex(new Vector3(1, 2, 1)));
            Vertices.Add(new Vertex(new Vector3(-1, -1, 1)));
            Vertices.Add(new Vertex(new Vector3(-1, 2, 1)));
            Triangles.Add(new Triangle(9, 11, 10));


            Vertices.Add(new Vertex(new Vector3(-1, -1, 1)));
            Vertices.Add(new Vertex(new Vector3(-1, 2, 1)));
            Vertices.Add(new Vertex(new Vector3(-1, -1, -1)));
            Triangles.Add(new Triangle(12, 13, 14));

            Vertices.Add(new Vertex(new Vector3(-1, 2, 1)));
            Vertices.Add(new Vertex(new Vector3(-1, 2, -1)));
            Vertices.Add(new Vertex(new Vector3(-1, -1, -1)));
            Triangles.Add(new Triangle(15, 16, 17));

            Vertices.Add(new Vertex(new Vector3(-1, -1, -1)));
            Vertices.Add(new Vertex(new Vector3(-1, 2, -1)));
            Vertices.Add(new Vertex(new Vector3(1, -1, -1)));
            Triangles.Add(new Triangle(18, 19, 20));

            Vertices.Add(new Vertex(new Vector3(-1, 2, -1)));
            Vertices.Add(new Vertex(new Vector3(1, -1, -1)));
            Vertices.Add(new Vertex(new Vector3(1, 2, -1)));
            Triangles.Add(new Triangle(21, 23, 22));
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

        public Vector3 GetNearestWall(List<Block> Walls)
        {
          

            Vector3 pos = this.position;
            Vector3 p1 = new Vector3(pos.X + TILE_SIZE, pos.Y, pos.Z);
            Vector3 p2 = new Vector3(pos.X - TILE_SIZE, pos.Y, pos.Z);
            Vector3 p3 = new Vector3(pos.X, pos.Y, pos.Z + TILE_SIZE);
            Vector3 p4 = new Vector3(pos.X, pos.Y, pos.Z - TILE_SIZE);

            foreach (Block wall in Walls)
            {

                if(CheckPosition(wall, p1))
                {
                    this.targetPosition = wall.position;
                }
                if(CheckPosition(wall, p2))
                {
                    this.targetPosition = wall.position;
                }
                if(CheckPosition(wall, p3))
                {
                    this.targetPosition = wall.position;
                }
                if(CheckPosition(wall, p4))
                {
                    this.targetPosition = wall.position;
                }
            }
            return new Vector3(0, 0, 0);
        }
        public bool CheckPosition(Block wall, Vector3 position)
        {
            if(wall.position.X == position.X && wall.position.Z == position.Z && !isOpened)
            {
                this.isOpening = true;
                return true;
            }
            return false;
        }
    }
}
