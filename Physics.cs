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
        public List<PushBlock> pushBlocks;

        public Physics()
        {
            enemies = new List<Enemy>();
            shots = new List<Shot>();
            pushBlocks = new List<PushBlock>();
            int floor_index = 16;
            while (floor_index < 700)
            {
                //pushBlocks.Add(new PushBlock(floor_index, 332, false, false));
                floor_index += 32;
            }
            pushBlocks.Add(new PushBlock(300, 300, true, true));
           // pushBlocks.Add(new PushBlock(300, 268, false, false));

            pushBlocks.Add(new PushBlock(300, 204, false, false));
            pushBlocks.Add(new PushBlock(300, 236, false, false));

            pushBlocks.Add(new PushBlock(564, 300, true, true));
            pushBlocks.Add(new PushBlock(628, 300, false, true));
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
                if (shots[i].offscreen())
                    shots.RemoveAt(i);
            }

            // DESTROY
            enemies.RemoveAll(enemy => enemy.destroyed);
            shots.RemoveAll(shot => shot.destroyed);
        }



    }
}
