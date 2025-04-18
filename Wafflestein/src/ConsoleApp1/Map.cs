using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Map
    {
        public int[][] map { get; set; }
        public static int[][] MapTranformation(string filename)
        {
            string[] content = File.ReadAllLines(filename);

            string[] parts = content[0].Split('x');

            int cols = int.Parse(parts[0]);
            int rows = int.Parse(parts[1]);

            int[][] grid = new int[rows][];

            for (int i = 1; i < content.Length; i++)
            {
                char[] characters = content[i].ToCharArray();
                int[] line = new int[cols];

                for (int j = 0; j < characters.Length; j++)
                {
                    int ascii = (int)characters[j];

                    if (ascii >= 97 && ascii <= 110) // a-n volný prostor
                    {
                        line[j] = 0;
                    }

                    else if (ascii >= 111 && ascii <= 122) // o-z zeď
                    {
                        line[j] = 1;
                    }
                    else if (ascii == 64) // @ start (volný prostor)
                    {
                        line[j] = 2;
                    }
                    else if (ascii == 42 || ascii == 94 || ascii == 33) // světla (volný prostor)
                    {
                        line[j] = 3;
                    }
                    else if (ascii >= 62 && ascii <= 71) // dveře (zeď)
                    {
                        line[j] = 4;
                    }
                    else if (ascii >= 72 && ascii <= 78) // pevné objekty (volný prostor)
                    {
                        line[j] = 5;
                    }
                    else if (ascii >= 79 && ascii <= 82) // postavy (volný prostor)
                    {
                        line[j] = 6;
                    }
                    else if (ascii >= 84 && ascii <= 90) // itemy pro sběr (volný prostor)
                    {
                        line[j] = 7;
                    }
                }
                grid[i - 1] = line;
            }
            return grid;
        }
    }
}
