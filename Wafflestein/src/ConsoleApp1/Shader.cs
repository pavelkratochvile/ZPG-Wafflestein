using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Shader
    {
        public int ID { get; set; }
        private Dictionary<string, int> uniforms = new Dictionary<string, int>();

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertecSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            int vertexShader = CompileShader(vertecSource, ShaderType.VertexShader, vertexPath);
            int fragmentShader = CompileShader(fragmentSource, ShaderType.FragmentShader, fragmentPath);
            LinkShader(vertexShader, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            LoadUniforms();
        }

        private int CompileShader(string source, ShaderType type, string filePath)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int succes);
            if (succes == 0)
            {
                string log = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"Error compiling {type}  shader ({filePath}) with bad {log}:\n");
                throw new Exception($"Shader Computation failed for {filePath}");
            }
            return shader;

        }

        private void LinkShader(params int[] shaders)
        {
            ID = GL.CreateProgram();
            foreach (int shader in shaders)
            {
                GL.AttachShader(ID, shader);
            }
            GL.LinkProgram(ID);

            GL.GetProgram(ID, GetProgramParameterName.LinkStatus, out int succes);
            if (succes == 0)
            {
                string log = GL.GetProgramInfoLog(ID);
                Console.WriteLine($"Error linking program \n{log}");
                throw new Exception($"Error linking program \n{log}");
            }

        }

        public void Use()
        {
            GL.UseProgram(ID);
        }

        private void LoadUniforms()
        {
            GL.GetProgram(ID, GetProgramParameterName.ActiveUniforms, out int uniformsCount);

            for (int i = 0; i < uniformsCount; i++)
            {
                GL.GetActiveUniform(ID, i, 256, out _, out _, out _, out string name);
                int location = GL.GetUniformLocation(ID, name);

                if (location != -1)
                {
                    uniforms[name] = location;
                }
            }
        }

        public int GetUniformLocation(string name)
        {
            if (uniforms.ContainsKey(name))
            {
                return uniforms[name];
            }
            return -1;
        }

        public void SetUniform<T>(string name, T value)
        {
            int location = GetUniformLocation(name);
            if (location == -1) return;

            switch (value)
            {
                case int v:
                    GL.Uniform1(location, v);
                    break;
                case float v:
                    GL.Uniform1(location, v);
                    break;
                case OpenTK.Mathematics.Vector2 v:
                    GL.Uniform2(location, v);
                    break;
                case OpenTK.Mathematics.Vector3 v:
                    GL.Uniform3(location, v);
                    break;
                case Vector4 v:
                    GL.Uniform4(location, v);
                    break;
                case Matrix4 v:
                    GL.UniformMatrix4(location, false, ref v);
                    break;
                default:
                    throw new NotSupportedException($"Uniform is not supported");
            }
        }
    }

}
