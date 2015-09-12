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
    class SpikeBall : Enemy
    {
        public SpikeBall(TmxObjectGroup.TmxObject obj)
            : this(obj.X, obj.Y)
        { }

        public SpikeBall(int x, int y)
            : base(x, y)
        {
            imageFile = "Enemies/spikeball";
            baseCollisionWidth = 0.625;
            baseCollisionHeight = 0.625;
            spriteY -= 16;
        }

        public override void Update(Level level)
        {
        }
            

        public override void Draw(SpriteBatch sb)
        {
            // Prevent damage animation
            spriteColor = Color.White;
            base.Draw(sb);
        }
    }
}
