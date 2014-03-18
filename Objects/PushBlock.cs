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
    class PushBlock : Sprite
    {
        public bool left;
        public bool right;
        public int frameIndex;

        public PushBlock(int x, int y, bool left, bool right)
            : base(x, y, 32, 32)
        {
            imageFile = "push_block.png";

            this.left = left;
            this.right = right;

            // Determine block image
            if (right && !left)
                frameIndex = 0;
            else if (left && !right)
                frameIndex = 32;
            else if (left && right)
                frameIndex = 64;
            else
                frameIndex = 96;
        }

        public void Update(Player player, Physics physics)
        {

        }

        public new void Draw(SpriteBatch sb)
        {
            sb.Draw(
                image,
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
                new Rectangle(frameIndex, 0, 32, 32),
                Color.White,
                0,
                new Vector2(16, 16),
                SpriteEffects.None,
                0
            );

        }
    }
}
