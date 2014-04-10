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
        public Dictionary<string, bool> powerup;
        public string newMap;
		public bool piped;
        public string pauseScreen;
        private bool powerShotCharging;
        public int lives;
        public TextField livesMessage;

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
        private int x_accel;
        private double friction;
        public double x_vel;
		public double y_vel;

        //Checkpoint positions, defaulted to initial
        public int checkpointXPos;
        public int checkpointYPos;

        // Internal calculations
        private int shotPoint;
        private int powerShotPoint;
        private int powerShotRelease;
        private int tryShotHydration;
        private int jumpPoint;
        private int jumpDelay;
        private int shotDelay;
        private int powerShotDelay;
        Random rand;
        int index;
        SoundEffectInstance instance, deathInstance;
        private const int NUM_LIVES = 5;
           

        // TODO: Move this

        public Player(int x, int y, int width, int height) : base(x, y, width, height)
        {
            // Objects
            powerup = new Dictionary<string, bool>();

            // Properties
            powerup["puddle"] = true;
            powerup["jetpack"] = false;
            powerup["charged"] = false;

            lives = NUM_LIVES;
            moving = false;
            grounded = false;
            puddled = false;
            faceLeft = false;
            shooting = false;
            pushing = false;
            collisionWidth = 16;
			piped = false;
            powerShotCharging = false;

            // Stats
            maxHydration = 100;
            hydration = maxHydration;
            hydrationRegen = maxHydration / 400;
            shotCost = 10;
            powerShotCost = shotCost * 2;
            jetpackCost = 20;
            puddleCost = 1.0;

            // Movement
			speed = 5;
            friction = .15;
            x_accel = 0;
            x_vel = 0;
            y_vel = 0;

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

        // Property determining if the character can be hurt
        public bool invulnerable
        {
            get { return (puddled && frameIndex == 5 * 32); }
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
            pauseScreen = String.Format("pause{0}", numPowers);
            if (level.paused)
                return;

            livesMessage.Message = String.Format("Lives Remaining:      x{0}", lives);
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
                x_accel += speed;
            else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
                x_accel -= speed;
            if (controls.onPress(Keys.Left, Buttons.DPadLeft))
                x_accel -= speed;
            else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                x_accel += speed;

            // Sideways Movement
            double playerFriction = pushing ? (friction * 3) : friction;
            x_vel = x_vel * (1 - playerFriction)
                + (frozen ? 0 : x_accel * .10);
            spriteX += Convert.ToInt32(x_vel);

			pushing = false;

			// Check left/right collisions
			foreach (Sprite s in level.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Collision with right block
					if (bottomWall > s.topWall &&
						rightWall - Convert.ToInt32(x_vel) < s.leftWall &&
						x_vel > 0)
					{
						// Push
						if (s is Block && ((Block)s).rightPushable && grounded)
						{
							((Block)s).x_vel = x_vel;
							pushing = true;
						}

						// Hit the wall
						else
						{
							while (rightWall >= s.leftWall)
								spriteX--;
						}
					}

					// Push to the left
					else if (bottomWall > s.topWall &&
						leftWall - Convert.ToInt32(x_vel) > s.rightWall &&
						x_vel < 0)
					{
						// Push
						if (s is Block && ((Block)s).leftPushable && grounded)
						{
							((Block)s).x_vel = x_vel;
							pushing = true;
						}

						// Hit the wall
						else
						{
							while (leftWall <= s.rightWall)
								spriteX++;
						}
					}
				}
			}

            // Gravity
            if (!grounded)
            {
				y_vel += level.gravity;
				if (y_vel > level.maxFallSpeed)
					y_vel = level.maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
            }
            else
            {
				y_vel = 1;
            }

			grounded = false;

			// Check up/down collisions
			foreach (Sprite s in level.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Up collision
					if (topWall - Convert.ToInt32(y_vel) > s.bottomWall)
					{
						y_vel = 0;
						while (topWall < s.bottomWall)
							spriteY++;
					}

					// Down collision
					else if (!grounded &&
						(bottomWall - Convert.ToInt32(y_vel)) < s.topWall)
					{
						grounded = true;
						while (bottomWall > s.topWall)
							spriteY--;
					}
				}
			}
				
			// Determine direction
			if (x_vel > 0.1)
				faceLeft = false;
			else if (x_vel < -0.1)
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

                   
               
                
                //else
                //{
                //    hydration += tryShotHydration;
                //    tryShotHydration = 0;
                //}


                //need a new if statement for big shots. the if will be current time, shot point, and charge time, bigShooting, amd hydration

                // Jetpack (Midair jump and downward shots)
                int currentTime2 = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                if ((currentTime2 - jumpPoint) >= jumpDelay && y_vel > 3 && powerup["jetpack"] &&
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
                    spriteY -= 1;
					y_vel = -4.5;
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
				y_vel = -11;
                jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
				grounded = false;
            }

            // Cut jump short on button release
            else if (controls.onRelease(Keys.S, Buttons.A) && y_vel < 0)
            {
                y_vel /= 2;
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
                   // newMap = "Content/Level2.tmx";
                }

				if (item is Pipe && Intersects(item) && (puddled && frameIndex == 5 * 32) && 
					!piped && Math.Abs(spriteX - item.spriteX) < 12)

                {

					Pipe p = (Pipe)item;
					if(p.name.Contains("endPipe"))
					{
						newMap = String.Format("Content/Level{0}.tmx", p.destination);
					}
					p.Action(level);
					piped = true;

                }

            }
        }

        private void HandleCollisions(Level level)
        {

        }

        public void Death(Level level)
        {
            spriteX = checkpointXPos;
            spriteY = checkpointYPos;
            y_vel = 0;
            puddled = false;
            hydration = maxHydration;
			piped = false;
            deathInstance = soundList["Sounds/Death.wav"].CreateInstance();
            deathInstance.Volume = 0.8f;
            deathInstance.Play();
            if (lives == 0)
            {
                newMap = level.name;
                lives = level.enterLives;
                powerup = level.enterPowerUps;
                return;
            }
            if(!level.name.Equals("Content/LevelSelect.tmx"))
                lives--;
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
                    else if (frameIndex < 2 * 32)
                        frameIndex += 32;
                }
                // Grounded, not Moving
                else if (Math.Abs(x_vel) < .5)
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
            images["stand"] = content.Load<Texture2D>("PC/stand.png");
            images["jump"] = content.Load<Texture2D>("PC/jump.png");
            images["walk"] = content.Load<Texture2D>("PC/walk.png");
            images["puddle"] = content.Load<Texture2D>("PC/puddle.png");
            images["block"] = content.Load<Texture2D>("blank.png");
            livesMessage = new TextField(
                String.Format("Lives Remaining:      x{0}", lives),
                new Vector2(160, 5),
                Color.White
            );

            livesMessage.loadContent(content);
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

            // Draw health
            sb.Draw(
                images["block"],
                new Rectangle(8, 8, Convert.ToInt32(maxHydration * 1.5), 16),
                Color.Navy
            );
            sb.Draw(
                images["block"],
                new Rectangle(8, 8, Convert.ToInt32(hydration * 1.5), 16),
                Color.Cyan
            );

            //Draw lives

            livesMessage.draw(sb);
            sb.Draw(
                images["stand"],
                new Rectangle(304, 0, 32, 32),
                Color.White
            );
        }

    }
}
