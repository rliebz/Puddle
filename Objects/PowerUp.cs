using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using TiledSharp;

namespace Puddle
{
    class PowerUp : Sprite
    {

        public PowerUp(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y)
        {
            imageFile = obj.Name + "";
            name = obj.Name.ToLower();
        }

		public override void Update(Level level)
		{
			if (level.player.powerup[name])
				destroyed = true;
		}

		public void Action(Player player, Level level)
        {
            player.powerup[name] = true;
			player.worldPowerUp = name;
			level.message_point = level.count;
			if (name == "jetpack")
				level.message = "Press and hold the jump button to jetpack!";
			else if (name == "charged")
				level.message = "Press and hold the right trigger to charge a super shot!";
            else if (name == "puddle")
                level.message = "Press and hold down to turn into a puddle and avoid damage!";
        }

    }
}
