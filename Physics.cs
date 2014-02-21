using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Puddle
{
    class Physics
    {
        public int ground = 300;
        public int gravity = 1;
        public int count = 0;

        public List<Enemy> enemies;
        public List<Shot> shots;

        public Physics()
        {
            enemies = new List<Enemy>();
            shots = new List<Shot>();
        }

        public void Update(ContentManager content) 
        {
            Random rnd = new Random();

            count++;

            // Generate enemies
            if (count % 100 == 0)
            {
                Enemy e = (rnd.NextDouble() > .5) ? 
                    new Enemy(900, 300) : new Enemy (-32, 300);
                e.LoadContent(content);
                enemies.Add(e);
            }

            // Move shots
            for (int i = shots.Count - 1; i >= 0; i--)
            {
                shots[i].Update(this);
                if (shots[i].offscreen())
                    shots.RemoveAt(i);
            }

            // DESTROY
            enemies.RemoveAll(enemy => enemy.destroyed);
            shots.RemoveAll(shot => shot.destroyed);
        }



    }
}
