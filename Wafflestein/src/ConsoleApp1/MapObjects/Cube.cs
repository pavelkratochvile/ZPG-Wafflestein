using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Block : Model
    {
        public bool isOpening = false;
        public bool isClosing = false;
        public bool isChanged = false;
        public Vector3 targetPosition = new Vector3(0, 0, 0);
        public Vector3 defaultposition = new Vector3(0, 0, 0);
        public bool isOpened = false;
        public Block(bool skipinit)
        {
            if (skipinit)
            {
                return;
            }
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
        

        public Vector3 GetNearestWall(List<Block> Walls)
        {
          

            Vector3 pos = position;
            Vector3 p1 = new Vector3(pos.X + TILE_SIZE, pos.Y, pos.Z);
            Vector3 p2 = new Vector3(pos.X - TILE_SIZE, pos.Y, pos.Z);
            Vector3 p3 = new Vector3(pos.X, pos.Y, pos.Z + TILE_SIZE);
            Vector3 p4 = new Vector3(pos.X, pos.Y, pos.Z - TILE_SIZE);

            foreach (Block wall in Walls)
            {

                if(CheckPosition(wall, p1))
                {
                    targetPosition = wall.position;
                }
                if(CheckPosition(wall, p2))
                {
                    targetPosition = wall.position;
                }
                if(CheckPosition(wall, p3))
                {
                    targetPosition = wall.position;
                }
                if(CheckPosition(wall, p4))
                {
                    targetPosition = wall.position;
                }
            }
            return new Vector3(0, 0, 0);
        }
        public bool CheckPosition(Block wall, Vector3 position)
        {
            if(wall.position.X == position.X && wall.position.Z == position.Z && !isOpened)
            {
                isOpening = true; 
                Changed = true;
                return true;
            }
            return false;
        }
    }
}
