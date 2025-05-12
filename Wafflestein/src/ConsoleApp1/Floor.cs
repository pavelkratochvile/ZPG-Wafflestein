using ConsoleApp1.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Floor
    {
        public List<ObjectC> Objects = new List<ObjectC>();
        public List<Block> Walls = new List<Block>();
        public List<Doors> Doors = new List<Doors>();
        public List<SecretDoors> secretDoors = new List<SecretDoors>();
        public List<Teleport> Teleports = new List<Teleport>();
        public List<Block> DoorsHitboxes = new List<Block>();
        public List<Flat> Ground = new List<Flat>();
        public List<Ceiling> Ceiling = new List<Ceiling>();
        public List<Hole> Holes = new List<Hole>();

        public Map Map = new Map();
        public int depth;
        public float height = 3;
        public float bottomColideBox = 0.7f;

        public Floor(String filename, int depth)
        {
            Map.map = Map.MapTranformation(filename);
            this.depth = depth;
        }
    }
}
