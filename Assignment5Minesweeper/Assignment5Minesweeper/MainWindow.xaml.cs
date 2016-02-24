//Extra features are noises and displaying the current time.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Assignment5Minesweeper
{
    public partial class MainWindow : Window
    {
        //map the player sees
        private char[,] playerMap;
        //map the player is playing against
        private char[,] hiddenMap;

        private char[,] gameGrid;

        //grid that i'll update over and over
        private Grid playerGrid;

        private Grid HUD;

        private TextBox textbox;
        private TextBox textbox2;
        private TextBox textbox3;

        private int ROWS;
        private int COLUMNS;
        private int BOMBS;
        private int bombsleft;
        private int timer;
        private int DIFFICULTY;

        private DispatcherTimer timer2;

        //EmptyMapGenerator() is a method that generators an 2D array with dashes in each spot
        public static char[,] EmptyMapGenerator(int ROWS, int COLUMNS)
        {
            // Arrays are flipped in C# so it's [y, x]
            char[,] array = new char[ROWS, COLUMNS];
            //This first loop is looping through the ROWS (y value), 5 rows
            for (int y = 0; y < ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    //This is generating an empty map
                    array[y, x] = '-';
                }
            }
            return array;
        }

        //This method randomly generates bomgbs in our matrix
        public static char[,] BombFiller(char[,] EmptyMap, int NUMBEROFBOMBS, int ROWS, int COLUMNS)
        {
            //r becomes a random number generator
            var r = new Random();
            //This loop creates randomized bombs
            for (int i = 0; i < NUMBEROFBOMBS; i++)
            {   //places random bombs on our grid
                int rowCoordinate = r.Next(ROWS);
                int columnCoordinate = r.Next(COLUMNS);
                if (EmptyMap[rowCoordinate, columnCoordinate] == 'X')
                {
                    //Since it already has a bomb we decrement "i" so it'll do it again.
                    i--;
                }
                EmptyMap[rowCoordinate, columnCoordinate] = 'X';
            }

            return EmptyMap;
        }

        //This method displays any map we put into it onto the console
        public static char[,] MapDisplay(char[,] Map, int ROWS, int COLUMNS)
        {
            //Console.WriteLine("  0 1 2 3 4 5 6 7 8 9");
            Console.Write("  ");
            for (int j = 0; j < ROWS; j++)
            {
                Console.Write(j + " ");
            }
            for (int y = 0; y < ROWS; y++)
            {
                Console.WriteLine();
                Console.Write(y + " ");
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    //This is generating an empty map
                    Console.Write(Map[y, x] + " ");
                }
            }
            return Map;
        }

        //This method will be called by another method.  
        //This one checks a single square to see if it has an X or not
        public static int CheckSpecificBox(char[,] map, int x, int y, int ROWS, int COLUMNS)
        {
            if (x < 0 || x > (COLUMNS - 1) || y < 0 || y > (ROWS - 1))
            {
                return 0;
            }
            else if (map[y, x] == 'X')
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //This method will use CheckSpecificBox many times to check the 8 boxes around a box to determine a square number
        //This method will be called by another method to update the entire map
        public static void CheckEightBoxes(char[,] map, int x, int y, int ROWS, int COLUMNS)
        {
            //If this space IS a bomb, do nothing and move on
            if (map[y, x] == 'X')
            {
                return;
            }
            else
            {
                //Check 9 squares including itself for nearby bombs
                //The reason we have the square check itself is for simplicity of the loops
                //Since the above IF statement catches if it's a bomb we don't have to worry

                //Counter tracks how many bombs are around the square
                int counter = 0;
                //Goes through each Y value nearby
                for (int yy = y - 1; yy < y + 2; yy++)
                {
                    //Goes through each X value nearby
                    for (int xx = x - 1; xx < x + 2; xx++)
                    {
                        //If there is a bomb at the specific counter we increment the counter
                        if (CheckSpecificBox(map, xx, yy, ROWS, COLUMNS) == 1)
                        {
                            counter++;
                        }
                        //counter = counter + CheckSpecificBox(map, xx, yy);
                    }
                }
                //Once it has counted between 0 and 8 bombs we update the square to reflect how many bombs are nearby
                if (counter > 0)
                {
                    if (counter == 1)
                    {
                        map[y, x] = '1';
                    }
                    else if (counter == 2)
                    {
                        map[y, x] = '2';
                    }
                    else if (counter == 3)
                    {
                        map[y, x] = '3';
                    }
                    else if (counter == 4)
                    {
                        map[y, x] = '4';
                    }
                    else if (counter == 5)
                    {
                        map[y, x] = '5';
                    }
                    else if (counter == 6)
                    {
                        map[y, x] = '6';
                    }
                    else if (counter == 7)
                    {
                        map[y, x] = '7';
                    }
                    else
                    {
                        map[y, x] = '8';
                    }
                }
            }
        }

        //This class checks the entire map and updates the number values to reflect how many bombs there are
        public static char[,] UpdateMapWithNumbers(char[,] map, int ROWS, int COLUMNS)
        {
            //This first loop is looping through all the ROWS (y value), 5 rows
            for (int y = 0; y < ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    CheckEightBoxes(map, x, y, ROWS, COLUMNS);
                }
            }
            return map;
        }

        //This generates the computer's map that we play against
        public static char[,] ComputerMap(int NUMBEROFBOMBS, int ROWS, int COLUMNS)
        {
            char[,] map = EmptyMapGenerator(ROWS, COLUMNS);
            map = BombFiller(map, NUMBEROFBOMBS, ROWS, COLUMNS);
            map = UpdateMapWithNumbers(map, ROWS, COLUMNS);
            return map;
        }

        //This is the map the player will see
        public static char[,] EmptyDisplayMapGenerator(int ROWS, int COLUMNS)
        {
            // Arrays are flipped in C# so it's [y, x]
            // y = 5
            // x = 10
            char[,] array = new char[ROWS, COLUMNS];
            //This first loop is looping through the ROWS (y value), 5 rows
            for (int y = 0; y < ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    //This is generating an empty map
                    array[y, x] = ' ';
                }
            }
            return array;
        }


        public static void CheckAllNearbySafeSquares(char[,] ComputerMap, char[,] PlayerMap, int x, int y, int ROWS, int COLUMNS)
        {
            //Goes through each Y value nearby
            for (int yy = y - 1; yy < y + 2; yy++)
            {
                //Goes through each X value nearby
                for (int xx = x - 1; xx < x + 2; xx++)
                {
                    if (xx < 0 || xx > (COLUMNS - 1) || yy < 0 || yy > (ROWS - 1))
                    {
                        //Do nothing since this square doesn't exist
                    }
                    //If the square is empty, reveal it
                    //Make sure to check that the player map is empty or the program will trigger
                    // an infinite recursive loop
                    else if (PlayerMap[yy, xx] == ' ' && ComputerMap[yy, xx] == '-')
                    {
                        //If the square is empty reveal it and then recursively check all nearby squares
                        PlayerMap[yy, xx] = ComputerMap[yy, xx];

                        //THIS IS THE RECURSION PART
                        CheckAllNearbySafeSquares(ComputerMap, PlayerMap, xx, yy, ROWS, COLUMNS);
                    }
                    else if (ComputerMap[yy, xx] == 'X')
                    {
                        //Do nothing if that square is a bomb
                    }
                    else
                    {
                        //If the square is a number reveal it but don't recursively check that one.
                        PlayerMap[yy, xx] = ComputerMap[yy, xx];
                    }
                }
            }
        }

        //This allows a user to designate a spot as a bomb, same as a "right-click"
        public static void DeclareBomb(char[,] PlayerMap, int x, int y)
        {
            PlayerMap[y, x] = 'X';
        }

        public static void Lose()
        {
            Console.WriteLine("Sorry you lose!");
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void Win()
        {
            Console.WriteLine("Congrats you win!");
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void CheckWinCondition(char[,] ComputerMap, char[,] PlayerMap, int ROWS, int COLUMNS)
        {
            //This first loop is looping through the ROWS (y value), 5 rows
            for (int y = 0; y < ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    //If they are both equal, keep going and see if we win
                    if (ComputerMap[y, x] == PlayerMap[y, x])
                    {
                        //Do nothing, but keep looping and see if we win
                    }
                    else if (ComputerMap[y, x] == 'X' && PlayerMap[y, x] == ' ')
                    {
                        //If the computer map has a bomb at a spot and the player has an empty square we keep checking to see if the player won
                    }
                    else
                    {
                        return;
                    }
                }
            }
            //If every square in the player matrix and the NPC matrix are the same you win
            MapDisplay(ComputerMap, ROWS, COLUMNS);
            Console.WriteLine();
            Win();
        }

        public static void CheckWinCondition2(char[,] ComputerMap, char[,] PlayerMap, int ROWS, int COLUMNS)
        {
            //This first loop is looping through the ROWS (y value), 5 rows
            for (int y = 0; y < ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    //If they are both equal, keep going and see if we win
                    if (ComputerMap[y, x] == PlayerMap[y, x])
                    {
                        //Do nothing, but keep looping and see if we win
                    }
                    else if (ComputerMap[y, x] == 'X' && PlayerMap[y, x] == ' ')
                    {
                        //If the computer map has a bomb at a spot and the player has an empty square we keep checking to see if the player won
                    }
                    else
                    {
                        return;
                    }
                }
            }
            //If every square in the player matrix and the NPC matrix are the same you win
            MapDisplay(ComputerMap, ROWS, COLUMNS);
            Console.WriteLine();
            Win();
        }

        //This is the same as a left-click, means the user thinks the spot is safe
        public static void Reveal(char[,] ComputerMap, char[,] PlayerMap, int x, int y, int ROWS, int COLUMNS)
        {
            //If the user clicks on a bomb they die.
            if (ComputerMap[y, x] == 'X')
            {
                Lose();
            }
            //If the user clicked on an empty square it reveals nearby squares
            else if (ComputerMap[y, x] == '-')
            {
                CheckAllNearbySafeSquares(ComputerMap, PlayerMap, x, y, ROWS, COLUMNS);
            }
            else
            {
                //If the user just clicked on a number, just reveal the number;
                PlayerMap[y, x] = ComputerMap[y, x];
            }
        }

        //If the player enters cheatcode lowercase they win
        public static void CheatCode(char[,] ComputerMap, char[,] PlayerMap, int ROWS, int COLUMNS)
        {
            //This first loop is looping through the ROWS (y value), 5 rows
            for (int y = 0; y < ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < COLUMNS; x++)
                {
                    //Updates the map so that the player map is equal to the computer's map
                    PlayerMap[y, x] = ComputerMap[y, x];
                }
            }
        }

        public static void PlayerInput(char[,] ComputerMap, char[,] PlayerMap, int NUMBEROFBOMBS, int ROWS, int COLUMNS)
        {
            //This clears the console so all they see is the map and options to pick
            Console.Clear();
            //Variables we need for the user input
            String UserInput, UserInputX, UserInputY;
            int x, y;

            //These lines are the output that pront the user what they should do
            Console.WriteLine("   Minesweeper map: ");
            Console.WriteLine("   Size of Gride: " + (ROWS * COLUMNS));
            Console.WriteLine("   Number of Bombs: " + NUMBEROFBOMBS);
            MapDisplay(PlayerMap, ROWS, COLUMNS);
            Console.WriteLine("\nWould you like to designate a square as safe or designate a square as a bomb?");
            Console.WriteLine("Enter 1 to designate a square as safe or 2 to designate a square as a bomb.");
            Console.WriteLine("Enter 'cheatcode' to win automatically or 'displaymap' to display the computer's map including bombs");
            Console.WriteLine("Enter '3bv' to get the 3bv value for this map - which is the minimum amount of clicks it would take to win.");
            Console.WriteLine("(Enter 3 to quit.)");

            //This line accepts the user input and puts it into "UserInput"
            UserInput = Console.ReadLine();

            //If the user says they want to designate a square as safe, do this:
            if (UserInput == "1")
            {
                Console.WriteLine("Designating a square as safe: ");
                Console.WriteLine("Please enter the X coordinate: ");
                UserInputX = Console.ReadLine();
                int value;
                if (int.TryParse(UserInputX, out value))
                { x = int.Parse(UserInputX); }
                else { return; }
                Console.WriteLine("Please enter the Y coordinate: ");
                UserInputY = Console.ReadLine();
                int value2;
                if (int.TryParse(UserInputY, out value2)) { y = int.Parse(UserInputY); }
                else { return; }
                Reveal(ComputerMap, PlayerMap, x, y, ROWS, COLUMNS);
            }
            //If the user says they want to designate a square as a bomb, do this:
            else if (UserInput == "2")
            {
                //Prompts the user for an X coordinate
                Console.WriteLine("Designating a square as a bomb: ");
                Console.WriteLine("Please enter the X coordinate: ");
                UserInputX = Console.ReadLine();
                int value;
                if (int.TryParse(UserInputX, out value))
                { x = int.Parse(UserInputX); }
                else { return; }
                Console.WriteLine("Please enter the Y coordinate: ");
                UserInputY = Console.ReadLine();
                int value2;
                if (int.TryParse(UserInputY, out value2)) { y = int.Parse(UserInputY); }
                else { return; }
                //This method "DeclareBomb" just designates a square as a bomb on the player's map.
                DeclareBomb(PlayerMap, x, y);
            }
            //If the user says they want to quit do this:
            else if (UserInput == "3")
            {
                Console.WriteLine("Thank you for playing!");
                Console.ReadLine();
                Environment.Exit(0);
            }

            else if (UserInput == "cheatcode")
            {
                Console.WriteLine();
                CheatCode(ComputerMap, PlayerMap, ROWS, COLUMNS);
                Console.WriteLine();
                MapDisplay(PlayerMap, ROWS, COLUMNS);
                Console.WriteLine();
            }

            else if (UserInput == "displaymap")
            {
                Console.WriteLine();
                MapDisplay(ComputerMap, ROWS, COLUMNS);
                Console.WriteLine();
                Console.ReadLine();
            }
            else if (UserInput == "3bv")
            {
                Console.WriteLine();
                Console.WriteLine(return3bv(ComputerMap, ROWS, COLUMNS));
                Console.WriteLine();
                Console.ReadLine();
            }
            //If the user enters invalid input then just restart the loop.
            else
            {
                return;
            }
            CheckWinCondition(ComputerMap, PlayerMap, ROWS, COLUMNS);
        }

        public static void CreateGame(int NUMBEROFBOMBS, int ROWS, int COLUMNS, int DIFFICULTY)
        {
            int[] checkDifficulty = difficulty(NUMBEROFBOMBS, ROWS, COLUMNS);
            bool readyMap = false;
            char[,] NPCMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
            char[,] TestMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
            char[,] PlayerMap = EmptyDisplayMapGenerator(ROWS, COLUMNS);

            do
            {
                TestMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
                readyMap = selectDifficulty(DIFFICULTY, checkDifficulty, return3bv(TestMap, ROWS, COLUMNS));
            } while (readyMap == false);

            NPCMap = TestMap;

            while (true)
            {
                PlayerInput(NPCMap, PlayerMap, NUMBEROFBOMBS, ROWS, COLUMNS);
            }
        }

        public static char[,] createPictureGame(int NUMBEROFBOMBS, int ROWS, int COLUMNS, int DIFFICULTY)
        {
            int[] checkDifficulty = difficulty(NUMBEROFBOMBS, ROWS, COLUMNS);
            bool readyMap = false;
            char[,] NPCMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
            char[,] TestMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
            char[,] PlayerMap = EmptyDisplayMapGenerator(ROWS, COLUMNS);

            do
            {
                TestMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
                readyMap = selectDifficulty(DIFFICULTY, checkDifficulty, return3bv(TestMap, ROWS, COLUMNS));
            } while (readyMap == false);

            return TestMap;
        }

        public static void PlayGame()
        {
            //variables I need
            int HOWMANYBOMBS = 0;
            int ROWS = 0;
            int COLUMNS = 0;
            int DIFFICULTY = 0;
            String userInput;
            bool bombs = true;
            bool rows = true;
            bool columns = true;
            bool difficulty = true;
            //bool rowscolumnsbombs = true;
            int value;

            Console.WriteLine("Hello let's play Minesweeper!");
            while (rows)
            {
                Console.WriteLine("How many rows would you like?");
                userInput = Console.ReadLine();
                if (int.TryParse(userInput, out value) && (int.Parse(userInput) > 0))
                {
                    ROWS = int.Parse(userInput);
                    rows = false;
                }
                else
                {
                    Console.WriteLine("Invalid value");
                }
            }

            while (columns)
            {
                Console.WriteLine("How many columns would you like?");
                userInput = Console.ReadLine();
                if (int.TryParse(userInput, out value) && (int.Parse(userInput) > 0))
                {
                    COLUMNS = int.Parse(userInput);
                    columns = false;
                }
                else
                {
                    Console.WriteLine("Invalid value");
                }
            }
            while (bombs)
            {
                Console.WriteLine("How many bombs would you like?");
                userInput = Console.ReadLine();
                if (int.TryParse(userInput, out value) && (int.Parse(userInput) > 0) && (int.Parse(userInput) < (COLUMNS * ROWS)))
                {
                    HOWMANYBOMBS = int.Parse(userInput);
                    bombs = false;
                }
                else
                {
                    Console.WriteLine("Invalid value");
                }
            }
            while (difficulty)
            {
                Console.WriteLine("What difficulty would you like?");
                Console.WriteLine("0 for Easy, 1 for Normal, 2 for Hard, 3 for Legendary Difficulty.");
                userInput = Console.ReadLine();
                if (int.TryParse(userInput, out value) && (int.Parse(userInput) >= 0) && (int.Parse(userInput) < 4))
                {
                    DIFFICULTY = int.Parse(userInput);
                    difficulty = false;
                }
                else
                {
                    Console.WriteLine("Invalid value");
                }
            }
            CreateGame(HOWMANYBOMBS, ROWS, COLUMNS, DIFFICULTY);
        }

        //calculates a 3BV once
        public static int threebeevee(int NUMBEROFBOMBS, int ROWS, int COLUMNS)
        {
            //numberOfClicks tracks the MINIMUM(3BV) number of clicks it would take to win a specific map
            int numberOfClicks = 0;
            //This is the map that has the bombs, numbers, and empty squares
            char[,] NPCMap = ComputerMap(NUMBEROFBOMBS, ROWS, COLUMNS);
            //This is an empty map that our computer will solve for the computer's map using the minimum amount of clicks
            char[,] TestMap = EmptyDisplayMapGenerator(ROWS, COLUMNS);

            //This HUGE loop goes through our test map and reveals all the bombs, empty squares, the numbers connected to empty squares
            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++)
                {
                    //If the player's map is currently empty THEN we'll check it
                    if (TestMap[y, x] == ' ')
                    {
                        if (NPCMap[y, x] == '-')
                        {
                            //If it's an empty square we'll use "reveal" and then increment the counter once
                            Reveal(NPCMap, TestMap, x, y, ROWS, COLUMNS);
                            numberOfClicks++;
                        }
                        //if it's a bomb we'll update the testmap but not increment the clicks
                        else if (NPCMap[y, x] == 'X')
                        {
                            TestMap[y, x] = 'X';
                        }
                        else
                        {
                            //If it's a number we do nothing at first since it may get revealed by the blank squares.
                            //In the next phase after we've revealed all the bombs we'll count all the empty squares to get an accurate count.
                        }
                    }
                }
            }
            //By this point our map has revealed every bomb, empty square, and number, so every remaining square is a number which increments
            //our number of clicks.  
            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++)
                {
                    if (TestMap[y, x] == ' ')
                    {
                        numberOfClicks++;
                    }
                }
            }

            return numberOfClicks;
        }

        public static int return3bv(char[,] NPCMap, int ROWS, int COLUMNS)
        {
            //numberOfClicks tracks the MINIMUM(3BV) number of clicks it would take to win a specific map
            int numberOfClicks = 0;
            //This is an empty map that our computer will solve for the computer's map using the minimum amount of clicks
            char[,] TestMap = EmptyDisplayMapGenerator(ROWS, COLUMNS);

            //This HUGE loop goes through our test map and reveals all the bombs, empty squares, the numbers connected to empty squares
            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++)
                {
                    //If the player's map is currently empty THEN we'll check it
                    if (TestMap[y, x] == ' ')
                    {
                        if (NPCMap[y, x] == '-')
                        {
                            //If it's an empty square we'll use "reveal" and then increment the counter once
                            Reveal(NPCMap, TestMap, x, y, ROWS, COLUMNS);
                            numberOfClicks++;
                        }
                        //if it's a bomb we'll update the testmap but not increment the clicks
                        else if (NPCMap[y, x] == 'X')
                        {
                            TestMap[y, x] = 'X';
                        }
                        else
                        {
                            //If it's a number we do nothing at first since it may get revealed by the blank squares.
                            //In the next phase after we've revealed all the bombs we'll count all the empty squares to get an accurate count.
                        }
                    }
                }
            }
            //By this point our map has revealed every bomb, empty square, and number, so every remaining square is a number which increments
            //our number of clicks.  
            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++)
                {
                    if (TestMap[y, x] == ' ')
                    {
                        numberOfClicks++;
                    }
                }
            }

            return numberOfClicks;
        }

        /*For the difficulty I decided to calculate difficulty by doing this:
         * FIRST: I'd get sample data of 100 randomly generated 3bv scores for maps with that
         * many bombs, rows, and columns.  
         * NEXT: I'd sort the array with the 100 sample values.  
         * FOR EASY: I'd use the array value of the lower quartile or 24th value and lower 3bv values for easy
         * FOR NORMAL: I'd use the array value of the lower quartile(24th value) through the median or 49th value for NORMAL
         * FOR HARD: I'd use the array value of the median(49th value) through the upper quartile(74th value) for HARD
         * FOR VERY HARD: I'd use the array value of the upper quartile(74th value) and above for LEGENDARY
         * After calculating the difficulty I'll have my method return an array with the lower quartile, median, and upper quartile
         * From there I can have another method ensure that the player gets a map with the correct difficulty.  
         */
        public static int[] difficulty(int NUMBEROFBOMBS, int ROWS, int COLUMNS)
        {
            int[] sampleData = new int[100];
            for (int i = 0; i < 100; i++)
            {
                sampleData[i] = threebeevee(NUMBEROFBOMBS, ROWS, COLUMNS);
            }
            Array.Sort(sampleData);

            int[] difficulty3BV = new int[3] { sampleData[24], sampleData[49], sampleData[74] };
            return difficulty3BV;
        }

        //All this method does is check if the map's 3bv qualifies to be a map for this difficulty.  
        public static bool selectDifficulty(int difficultyChoice, int[] difficulty, int threebeevee)
        {
            //Easy
            if (difficultyChoice == 0)
            {
                if (threebeevee <= difficulty[0])
                {
                    return true;
                }
                else { return false; }
            }
            //normal
            else if (difficultyChoice == 1)
            {
                if (threebeevee >= difficulty[0] && threebeevee <= difficulty[1])
                {
                    return true;
                }
                else { return false; }
            }
            //hard
            else if (difficultyChoice == 2)
            {
                if (threebeevee >= difficulty[1] && threebeevee < difficulty[2])
                {
                    return true;
                }
                else { return false; }
            }
            //legendary
            else if (difficultyChoice == 3)
            {
                if (threebeevee >= difficulty[2])
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }
        
        //This is the MAIN, it just calls our methods to play the game
        /*
        public static void Main(string[] args)
        {
            PlayGame();
        }
         */

        public void newGridGame()
        {
            Console.WriteLine("This will create a new game");
        }

/*
 *THIS IS THE END
 *OF THE CONSOLE PART OF THE PROGRAM 
 */
        public void createPlaceholder()
        {
            this.HUD = new Grid();
            HUD.Width = 320;
            HUD.Height = 80;
            HUD.HorizontalAlignment = HorizontalAlignment.Center;
            HUD.VerticalAlignment = VerticalAlignment.Top;

            for (int i = 0; i < 4; i++)
            {HUD.ColumnDefinitions.Add(new ColumnDefinition());}
            for (int j = 0; j < 1; j++)
            {HUD.RowDefinitions.Add(new RowDefinition());}

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    Button HUDbutton = new Button();
                    HUDbutton.Content = "placeholder";
                    if (i == 0){
                        HUDbutton.Content = "EASY";
                        HUDbutton.Click += easyButton;
                    }
                    else if (i == 1){
                        HUDbutton.Content = "MEDIUM";
                        HUDbutton.Click += mediumButton;
                    }
                    else if (i == 2){
                        HUDbutton.Content = "HARD";
                        HUDbutton.Click += hardButton;
                    }
                    else if (i == 3){
                        HUDbutton.Content = "RESET";
                        HUDbutton.Click += ButtonClick2;
                    }
                    else{HUDbutton.Content = "placeholder";}
                    Grid.SetColumn(HUDbutton, i);
                    Grid.SetRow(HUDbutton, j);
                    HUD.Children.Add(HUDbutton);
                }
            }
        }

        public void updateBombCounter(){
            this.HUD = new Grid();
            this.HUD.Width = 320;
            this.HUD.Height = 80;
            this.HUD.HorizontalAlignment = HorizontalAlignment.Center;
            this.HUD.VerticalAlignment = VerticalAlignment.Top;

            for (int i = 0; i < 4; i++)
            { this.HUD.ColumnDefinitions.Add(new ColumnDefinition()); }
            for (int j = 0; j < 1; j++)
            { this.HUD.RowDefinitions.Add(new RowDefinition()); }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    Button HUDbutton = new Button();
                    HUDbutton.Content = "placeholder";
                    if (i == 0) { HUDbutton.Content = "EASY"; }
                    else if (i == 1) { HUDbutton.Content = "00:01"; }
                    else if (i == 2)
                    {
                        HUDbutton.Content = "RESET";
                        HUDbutton.Click += ButtonClick2;
                    }
                    else if (i == 3)
                    {
                        HUDbutton.Content = this.BOMBS;
                    }
                    else { HUDbutton.Content = "placeholder"; }
                    Grid.SetColumn(HUDbutton, i);
                    Grid.SetRow(HUDbutton, j);
                    HUD.Children.Add(HUDbutton);
                }
            }
        }

        public Grid createFilledMap(char[,] gameGrid)
        {
            Grid grid = new Grid();
            grid.Width = 320;
            grid.Height = 240;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;

            // i is column
            for (int i = 0; i < 10; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            // j is rows
            for (int j = 0; j < 8; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();
                    //button.Content = "*";
                    button.Content = gameGrid[j, i];
                    button.Click += ButtonClick;
                    button.MouseRightButtonUp += RightClick;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    grid.Children.Add(button);
                }
            }
            return grid;
        }

        public void createEmptyMap(char[,] gameGrid)
        {
            playerGrid = new Grid();
            playerGrid.Width = 320;
            playerGrid.Height = 240;
            playerGrid.HorizontalAlignment = HorizontalAlignment.Center;
            playerGrid.VerticalAlignment = VerticalAlignment.Center;

            // i is column
            for (int i = 0; i < 10; i++)
            {
                playerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            // j is rows
            for (int j = 0; j < 8; j++)
            {
                playerGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();
                    button.Content = " ";
                    //button.Content = gameGrid[j, i];
                    button.Click += ButtonClick;
                    button.MouseRightButtonUp += RightClick;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    playerGrid.Children.Add(button);
                }
            }
        }

        public void updateMap()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();
                    button.Content = playerMap[j, i];
                    if (playerMap[j, i] == '-')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else if (playerMap[j, i] == '1')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.Black);
                    }else if(playerMap[j, i] == '2'){
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.PeachPuff);
                    }
                    else if (playerMap[j, i] == '3')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.Brown);
                    }
                    else if (playerMap[j, i] == '4')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.IndianRed);
                    }
                    else if (playerMap[j, i] == '5')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else if (playerMap[j, i] == '6')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.Orange);
                    }
                    else if (playerMap[j, i] == '7')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.Pink);
                    }
                    else if (playerMap[j, i] == '8')
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        button.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (playerMap[j, i] == 'X')
                    {
                        button.Background = new SolidColorBrush(Colors.Orange);
                        button.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    button.Click += ButtonClick;
                    button.MouseRightButtonUp += RightClick;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    playerGrid.Children.Add(button);
                }
            }
        }

        //CODENAME SOREASAN
        public void resetTheMap()
        {
            this.BOMBS = 5;
            updateBombCounter2();
            this.timer = 0;
            this.hiddenMap = createPictureGame(this.BOMBS, this.ROWS, this.COLUMNS, 0);
            this.playerMap = EmptyDisplayMapGenerator(this.ROWS, this.COLUMNS);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();
                    button.Content = playerMap[j, i];
                    button.Click += ButtonClick;
                    button.MouseRightButtonUp += RightClick;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    playerGrid.Children.Add(button);
                }
            }
        }

        public void generateDifficulty(int difficulty)
        {
            if (difficulty == 0)
            {
                this.BOMBS = 5;
                this.DIFFICULTY = 0;
            }
            else if (difficulty == 1)
            {
                this.BOMBS = 10;
                this.DIFFICULTY = 1;
            }
            else if (difficulty == 2)
            {
                this.BOMBS = 15;
                this.DIFFICULTY = 1;
            }
            else
            {
                this.BOMBS = 20;
                this.DIFFICULTY = 1;
            }

            //this stuff happens every time
            updateBombCounter2();
            this.timer = 0;
            this.hiddenMap = createPictureGame(this.BOMBS, this.ROWS, this.COLUMNS, this.DIFFICULTY);
            this.playerMap = EmptyDisplayMapGenerator(this.ROWS, this.COLUMNS);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();
                    button.Content = playerMap[j, i];
                    button.Click += ButtonClick;
                    button.MouseRightButtonUp += RightClick;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    playerGrid.Children.Add(button);
                }
            }
        }

        
        /// <summary>
        /// This removes the ability for buttons to do stuff. Used when you lose the game usually.  
        /// </summary>
        public void deactivateMap()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = new Button();
                    if (hiddenMap[j, i] == 'X')
                    {
                        button.Content = 'X';
                        button.Background = new SolidColorBrush(Colors.Orange);
                        button.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        button.Content = playerMap[j, i];
                        if (playerMap[j, i] == '-')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else if (playerMap[j, i] == '1')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.Black);
                        }
                        else if (playerMap[j, i] == '2')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.PeachPuff);
                        }
                        else if (playerMap[j, i] == '3')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.Brown);
                        }
                        else if (playerMap[j, i] == '4')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.IndianRed);
                        }
                        else if (playerMap[j, i] == '5')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        }
                        else if (playerMap[j, i] == '6')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.Orange);
                        }
                        else if (playerMap[j, i] == '7')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.Pink);
                        }
                        else if (playerMap[j, i] == '8')
                        {
                            button.Background = new SolidColorBrush(Colors.White);
                            button.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else if (playerMap[j, i] == 'X')
                        {
                            button.Background = new SolidColorBrush(Colors.Orange);
                            button.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                    //button.Content = gameGrid[j, i];
                    //button.Click += ButtonClick;
                    //button.MouseRightButtonUp += RightClick;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    playerGrid.Children.Add(button);
                }
            }
        }



        public void lose()
        {
            MediaPlayer mplayer = new MediaPlayer();
            mplayer.Open(new Uri(@"C:\Users\Kenneth\Desktop\Summer 2015\CS 3280 C#\Assignment5Minesweeper\Assignment5Minesweeper\explosion.wav", UriKind.Relative));
            mplayer.Play();
            MessageBox.Show("You lose!");
        }

        public void CheckWinConditionGUI()
        {
            //This first loop is looping through the ROWS (y value), 5 rows
            for (int y = 0; y < this.ROWS; y++)
            {
                //This second loop is looping through the COLUMNS (x value), 10 columns
                for (int x = 0; x < this.COLUMNS; x++)
                {
                    //If they are both equal, keep going and see if we win
                    if (this.hiddenMap[y, x] == this.playerMap[y, x])
                    {
                        //Do nothing, but keep looping and see if we win
                    }
                    else if (this.hiddenMap[y, x] == 'X' && this.playerMap[y, x] == ' ')
                    {
                        //If the computer map has a bomb at a spot and the player has an empty square we keep checking to see if the player won
                    }
                    else
                    {
                        return;
                    }
                }
            }
            //If every square in the player matrix and the NPC matrix are the same you win
            //System.Media.SystemSounds.Beep.Play();
            MediaPlayer mplayer = new MediaPlayer();
            mplayer.Open(new Uri(@"..\..\win.wav", UriKind.Relative));
            mplayer.Play();
            //MessageBox.Show("You lose!");
            MessageBox.Show("You won in " + timer + " seconds!");
            deactivateMap();
        }


        //Left click on grid buttons
        public void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int col = Grid.GetColumn(button);
            int row = Grid.GetRow(button);
            //button.Content = this.hiddenMap[row, col];
            //Console.WriteLine("L: {0} x {1}", col, row);
            if (this.hiddenMap[row, col] == 'X')
            {
                //System.Media.SystemSounds.Beep.Play();
                MediaPlayer mplayer = new MediaPlayer();
                mplayer.Open(new Uri(@"..\..\explosion.wav", UriKind.Relative));
                mplayer.Play();
                MessageBox.Show("You lost in " + timer + " seconds!");
                deactivateMap();
            }
            else if (this.hiddenMap[row, col] == '-' && this.playerMap[row, col] == ' ')
            {
                CheckAllNearbySafeSquares(this.hiddenMap, this.playerMap, col, row, 8, 10);
                updateMap();
            }
            else
            {
                this.playerMap[row, col] = this.hiddenMap[row, col];
                updateMap();
                InitializeComponent();
            }
            CheckWinConditionGUI();
        }

        //Rightclick on grid buttons
        public void RightClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int col = Grid.GetColumn(button);
            int row = Grid.GetRow(button);
            //Console.WriteLine("R: {0} x {1}", col, row);
            if (playerMap[row, col] == 'X')
            {
                playerMap[row, col] = ' ';
                this.BOMBS = this.BOMBS + 1 ;
                //updateBombCounter();
                updateBombCounter2();
                updateMap();
                //button.Content = " ";
            }
            else if (button.Content.Equals('-')||button.Content.Equals('1')||button.Content.Equals('2')||button.Content.Equals('3')||button.Content.Equals('4')||button.Content.Equals('5')||button.Content.Equals('6') || button.Content.Equals('7')||button.Content.Equals('8'))
            {/*Do nothing since it's a number or revealed*/}
            else{
                playerMap[row, col] = 'X';
                this.BOMBS = this.BOMBS - 1;
                updateBombCounter2();
                updateMap();
                //button.Content = "X";
            }
        }

        public void ButtonClick2(object sender, RoutedEventArgs e)
        {
            //Button button = sender as Button;
            resetTheMap();
        }
        
        public void easyButton(object sender, RoutedEventArgs e)
        {
            generateDifficulty(0);
        }
        public void mediumButton(object sender, RoutedEventArgs e)
        {
            generateDifficulty(1);
        }
        public void hardButton(object sender, RoutedEventArgs e)
        {
            generateDifficulty(2);
        }

        

        public void newGame(){
            this.ROWS = 8;
            this.COLUMNS = 10;
            this.BOMBS = 5;
            this.timer = 0;
            this.hiddenMap = createPictureGame(this.BOMBS, this.ROWS, this.COLUMNS, 0);
            this.playerMap = EmptyDisplayMapGenerator(this.ROWS, this.COLUMNS);
            InitializeComponent();
            createPlaceholder();
            createEmptyMap(gameGrid);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_tick;
            timer.Start();

            textbox = new TextBox(){};

            this.timer2 = new System.Windows.Threading.DispatcherTimer();
            this.timer2.Tick += new EventHandler(timer_tick2);
            this.timer2.Interval = new TimeSpan(0, 0, 1);
            this.timer2.Start();

            textbox2 = new TextBox() { };

            textbox3 = new TextBox() {
                Text = (BOMBS + " bombs left")
            };

            myArea.Children.Add(textbox);
            myArea.Children.Add(textbox2);
            myArea.Children.Add(textbox3);
            myArea.Children.Add(HUD);
            myArea.Children.Add(playerGrid);
        }

        public void resetGame()
        {
            this.BOMBS = 5;
            updateBombCounter2();
            this.timer = 0;
            //this.timer2.Start();
            this.hiddenMap = createPictureGame(this.BOMBS, this.ROWS, this.COLUMNS, 0);
            this.playerMap = EmptyDisplayMapGenerator(this.ROWS, this.COLUMNS);
            InitializeComponent();
            createPlaceholder();
            createEmptyMap(gameGrid);
            updateMap();
        }

        public void timer_tick(object sender, EventArgs e)
        {
            this.textbox.Text = "Current time: " + DateTime.Now.ToLongTimeString();
        }

        public void timer_tick2(object sender, EventArgs e)
        {
            this.timer++;
            this.textbox2.Text = "Time Elapsed: " + this.timer;
                //"" + DateTime.Now.Second;
        }

        public void updateBombCounter2()
        {
            textbox3.Text = (this.BOMBS + " bombs left");
        }

        public MainWindow()
        {
            newGame();
        }//End of Main Window
    }//END OF MY GIANT MAINWINDOW CLASS
}//End of Assignment 4 Namespace