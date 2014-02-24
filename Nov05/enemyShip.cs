using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Windows.Media.Imaging;

namespace Nov05
{
    public class enemyShip : ContentControl, IGameStuff 
    {
        // use a random number to change this.
        private int velocity = 0;
        private int missileTimer = 0;

        // added to manage collisions
        private bool exploded = false;
        public int hits = 12;
        public enemyShip()
        {
            /*
             * load the image during construction
             * set the property of a contentcontrol as 
             * in the player ship.
             */
            Image enemyShipImage = new Image();

            enemyShipImage.Source = new BitmapImage(new Uri("/octopus.png", UriKind.RelativeOrAbsolute));
            enemyShipImage.Height = 20;

            this.Content = enemyShipImage;

            /*
             * use random number generate to position on 
             * the screen and to set the speed
             */
            Random random = new Random();

            Canvas.SetLeft(this, 1200);
            Canvas.SetTop(this, random.Next(10, 250));

            velocity = random.Next(2, 2);


        } // end of constructor

        public void Move(Direction thisDirection)
        {
            Canvas.SetLeft(this, 
                           Canvas.GetLeft(this) - velocity);
        } // end of Move

        //public void Update(Canvas theCanvas)
        public void Update(IGameEntityMgr theMgr)
        {
            // check for collision with the player ship
            if (theMgr.DetectCollision(this, theMgr.PlayerShip))
            {
                this.Explode();
                theMgr.RemainingHits--;
            }

            if (exploded)   // if its true
            {
                
                theMgr.RemoveEnemy(this);
                
            }
            
            else
            {
                missileTimer++;
                if ((missileTimer >= 48) &&
                    (Canvas.GetLeft(this) < 700))
                {
                    missileTimer = 0;
                    Fire(theMgr);
                }
                Move(Direction.Left);
                if (Canvas.GetLeft(this) < -100)
                {
                    //theCanvas.Children.Remove(this);
                    theMgr.RemoveGameEntity(this);
                }

            }   // end if(exploded)


        } // end of Update

        //public void Fire(Canvas theCanvas)
        public void Fire(IGameEntityMgr theMgr)
        {
            // same as player, only opposite direction
            enemyMissile myMissile = new enemyMissile(Canvas.GetLeft(this),
                                            Canvas.GetTop(this) + (this.ActualHeight / 2) );
            
            
            //theCanvas.Children.Add(myMissile);
            theMgr.AddGameEntity(myMissile);
            
        } // end of Fire

        public void Explode()
        {
            exploded = true;
        }
    }

    public class enemyMissile : ContentControl, IGameStuff
    {
        private int missileVelocity = 4;

        public enemyMissile(double left, double top)
        {
            // use this to create the missile on the fly.
            // calling function is going to supply the 
            // X Y Coordinate to create at.
            // set up a rectangle
            // fill in the values - background, picture, 
            StackPanel panel = new StackPanel();
            Rectangle missileRect = new Rectangle();
            missileRect.Height = 3;
            missileRect.Width = 12;
            missileRect.Fill = new SolidColorBrush(Colors.Black );
            missileRect.Stroke = new SolidColorBrush(Colors.Yellow);
            missileRect.RadiusX = 1;
            missileRect.RadiusY = 1;
            panel.Children.Add(missileRect);

            MediaElement me = new MediaElement();
            me.Source = new Uri("/laser.mp3", UriKind.Relative);
            panel.Children.Add(me);
            

            this.Content = panel;
            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);
            me.Play();
        }

        //public void Update(Canvas theCanvas)
        public void Update(IGameEntityMgr theMgr)
        {
            

            Move(Direction.Left);
            if (Canvas.GetLeft(this) < -25)
            {
                //theCanvas.Children.Remove(this);
                theMgr.RemoveGameEntity(this);
            }
        }

        public void Move(Direction direction)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) - missileVelocity);
        }
    } // end of class Missile


}
