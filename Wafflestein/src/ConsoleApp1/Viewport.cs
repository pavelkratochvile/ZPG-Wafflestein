using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ViewPort
    {
        public double Top { get; set; }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public MyGameWindow Control { get; set; }

        public void Set()
        {
            GL.Viewport(
                (int)(Left * Control.Size.X),
                (int)((1 - Top - Height) * Control.Size.Y),
                (int)(Width * Control.Size.X),
                (int)(Height * Control.Size.Y)
                );
        }

        public void Clear()
        {
            GL.Enable(EnableCap.ScissorTest);
            GL.Scissor((int)(Left * Control.Size.X),
                (int)((1 - Top - Height) * Control.Size.Y),
                (int)(Width * Control.Size.X),
                (int)(Height * Control.Size.Y)
                );
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public Vector2 WindowViewport(int x, int y)
        {
            return new Vector2(
                ((double)x / Control.Size.X - Left) / Width * 2 - 1,
                -(((double)y / Control.Size.Y - Top) / Height * 2 - 1)
                );
        }

    }
}
