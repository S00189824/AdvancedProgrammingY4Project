using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AdvProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        List<int> playerGuesses = new List<int>();
        List<int> aiGuesses = new List<int>();

        SolidColorBrush noSelect = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        SolidColorBrush select = new SolidColorBrush(Color.FromRgb(0, 200, 255));

        public Players p1 = new Players(0, "Player 1");
        public Players p2 = new Players(0, "Player 2");

        int guessCount = 3;
        Thread t1;
        Thread t2;


        static SaveStorage saveStorage;

        public object locker = new object();

        [ThreadStatic()]
        static int index = 0;

        int score = 0;
        bool lose = false;

        public MainWindow()
        {
            InitializeComponent();

            //prevents buttons from being pressed without disabling them
            btnRed.IsHitTestVisible = false;
            btnBlue.IsHitTestVisible = false;
            btnGreen.IsHitTestVisible = false;

            btnRed.Focusable = false;
            btnBlue.Focusable = false;
            btnGreen.Focusable = false;

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            ThreadStart ts = new ThreadStart(AIColourSelection);
            t1 = new Thread(ts);


            ThreadStart fs = new ThreadStart(FlashingSequence);
            t2 = new Thread(fs);

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int progress;

            foreach (var x in playerGuesses)
            {
                for (int i = 0; i < 3; i++)
                {
                    if(playerGuesses == aiGuesses)
                    {
                        progressbar.Value = x;
                        progressbar.Value += playerGuesses[i];
                    }
                }
            }
        }

        private void FlashingSequence()
        {
            Monitor.Enter(locker);

            try
            {
                while (index < 3)
                {
                    if (aiGuesses[index] == 0)
                    {
                        Dispatcher.Invoke(() => btnRed.BorderBrush = select);
                    }
                    else if (aiGuesses[index] == 1)
                    {
                        Dispatcher.Invoke(() => btnBlue.BorderBrush = select);
                    }
                    else if (aiGuesses[index] == 2)
                    {
                        Dispatcher.Invoke(() => btnGreen.BorderBrush = select);
                    }
                    index++;
                    Thread.Sleep(1000);

                    Dispatcher.Invoke(() => btnRed.BorderBrush = noSelect);
                    Dispatcher.Invoke(() => btnGreen.BorderBrush = noSelect);
                    Dispatcher.Invoke(() => btnBlue.BorderBrush = noSelect);

                    Thread.Sleep(1000);

                    Monitor.Pulse(locker);
                    Monitor.Wait(locker);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Dispatcher.Invoke(() => btnRed.IsHitTestVisible = true);
                Dispatcher.Invoke(() => btnBlue.IsHitTestVisible = true);
                Dispatcher.Invoke(() => btnGreen.IsHitTestVisible = true);

                Dispatcher.Invoke(() => btnRed.Focusable = true);
                Dispatcher.Invoke(() => btnBlue.Focusable = true);
                Dispatcher.Invoke(() => btnGreen.Focusable = true);


                Monitor.Pulse(locker);
                Monitor.Exit(locker);
            }

        }

        void CheckGuess()
        {
            for (int i = 0; i < guessCount; i++)
            {
                if (playerGuesses[i] != aiGuesses[i])
                {
                    //show messagebox saying wrong guess
                    //save the score to iso storage
                    //reset game and score
                    lose = true;
                    break;
                }
            }

            if(lose)
            {

                MessageBox.Show("You lose");
            }
            else
            {
                MessageBox.Show("Correct");
                score++;
                txtScore.Content = "Score : " + score;
            }

            btnControlTurn.Content = "Start Game";

        }
        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            playerGuesses.Add(0);
            CheckGuessLimit();
        }

        private void btnBlue_Click(object sender, RoutedEventArgs e)
        {
            playerGuesses.Add(1);
            CheckGuessLimit();
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            playerGuesses.Add(2);
            CheckGuessLimit();
        }

        void CheckGuessLimit()
        {
            if (playerGuesses.Count >= 3)
            {
                btnRed.IsHitTestVisible = false;
                btnBlue.IsHitTestVisible = false;
                btnGreen.IsHitTestVisible = false;

                btnRed.Focusable = false;
                btnBlue.Focusable = false;
                btnGreen.Focusable = false;

                btnControlTurn.IsEnabled = true;
                btnControlTurn.Content = "Check";
            }
        }

        public void AIColourSelection()
        {
            Monitor.Enter(locker);

            try
            {
                while (index < 3)
                {
                    Random rng = new Random();
                    aiGuesses.Add(rng.Next(0, 3));
                    index++;

                    Thread.Sleep(1000);

                    Monitor.Pulse(locker);
                    Monitor.Wait(locker);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Monitor.Pulse(locker);
                Monitor.Exit(locker);
            }
        }

        private void btnControlTurn_Click(object sender, RoutedEventArgs e)
        {


            if (playerGuesses.Count >= 3)
            {
                CheckGuess();
                playerGuesses.Clear();
            }
            else
            {
                aiGuesses.Clear();
                btnControlTurn.IsEnabled = false;

                ThreadStart ts = new ThreadStart(AIColourSelection);
                t1 = new Thread(ts);

                ThreadStart fs = new ThreadStart(FlashingSequence);
                t2 = new Thread(fs);

                t1.Start();
                Thread.Sleep(1000);
                t2.Start();
            }

        }

        private void WriteToStorage()
        {

        }
    }

    public class Players
    {
        string player;
        int playerScore = 0;

        public Players(int player_Score, string Player)
        {
            player = Player;
            playerScore = player_Score;
        }
    }
}
