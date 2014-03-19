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

        public bool rCol;
        public bool lCol;
        public bool dCol;
        public bool uCol;

        public double x_vel;

        private PushBlock uBlock;

        public int frameIndex;

        public PushBlock(int x, int y, bool left, bool right)
            : base(x, y, 32, 32)
        {
            imageFile = "push_block.png";

            this.left = left;
            this.right = right;

            this.rCol = false;
            this.lCol = false;
            this.dCol = false;
            this.uCol = false;

            this.x_vel = 0;

            uBlock = null;

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

        public void CheckCollisions(Physics physics)
        {
            // Assume no collisions
            rCol = false;
            lCol = false;
            dCol = false;
            uCol = false;

            // Check collisions with other blocks
            foreach (PushBlock b in physics.pushBlocks)
            {
                if (this == b)
                    continue;

                if (Math.Sqrt(Math.Pow(spriteX - b.spriteX, 2) +
                    Math.Pow(spriteY - b.spriteY, 2)) <= 32)
                {
                    // Determine direction
                    if (spriteX < b.spriteX)
                        rCol = true;
                    if (spriteX > b.spriteX)
                        lCol = true;
                    if (spriteY < b.spriteY)
                        dCol = true;
                    if (spriteY > b.spriteY)
                    {
                        uCol = true;
                        uBlock = b;
                    }
                }
            }
        }

        public void Move(Physics physics)
        {
            spriteX += Convert.ToInt32(x_vel);
            if (uCol)
            {
                uBlock.x_vel = x_vel;
                uBlock.CheckCollisions(physics);
                uBlock.Move(physics);
            }
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
