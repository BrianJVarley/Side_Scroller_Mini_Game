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



namespace Nov05
{
    public enum Direction { Left, Right, Up, Down }

    

    

    public interface IGameStuff
    {
        //void Update(Canvas theHive);
        void Update(IGameEntityMgr theMgr);
        void Move(Direction theDirection);
        
    }

    /*
     *  A Content Control is a UI control.
     *  It receives input, has width, height, 
     *  displays a background
     *  BUT
     *  can only display one piece of arbitrary content
     *  ie the picture
     *
     */

  


    
    public class Missile : ContentControl, IGameStuff
    {
        
        private int missileVelocity = 20;

        public Missile(double left, double top)
        {
            // use this to create the missile on the fly.
            // calling function is going to supply the 
            // X Y Coordinate to create at.
            // set up a rectangle
            // fill in the values - background, picture, 
            Rectangle missileRect = new Rectangle();
            missileRect.Height = 3;
            missileRect.Width = 12;
            missileRect.Fill = new SolidColorBrush(Colors.Red);
            missileRect.Stroke = new SolidColorBrush(Colors.Yellow);
            missileRect.RadiusX = 1;
            missileRect.RadiusY = 1;

            this.Content = missileRect;
            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);

        }

       
        

        //public void Update(Canvas theCanvas)
        public void Update(IGameEntityMgr theMgr)
        {

            Move(Direction.Right);
            if (Canvas.GetLeft(this) > 1500)
            {
                //theCanvas.Children.Remove(this);
                theMgr.RemoveGameEntity(this);
            }
            else
            {
                /*
                for (Ship theShip in theMgr.PlayerShip)
                {
                    if (theMgr.DetectCollision(theShip, this))
                    {
                        theMgr.RemoveGameEntity(this);
                    }
                }
                */
                // check for collisions enemy
                foreach (enemyShip theEnemy in theMgr.Enemies)
                {
                    if(theMgr.DetectCollision(theEnemy, this))
                    {
                        theMgr.RemoveGameEntity(this);
                        theEnemy.Explode();
                        //MessageBox.Show("Collision detected");
                        
                    }
                    //else if (theMgr.DetectCollision(theEnemy, this))
                    //{
                    //    theMgr.RemoveGameEntity(this);
                    //    theEnemy.Explode();

                    //}
                    
                }
              
            }//end else
        }

        public void Move(Direction direction)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + missileVelocity);
        }

    } // end of class Missile

    public class Ship : ContentControl, IGameStuff
    {
        private int shipVelocity = 5;

        public Ship()
        {

        }

        ///


        //public void Update(Canvas theCanvas)
        public void Update(IGameEntityMgr theMgr)
        {
        }

        public void Move(Direction direction)
        {
            if (direction == Direction.Right)
                Canvas.SetLeft(this, 
                    Canvas.GetLeft(this) + shipVelocity);
            else if (direction == Direction.Left)
                Canvas.SetLeft(this, 
                    Canvas.GetLeft(this) - shipVelocity);
            else if (direction == Direction.Up)
                Canvas.SetTop(this, 
                    Canvas.GetTop(this) - shipVelocity);
            else if (direction == Direction.Down)
                Canvas.SetTop(this, 
                    Canvas.GetTop(this) + shipVelocity);
        }

        //public void Fire(Canvas theCanvas)
        public void Fire(IGameEntityMgr theMgr)
        {
            Missile myMissile = new Missile(Canvas.GetLeft(this) + 30,
                                            Canvas.GetTop(this) + 15);
            //theCanvas.Children.Add(myMissile);
            theMgr.AddGameEntity(myMissile);
        }

       
        

    } // end of class Ship




}
