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
		public double gravity = .35;
		public int maxFallSpeed = 10;
        public int count = 0;

        public List<Enemy> enemies;
		public List<Sprite> projectiles;
        public List<Sprite> items;
        public Player player;
		public string message;
		public int message_point;
        public string name;
        public int enterLives;
        public Dictionary<string, bool> enterPowerUps;
		public ContentManager content;

		public Level(Player p, string levelName, ContentManager c)
        {
            player = p;
            enterPowerUps = new Dictionary<string, bool>(p.powerup);
            enterLives = p.lives;
            enemies = new List<Enemy>();
			projectiles = new List<Sprite>();
            items = new List<Sprite>();
			message = null;
			message_point = 0;
            name = levelName;
			content = c;
        }

        public Level(Level l)
        {
            player = l.player;
            enterPowerUps = new Dictionary<string,bool>(l.enterPowerUps);
            enterLives = l.player.lives;
            enemies = new List<Enemy>(l.enemies);
            projectiles = new List<Sprite>(l.projectiles);
            items = new List<Sprite>(l.items);
            message = l.message;
            message_point = l.message_point;
            name = l.name;
			content = l.content;
        }

        public void Update() 
        {
			count++;

			if (String.IsNullOrEmpty(message) && (count - message_point) >= 400)
				message = null;

            // Move shots
			foreach (Sprite s in projectiles)
				s.Update(this);

			foreach (Enemy e in enemies)
				e.Update(this);

			foreach (Sprite s in items)
				s.Update(this);

            // DESTROY
            enemies.RemoveAll(enemy => enemy.destroyed);
            projectiles.RemoveAll(shot => shot.destroyed);
            items.RemoveAll(item => item.destroyed);
        }

		public virtual void Draw(SpriteBatch sb)
		{
			foreach (Sprite item in items)
				item.Draw(sb);
			foreach (Sprite s in projectiles)
				s.Draw(sb);
			foreach (Enemy e in enemies)
				e.Draw(sb);
			player.Draw(sb);
		}
    }
}
