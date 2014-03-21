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
        public int gravity = 1;
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
            /*int floor_index = 16;
            while (floor_index < 700)
            {
                blocks.Add(new Block(floor_index, 332, false, false));
                floor_index += 32;
            }
            floor_index = 16;
            while (floor_index < 900)
            {
                blocks.Add(new Block(floor_index, 364, false, false));
                floor_index += 32;
            }*/

            /*blocks.Add(new Block(400, 300, true, true));
            blocks.Add(new Block(400, 268, false, false));
            blocks.Add(new Block(400, 236, false, false));
            blocks.Add(new Block(400, 204, false, false));

            blocks.Add(new Block(300, 204, false, false));
            blocks.Add(new Block(300, 236, false, false));
            blocks.Add(new Block(300, 268, false, false));
            blocks.Add(new Block(300, 300, true, true));

            blocks.Add(new Block(564, 300, true, true));
            blocks.Add(new Block(628, 300, false, true));*/
        }

        public void Update(ContentManager content) 
        {
            Random rnd = new Random();

            count++;

            // Generate enemies
            if (count % 100 == -1) // Change -1 to 0 to spawn enemies
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
                if (shots[i].offScreen)
                    shots.RemoveAt(i);
            }

            for (int i = fireballs.Count - 1; i >= 0; i--)
            {
                fireballs[i].Update(this);
                //if (fireballs[i].offScreen)
                //    fireballs.RemoveAt(i);
            }

            // DESTROY
            enemies.RemoveAll(enemy => enemy.destroyed);
            shots.RemoveAll(shot => shot.destroyed);
            items.RemoveAll(item => item.destroyed);
        }



    }
}
