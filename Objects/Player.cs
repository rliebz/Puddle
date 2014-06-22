using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace Puddle
{
    class Player : Sprite
    {
        // Traits
        public bool moving;
        public bool grounded;
        public bool puddled;
        public bool shooting;
        public bool pushing;
		public double rollerVel;
		public int movedX, movedY;
        public Dictionary<string, bool> powerup;
        public string newMap;
		public bool piped;
        public string pauseScreen;
        private bool powerShotCharging;
        public int lives;
		public string worldPowerUp;

        // Stats
        public double maxHydration;
        public double hydration;
        private double shotCost;
        private double powerShotCost;
        private double jetpackCost;
        private double puddleCost;
        private double hydrationRegen;


        // Movement
        private int speed;
        private int xAccel;
        private double friction;
        public double xVel;
		public double yVel;

        //Checkpoint positions, defaulted to initial
        public int checkpointXPos;
        public int checkpointYPos;

        // Internal calculations
        private int shotPoint;
        private int powerShotPoint;
        private int powerShotRelease;
        private int tryShotHydration;
		private int jumpHeight;
        private int jumpPoint;
        private int jumpDelay;
        private int shotDelay;
        private int powerShotDelay;
        Random rand;
        int index;
        SoundEffectInstance instance, deathInstance;
		public const int MAX_LIVES = 5;
           

        // TODO: Move this

		public Player(int x, int y) : base(x, y)
        {
            // Objects
            powerup = new Dictionary<string, bool>();

            // Properties
			bool hasPowerUps = false;
			powerup["puddle"] = hasPowerUps;
			powerup["jetpack"] = hasPowerUps;
			powerup["charged"] = hasPowerUps;
			worldPowerUp = null;

            lives = MAX_LIVES;
            moving = false;
            grounded = false;
            puddled = false;
            faceLeft = false;
            shooting = false;
            pushing = false;
			rollerVel = 0;
			movedX = 0;
			movedY = 0;
			collisionWidth = 18;
			collisionHeight = 30;
			piped = false;
            powerShotCharging = false;

            // Stats
            maxHydration = 100;
            hydration = maxHydration;
			hydrationRegen = maxHydration / 300;
            shotCost = 10;
            powerShotCost = shotCost * 2;
			jetpackCost = shotCost * 2;
            puddleCost = 1.0;

            // Movement
			speed = 3;
			friction = .075;
			jumpHeight = 10;
			xAccel = 0;
            xVel = 0;
            yVel = 0;

            // Internal calculations
            shotDelay = 160;
            powerShotDelay = 400;
            jumpDelay = 282;
            shotPoint = 0;
            powerShotPoint = 0;
            powerShotRelease = 0;
            tryShotHydration = 0;
            jumpPoint = 0;
            rand = new Random();
            index = 0;

            //Initial position information
            checkpointXPos = x;
            checkpointYPos = y;

            // Sprite Information
            frameIndex = 0;

            //Sounds
            soundFiles.Add("Sounds/Jump.wav");
            soundFiles.Add("Sounds/Shot1.wav");
            soundFiles.Add("Sounds/Shot2.wav");
            soundFiles.Add("Sounds/Shot3.wav");
            soundFiles.Add("Sounds/Shot4.wav");
            soundFiles.Add("Sounds/Powerup.wav");
            soundFiles.Add("Sounds/Death.wav");

        }

        // Property determining if the character can act
        public bool frozen
        {
            get { return (puddled); }
        }

		// Property determining if the character is fully puddle
		public bool fullyPuddled
		{
			get { return (puddled && frameIndex == 5 * 32); }
		}

        // Property determining if the character can be hurt
        public bool invulnerable
        {
			// Or each "invulnerable" condition
			get { return fullyPuddled; }
        }

		// Note that the variable collision height may cause unexpected issues
		public override int topWall
		{
			get 
			{
				return puddled ? base.topWall + frameIndex / 32 * 5 : base.topWall;
			}

		}

		public int numPowers
		{
			get 
			{
				int output = 0;
				if (powerup["jetpack"])
					output++;
				if (powerup["puddle"])
					output++;
				if (powerup["charged"])
					output++;

				return output;
			}
		}

        public void Update(Controls controls, Level level, 
            ContentManager content, GameTime gameTime)
        {
			pauseScreen = String.Format("Slides/pause{0}", numPowers);
				
            if (hydration + hydrationRegen <= maxHydration && !powerShotCharging)
                hydration += hydrationRegen;

            Move(controls, level);

            Puddle(controls);

            Shoot(controls, level, content ,gameTime);

            Jump(controls, level, gameTime);

            CheckCollisions(level);

            HandleCollisions(level);

            Animate(controls, level, gameTime);
        }

        private void Move(Controls controls, Level level)
        {
            // Sideways Acceleration
            if (controls.onPress(Keys.Right, Buttons.DPadRight))
                xAccel += speed;
            else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
                xAccel -= speed;
            if (controls.onPress(Keys.Left, Buttons.DPadLeft))
                xAccel -= speed;
            else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                xAccel += speed;

            // Sideways Movement
            double playerFriction = pushing ? (friction * 3) : friction;
            xVel = xVel * (1 - playerFriction)
                + (frozen ? 0 : xAccel * .10);
			movedX = Convert.ToInt32(xVel + rollerVel);
			spriteX += movedX;

			pushing = false;
			rollerVel = 0;

			// Check left/right collisions
			checkXCollisions(level);

            // Gravity
			yVel += level.gravity;
			if (yVel > level.maxFallSpeed)
				yVel = level.maxFallSpeed;
			// Round up to force movement every step
			movedY = Convert.ToInt32(Math.Ceiling(yVel));
			spriteY += movedY;
			grounded = false;

			// Check up/down collisions
			checkYCollisions(level);

			// Determine direction
			if (xVel > 0.1)
				faceLeft = false;
			else if (xVel < -0.1)
				faceLeft = true;
				
        }

        private void Puddle(Controls controls)
        {
            if (controls.isPressed(Keys.Down, Buttons.DPadDown) &&
                !frozen && grounded && powerup["puddle"])
            {
                puddled = true;
            }

            if (puddled)
            {
				if (hydration >= puddleCost)
					hydration -= puddleCost;
				else 
				{
					puddled = false;
					piped = false;
				}
            }
        }

        private void Shoot(Controls controls, Level level, ContentManager content, GameTime gameTime)
        {
            index = rand.Next(4);
            // New shots
            if (controls.onPress(Keys.D, Buttons.RightShoulder) && !powerShotCharging)
            {
                shooting = true;
                shotPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
            }
            else if (controls.onRelease(Keys.D, Buttons.RightShoulder))
            {
                shooting = false;
            }

            if (powerup["charged"])
            {
                if (controls.onPress(Keys.X, Buttons.RightTrigger))
                {
                    if (hydration >= powerShotCost)
                        powerShotCharging = true;
                    powerShotPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                    powerShotRelease = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                    tryShotHydration = 0;
                }
                else if (controls.isPressed(Keys.X, Buttons.RightTrigger) && powerShotCharging)
                {
                    if (tryShotHydration < powerShotCost)
                    {
                        tryShotHydration += 1;
                        hydration -= 1;
                    }
                }
                else if (controls.onRelease(Keys.X, Buttons.RightTrigger) && powerShotCharging)
                {
                    powerShotRelease = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                    if (((powerShotRelease - powerShotPoint) >= powerShotDelay) && tryShotHydration == powerShotCost)
                    {
                        powerShotCharging = false;
                        tryShotHydration = 0;
                        string dir = controls.isPressed(Keys.Up, Buttons.DPadUp) ? "up" : "none";
                        PowerShot s = new PowerShot(this, dir);
                        if (index == 0)
                        {
                            soundList["Sounds/Shot1.wav"].Play();
                        }
                        else if (index == 1)
                        {
                            soundList["Sounds/Shot2.wav"].Play();
                        }
                        else if (index == 2)
                        {
                            soundList["Sounds/Shot3.wav"].Play();
                        }
                        else
                        {
                            soundList["Sounds/Shot4.wav"].Play();
                        }
                        s.LoadContent(content);
                        level.projectiles.Add(s);
                    }
                    else
                    {
                        powerShotCharging = false;
                        hydration += tryShotHydration;
                        tryShotHydration = 0;
                    }
                }
            }

            // Deal with shot creation and delay
            if (!frozen)
            {
                // Generate regular shots
                int currentTime1 = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                if (((currentTime1 - shotPoint) >= shotDelay || (currentTime1 - shotPoint) == 0)
                    && shooting && hydration >= shotCost)
                {
                    shotPoint = currentTime1;
                    string dir = controls.isPressed(Keys.Up, Buttons.DPadUp) ? "up" : "none";
                    Shot s = new Shot(this, dir);
                    if(index==0) 
                    {
                        soundList["Sounds/Shot1.wav"].Play();
                    }
                    else if (index == 1)
                    {
                        soundList["Sounds/Shot2.wav"].Play();
                    }
                    else if (index == 2)
                    {
                        soundList["Sounds/Shot3.wav"].Play();
                    }
                    else
                    {
                        soundList["Sounds/Shot4.wav"].Play();
                    }
                    s.LoadContent(content);
                    level.projectiles.Add(s);
                    hydration -= shotCost;
                }
					
                // Jetpack (Midair jump and downward shots)
                int currentTime2 = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                if ((currentTime2 - jumpPoint) >= jumpDelay && yVel > 3 && powerup["jetpack"] &&
                    hydration >= jetpackCost && !grounded && controls.isPressed(Keys.S, Buttons.A))
                {
                    // New shot
                    jumpPoint = currentTime2;
                    Shot s = new Shot(this, "down");
                    if (index == 0)
                    {
                        soundList["Sounds/Shot1.wav"].Play();
                    }
                    else if (index == 1)
                    {
                        soundList["Sounds/Shot2.wav"].Play();
                    }
                    else if (index == 2)
                    {
                        soundList["Sounds/Shot3.wav"].Play();
                    }
                    else
                    {
                        soundList["Sounds/Shot4.wav"].Play();
                    }
                    s.LoadContent(content);
                    level.projectiles.Add(s);
                    hydration -= jetpackCost;

                    // Slight upward boost
                    spriteY--;
					movedY--;
					yVel = -4.5;
                }
            }
        }

        private void Jump(Controls controls, Level level, GameTime gameTime)
        {
            // Jump on button press
			if (controls.onPress(Keys.S, Buttons.A) && !frozen && grounded)
            {
                instance = soundList["Sounds/Jump.wav"].CreateInstance();
                instance.Volume = 0.4f;
                instance.Play();
				yVel = -jumpHeight;
                jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
				//grounded = false;
            }

            // Cut jump short on button release
            else if (controls.onRelease(Keys.S, Buttons.A) && yVel < 0)
            {
                yVel /= 2;
            }
        }

        private void CheckCollisions(Level level)
        {

            // Check enemy collisions
            if (!invulnerable)
            {
                foreach (Enemy e in level.enemies)
                {
                    if (this.Intersects(e))
                    {
                        Death(level);
                    }
                }
            }

			// Check misc. collisions
            foreach (Sprite item in level.items)
            {
				// Pick up powerups 
                if (item is PowerUp && Intersects(item))
                {
					((PowerUp)item).Action(this, level);
                    item.destroyed = true;
                    SoundEffectInstance powerup = soundList["Sounds/Powerup.wav"].CreateInstance();
                    powerup.Volume = 0.3f;
                    powerup.Play();
                }


            }
        }

		private void checkXCollisions(Level level)
		{
			foreach (Sprite s in level.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Pipe
					if (s is Pipe && ((Pipe)s).direction != "up" && !piped)
					{
						Pipe p = (Pipe)s;
						if(p.name.Contains("endPipe"))
						{
							newMap = String.Format("Content/Levels/Level{0}.tmx", p.destination);
						}
						else
						{
							p.Action(level);
							piped = true;
						}
					}

					// Collision with right block
					if (rightWall - movedX < s.leftWall)
					{
						// Push
						if (s is Block && ((Block)s).rightPushable && grounded)
						{
							((Block)s).x_vel = movedX;
							pushing = true;
						}

						// Hit the wall
						else
						{
                            while (rightWall >= s.leftWall)
                            {
                                spriteX--;
                                movedX--;
                            }
						}
					}

					// Push to the left
					else if (leftWall - movedX > s.rightWall)
					{
						// Push
						if (s is Block && ((Block)s).leftPushable && grounded)
						{
							((Block)s).x_vel = movedX;
							pushing = true;
						}

						// Hit the wall
						else
						{
                            while (leftWall <= s.rightWall)
                            {
                                spriteX++;
                                movedX++;
                            }
						}
					}
				}
			}
		}

		private void checkYCollisions(Level level)
		{

			foreach (Sprite s in level.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Pipe
					if (s is Pipe && !piped && fullyPuddled && 
						Math.Abs(spriteX - s.spriteX) < 12)
					{
						Pipe p = (Pipe)s;
						if(p.name.Contains("endPipe"))
						{
							newMap = String.Format("Content/Levels/Level{0}.tmx", p.destination);
						}
						else
						{
							p.Action(level);
							piped = true;
						}
					}

					// Up collision
					if (topWall - movedY > s.bottomWall)
					{
						yVel = 0;
						while (topWall <= s.bottomWall)
						{
							spriteY++;
							movedY++;
						}
					}

					// Down collision
					else if ((bottomWall - movedY) < s.topWall)
					{
						grounded = true;
						yVel = 0;
						while (bottomWall >= s.topWall)
						{
							spriteY--;
							movedY--;
						}

						// Roller
						if (s is Roller)
						{
							rollerVel = ((Roller)s).speed;
						}
					}
				}
			}
		}

        private void HandleCollisions(Level level)
        {

        }

        public void Death(Level level)
        {
			// Go to checkpoint
            spriteX = checkpointXPos;
            spriteY = checkpointYPos;

			// reset fields
			xVel = 0;
            yVel = 0;
			puddled = false;
			piped = false;
            hydration = maxHydration;

            deathInstance = soundList["Sounds/Death.wav"].CreateInstance();
            deathInstance.Volume = 0.8f;
            deathInstance.Play();

			if(!level.name.Equals("Content/Levels/LevelSelect.tmx"))
				lives--;

            if (lives == 0)
            {
                level.player.lives = Player.MAX_LIVES;
				newMap = level.name;
            }
        }

        private void Animate(Controls controls, Level level, GameTime gameTime)
        { 
            // Determine type of movement
            if (!frozen)
            {
                // Jumping
                if (!grounded)
                {
                    // Initialize sprite
                    if (image != images["jump"])
                    {
                        image = images["jump"];
                        frameIndex = 0;
                    }
                    // Animate
					else if (frameIndex < 2 * 32 && level.count % 6 == 0)
                        frameIndex += 32;
                }
                // Grounded, not Moving
				else if (Math.Abs(xVel) < 1)
                {
                    // Initialize sprite. No animation.
                    if (image != images["stand"])
                    {
                        image = images["stand"];
                        frameIndex = 0;
                    }
                }
                // Grounded, yes moving
                else
                {
                    // Initialize sprite
                    if (image != images["walk"])
                        image = images["walk"];
                    // Animate
                    //frameIndex = (level.count / 8 % 4) * 32;
                    frameIndex = ((int)(gameTime.TotalGameTime.TotalMilliseconds)/128 % 4)*32;
                }
            }

            // Puddle, if puddling
            if (puddled)
            {
                // Initialize sprite
                if (image != images["puddle"])
                {
                    image = images["puddle"];
                    frameIndex = 0;
                }
                // Animate downward if puddling
                else if (controls.isPressed(Keys.Down, Buttons.DPadDown))
                {
                    if (frameIndex < 5 * 32)
                        frameIndex += 32;
                }
                // Animate upward if unpuddling
                else
                {
                    frameIndex -= 32;
					if (frameIndex <= 0)
					{
						puddled = false;
						piped = false;
					}
                }
            }
        }

        public new void LoadContent(ContentManager content)
        {
			blankImage = content.Load<Texture2D>("blank.png");
            images["stand"] = content.Load<Texture2D>("PC/stand.png");
            images["jump"] = content.Load<Texture2D>("PC/jump.png");
            images["walk"] = content.Load<Texture2D>("PC/walk.png");
            images["puddle"] = content.Load<Texture2D>("PC/puddle.png");
            images["block"] = content.Load<Texture2D>("blank.png");
			images["hydration"] = content.Load<Texture2D>("puddle.png");
			images["heart"] = content.Load<Texture2D>("heart.png");
            image = images["stand"];
            foreach (string file in soundFiles)
            {
                if (!soundList.ContainsKey(file))
                {
                    SoundEffect effect = content.Load<SoundEffect>(file);
                    soundList.Add(file, effect);
                }
            }
        }

        public new void Draw(SpriteBatch sb)
        {
			// Draw the player
            base.Draw(sb);

			// Draw hydration level
            sb.Draw(
                images["block"],
				new Rectangle(8, 36, 16, Convert.ToInt32(maxHydration * 1.5)),
                Color.Navy
            );
            sb.Draw(
                images["block"],
				new Rectangle(
					8, 
					36 + Convert.ToInt32(maxHydration * 1.5) - Convert.ToInt32(hydration * 1.5), 
					16, 
					Convert.ToInt32(hydration * 1.5)
				),
				new Color(0, 160, 232)
            );

            // Draw hydration frame (3 sided)
            sb.Draw(
                images["block"],
                new Rectangle(8, 36, 1, Convert.ToInt32(maxHydration * 1.5)),
                Color.Black
            );
            sb.Draw(
                images["block"],
                new Rectangle(23, 36, 1, Convert.ToInt32(maxHydration * 1.5)),
                Color.Black
            );
            sb.Draw(
                images["block"],
                new Rectangle(8, 36, 16, 1),
                Color.Black
            );

            // Draw hydration icon
			sb.Draw(
				images["hydration"],
				new Rectangle(0, 12 + Convert.ToInt32(maxHydration * 1.5), 32, 32),
				Color.White);

            //Draw lives
			for (int i=0; i < lives; i++)
			{
				sb.Draw(
					images["heart"],
					new Rectangle(32 + 32 * i, 0, 32, 32),
					Color.White
				);
			}
        }

    }
}
