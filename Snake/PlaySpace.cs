using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Snake
{
    internal enum Direction
    {
        EAST,
        NORTH,
        SOUTH,
        WEST
    }
    internal enum GameState
    {
        DEAD,
        GOBBLE,
        ALIVE,
        WIN,
    }
    internal class PlaySpace
    {
        private int width;
        private int height;
        private int xhead;
        private int yhead;
        private int snakeAge;
        private Cell[,] cells;

        public CellType[,] GetGameState()
        {
            CellType[,] gameGrid = new CellType[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    gameGrid[i, j] = cells[i, j].GetCellType();
                }
            }
            return gameGrid;
        }
        public int GetCellAge(int x, int y) { return cells[x, y].GetSnakePartAge(); }
        public int[] GetHeadOfSnake()
        {
            return [xhead, yhead];
        }
        public int GetWidth() { return width; }
        public int GetHeight() { return height; }

        public PlaySpace()
        {
            snakeAge = 3;
            width = 9; height = 9;
            xhead = width/2; yhead = height/2;
            cells = new Cell[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (i == xhead && j == yhead) cells[i, j] = new Cell(CellType.SNAKEPART, snakeAge);
                    else cells[i, j] = new Cell(CellType.EMPTY, snakeAge);
                }
            }
            MakeFood(new bool[width, height], new Random());
        }
        public PlaySpace(int x, int y) 
        {
            snakeAge = 3;
            this.width = x;
            this.height = y;
            xhead = x / 2; yhead = y/2;
            cells = new Cell[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (i == xhead && j == yhead) cells[i, j] = new Cell(CellType.SNAKEPART, snakeAge);
                    else cells[i, j] = new Cell(CellType.EMPTY, snakeAge);
                }
            }
            MakeFood(new bool[width, height], new Random());
        }
        private GameState MakeFood(bool[,] visited, Random rand)
        {
            List<int[]> ints = [];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (!visited[i, j]) ints.Add([i, j]);
                }
            }
            if (ints.Count > 0)
            {
                int pick = rand.Next(ints.Count);
                visited[ints[pick][0], ints[pick][1]] = true;
                if (cells[ints[pick][0], ints[pick][1]].MakeFood()) return GameState.GOBBLE;
                else return MakeFood(visited, rand);
            }
            return GameState.WIN;
        }
        public GameState update(Direction facing)
        {
            GameState state = GameState.ALIVE;

            foreach (Cell cell in cells)
            {
                cell.Update(state);
            }

            switch (facing)
            {
                case Direction.EAST:
                    xhead++;
                    if (xhead >= width) xhead = 0;
                    state = cells[xhead, yhead].Enter(snakeAge);
                    break;
                case Direction.NORTH:
                    yhead--;
                    if (yhead < 0) yhead = height - 1;
                    state = cells[xhead, yhead].Enter(snakeAge);
                    break;
                case Direction.SOUTH:
                    yhead++;
                    if (yhead >= height) yhead = 0;
                    state = cells[xhead, yhead].Enter(snakeAge);
                    break;
                case Direction.WEST:
                    xhead--;
                    if (xhead < 0 ) xhead = width - 1;
                    state = cells[xhead, yhead].Enter(snakeAge);
                    break;
                default:
                    state = GameState.ALIVE;
                    break;
            }

            if (state == GameState.GOBBLE)
            {
                state = MakeFood(new bool[width, height], new Random());
                snakeAge++;
            }
            return state;
        }
    }
}
