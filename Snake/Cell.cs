using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    internal enum CellType
    {
        WALL,
        EMPTY,
        FOOD,
        SNAKEPART
    }
    internal class Cell
    {
        private CellType _type;
        private int snakePartAge;

        /// <summary>
        /// Initialises the cell
        /// </summary>
        /// <param name="snakePart">Is this cell part of the snake?</param>
        /// <param name="snakePartAge">If part of the snake, how long is the snake?</param>
        /// <param name="wall">Is there a wall here?</param>
        public Cell(CellType type, int snakeAge)
        {
            _type = type;
            if (_type == CellType.SNAKEPART) this.snakePartAge = snakeAge; else this.snakePartAge = 0;
        }

        /// <summary>
        /// Put some yummy food here for snake to eat!
        /// </summary>
        /// <returns>False if food cannot be placed here</returns>
        public bool MakeFood()
        {
            if (_type == CellType.EMPTY)
            {
                _type = CellType.FOOD;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Head of the snake enters this cell
        /// </summary>
        /// <param name="snakeAge">how long is the snake?</param>
        /// <returns>Result of entering. if food is eaten gobble will be true, if hit a wall or itself dead :(</returns>
        public GameState Enter(int snakeAge)
        {
            if (_type == CellType.FOOD)
            {
                _type = CellType.SNAKEPART;
                this.snakePartAge = snakeAge + 1;
                return GameState.GOBBLE;
            }
            if (_type == CellType.WALL) return GameState.DEAD;
            if (_type == CellType.SNAKEPART) return GameState.DEAD;
            if (_type == CellType.EMPTY)
            {
                _type = CellType.SNAKEPART;
                this.snakePartAge = snakeAge;
                return GameState.ALIVE;
            }
            return GameState.ALIVE;
        }

        /// <summary>
        /// Each frame cell is updated to move snake
        /// </summary>
        /// <param name="gobble">Has a good piece been eaten this turn?</param>
        public void Update(GameState state)
        {
            if (_type == CellType.SNAKEPART) 
            { 
                snakePartAge--; 

                if (snakePartAge <= 0) 
                { 
                    _type = CellType.EMPTY; 
                } 
            }
        }

        public CellType GetCellType() { return _type; }
        public int GetSnakePartAge() { return snakePartAge; }
    }
}
