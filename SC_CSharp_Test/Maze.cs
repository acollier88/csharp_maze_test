using System;
using System.Collections.Generic;

namespace SC_CSharp_Test
{
	public class Maze
	{
		public int width { get; set; }
		public int height { get; set; }
		public int[,] gridData { get; set; }
		public Dictionary<int, string> dataEnum { get; set; }
		public Dictionary<string, int> typeEnum { get; set; }

		public Maze(int width, int height, int[,] gridData)
		{
			this.width = width;
			this.height = height;
			this.gridData = gridData;
			this.dataEnum = new Dictionary<int, string>()
			{
				{1,"UP"},
				{2,"RIGHT"},
				{4,"DOWN"},
				{8,"LEFT"},
				{16,"START"},
				{32,"END"},
				{64,"MINE"}
			};
			this.typeEnum = new Dictionary<string, int>()
			{
				{"UP", 1},
				{"RIGHT", 2},
				{"DOWN", 4},
				{"LEFT", 8},
				{"START", 16},
				{"END", 32},
				{"MINE", 64}
			};
		}

		public int GetCellData(int coordX, int coordY)
		{
			if (coordX > this.width || coordX < 0)
			{
				string error = String.Format("Invalid X Coord {0} is not in the range 0 to {1}",
							 coordX, this.width);
			
				throw new System.ArgumentException(error, nameof(coordX));
			}
			if (coordY > this.height || coordY < 0)
			{
				string error = String.Format("Invalid Y Coord {0} is not in the range 0 to {1}",
							 coordX, this.height);
				throw new System.ArgumentException(error, nameof(coordY));
			}

			return this.gridData[coordY,coordX];

		}

		public Dictionary<string, int> CellToDict(int coordY, int coordX)
		{
			if (coordX > this.width || coordX < 0)
			{
				string error = String.Format("Invalid X Coord {0} is not in the range 0 to {1}",
							 coordX, this.width);
			
				throw new System.ArgumentException(error, nameof(coordX));
			}
			if (coordY > this.height || coordY < 0)
			{
				string error = String.Format("Invalid Y Coord {0} is not in the range 0 to {1}",
							 coordX, this.height);
				throw new System.ArgumentException(error, nameof(coordY));
			}
            int cell = this.gridData[coordY, coordX ];

			return new Dictionary<string, int>()
			{
				{"UP", cell & 1},
				{"RIGHT", cell & 2},
				{"DOWN", cell & 4},
				{"LEFT", cell & 8},
				{"START", cell & 16},
				{"END", cell & 32},
				{"MINE", cell & 64},
			};
		}

		public Tuple<int, int> TranslateMovement(string movement, int x, int y)
        {
            switch (movement)
            {
                case "UP":
                    y--;
                    break;
				case "RIGHT":
					x++;
					break;
				case "DOWN":
					y++;
					break;
				case "LEFT":
                    x--;
					break;
            }
            return new Tuple<int, int>(y, x);
        }

	}

	public class MovementNode
	{
		public int x { get; set; }
        public int y { get; set; }
		public int value { get; set; }
		public int parentX { get; set; }
        public int parentY { get; set; }
		public string parentMove { get; set; }
		public bool isMined { get; set; }

		public MovementNode(int x, int y, int value, int parentX, int parentY,
						   string parentMove)
		{
			this.x = x;
            this.y = y;
			this.value = value;
			this.parentX = parentX;
            this.parentY = parentY;
			this.parentMove = parentMove;
			this.isMined = isMined;
		}

	}
}