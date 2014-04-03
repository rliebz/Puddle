using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using TiledSharp;
namespace Puddle
{
    class Block : Sprite
    {
		private bool pushLeft;
		private bool pushRight;
		private bool gravity;

		private bool rCol;
		private bool lCol;
		private bool dCol;
		private bool uCol;

        public double x_vel;
		public double y_vel;

        public string blockType;

        private Block uBlock;
        private SoundEffectInstance sound;

		// Tiled constructor
		public Block(TmxObjectGroup.TmxObject obj)
			: this(
				obj.X, obj.Y, 
				obj.Properties.ContainsKey("left") ? Boolean.Parse(obj.Properties["left"]) : false, 
				obj.Properties.ContainsKey("right") ? Boolean.Parse(obj.Properties["right"]) : false,
				obj.Properties.ContainsKey("gravity") ? Boolean.Parse(obj.Properties["gravity"]) : false,
				obj.Name
			)
		{ }

		public Block(int x, int y, bool left=false, bool right=false, bool gravity=false, string name="Block 0")
            : base(x, y, 32, 32)
        {

            this.pushLeft = left;
            this.pushRight = right;
            this.gravity = gravity;
			this.name = name;

			this.isSolid = true;

            this.rCol = false;
            this.lCol = false;
            this.dCol = false;
            this.uCol = false;
            soundFiles.Add("Sounds/Slide.wav");


            this.x_vel = 0;

            uBlock = null;
            this.blockType = "push";

            // Determine block image
            if (!this.gravity)
            {
                frameIndex = 0;
                this.blockType = "metal";
            }
            else if (right && !left)
                frameIndex = 0;
            else if (left && !right)
                frameIndex = 32;
            else if (left && right)
                frameIndex = 64;
            else
                frameIndex = 96;
        }

		public bool rightPushable
		{
			get{ return (blockType == "push" && pushRight && !rCol); }
		}

		public bool leftPushable
		{
			get{ return (blockType == "push" && pushLeft && !lCol); }
		}

        public void changeType(string newType)
        {
            this.blockType = newType;
            image = images[newType];
            if (newType == "metal")
            {
                gravity = false;
                frameIndex = 0;
            }
            else
            {
                gravity = true;

                if (pushRight && !pushLeft)
                    frameIndex = 0;
                else if (pushLeft && !pushRight)
                    frameIndex = 32;
                else if (pushLeft && pushRight)
                    frameIndex = 64;
                else
                    frameIndex = 96;
            }
        }

        public override void Update(Level level)
        {
            Move(level);
            CheckCollisions(level);

        }

        public void Move(Level level)
        {
            // Gravity
            if (gravity)
            {
				y_vel += level.gravity;
				if (y_vel > level.maxFallSpeed)
					y_vel = level.maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
            }

            // Move sideways
            spriteX += Convert.ToInt32(x_vel);
            if (uCol)
            {
                uBlock.x_vel = x_vel;
                uBlock.Move(level);
            }

            if (x_vel != 0)
            {
                if (sound.State != SoundState.Playing)
                {
                    sound.Volume = 0.2f;
                    sound.Play();
                }
            }
            else
            {
                sound.Stop();
            }

			x_vel = 0;
        }

        public void CheckCollisions(Level level)
        {
            // Assume no collisions
            rCol = false;
            lCol = false;
            dCol = false;
            uCol = false;

			if (blockType != "push")
				return;

            // Check collisions with other blocks
			foreach (Sprite s in level.items)
            {
				if (this != s && s.isSolid && Intersects(s))
                {
					// Collide with block below
					if (spriteY < s.spriteY && bottomWall >= s.topWall) 
					{
						dCol = true;
						y_vel = 0;
						while (bottomWall >= s.topWall)
							spriteY--;
					}

					// Collide with block on right
					else if (spriteX < s.spriteX && rightWall >= s.leftWall) 
					{
						rCol = true;
						while (Intersects(s))
							spriteX--;
						while (Intersects(level.player))
							level.player.spriteX--;
					}

					// Collide with block on left
					else if (spriteX > s.spriteX && leftWall <= s.rightWall) 
					{
						lCol = true;
						while (Intersects(s))
							spriteX++;
						while (Intersects(level.player))
							level.player.spriteX++;
					}
                }
            }
        }

		public override void LoadContent(ContentManager content)
        {
            images["push"] = content.Load<Texture2D>("push_block.png");
            images["metal"] = content.Load<Texture2D>("brick.png");
            image = images[this.blockType];
            foreach (string file in soundFiles)
            {
                if (!soundList.ContainsKey(file))
                {
                    SoundEffect effect = content.Load<SoundEffect>(file);
                    soundList.Add(file, effect);
                    this.sound = soundList["Sounds/Slide.wav"].CreateInstance();
                }
            }
        }

		public override void Draw(SpriteBatch sb)
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
