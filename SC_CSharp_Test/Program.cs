using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace SC_CSharp_Test
{
    public class Solver
    {
        public string dataDirectory { get; set; }
        public string mazesFile { get; set; }
        public List<Maze> mazeList { get; set; }

        public Solver(string mf)
        {
            this.dataDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            this.mazesFile = mf;
            this.mazeList = new List<Maze>();
        }


        public MovementNode FindStart(Maze maze)
        {
            for (int h = 0; h <= maze.gridData.GetUpperBound(0); h++)
            {
                for (int w = 0; w <= maze.gridData.GetUpperBound(1); w++)
                {
                    if ((maze.gridData[h, w] & maze.typeEnum["START"]) != 0)
                    {
                        MovementNode start = new MovementNode(w, h,
                                                maze.gridData[h, w],
                                                -1, -1, null);
                        return start;
                    }
                }
            }
            throw new System.ArgumentException("No Start Found");
        }


        public bool VerifySurvives(Character character, MovementNode node,
                                   Dictionary<string, MovementNode> graph)
        {
            int deathCounter = 0;
            while (node.parentX != -1 && node.parentY != -1)
            {
                bool mined = node.isMined;
                if (mined) deathCounter++;
                if (deathCounter >= character.maxLives) return false;
                string key = node.parentY.ToString() + "," +
                                 node.parentX.ToString();
                node = graph[key];
            }
            return true;
        }

        public List<string> PrintInstructions(MovementNode node,
                                              Dictionary<string,
                                              MovementNode> graph,
                                              bool unitTest)
        {
            List<string> instructions = new List<string>();
            string text = node.parentMove;
            while (node.parentX != -1 && node.parentY != -1)
            {
                instructions.Add(text.ToLower());
                string key = node.parentY.ToString() + ","
                                 + node.parentX.ToString();
                node = graph[key];
                text = node.parentMove;
            }
            instructions.Reverse();
            Console.Write("['");
            if (!unitTest)
            {
                Console.Write(String.Join("','", instructions));
            }
            Console.WriteLine("']");
            return instructions;
        }

        public void PrintDebug(Maze maze, HashSet<Tuple<int, int>> visited)
        {
            for (int h = 0; h <= maze.gridData.GetUpperBound(0); h++)
            {
                for (int w = 0; w <= maze.gridData.GetUpperBound(1); w++)
                {
                    Tuple<int, int> location = new Tuple<int, int>(h, w);
                    if ((maze.gridData[h, w] & maze.typeEnum["START"]) != 0)
                    {
                        Console.Write("STRT");
                    }
                    else if ((maze.gridData[h, w] & maze.typeEnum["END"]) != 0)
                    {
                        Console.Write("END_");
                    }
                    else if ((maze.gridData[h, w] & maze.typeEnum["MINE"]) != 0)
                    {
                        Console.Write("MINE");
                    }
                    else if (visited.Contains(location))
                    {

                        Console.Write(((h * maze.width) + w).ToString().PadLeft(4, '0'));
                    }
                    else
                    {
                        System.Text.StringBuilder text =
                            new System.Text.StringBuilder();
                        if ((maze.gridData[h, w] & maze.typeEnum["LEFT"]) != 0)
                        {
                            text.Append('L');
                        }
                        else text.Append('-');
                        if ((maze.gridData[h, w] & maze.typeEnum["UP"]) != 0)
                        {
                            text.Append('U');
                        }
                        else text.Append('-');
                        if ((maze.gridData[h, w] & maze.typeEnum["DOWN"]) != 0)
                        {
                            text.Append('D');
                        }
                        else text.Append('-');
                        if ((maze.gridData[h, w] & maze.typeEnum["RIGHT"]) != 0)
                        {
                            text.Append('R');
                        }
                        else text.Append('-');
                        Console.Write(text.ToString());
                    }
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }


        public void ImportMazes()
        {
            string filePath = this.dataDirectory + this.mazesFile;
            var Lines = File.ReadLines(@filePath);
            var CSV = from line in Lines
                      where !String.IsNullOrWhiteSpace(line)
                      let data = line.Split('-')
                      select new
                      {
                          sizeTuple = data[0].Substring(1, data[0].Length - 2)
                                           .Split(',')
                                           .ToArray(),
                          gridData = data[1].Substring(1, data[1].Length - 2)
                                          .Split(',')
                                          .ToArray()

                      };

            foreach (var row in CSV)
            {
                int height = Int32.Parse(row.sizeTuple[0]);
                int width = Int32.Parse(row.sizeTuple[1]);

                int[,] mazeData = new int[height, width];
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        int index = (h * width) + w;
                        mazeData[h, w] = Int32.Parse(row.gridData[index]);
                    }
                }
                Maze newMaze = new Maze(width, height, mazeData);
                this.mazeList.Add(newMaze);
            }
        }

        public List<List<string>> SolveAllMazes(Character character,
                                                bool debug,
                                                bool unitTest)
        {
            List<List<string>> solutions = new List<List<string>>();
            foreach (Maze maze in this.mazeList)
            {
                string mazeSpecs = String.Format("maze {0}x{1}",
                                                 maze.width, maze.height);
                if (!unitTest) Console.WriteLine(mazeSpecs);

                HashSet<Tuple<int, int>> visited =
                    new HashSet<Tuple<int, int>>();
                Queue<MovementNode> tryCells =
                    new Queue<SC_CSharp_Test.MovementNode>();
                Dictionary<string, MovementNode> graph =
                    new Dictionary<string, SC_CSharp_Test.MovementNode>();

                MovementNode start = FindStart(maze);
                int x = start.x;
                int y = start.y;

                visited.Add(new Tuple<int, int>(y, x));
                tryCells.Enqueue(start);

                //Used modified version of Breadth First Search, as mazes tend 
                //to be too random to benefit from A*

                while (tryCells.Any())
                {
                    MovementNode current = tryCells.Dequeue();
                    if ((current.value & maze.typeEnum["END"]) == maze.typeEnum["END"])
                    {
                        if (VerifySurvives(character, current, graph) == true)
                        {
                            solutions.Add(PrintInstructions(current,
                                                            graph,
                                                            unitTest));
                            break;
                        }
                    }
                    Dictionary<string, int> options = maze.CellToDict(current.y,
                                                                      current.x);
                    bool survival = true;
                    if (options["MINE"] != 0)
                    {
                        current.isMined = true;
                        survival = VerifySurvives(character, current, graph);
                    }
                    string graphKey = current.y.ToString() + ","
                                 + current.x.ToString();
                    if (survival == true) graph[graphKey] = current;
                    else continue;

                    foreach (KeyValuePair<string, int> entry in options)
                    {
                        if (entry.Value == 0) continue;
                        if ((entry.Key == "MINE") || (entry.Key == "START"))
                        {
                            continue;
                        }
                        Tuple<int, int> newIndex =
                            maze.TranslateMovement(entry.Key,
                                                   current.x,
                                                   current.y);
                        if (visited.Contains(newIndex) == false)
                        {
                            visited.Add(newIndex);

                            int newX = newIndex.Item2;
                            int newY = newIndex.Item1;
                            int newValue = maze.gridData[newY, newX];
                            MovementNode newMove = new MovementNode(newX,
                                                                    newY,
                                                                    newValue,
                                                                    current.x,
                                                                    current.y,
                                                                    entry.Key);
                            tryCells.Enqueue(newMove);

                        }


                    }
                }
                if (debug == true) PrintDebug(maze, visited);
            }
            return solutions;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Character andrew = new Character(3);
            Solver start = new Solver("mazes.txt");
            start.ImportMazes();
            start.SolveAllMazes(andrew, false, false);
        }
    }
}
