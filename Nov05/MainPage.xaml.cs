using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;



namespace Nov05
{
    public partial class MainPage : UserControl, IGameEntityMgr
    {
        private int enemyTimer = 0; // control the spawn rate
        private int shipVelocity = 5;

        private IList<enemyShip> myEnemies = new List<enemyShip>();
        private bool IsPause = false;
        private int Lives = 1;

        public MainPage()
        {
            InitializeComponent();

            /*
             * add an event handler for the keystrokes
             * in the current object.  C# refers to the
             * current object using the "this" keyword
             */
            this.KeyDown += new KeyEventHandler(gameControl_KeyDown);
            //MainMenu.Visibility = System.Windows.Visibility.Visible;
            theShip.Visibility = System.Windows.Visibility.Collapsed;
            //StartGame();
        }

        private void StartGame()
        {
            /*
             * silverlight has one CompositionTarget.Rendering event
             * This will work fine for a single ship, won't have to
             * create a class hierarchy to control.  When we add more
             * objects, we have to work with classes and interfaces.
             */
            btnQuit.Visibility = System.Windows.Visibility.Collapsed;
            btnStart.Visibility = System.Windows.Visibility.Collapsed;
            //lblHits.Visibility = System.Windows.Visibility.Visible;
            spLives.Visibility = System.Windows.Visibility.Visible;
            MyProgressBar.Visibility = System.Windows.Visibility.Visible;
            lblLives.Visibility = System.Windows.Visibility.Visible;
            lblGameOver.Visibility = System.Windows.Visibility.Collapsed;
            MainMenu.Visibility = System.Windows.Visibility.Collapsed;
            theBackground.Visibility = System.Windows.Visibility.Visible;
            theShip.Visibility = System.Windows.Visibility.Visible;
            theShip.SetValue(Canvas.LeftProperty, Convert.ToDouble(100));
            theShip.SetValue(Canvas.TopProperty, Convert.ToDouble(100));
            (this as IGameEntityMgr).RemainingHits = 12;
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            backgroundMusic.Source = new Uri("/background.mp3", UriKind.Relative);
            backgroundMusic.Play();
        }

        private void StopGame(bool IsGameOver)
        {
            if (IsGameOver)
                lblGameOver.Visibility = Visibility.Visible;
            backgroundMusic.Stop();
            RemoveAllEnemies();
            theShip.Visibility = Visibility.Collapsed;
            btnQuit.Visibility = Visibility.Visible;
            btnStart.Visibility = System.Windows.Visibility.Visible;
            Lives--;
            theBackground.Visibility = System.Windows.Visibility.Collapsed;
            MainMenu.Visibility = System.Windows.Visibility.Visible;
            //lblHits.Visibility = Visibility.Collapsed;
            spLives.Visibility = Visibility.Collapsed;
            MyProgressBar.Visibility = Visibility.Collapsed;
            lblLives.Visibility = System.Windows.Visibility.Collapsed;
            CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
        }

        private void RemoveAllEnemies()
        {
            for (int childIndex = 0; childIndex < gameCanvas.Children.Count; childIndex++)
            {
                //enemyShip gameObject =
                //    gameCanvas.Children[childIndex] as enemyShip;
                //if (gameObject != null)
                //    gameCanvas.Children.Remove(gameCanvas.Children[childIndex]);


                UIElement objElement = gameCanvas.Children[childIndex];
                if (objElement as enemyShip != null)
                    this.RemoveEnemy(objElement as enemyShip);
                else if (objElement as enemyMissile != null)
                {
                    (this as IGameEntityMgr).RemoveGameEntity(objElement as enemyMissile);
                    //gameCanvas.Children.Remove(gameCanvas.Children[childIndex]);
                }
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // add the enemy ships here.
            enemyTimer++;
            if (enemyTimer == 48)
            {
                enemyTimer = 0;
                //gameCanvas.Children.Add(new enemyShip());
                this.AddEnemy(new enemyShip());
            }

            Canvas.SetLeft(theBackground, Canvas.GetLeft(theBackground) - 1);
            if (Canvas.GetLeft(theBackground) < -300
                )
            {
                Canvas.SetLeft(theBackground, 0);
            }

            /*
             * add a loop to check each of the child objects
             * of the canvas for an update function
             * if child has an update function, then call it.
             */
            for (int childIndex = 0;
                childIndex < gameCanvas.Children.Count;
                childIndex++)
            {
                IGameStuff gameObject =
                    gameCanvas.Children[childIndex] as IGameStuff;

                if (gameObject != null)
                {
                    gameObject.Update(this);
                    if ((this as IGameEntityMgr).RemainingHits > 0)
                    {
                        //lblHits.Text = "Remaining Hits : " + (this as IGameEntityMgr).RemainingHits;
                        MyProgressBar.Value = (this as IGameEntityMgr).RemainingHits;
                    }
                    else
                    {
                        Lives--;
                        lblLives.Text = "Remaining Lives : " + Lives;

                        if (Lives == 0)
                        {
                            StopGame(true);
                            return;
                        }
                        CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);

                        StartGame();
                    }
                }
            } // end for (int childIndex = 0
        }

        void gameControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
                theShip.Move(Direction.Right);
            else if (e.Key == Key.Left)
                theShip.Move(Direction.Left);
            else if (e.Key == Key.Up)
                theShip.Move(Direction.Up);
            else if (e.Key == Key.Down)
                theShip.Move(Direction.Down);
            else if (e.Key == Key.Space)
                theShip.Fire(this);
            else if (e.Key == Key.P)
                Pause();
        }

        private void Pause()
        {
            if (IsPause)
            {
                CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
                backgroundMusic.Play();
                IsPause = false;
            }
            else
            {
                CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
                backgroundMusic.Pause();
                IsPause = true;
            }
        }

        // add the new game manager functions here FIRST
        #region Game Management Functions

        public void AddGameEntity(ContentControl control)
        {
            gameCanvas.Children.Add(control);
        } // end AddGameEntity

        public void RemoveGameEntity(ContentControl control)
        {
            gameCanvas.Children.Remove(control);
        } // end RemoveGameEntity

        public void AddEnemy(enemyShip enemy)
        {
            myEnemies.Add(enemy);   // adds to the list
            this.AddGameEntity(enemy);  // adds to the canvas
        } // end AddEnemy

        public void RemoveEnemy(enemyShip enemy)
        {
            this.RemoveGameEntity(enemy);
            myEnemies.Remove(enemy);
        } // end RemoveEnemy

        public Ship PlayerShip
        {
            get
            {
                return theShip;
            }
        }

        public IEnumerable<enemyShip> Enemies
        {
            get
            {
                return myEnemies;
            }
        }

        public int RemainingHits { get; set; }

        public bool DetectCollision(ContentControl controlOne, ContentControl controlTwo)
        {

            // new Rect(X1, Y1, X2, Y2);
            Rect c1Rect = new Rect(
                new Point(Convert.ToDouble(controlOne.GetValue(Canvas.LeftProperty)),
                           Convert.ToDouble(controlOne.GetValue(Canvas.TopProperty))
                           ),
                new Point(Convert.ToDouble(controlOne.GetValue(Canvas.LeftProperty)) + controlOne.ActualWidth,
                           Convert.ToDouble(controlOne.GetValue(Canvas.TopProperty)) + controlOne.ActualHeight
                           )
                           );

            Rect c2Rect = new Rect(
                new Point(Convert.ToDouble(controlTwo.GetValue(Canvas.LeftProperty)),
                           Convert.ToDouble(controlTwo.GetValue(Canvas.TopProperty))
                           ),
                new Point(Convert.ToDouble(controlTwo.GetValue(Canvas.LeftProperty)) + controlTwo.ActualWidth,
                           Convert.ToDouble(controlTwo.GetValue(Canvas.TopProperty)) + controlTwo.ActualHeight
                           )
                           );

            c1Rect.Intersect(c2Rect);

            return !(c1Rect == Rect.Empty);
        }  // end detectcollision

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Invoke("close");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Lives = 3;
            StartGame();
        }



        #endregion



    }
}
