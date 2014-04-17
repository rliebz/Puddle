using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puddle
{
    class PowerShot : Shot
    {
        public PowerShot(Player p, string dir = "none")
            : base(p, dir)
            //(p.spriteX - 8, p.spriteY - 8, 48, 48)
        {
			imageFile = "charged.png";
			collisionWidth = 18;
			collisionHeight = 18;
            damage = 3;
        }


        public override void CheckCollisions(Level level)
        {
            base.CheckCollisions(level);
            foreach (Sprite s in level.items)
            {
                if (s is Block && Intersects(s))
                {
                    Block b = (Block)s;
                    if (b.blockType == "break")
                        b.destroyed = true;
                }
            }
        }
    }
}
