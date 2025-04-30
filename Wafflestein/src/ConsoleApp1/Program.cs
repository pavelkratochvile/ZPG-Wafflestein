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
using ConsoleApp1.MapObjects;

namespace ConsoleApp1
{
    public class MyGameWindow : GameWindow
    {
        public List<Floor> Floors = new List<Floor>();
        public Floor curentFloor;
        Light light = new Light(new Vector3(10, 10, 10), false);

        private float speed = 5f;
        private float doorspeed = 1f;
        private double[] movingVector = new double[] { 0, 0 };
        private string mapFilename;
        private ViewPort Viewport { get; set; }
        private ViewPort MinimapViewport { get; set; }
        private double deltaTime = 0.0;
        private Camera Camera { get; set; }

        private Stopwatch stopwatch = new Stopwatch();
        private int frameCount = 0;
        private double lastTime = 0;
        private double fps = 0;



        public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, string[] args) : base(gameWindowSettings, nativeWindowSettings)
        {
            LoadMaps(args);
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
            this.Title = Floors.Count.ToString() ;
            MakeCurrent();
            Console.WriteLine(Camera.hasTeleported.ToString());
            GetMovingVector();
            turnLight();
            MakeMove(deltaTime);
            CheckDoors(deltaTime, Floors[0]);
            CheckSecretDoors(deltaTime, Floors[0]);
            CheckTeleports(Floors[0]);
            IsExiting();
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            for (int i = 0; i < Floors.Count; i++)
            {
                DrawMap(Floors[i]);
            }
            
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
            
            Camera.map = Floors[0].Map.map;
            
            for(int i = 0; i < Floors.Count; i++)
            {
                GenerateMap(shader, Floors[i]);
            }
            curentFloor = Floors[0];
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

            if (KeyboardState.IsKeyDown(Keys.A)) {Camera.Move(movement * dx, 0, curentFloor.Walls, curentFloor.DoorsHitboxes);}
            if (KeyboardState.IsKeyDown(Keys.D)) {Camera.Move(movement * dx, 0, curentFloor.Walls, curentFloor.DoorsHitboxes);}
            if (KeyboardState.IsKeyDown(Keys.W)) {Camera.Move(0, movement * dy, curentFloor.Walls, curentFloor.DoorsHitboxes);}
            if (KeyboardState.IsKeyDown(Keys.S)) {Camera.Move(0, movement * dy, curentFloor.Walls, curentFloor.DoorsHitboxes);}
        }

        private void CheckDoors(float deltaTime, Floor floor)
        {
            float radius = 3f;
          
            for (int i = 0; i < floor.Doors.Count; i++)
            {
                float distance = (float)Math.Sqrt((-Camera.x - floor.Doors[i].defaultposition.X) * (-Camera.x - floor.Doors[i].defaultposition.X) + (-Camera.z - floor.Doors[i].defaultposition.Z) * (-Camera.z - floor.Doors[i].defaultposition.Z));
                if (distance < radius && KeyboardState.IsKeyDown(Keys.E))
                {
                    Console.WriteLine("oteviraji se dvere:" + floor.Doors[i].isOpening);
                    Console.WriteLine("zaviraji se dvere:" + floor.Doors[i].isClosing);
                    
                    if(floor.Doors[i].isOpening == false && !floor.Doors[i].Changed)
                    {
                        floor.Doors[i].GetNearestWall(floor.Walls);
                    }
                }
                if (floor.Doors[i].isOpening)
                {
                    DoorMove(floor.Doors[i], deltaTime);
                }
            }


            for (int i = 0; i < floor.Doors.Count; i++)
            {
                float distance = (float)Math.Sqrt((-Camera.x - floor.Doors[i].defaultposition.X) * (-Camera.x - floor.Doors[i].defaultposition.X) + (-Camera.z - floor.Doors[i].defaultposition.Z) * (-Camera.z - floor.Doors[i].defaultposition.Z));
                if (distance < radius && distance > Math.Sqrt(2 * Math.Pow(floor.Doors[i].TILE_SIZE / 2, 2)) && KeyboardState.IsKeyDown(Keys.Q) && floor.Doors[i].Changed == true)
                {
                    floor.Doors[i].Changed = false;
                    if (floor.Doors[i].isClosing == false)
                    {
                        floor.Doors[i].targetPosition = floor.Doors[i].defaultposition;
                        floor.Doors[i].isClosing = true;
                    }
                }
                if (floor.Doors[i].isClosing)
                {
                    DoorMove(floor.Doors[i], deltaTime);
                }
            }
        }

        private void CheckSecretDoors(float deltaTime, Floor floor)
        {
            float radius = 3f;

            for (int i = 0; i < floor.secretDoors.Count; i++)
            {
                float distance = (float)Math.Sqrt((-Camera.x - floor.secretDoors[i].defaultposition.X) * (-Camera.x - floor.secretDoors[i].defaultposition.X) + (-Camera.z - floor.secretDoors[i].defaultposition.Z) * (-Camera.z - floor.secretDoors[i].defaultposition.Z));
                if (distance < radius && KeyboardState.IsKeyDown(Keys.E))
                {
                    Console.WriteLine("oteviraji se dvere:" + floor.Doors[i].isOpening);
                    Console.WriteLine("zaviraji se dvere:" + floor.Doors[i].isClosing);

                    if (floor.secretDoors[i].isOpening == false && !floor.secretDoors[i].Changed)
                    {
                        floor.secretDoors[i].GetNearestWall(floor.Walls);
                    }
                }
                if (floor.secretDoors[i].isOpening)
                {
                    DoorMove(floor.secretDoors[i], deltaTime);
                }
            }


            for (int i = 0; i < floor.secretDoors.Count; i++)
            {
                float distance = (float)Math.Sqrt((-Camera.x - floor.secretDoors[i].defaultposition.X) * (-Camera.x - floor.secretDoors[i].defaultposition.X) + (-Camera.z - floor.secretDoors[i].defaultposition.Z) * (-Camera.z - floor.secretDoors[i].defaultposition.Z));
                if (distance < radius && distance > Math.Sqrt(2 * Math.Pow(floor.secretDoors[i].TILE_SIZE / 2, 2)) && KeyboardState.IsKeyDown(Keys.Q) && floor.secretDoors[i].Changed == true)
                {
                    floor.secretDoors[i].Changed = false;
                    if (floor.secretDoors[i].isClosing == false)
                    {
                        floor.secretDoors[i].targetPosition = floor.secretDoors[i].defaultposition;
                        floor.secretDoors[i].isClosing = true;
                    }
                }
                if (floor.secretDoors[i].isClosing)
                {
                    DoorMove(floor.secretDoors[i], deltaTime);
                }
            }
        }

        public void DoorMove(Block doors, float deltaTime)
        {
            float movement = deltaTime * doorspeed;

            Vector3 direction = Vector3.Normalize(doors.targetPosition - doors.position);
            Vector3 step = direction * movement;
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
        public void CheckTeleports(Floor floor)
        {
            float playerPosX = -Camera.x;
            float playerPosZ = -Camera.z;
            bool isOutside = true;

            for (int i = 0; i < floor.Teleports.Count; i++)
            {
                if (isColiding(playerPosX, playerPosZ, floor.Teleports[i]))
                {
                    Camera.beforeTP.Start();
                    isOutside = false;
                    if (this.Camera.sw.Elapsed.TotalSeconds > 4 && this.Camera.beforeTP.Elapsed.TotalSeconds > 2)
                    {
                        Camera.beforeTP.Reset();
                        Camera.afterTP.Start();
                        Camera.hasTeleported = true;
                        Camera.sw.Restart();

                        
                        Random random = new Random();
                        int randomIndex = random.Next(0, floor.Teleports.Count);
                        while(randomIndex == i)
                        {
                            randomIndex = random.Next(0, floor.Teleports.Count);
                        }

                        this.Camera.x = -floor.Teleports[randomIndex].position.X;
                        this.Camera.z = -floor.Teleports[randomIndex].position.Z;
                    }
                }
            }
            if(Camera.afterTP.Elapsed.TotalSeconds > 2)
            {
                Camera.afterTP.Reset();
                Camera.beforeTP.Start();
                Camera.hasTeleported = false;
            }

            if (isOutside || Camera.hasTeleported == true)
            {
                Camera.beforeTP.Reset();
            }
        }

        public bool isColiding(float playerPosX, float playerPosZ, Teleport teleport)
        {
            float max_x = (float)teleport.position.X + teleport.TILE_SIZE / 2;
            float min_x = (float)teleport.position.X - teleport.TILE_SIZE / 2;
            float max_z = (float)teleport.position.Z + teleport.TILE_SIZE / 2;
            float min_z = (float)teleport.position.Z - teleport.TILE_SIZE / 2;

            if (playerPosX < max_x && playerPosX > min_x && playerPosZ < max_z && playerPosZ > min_z)
            {
                return true;
            }
            return false;
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

        public void DrawMap(Floor floor)
        {
            floor.Walls.Sort((a, b) => (b.position.Z).CompareTo(a.position.Z));
            floor.DoorsHitboxes.Sort((a, b) => (b.position.Z).CompareTo(a.position.Z));
            floor.Ground.Sort((a, b) => (b.position.Z).CompareTo(a.position.Z));
            float beforeTP = (float)Camera.beforeTP.Elapsed.TotalMilliseconds;
            float afterTP = (float)Camera.afterTP.Elapsed.TotalMilliseconds;

            foreach (Block block in floor.Walls)
            {
                block.Draw(Camera, light, Camera.hasTeleported, beforeTP, afterTP);
            }
            foreach (Block door in floor.DoorsHitboxes)
            {
                door.Draw(Camera,light, Camera.hasTeleported, beforeTP, afterTP);
            }
            foreach (Flat flat in floor.Ground)
            {
                flat.Draw(Camera, light, Camera.hasTeleported, beforeTP, afterTP);
            }
            foreach (ObjectC obj in floor.Objects)
            {
                obj.Draw(Camera, light, Camera.hasTeleported, beforeTP, afterTP);
            }
            foreach (Teleport tp in floor.Teleports)
            {
                tp.Draw(Camera, light, Camera.hasTeleported, beforeTP, afterTP);
            }
        }
        public void GenerateMap(Shader shader, Floor floor)
        {
            int TILE_SIZE = 2;

            for (int i = 0; i < floor.Map.map.Length; i++)
            {
                for (int j = 0; j < floor.Map.map[0].Length; j++)
                {
                    int posX = (+1) * i * TILE_SIZE;
                    int posZ = (-1) * j * TILE_SIZE;


                    if (floor.Map.map[i][j] != 10)
                    {
                        floor.Ground.Add(MakeFlat(posX, posZ, floor.depth, floor.height, shader));
                    }

                    if (floor.Map.map[i][j] == 1)
                    {
                        floor.Walls.Add(MakeBlock(posX, posZ, floor.depth, floor.height, shader));
                    }
                    if (floor.Map.map[i][j] == 2)
                    {
                        SetPlayer(posX, posZ, 0.7f - floor.depth * floor.height);
                        //MakeObject(posX, posZ, shader, "Objects/Flashlight.obj");
                    }
                    if (floor.Map.map[i][j] == 4)
                    {
                        Doors doors = MakeDoors(posX, posZ, floor.depth, floor.height, shader);
                        floor.Doors.Add(doors);
                        floor.DoorsHitboxes.Add(doors);
                    }
                    if (floor.Map.map[i][j] == 8)
                    {
                        SecretDoors secretdoors = MakeSecretDoors(posX, posZ, floor.depth, floor.height, shader);
                        floor.secretDoors.Add(secretdoors);
                        floor.DoorsHitboxes.Add(secretdoors);
                    }
                    if (floor.Map.map[i][j] == 9)
                    {
                        Teleport teleport = MakeTeleport(posX, posZ, floor.depth, floor.height, shader);
                        floor.Teleports.Add(teleport);
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

        public Flat MakeFlat(int posX, int posZ, int depth, float height, Shader shader)
        {
            Flat flat = new Flat();
            flat.position = new Vector3(posX, -depth * height, posZ);
            flat.Shader = shader;
            flat.Material = new Material(new Vector3(0.5f, 0.01f, 0.01f), new Vector3(0.8f), 20.0f);
            flat.countNormals();
            return flat;
        }
        public SecretDoors MakeSecretDoors(int posX, int posZ, int depth, float height, Shader shader)
        {
            SecretDoors doors = new SecretDoors();
            doors.position = new Vector3(posX, -depth * height, posZ);
            doors.defaultposition = new Vector3(posX, -depth * height, posZ);
            doors.Shader = shader;
            doors.Material = new Material(new Vector3(0.5f, 0.01f, 0.01f), new Vector3(0.8f), 20.0f);
            doors.countNormals();
            return doors;
        }
        public Teleport MakeTeleport(int posX, int posZ, int depth, float height,Shader shader)
        {
            Teleport teleport = new Teleport();
            teleport.position = new Vector3(posX, -depth * height, posZ);
            teleport.Shader = shader;
            teleport.Material = new Material(new Vector3(0.0f, 0.3f, 0.1f), new Vector3(0.8f), 20.0f);
            teleport.countNormals();
            return teleport;
        }

        public Block MakeBlock(int posX, int posZ, int depth, float height, Shader shader)
        {
            Block block = new Block(false);
            block.position = new Vector3(posX, -depth * height, posZ);
            block.Shader = shader;
            block.Material = new Material(new Vector3(0.5f, 0.01f, 0.01f), new Vector3(0.8f), 20.0f);
            block.countNormals();
            return block;
        }

        public Doors MakeDoors(int posX, int posZ, int depth, float height, Shader shader)
        {
            Doors doors = new Doors();
            doors.position = new Vector3(posX, -depth * height, posZ);
            doors.defaultposition = new Vector3(posX, -depth * height, posZ);
            doors.Shader = shader;
            doors.Material = new Material(new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.8f), 40.0f);
            doors.countNormals();
            return doors;
        }

        public void LoadMaps(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                string mapFilename = "Maps/" + args[i];
                Floor floor = new Floor(mapFilename, i);
                Floors.Add(floor);
            }
        }

        public static void Main(string[] args)
        {
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
            var zpg = new MyGameWindow(gws, nws, args);
            zpg.Run();
        }
    }
}