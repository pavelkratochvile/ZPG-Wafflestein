using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using ConsoleApp1.Cameras;
using FreeTypeSharp;
using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    public class MyGameWindow : GameWindow
    {
        private List<ObjectC> Objects = new List<ObjectC>();
        private List<Block> Walls = new List<Block>();
        private List<Block> Doors = new List<Block>();
        private List<Block> DoorsHitboxes = new List<Block>();
        private List<Flat> Floor = new List<Flat>();
        Light light = new Light(new Vector3(10, 10, 10), false);

        private float speed = 5f;
        private float doorspeed = 1f;
        private double[] movingVector = new double[] { 0, 0 };
        private string mapFilename;
        private ViewPort Viewport { get; set; }
        private double deltaTime = 0.0;
        private Camera Camera { get; set; }
        private Map Map = new Map();

        private Stopwatch stopwatch = new Stopwatch();
        private int frameCount = 0;
        private double lastTime = 0;
        private double fps = 0;



        public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, string mapFilename) : base(gameWindowSettings, nativeWindowSettings)
        { 
            this.mapFilename = mapFilename;
            Viewport = new ViewPort()
            {
                Top = 0,
                Left = 0,
                Width = 1,
                Height = 1,
                Control = this
            }
            ;
            Camera = new Camera(Viewport);
            this.CursorState = CursorState.Grabbed;
            MouseWheel += OnMouseWheel;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float deltaTime = (float)args.Time;
            
            CountFPS();
            MakeCurrent();
            GetMovingVector();
            turnLight();
            MakeMove(deltaTime);
            CheckDoors(deltaTime);
            IsExiting();
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawMap();
            this.SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            Viewport.Set();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Shader shader = new Shader("Shaders/Basic.vert", "Shaders/Basic.frag");

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.ClearDepth(1.0f);
            
            stopwatch.Start();

            Map.map = Map.MapTranformation(mapFilename);
            Camera.map = Map.map;
            GenerateMap(shader);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            float sensitivity = 0.002f;
            float deltaX = e.Delta.X * sensitivity;
            float deltaY = e.Delta.Y * sensitivity;

            Camera.RotateY(deltaX);
            Camera.RotateX(deltaY);
        }

        private void IsExiting()
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }
        
        public void CountFPS()
        {
            frameCount++;
            double currentTime = stopwatch.Elapsed.TotalSeconds;

            if(currentTime - lastTime >= 1)
            {
                fps = frameCount / (currentTime - lastTime);
                frameCount = 0;
                lastTime = currentTime;
            }
        }
        private void OnMouseWheel(MouseWheelEventArgs e)
        {
            Camera.Zoom(-e.OffsetY / 5);
        }

        private void turnLight()
        {
            if(MouseState.IsButtonDown(MouseButton.Left))
            {
                light.lightOn = 1;
            }
            else if (MouseState.IsButtonDown(MouseButton.Right))
            {
                light.lightOn = 0;
            }
        }

        private void MakeMove(float deltaTime)
        {
            float movement = deltaTime * speed;
            float dx = (float)(movingVector[0]);
            float dy = (float)(movingVector[1]);

            if (KeyboardState.IsKeyDown(Keys.A)) {Camera.Move(movement * dx, 0, Walls, DoorsHitboxes);}
            if (KeyboardState.IsKeyDown(Keys.D)) {Camera.Move(movement * dx, 0, Walls, DoorsHitboxes);}
            if (KeyboardState.IsKeyDown(Keys.W)) { Camera.Move(0, movement * dy, Walls, DoorsHitboxes);}
            if (KeyboardState.IsKeyDown(Keys.S)) {Camera.Move(0, movement * dy, Walls, DoorsHitboxes);}
        }

        private void CheckDoors(float deltaTime)
        {
            float radius = 3f;
          
            for (int i = 0; i < Doors.Count; i++)
            {
                float distance = (float)Math.Sqrt((-Camera.x - Doors[i].defaultposition.X) * (-Camera.x - Doors[i].defaultposition.X) + (-Camera.z - Doors[i].defaultposition.Z) * (-Camera.z - Doors[i].defaultposition.Z));
                if (distance < radius && KeyboardState.IsKeyDown(Keys.E))
                {
                    Console.WriteLine("oteviraji se dvere:" + Doors[i].isOpening);
                    Console.WriteLine("zaviraji se dvere:" + Doors[i].isClosing);
                    
                    if(Doors[i].isOpening == false && !Doors[i].Changed)
                    {
                        Doors[i].GetNearestWall(Walls);
                    }
                }
                if (Doors[i].isOpening)
                {
                    DoorMove(Doors[i], deltaTime);
                }
            }


            for (int i = 0; i < Doors.Count; i++)
            {
                float distance = (float)Math.Sqrt((-Camera.x - Doors[i].defaultposition.X) * (-Camera.x - Doors[i].defaultposition.X) + (-Camera.z - Doors[i].defaultposition.Z) * (-Camera.z - Doors[i].defaultposition.Z));
                if (distance < radius && distance > Math.Sqrt(2 * Math.Pow(Doors[i].TILE_SIZE / 2, 2)) && KeyboardState.IsKeyDown(Keys.Q) && Doors[i].Changed == true)
                {
                    Doors[i].Changed = false;
                    if (Doors[i].isClosing == false)
                    {
                        Doors[i].targetPosition = Doors[i].defaultposition;
                        Doors[i].isClosing = true;
                    }
                }
                if (Doors[i].isClosing)
                {
                    DoorMove(Doors[i], deltaTime);
                }
            }
        }

        public void DoorMove(Block doors, float deltaTime)
        {
            float movement = deltaTime * doorspeed;

            Vector3 direction = Vector3.Normalize(doors.targetPosition - doors.position);
            Vector3 step = direction * movement;

            // Zkontroluj, jestli jsme nepřekročili cílovou pozici:
            if (Vector3.Distance(doors.position, doors.targetPosition) <= movement)
            {
                doors.position = doors.targetPosition;
                doors.isClosing = false;
                doors.isOpening = false;
            }
            else
            {
                doors.position += step;
                Console.WriteLine(doors.position);
            }
        }

        private void GetMovingVector()
        {
            double x = 0;
            double y = 0;

            if (KeyboardState.IsKeyDown(Keys.A)) { x -= 1; }
            if (KeyboardState.IsKeyDown(Keys.D)) { x += 1; }
            if (KeyboardState.IsKeyDown(Keys.W)) { y += 1; }
            if (KeyboardState.IsKeyDown(Keys.S)) { y -= 1; }

            double length = Math.Sqrt(x * x + y * y);
            
            if (length != 0)
            {
                x /= length;
                y /= length;
            }
            movingVector[0] = x;
            movingVector[1] = y;
        }

        public void DrawMap()
        {
            Walls.Sort((a, b) => (b.position.Z).CompareTo(a.position.Z));
            DoorsHitboxes.Sort((a, b) => (b.position.Z).CompareTo(a.position.Z));
            Floor.Sort((a, b) => (b.position.Z).CompareTo(a.position.Z));

            foreach (Block block in Walls)
            {
                block.Draw(Camera, light);
            }
            foreach (Block door in DoorsHitboxes)
            {
                door.Draw(Camera,light);
            }
            foreach (Flat flat in Floor)
            {
                flat.Draw(Camera, light);
            }
            foreach (ObjectC obj in Objects)
            {
                obj.Draw(Camera, light);
            }
        }
        public void GenerateMap(Shader shader)
        {
            int TILE_SIZE = 2;

            for (int i = 0; i < Map.map.Length; i++)
            {
                for (int j = 0; j < Map.map[0].Length; j++)
                {
                    int posX = (+1) * i * TILE_SIZE;
                    int posZ = (-1) * j * TILE_SIZE;

                    Floor.Add(MakeFlat(posX, posZ, shader));

                    if (Map.map[i][j] == 1)
                    {
                        Walls.Add(MakeBlock(posX, posZ, shader));
                    }
                    if (Map.map[i][j] == 2)
                    {
                        SetPlayer(posX, posZ, 0.7f);
                        //MakeObject(posX, posZ, shader, "Objects/Flashlight.obj");
                    }
                    if (Map.map[i][j] == 4)
                    {
                        Block doors = MakeDoors(posX, posZ, shader);
                        Doors.Add(doors);
                        DoorsHitboxes.Add(doors);
                    }
                }
            }
        }

        public void SetPlayer(int posX, int posZ, float height)
        {
            Camera.x = -posX;
            Camera.y = -height;
            Camera.z = -posZ;
        }

        public Flat MakeFlat(int posX, int posZ, Shader shader)
        {
            Flat flat = new Flat();
            flat.position = new Vector3(posX, 0, posZ);
            flat.Shader = shader;
            flat.Material = new Material(new Vector3(0.5f, 0.01f, 0.01f), new Vector3(0.8f), 20.0f);
            flat.countNormals();
            return flat;
        }

        public Block MakeBlock(int posX, int posZ, Shader shader)
        {
            Block block = new Block();
            block.position = new Vector3(posX, 0, posZ);
            block.Shader = shader;
            block.Material = new Material(new Vector3(0.5f, 0.01f, 0.01f), new Vector3(0.8f), 20.0f);
            block.countNormals();
            return block;
        }

        public Block MakeDoors(int posX, int posZ, Shader shader)
        {
            Block doors = new Block();
            doors.position = new Vector3(posX, 0, posZ);
            doors.defaultposition = new Vector3(posX, 0, posZ);
            doors.Shader = shader;
            doors.Material = new Material(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.8f), 40.0f);
            doors.countNormals();
            return doors;
        }

        public ObjectC MakeObject(int posX, int posZ, Shader shader, string objFilename)
        {
            ObjectC obj = new ObjectC(objFilename);
            obj.position = new Vector3(posX, 1, posZ);
            obj.Shader = shader;
            obj.Material = new Material(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.8f), 40.0f);
            Objects.Add(obj);
            return obj;
        }

        public static void Main(string[] args)
        {
            string mapFilename;
            if (args.Length > 0)
            {
                mapFilename = "Maps/" + args[0];
            }
            else
            {
                mapFilename = "Maps/map1.txt";
            }

            GameWindowSettings gws = new GameWindowSettings
            {
                UpdateFrequency = 0.0,
            };
            
            NativeWindowSettings nws = new NativeWindowSettings()
            {
                Profile = OpenTK.Windowing.Common.ContextProfile.Compatability,
                Title = "ZPG",
                ClientSize = new OpenTK.Mathematics.Vector2i(800, 500),
                DepthBits = 24,

            };
            var zpg = new MyGameWindow(gws, nws, mapFilename);
            zpg.Run();
        }
    }
}