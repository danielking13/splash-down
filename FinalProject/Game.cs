using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinalProject
{
    class Game
    {
        public Grid[] gameBoard;
        public Game(Grid firingGrid)
        {
            gameBoard = new Grid[100];
            firingGrid.Children.CopyTo(gameBoard, 0);
            for (int i = 0; i < 100; i++)
            {
                gameBoard[i].Tag = "water";
            }
            PlaceShips();
        }
        
        /// <summary>
        /// Randomly place the ships on the game board
        /// </summary>
        private void PlaceShips()
        {
            Random random = new Random();
            int[] shipSizes = new int[] { 2, 3, 3, 4, 5 };
            string[] ships = new string[] { "patrolBoat", "destroyer", "submarine", "battleship", "carrier" };
            int sizeOfShip, index;
            string currentShip;
            Orientation orientation;
            bool unavailableIndex = true;

            //loop through all the ships
            for (int i = 0; i < shipSizes.Length; i++) 
            {
                //Set size and ship type
                sizeOfShip = shipSizes[i];
                currentShip = ships[i];
                unavailableIndex = true;

                if (random.Next(0, 2) == 0) //decide if the ship will be horizontal or vertical
                    orientation = Orientation.Horizontal;
                else
                    orientation = Orientation.Vertical;

                //Set our ships up
                if (orientation.Equals(Orientation.Horizontal))
                {
                    index = random.Next(0, 100); //grab a random index
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while ((index + sizeOfShip - 1) % 10 < sizeOfShip - 1) //check to make sure our boat won't run off the end of the board
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < sizeOfShip; j++)
                        {
                            if (index + j > 99 || !gameBoard[index + j].Tag.Equals("water")) //is the index out of bounds or already a ship?
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < sizeOfShip; j++) // set the ship into a horizontal position since we know our starting index is valid
                    {
                        gameBoard[index + j].Tag = currentShip;
                    }
                }
                else //vertical ship 
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        for (int j = 0; j < sizeOfShip * 10; j += 10)
                        {
                            if (index + j > 99 || !gameBoard[index + j].Tag.Equals("water")) //is the index out of bounds or already a ship?
                            {
                                index = random.Next(0, 100); //if so let's get a new index and start over
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }

                    for (int j = 0; j < sizeOfShip * 10; j += 10) //finally we can set the ship in indexed vertical position
                    {
                        gameBoard[index + j].Tag = currentShip;
                    }
                }

            }
        }

        /// <summary>
        /// Color each ship as red and the rest as gray
        /// and disable the grid so user can't play
        /// </summary>
        public void DisableGameboard()
        {
            foreach (var element in gameBoard)
            {
                if (element.Tag.Equals("hit"))
                {
                    element.Background = new SolidColorBrush(Colors.IndianRed);
                }
               else
                {
                    element.Background = new SolidColorBrush(Colors.LightGray);
                }
                element.IsEnabled = false;
            }

        }

        /// <summary>
        /// Used only for time attack mode, so they can't start playing before
        /// the timer starts
        /// </summary>
        public void DisableGameBeforeStart()
        {
            foreach(var element in gameBoard)
            {
                element.IsEnabled = false;
                element.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        public void EnableOnStart()
        {
            foreach (var element in gameBoard)
            {
                element.IsEnabled = true;
                element.Background = new SolidColorBrush(Colors.LightSlateGray);
            }
        }
    }
}
