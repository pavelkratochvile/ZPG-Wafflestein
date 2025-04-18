using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.Cameras
{
    public class Camera: ICamera
    {
        public ViewPort viewPort;

        private float zoom = 1f;
        public int[][] map { get; set; }

        public float x;
        public float y;
        public float z;
 
        public float rx;
        public float ry;

        public Matrix4 Projection
        {
            get
            {
                float ratio = (float)(viewPort.Width * viewPort.Control.Size.X / (viewPort.Height * viewPort.Control.Size.Y));
                //Matrix4 projection = Matrix4.CreateOrthographic(zoom * 2, zoom * 2 / ratio, -10, 10);
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(zoom, ratio, 0.1f, 100f);
                return projection;
            }
        }

        public Matrix4 View
        {
            get
            {
                Matrix4 view;
                view = Matrix4.Identity;
                view *= Matrix4.CreateTranslation(x, y, z);
                view *= Matrix4.CreateRotationY(ry);
                view *= Matrix4.CreateRotationX(rx);

                return view;
            }
        }
        public Vector3 GetPosition()
        {
            return new Vector3(x, y, z);
        }

        public Camera(ViewPort viewPort)
        {
            this.viewPort = viewPort;
        }

        public void Zoom(float coef)
        {
            //zoom *= coef;

            zoom += coef / 10f;
            zoom = Math.Max(0.2f, Math.Min(2, zoom));
        }

        public void Move(float x, float y, List<Block> Walls, List<Block> Doors)
        {
            bool isMoveX = true;
            bool isMoveZ = true;
            
            List<int> walls_indices = GetCubesInRadius(Walls, 2.1f);
            List<int> doors_indices = GetCubesInRadius(Doors, 2.1f);

            float dx = (float)(x * Math.Cos(ry) + y * Math.Sin(ry));
            float dz = (float)(x * Math.Sin(ry) - y * Math.Cos(ry));

            if (!IsMoveXok(Walls, walls_indices, dx) || !IsMoveXok(Doors, doors_indices,dx))
            {
                isMoveX = false;
            }
            if (!IsMoveZok(Walls, walls_indices, dz) || !IsMoveZok(Doors,doors_indices,dz))
            {
                isMoveZ = false;
            }


            if (isMoveZ && isMoveX)
            {
                this.x -= dx;
                z -= dz;
                return;
            }

            if (isMoveZ)
            {
                z -= dz;
            }

            if (isMoveX)
            {
                this.x -= dx;
            }

        }
        public Vector3 GetDirection()
        {
            float cosPitch = MathF.Cos(rx);
            float sinPitch = MathF.Sin(rx);
            float cosYaw = MathF.Cos(ry);
            float sinYaw = MathF.Sin(ry);

            Vector3 direction = new Vector3(
                (cosPitch * sinYaw),
                -sinPitch,
                -cosPitch * cosYaw
            );

            direction.Normalize();
            return direction;
        }
        
        public Vector3 RotateAroundY(Vector3 vector, float angleDegrees)
        {
            float angleRadians = MathF.PI * angleDegrees / 180f;
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);

            Vector3 result = new Vector3
            {
                X = vector.X * cos + vector.Z * sin,
                Y = vector.Y,
                Z = -vector.X * sin + vector.Z * cos
            };

            return Vector3.Normalize(result);
        }

        private bool IsMoveXok(List<Block> Walls, List<int> indices, float movex)
        {
            float offset = 0.2f;
            foreach (int i in indices) 
            {
                Block cube = Walls[i];
                float max_x = (float)cube.position.X + cube.TILE_SIZE / 2 + offset;
                float min_x = (float)cube.position.X - cube.TILE_SIZE / 2 - offset;
                float max_z = (float)cube.position.Z + cube.TILE_SIZE / 2 + offset;
                float min_z = (float)cube.position.Z - cube.TILE_SIZE / 2 - offset;
                
                float posx = -x;
                float posz = -z;

                if (posz > min_z && posz < max_z)
                {   
                    if((posx + movex >= min_x && posx + movex <= max_x))
                    {
                        return false;
                    }
                }
                
            }
            if (IsOusideX(movex))
            {
                return false;
            }
            return true;
        }
        private bool IsMoveZok(List<Block> Walls, List<int> indices, float movez)
        {
            float offset = 0.2f;
            foreach (int i in indices)
            {
                Block cube = Walls[i];
                float max_x = (float)cube.position.X + cube.TILE_SIZE / 2 + offset;
                float min_x = (float)cube.position.X - cube.TILE_SIZE / 2 - offset;
                float max_z = (float)cube.position.Z + cube.TILE_SIZE / 2 + offset;
                float min_z = (float)cube.position.Z - cube.TILE_SIZE / 2 - offset;

                float posx = -x;
                float posz = -z;

                if (posx > min_x && posx < max_x)
                {
                    if (posz + movez >= min_z && posz + movez <= max_z)
                    {
                        return false;
                    }
                }
            }
            if (IsOusideZ(movez))
            {
                return false;
            }
            return true;
        }

        private bool IsOusideZ(float movez)
        {
            if(-z + movez < -(map[0].Length * 2 - 1))
            {
                return true;

            }
            else if (-z + movez > 1)
            {
                return true;
            }
                return false;
        }
        private bool IsOusideX(float movex)
        {
            if (-x + movex > map.Length * 2 - 1)
            {
                return true;

            }
            else if (-x + movex < -1)
            {
                return true;
            }
                return false;
        }

        private int[] GetNormalVector(int cubeIndex, List<Block> Walls)
        {
            int[] vector = new int[2];
            float posx = -x;
            float posz = -z;

            Block cube = Walls[cubeIndex];
            int max_x = (int)cube.position.X + cube.TILE_SIZE / 2;
            int min_x = (int)cube.position.X - cube.TILE_SIZE / 2;
            int max_z = (int)cube.position.Z + cube.TILE_SIZE / 2;
            int min_z = (int)cube.position.Z - cube.TILE_SIZE / 2;

            if (posx < max_x && posx > min_x && posz < min_z) { return new int[] { 0, -1 }; }
            if (posx < max_x && posx > min_x && posz > max_z) { return new int[] { 0, 1 }; }
            if (posz < max_z && posz > min_z && posx < min_x) { return new int[] { -1, 0 }; }
            if (posz < max_z && posz > min_z && posx > max_x) { return new int[] { 1, 0 }; }

            // Nejsme před kostkou
            return new int[] { -1, -1 };
        }
        private List<int> GetCubesInRadius(List<Block> Walls, float radius)
        {
            List<int> indices = new List<int>();
            float posx = -x;
            float posz = -z;

            for (int i = 0; i < Walls.Count; i++)
            {
                Block block = Walls[i];
                float distance = (float)Math.Sqrt((block.position.X - posx) * (block.position.X - posx) +
                                                  (block.position.Z - posz) * (block.position.Z - posz));
                if (distance <= radius)
                {
                    indices.Add(i);
                }
            }
            return indices;
        }

        public void RotateX(float a)
        {
            rx += a;
            rx = (float)Math.Max(-Math.PI / 2, Math.Min(Math.PI / 2, rx));
        }
        public void RotateY(float a)
        {
            ry += a;
        }
    }
}
