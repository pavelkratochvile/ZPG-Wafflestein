using ConsoleApp1.Cameras;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.MapObjects
{
    public class Model : IDisposable
    {
        public Shader Shader { get; set; }
        public Material Material { get; set; }
        public BindingList<Vertex> Vertices { get; set; }
        public BindingList<Triangle> Triangles { get; set; }
        
        public int vao;
        public int vbo;
        public int ibo;
        
        public int vboSize;
        public int iboSize;
        public bool Changed { get; set; } = true;
        
        public Vector3 position = new Vector3(0, 0, 0);
        public int TILE_SIZE = 2;   

        float startTime;
        float cutoff = 0.9763f;


        public Model()
        {
            startTime = Environment.TickCount;
            GL.GenVertexArrays(1, out vao);
            GL.BindVertexArray(vao);

            Vertices = new BindingList<Vertex>();
            vbo = GL.GenBuffer();
            vboSize = 50000;

            Triangles = new BindingList<Triangle>();
            ibo = GL.GenBuffer();
            iboSize = 50000;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vboSize * VertexGL.SizeOf(), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, iboSize * TriangleGL.SizeOf(), nint.Zero, BufferUsageHint.DynamicDraw);
        }

        public VertexGL[] GenVertexGLData()
        {
            var array = new VertexGL[Vertices.Count];

            for (var i = 0; i < Vertices.Count; i++)
            {
                var posX = Vertices[i].Position.X + position.X;
                var posY = Vertices[i].Position.Y + position.Y;
                var posZ = Vertices[i].Position.Z + position.Z;

                var pos = Vertices[i].Position;

                var norm = Vertices[i].normal;

                array[i] = new VertexGL
                {
                    position = new Vector3(pos.X, pos.Y, pos.Z),
                    normal = new Vector3(norm.X, norm.Y, norm.Z),
                };
            }
            return array;
        }

        public TriangleGL[] GenTriangleGLData()
        {
            var array = new TriangleGL[Triangles.Count];

            for (int i = 0; i < Triangles.Count; i++)
            {
                array[i] = new TriangleGL
                {
                    i1 = Triangles[i].I1,
                    i2 = Triangles[i].I2,
                    i3 = Triangles[i].I3,
                };
            }
            return array;
        }
        public Vector3 TiltVectorDown(Vector3 v, float degrees)
        {
            float radians = degrees * (MathF.PI / 180f);
            float cosA = MathF.Cos(radians);
            float sinA = MathF.Sin(radians);

            float newY = v.Y + MathF.Sin(radians);

            return new Vector3(v.X, newY, v.Z);
        }

        public void Draw(ICamera camera, Light light, bool hasTeleported, float beforeTP, float afterTP)
        {
            float elapsed = (Environment.TickCount - startTime) / 1000f;
            Matrix4 translate = Matrix4.CreateTranslation(position.X, position.Y, position.Z);

            Vector3 cameraDirection = -camera.GetDirection();
            cameraDirection.Normalize();
            cameraDirection = TiltVectorDown(cameraDirection, 2);
            Vector3 cp = -camera.GetPosition();

            Shader.Use();
            Shader.SetUniform("projection", camera.Projection);
            Shader.SetUniform("view", camera.View);
            Shader.SetUniform("model", translate);

            Shader.SetUniform("hasTeleported", hasTeleported);
            Shader.SetUniform("lightOn", light.lightOn);
            Shader.SetUniform("cameraPosWorld", cp);
            Shader.SetUniform("lightPosWorld", new Vector3(cp.X, cp.Y + 0.35f, cp.Z));
            Shader.SetUniform("lightDirWorld", cameraDirection);
            Shader.SetUniform("inerCutOff", light.inerCutOff);
            Shader.SetUniform("outerCutOff", light.outerCutOff);
            Shader.SetUniform("lightColor", light.color);
            Shader.SetUniform("lightIntensity", light.intensity);
            Shader.SetUniform("beforeTeleportTime", beforeTP);
            Shader.SetUniform("afterTeleportTime", afterTP);
            Material.SetUniforms(Shader);

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

            if (Changed)
            {
                var vertices = GenVertexGLData();
                if (vertices.Length > vboSize)
                {
                    vboSize *= 2;
                    GL.BufferData(BufferTarget.ArrayBuffer, vboSize * VertexGL.SizeOf(), nint.Zero, BufferUsageHint.DynamicDraw);
                }

                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * VertexGL.SizeOf(), vertices);

                var triangles = GenTriangleGLData();

                if (triangles.Length > iboSize)
                {
                    iboSize *= 2;
                    GL.BufferData(BufferTarget.ElementArrayBuffer, iboSize * TriangleGL.SizeOf(), nint.Zero, BufferUsageHint.DynamicDraw);
                }
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, 0, triangles.Length * TriangleGL.SizeOf(), triangles);

                Changed = false;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexGL.SizeOf(), nint.Zero);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, VertexGL.SizeOf(), Vector3.SizeInBytes);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.LineWidth(5);
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawElements(PrimitiveType.Triangles, 3 * Triangles.Count, DrawElementsType.UnsignedInt, nint.Zero);
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

                Vertices[i1].normal += normal;
                Vertices[i2].normal += normal;
                Vertices[i3].normal += normal;
            }

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].normal = Vertices[i].normal.Normalized();
            }
        }

        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                GL.DeleteBuffer(vbo);
            }
            GC.SuppressFinalize(this);
        }
        ~Model() => Dispose();
    }
}
