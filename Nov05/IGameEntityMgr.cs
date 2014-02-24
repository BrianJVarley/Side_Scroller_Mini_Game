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

using System.Collections.Generic;   // for ienumerable

/*
 * IGameStuff interface represented every game element
 * on the screen.
 * update function - parameter was theCanvas.
 * Problem - causes very tight coupling between every game
 * entity and the canvas - unit testing is difficult
 * Problem - entities adding and removing themselves leaves
 * no way to intercept this behaviour.
 * 
 * fix this by adding the GameEntityManager that abstracts
 * away the need for direct access to the instance of the 
 * canvas.
 */

namespace Nov05
{
    public interface IGameEntityMgr
    {
        void AddGameEntity(ContentControl control);
        void RemoveGameEntity(ContentControl control);
        void AddEnemy(enemyShip enemy);
        void RemoveEnemy(enemyShip enemy);
        Ship PlayerShip { get; }
        IEnumerable<enemyShip> Enemies { get; }
        int RemainingHits { get; set; }
        bool DetectCollision(ContentControl controlOne, 
                             ContentControl controlTwo);


    }
}
