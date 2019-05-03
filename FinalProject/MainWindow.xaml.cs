using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using System;

/// <summary>
/// Created by Daniel King Spring 2019 
/// </summary>
namespace FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int turnCount = 0;
        int carrierCount = 5;
        int battleshipCount = 4;
        int submarineCount = 3;
        int destroyerCount = 3;
        int patrolBoatCount = 2;
        string playerName;
        Game game;
        Timer timer;
        bool isNormalGameMode = true;
        public MainWindow()
        {
            InitializeComponent();
            //make sure our game will never get cut off
            this.MinWidth = 850;
            this.MinHeight = 500;
            this.Width = 850;
            this.Height = 500;
            game = new Game(FiringGrid); // create gameboard
            timer = new Timer(500); //create timer for time attack
            TurnCounterTextBox.Text = turnCount.ToString();
            SubmitBtn.IsEnabled = false;
            //hide buttons until needed
            TimeAttackBtn.Visibility = Visibility.Hidden; 
            PlayAgainButton.Visibility = Visibility.Hidden;
        }

        private DateTime TimerStart { get; set; }

        private void Square_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set sender to square chosen
            Grid square = (Grid)sender;

            switch (square.Tag.ToString())
            {
                case "water":
                    square.Tag = "miss";
                    square.Background = new SolidColorBrush(Colors.DeepSkyBlue);
                    
                    if(isNormalGameMode)
                    {
                        turnCount++;
                        TurnCounterTextBox.Text = turnCount.ToString();
                    }
                    return;
                case "miss": //if the user clicks on something that has already been hit or missed we don't do anything
                case "hit":
                    return;
                case "carrier":
                    carrierCount--;
                    break;
                case "battleship":
                    battleshipCount--;
                    break;
                case "destroyer":
                    destroyerCount--;
                    break;
                case "submarine":
                    submarineCount--;
                    break;
                case "patrolBoat":
                    patrolBoatCount--;
                    break;
            }
            square.Tag = "hit";
            square.Background = new SolidColorBrush(Colors.Red);
            if (isNormalGameMode)
            {
                turnCount++;
                TurnCounterTextBox.Text = turnCount.ToString();
            }
            checkVictoryConditions();
        }
        
        private void checkVictoryConditions()
        {
            //Note: Have to set to -1 otherwise we keep displaying 'You sunk my ...' 
            if (carrierCount == 0)
            {
                carrierCount = -1;
                MessageBox.Show("You sunk my Aircraft Carrier!");
            }
            if (patrolBoatCount == 0)
            {
                patrolBoatCount = -1;
                MessageBox.Show("You sunk my Patrol Boat!");
            }
            if (destroyerCount == 0)
            {
                destroyerCount = -1;
                MessageBox.Show("You sunk my Destroyer!");
            }
            if (battleshipCount == 0)
            {
                battleshipCount = -1;
                MessageBox.Show("You sunk my Battleship!");
            }
            if (submarineCount == 0)
            {
                submarineCount = -1;
                MessageBox.Show("You sunk my Submarine!");
            }

            //victory conditions
            if (carrierCount == -1 && battleshipCount == -1 && submarineCount == -1 && patrolBoatCount == -1 && destroyerCount == -1) 
            {
                timer.Stop();
                var finalTime = (int)(DateTime.Now - TimerStart).TotalSeconds;
                if (string.IsNullOrWhiteSpace(playerName))
                    playerName = "Player1"; //default name to player1 if they don't input one

                //Update highscore block based on gameType and notify user of victory
                HighScoreTextBlock.Content += playerName + "\n";
                if (isNormalGameMode)
                {
                    TurnColumn.Content += turnCount.ToString() + " turns\n";
                    MessageBox.Show("You won in " + turnCount + " turns");
                } else
                {
                    TurnColumn.Content += finalTime + " sec\n";
                    MessageBox.Show("You won in " + finalTime + " seconds");
                    TimeAttackBtn.IsEnabled = false;
                }
                PlayAgainButton.Visibility = Visibility.Visible; 
                game.DisableGameboard(); //don't allow player to keep clicking the board
            }
        }

        /// <summary>
        /// Reset the gameboard with new locations for the ships 
        /// </summary>
        private void Reset()
        {
            turnCount = 0;
            TurnCounterTextBox.Text = turnCount.ToString();
            carrierCount = 5;
            battleshipCount = 4;
            submarineCount = 3;
            destroyerCount = 3;
            patrolBoatCount = 2;
            game = new Game(FiringGrid);

            foreach (var element in game.gameBoard)
            {
                element.Background = new SolidColorBrush(Colors.LightSlateGray);
                element.IsEnabled = true;
            }
        }

        private void RegularGame_Click(object sender, RoutedEventArgs e)
        {
            isNormalGameMode = true;
            timer.Stop(); //make sure that if they switched from time attack mid game that the timer isn't still going
            TimeAttackBtn.Visibility = Visibility.Hidden;
            ScoreLabel.Text = "Turns";
            Reset();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("  GAME PLAY \n" +
                "This game is similar to how you would play BattleShip. The grid in the middle has 5 different ships ranging in size from 2-5 squares that are hidden at the start." +
                " An aircraft carrier is 5 squares long, a battleship is 4 squares long, both a submarine and a destroyer are three squares long and a patrol boat is two squares long." +
                " The computer will randomly select different positions on the board for the ships. The user will pick a square at random to fire on. " +
                "If the AI has any part of one of its ship on it, it is a hit and will be marked on their grid which part has been hit in red. If it is a miss, it will be marked in blue." +
                " If the user successfully locates all of the ships by hitting each square they occupy they have won as all ships have been destroyed. \n" +
                "  OBJECTIVE\n" + 
                "Take as few turns as possible to hit all the ships. The lower your turn count when you win, the better!\n" +
                "  ALTERNATE GAME MODE: TIME ATTACK\n" +
                "In this mode you battle against the clock to see how fast you can locate all the boats. \n " +
                " HIGHSCORES \n" +
                "You can add your name to the high score board by entering it into the name box or scores will automatically update under the name Player1 \n\n" +
                "This game was developed by Daniel King as a final project in IT 4400", "About SplashDown"
                );
        }

        private void AddName_Click(object sender, RoutedEventArgs e)
        {
            playerName = NameInput.Text;
            MessageBox.Show("You have successfully updated your player name.", "Success");
            NameInput.Text = "";
            SubmitBtn.IsEnabled = false;
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitBtn.IsEnabled = true;
        }

        /// <summary>
        /// Sets board up for a new game of time attack
        /// </summary>
        private void TimeAttackGame_Click(object sender, RoutedEventArgs e)
        {
            TimeAttackBtn.IsEnabled = true;
            TimeAttackBtn.Visibility = Visibility.Visible;
            ScoreLabel.Text = "SECONDS";
            isNormalGameMode = false;
            Reset();
            game.DisableGameBeforeStart();
        }

        /// <summary>
        /// Starts the game of Time Attack.
        /// </summary>
        private void TimeAttackBtn_Click(object sender, RoutedEventArgs e)
        {
            game.EnableOnStart();
            TimerStart = DateTime.Now;
            timer.Start();
            timer.Elapsed += CountSeconds;
            TimeAttackBtn.IsEnabled = false; //don't want them to restart the timer
        }

        private void CountSeconds(object source, ElapsedEventArgs e)
        {
            var secondsElapsed = DateTime.Now - TimerStart;
            var wholeSeconds = (int)secondsElapsed.TotalSeconds;
            this.Dispatcher.Invoke(() =>
            {
                TurnCounterTextBox.Text = wholeSeconds.ToString();
            });
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            PlayAgainButton.Visibility = Visibility.Hidden;
            TimeAttackBtn.IsEnabled = true;
            if(!isNormalGameMode)
                game.DisableGameBeforeStart();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Dispose();
        }
    }
}
