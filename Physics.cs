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
        public int ground = 900;
		public double gravity = .5;
		public int maxFallSpeed = 10;
        public int count = 0;

        public List<Enemy> enemies;
        public List<Shot> shots;
        public List<Block> blocks;
        public List<Fireball> fireballs;
        public List<Sprite> items;
        public Player player;

        public Physics(Player p)
        {
            player = p;
            enemies = new List<Enemy>();
            shots = new List<Shot>();
            fireballs = new List<Fireball>();
            blocks = new List<Block>();
            items = new List<Sprite>();
        }

        public void Update(ContentManager content) 
        {
            Random rnd = new Random();

            count++;

            // Generate enemies
			if (count % 500 == 0) // Change -1 to 0 to spawn enemies
            {
                Enemy e = (rnd.NextDouble() > .5) ? 
					new Enemy(300, 300) : new Enemy (100, 0);
                e.LoadContent(content);
                enemies.Add(e);
            }

            // Move shots
            for (int i = shots.Count - 1; i >= 0; i--)
            {
                shots[i].Update(this);
                if (shots[i].offScreen)
                    shots.RemoveAt(i);
            }

            for (int i = fireballs.Count - 1; i >= 0; i--)
            {
                fireballs[i].Update(this);
                if (fireballs[i].offScreen)
                    fireballs.RemoveAt(i);
            }

            // DESTROY
            enemies.RemoveAll(enemy => enemy.destroyed);
            shots.RemoveAll(shot => shot.destroyed);
            items.RemoveAll(item => item.destroyed);
            fireballs.RemoveAll(fireball => fireball.destroyed);
        }



    }
}
