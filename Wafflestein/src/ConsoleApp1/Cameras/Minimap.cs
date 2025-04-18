using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Cameras
{
    public class Minimap : Camera
    {
        public ViewPort viewPort;

        private float zoom = 5f;

        float x = 0f;
        float y = 0f;
        float z = 0f;

        float rx = (float)Math.PI / 2;
        float ry;


        public Matrix4 Projection
        {
            get
            {
                float ratio = (float)(viewPort.Width * viewPort.Control.Size.X / (viewPort.Height * viewPort.Control.Size.Y));
                Matrix4 projection = Matrix4.CreateOrthographic(zoom * 2, zoom * 2 / ratio, -10, 10);
                //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(zoom, ratio, 0.1f, 100f);
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

        public Minimap(ViewPort viewPort) : base(viewPort)
        {
            this.viewPort = viewPort;
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
        public void Move(float x, float y)
        {
            this.x -= (float)(x * Math.Cos(ry) + y * Math.Sin(ry));
            z -= (float)(x * Math.Sin(ry) - y * Math.Cos(ry));
        }
    }
}
