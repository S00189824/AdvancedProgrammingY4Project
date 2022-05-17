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
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace AP_Project
{
    
    public partial class MainWindow : Window
    {
        //Create backgroundWorker
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        List<Task> tasks = new List<Task>();

        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Ellipse> removeThis = new List<Ellipse>();
        GameSelect gameSelectMenu = new GameSelect();

        int spawnRate = 20;
        int currentRate;
        int lastScore = 0;
        int health = 350;
        int posX;
        int posY;
        int score = 0;

        double growthRate = 0.6;

        Random rnd = new Random();

        MediaPlayer playClickSound = new MediaPlayer();
        MediaPlayer playPopSound = new MediaPlayer();
        

        Uri ClickedSound;
        Uri PopedSound;

        Brush brush;

        public MainWindow()
        {
            InitializeComponent();

            //background worker
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            

            ClickedSound = new Uri("Pack://siteoforigins:,,,/Sound/clickedpop.mp3");
            PopedSound = new Uri("Pack://siteoforigins:,,,/Sound/pop.mp3");

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            currentRate = spawnRate;

        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            txtLastScore.Content = "Last Score " + lastScore;

            currentRate = -2;

            if(currentRate < 1)
            {
                currentRate = spawnRate;

                posX = rnd.Next(15, 700);
                posY = rnd.Next(50, 350);

                brush = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));

                Ellipse circle = new Ellipse
                {
                    Tag = "circle",
                    Height = 10,
                    Width = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = brush
                };

                Canvas.SetLeft(circle, posX);
                Canvas.SetTop(circle, posY);

                MyCanvas.Children.Add(circle);
            }

            foreach (var x in MyCanvas.Children.OfType<Ellipse>())
            {
                x.Height += growthRate;
                x.Width += growthRate;
                x.RenderTransformOrigin = new Point(0.5, 0.5);

                if(x.Width > 70)
                {
                    removeThis.Add(x);
                    health -= 15;
                    playPopSound.Open(PopedSound);
                    playPopSound.Play();
                }
            }

            if(health > 1)
            {
                healthbar.Width = health;
            }
            else
            {
                //GameOverFunction();
                //gameSelectMenu.Show();
            }

            foreach (Ellipse item in removeThis)
            {
                MyCanvas.Children.Remove(item);
            }

            if(score > 5)
            {
                spawnRate = 20;
            }

            if(spawnRate > 15)
            {
                //spawnRate = 10;
                growthRate = 1.5;
            }
        }

        public void GameOverFunction()
        {
            gameTimer.Stop();

            MessageBox.Show("Game Over" + Environment.NewLine + "You Scored: " + score + Environment.NewLine + "Click ok to play next game");

            foreach (var y in MyCanvas.Children.OfType<Ellipse>())
            {
                removeThis.Add(y);
            }
            foreach (Ellipse item in removeThis)
            {
                MyCanvas.Children.Remove(item);
            }

            growthRate = .6;
            spawnRate = 40;
            health = 350;
            lastScore = score;
            score = 0;
            currentRate = 5;
            removeThis.Clear();

            gameTimer.Start();

            //MainWindow main = this;
            //main.Close();
        }

        private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is Ellipse)
            {
                Ellipse circle = (Ellipse)e.OriginalSource;

                MyCanvas.Children.Remove(circle);

                score++;

                playClickSound.Open(ClickedSound);
                playClickSound.Play();
            }
        }
    }
}
