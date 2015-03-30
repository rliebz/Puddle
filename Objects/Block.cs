using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
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
        private bool canBreak;
        private bool transparent;
        public bool neighborsFound;

        public double x_vel;
		public double y_vel;

        public string blockType;
        public Color metalColor;
        public Color tempColor;
        public Color breakColor;

        private SoundEffectInstance sound;

		// Tiled constructor
		public Block(TmxObjectGroup.TmxObject obj)
			: this(
				obj.X, obj.Y, 
				obj.Properties.ContainsKey("left") ? Boolean.Parse(obj.Properties["left"]) : false, 
				obj.Properties.ContainsKey("right") ? Boolean.Parse(obj.Properties["right"]) : false,
				obj.Properties.ContainsKey("gravity") ? Boolean.Parse(obj.Properties["gravity"]) : false,
                obj.Properties.ContainsKey("canBreak") ? Boolean.Parse(obj.Properties["canBreak"]) : false,
                obj.Properties.ContainsKey("transparent") ? Boolean.Parse(obj.Properties["transparent"]) : false,
                obj.Properties.ContainsKey("solid") ? Boolean.Parse(obj.Properties["solid"]) : true,
				obj.Name
			)
		{ }

		public Block(int x, int y, bool left=false, bool right=false, bool gravity=false, bool canBreak=false, bool transparent=false, bool solid=true, string name="Block 0")
            : base(x, y)
        {
            this.pushLeft = left;
            this.pushRight = right;
            this.gravity = gravity;
			this.name = name;
            this.canBreak = canBreak;
            this.transparent = transparent;
            metalColor = new Color(40, 50, 40);
            tempColor = new Color(60, 20, 60);
            breakColor = new Color(90, 130, 90);

            neighborsFound = false;

			isSolid = solid;

            rCol = false;
            lCol = false;
            soundFiles.Add("Sounds/Slide");
            soundFiles.Add("Sounds/BlockFall");

            x_vel = 0;

            blockType = "push";
            spriteColor = Color.White;

            // Determine block image
            if (!this.gravity)
            {
                frameIndexX = 0;
                if (this.canBreak)
                {
                    blockType = "break";
                    spriteColor = breakColor;
                }
                else if (name.Contains("Gate") || name.Contains("Invis"))
                {
                    blockType = "temp";
                    spriteColor = tempColor;
                }
                else {
                    blockType = "metal";
                    spriteColor = metalColor;
                }
            }

        }

		public bool rightPushable
		{
			get{ return (blockType == "push" && pushRight && !rCol); }
		}

		public bool leftPushable
		{
			get{ return (blockType == "push" && pushLeft && !lCol); }
		}

        public void setFrameIndex(Level level)
        {

            if (neighborsFound)
                return;

            neighborsFound = true;

            if (blockType == "push")
            {
                frameIndexY = 0;
                if (pushRight && !pushLeft)
                    frameIndexX = 0;
                else if (pushLeft && !pushRight)
                    frameIndexX = 32;
                else if (pushLeft && pushRight)
                    frameIndexX = 64;
                else
                    frameIndexX = 96;
            }

            if (blockType != "metal")
                return;

            baseCollisionWidth += 0.0625;
            baseCollisionHeight += 0.0625;
            bool u, d, l, r;
            u = false;
            d = false;
            l = false;
            r = false;

            // Find neighboring blocks
            foreach (Sprite item in level.items)
            {
                if (Intersects(item) && item is Block &&
                    ((Block)item).blockType == "metal" && !((Block)item).transparent)
                {
                    // TODO: Direction changing logic
                    if (item.spriteY == spriteY)
                    {
                        if (item.spriteX == spriteX + 32)
                            r = true;
                        else if (item.spriteX == spriteX - 32)
                            l = true;
                    }
                    else if (item.spriteX == spriteX)
                    {
                        if (item.spriteY == spriteY + 32)
                            d = true;
                        else if (item.spriteY == spriteY - 32)
                            u = true;
                    }
                }
            }

            // Determine facing
            if (!u && !d && !l && !r)
            {
                frameIndexY = 0;
                frameIndexX = 0;
            }
            if (u && d && l && r)
            {
                frameIndexY = 0;
                frameIndexX = 32;
            }
            else if (u && d && !l && !r)
            {
                frameIndexY = 0;
                frameIndexX = 64;
            }
            else if (!u && !d && l && r)
            {
                frameIndexY = 0;
                frameIndexX = 96;
            }
            // End pieces
            else if (!u && !d && !l && r)
            {
                frameIndexY = 32;
                frameIndexX = 0;
            }
            else if (!u && !d && l && !r)
            {
                frameIndexY = 32;
                frameIndexX = 32;
            }
            else if (u && !d && !l && !r)
            {
                frameIndexY = 32;
                frameIndexX = 64;
            }
            else if (!u && d && !l && !r)
            {
                frameIndexY = 32;
                frameIndexX = 96;
            }
            // Corner pieces
            else if (!u && d && l && !r)
            {
                frameIndexY = 64;
                frameIndexX = 0;
            }
            else if (u && !d && l && !r)
            {
                frameIndexY = 64;
                frameIndexX = 32;
            }
            else if (u && !d && !l && r)
            {
                frameIndexY = 64;
                frameIndexX = 64;
            }
            else if (!u && d && !l && r)
            {
                frameIndexY = 64;
                frameIndexX = 96;
            }
            // Mostly surrounded pieces
            else if (!u && d && l && r)
            {
                frameIndexY = 96;
                frameIndexX = 0;
            }
            else if (u && !d && l && r)
            {
                frameIndexY = 96;
                frameIndexX = 32;
            }
            else if (u && d && !l && r)
            {
                frameIndexY = 96;
                frameIndexX = 64;
            }
            else if (u && d && l && !r)
            {
                frameIndexY = 96;
                frameIndexX = 96;
            }

            baseCollisionWidth -= 0.0625;
            baseCollisionHeight -= 0.0625;
        }

        public void changeType(string newType)
        {
            blockType = newType;

            if (newType == "transparent")
            {
                isSolid = false;
                transparent = true;
                return;
            }

            isSolid = true;
            transparent = false;
            image = images[newType];
            neighborsFound = false;
            if (newType == "metal")
            {
                gravity = false;
                spriteColor = metalColor;
            }
            else if (newType == "push")
            {
                gravity = true;
                spriteColor = Color.White;
            }
            else if (newType == "temp")
            {
                gravity = false;
                spriteColor = tempColor;
            }
        }


        public override void Update(Level level)
        {
            setFrameIndex(level);
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
                        if (y_vel > 3)
                        {
                            soundList["Sounds/BlockFall"].Play();
                        }						
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
            images["push"] = content.Load<Texture2D>("push_block");
            images["metal"] = content.Load<Texture2D>("metal_block");
            images["break"] = content.Load<Texture2D>("break_block");
            images["temp"] = content.Load<Texture2D>("temp_block");
            image = images[this.blockType];
            foreach (string file in soundFiles)
            {
                if (!soundList.ContainsKey(file))
                {
                    SoundEffect effect = content.Load<SoundEffect>(file);
                    soundList.Add(file, effect);
                    this.sound = soundList["Sounds/Slide"].CreateInstance();
                }
            }
        }

		public override void Draw(SpriteBatch sb)
        {
            if (!transparent)
            {
                base.Draw(sb);
            }

        }
    }
}
