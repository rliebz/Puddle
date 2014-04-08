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
    class Level
    {
        public int ground = 900;
		public double gravity = .5;
		public int maxFallSpeed = 10;
        public int count = 0;

        public List<Enemy> enemies;
		public List<Sprite> projectiles;
        public List<Sprite> items;
        public Player player;
		public string message;
		public int message_point;
        public string name;

        public Level(Player p, string levelName)
        {
            player = p;
            enemies = new List<Enemy>();
			projectiles = new List<Sprite>();
            items = new List<Sprite>();
			message = "";
			message_point = 0;
            name = levelName;
        }

        public void Update(ContentManager content) 
        {
			count++;

			if (message != "" && (count - message_point) >= 400)
				message = "";

            // Move shots
			foreach (Sprite s in projectiles)
				s.Update(this);

			foreach (Enemy e in enemies)
			{
				e.Update(this);
				e.Update(this, content);
			}

			foreach (Sprite s in items)
			{
				s.Update(this);
				s.Update(this, content);
			}

            // DESTROY
            enemies.RemoveAll(enemy => enemy.destroyed);
            projectiles.RemoveAll(shot => shot.destroyed);
            items.RemoveAll(item => item.destroyed);
        }

		public virtual void Draw(SpriteBatch sb)
		{
			foreach (Sprite s in projectiles)
				s.Draw(sb);
			foreach (Enemy e in enemies)
				e.Draw(sb);
			foreach (Sprite item in items)
				item.Draw(sb);
			player.Draw(sb);
		}
    }
}
