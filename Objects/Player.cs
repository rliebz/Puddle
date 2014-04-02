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

        // Stats
        public double maxHydration;
        public double hydration;
        private double shotCost;
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
        private int checkpointXPos;
        private int checkpointYPos;

        // Internal calculations
        private int shotPoint;
        private int jumpPoint;
        private int jumpDelay;
        private int shotDelay;
        Random rand;
        int index;

        // TODO: Move this

        public Player(int x, int y, int width, int height) : base(x, y, width, height)
        {
            // Objects
            powerup = new Dictionary<string, bool>();

            // Properties
            powerup["puddle"] = true;
            powerup["jetpack"] = false;
            moving = false;
            grounded = false;
            puddled = false;
            faceLeft = false;
            shooting = false;
            pushing = false;
            collisionWidth = 16;

            // Stats
            maxHydration = 100;
            hydration = maxHydration;
            hydrationRegen = maxHydration / 400;
            shotCost = 10;
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
            jumpDelay = 282;
            shotPoint = 0;
            jumpPoint = 0;
            rand = new Random();
            index = 0;

            //Initial position information
            checkpointXPos = x;
            checkpointYPos = y;

            // Sprite Information
            frameIndex = 0;

            //Sounds
            soundFiles.Add("Jump.wav");
            soundFiles.Add("Shot1.wav");
            soundFiles.Add("Shot2.wav");
            soundFiles.Add("Shot3.wav");
            soundFiles.Add("Shot4.wav");
            soundFiles.Add("Powerup.wav");
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

        public void Update(Controls controls, Physics physics, 
            ContentManager content, GameTime gameTime)
        {
            if (hydration + hydrationRegen <= maxHydration)
                hydration += hydrationRegen;

            Move(controls, physics);

            Puddle(controls);

            Shoot(controls, physics, content ,gameTime);

            Jump(controls, physics, gameTime);

            CheckCollisions(physics);

            HandleCollisions(physics);

            Animate(controls, physics, gameTime);
        }

        private void Move(Controls controls, Physics physics)
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

            // Determine direction
            if (x_vel > 0.1)
                faceLeft = false;
            else if (x_vel < -0.1)
                faceLeft = true;

            // Gravity
            if (!grounded)
            {
				y_vel += physics.gravity;
				if (y_vel > physics.maxFallSpeed)
					y_vel = physics.maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
            }
            else
            {
				y_vel = 1;
            }
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
                    puddled = false;
            }
        }

        private void Shoot(Controls controls, Physics physics, ContentManager content, GameTime gameTime)
        {
            index = rand.Next(4);
            // New shots
            if (controls.onPress(Keys.D, Buttons.RightShoulder))
            {
                shooting = true;
                //shotPoint = physics.count;
                shotPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
            }
            else if (controls.onRelease(Keys.D, Buttons.RightShoulder))
            {
                shooting = false;
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
                        soundList["Shot1.wav"].Play();
                    }
                    else if (index == 1)
                    {
                        soundList["Shot2.wav"].Play();
                    }
                    else if (index == 2)
                    {
                        soundList["Shot3.wav"].Play();
                    }
                    else
                    {
                        soundList["Shot4.wav"].Play();
                    }
                    s.LoadContent(content);
                    physics.shots.Add(s);
                    hydration -= shotCost;
                }

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
                        soundList["Shot1.wav"].Play();
                    }
                    else if (index == 1)
                    {
                        soundList["Shot2.wav"].Play();
                    }
                    else if (index == 2)
                    {
                        soundList["Shot3.wav"].Play();
                    }
                    else
                    {
                        soundList["Shot4.wav"].Play();
                    }
                    s.LoadContent(content);
                    physics.shots.Add(s);
                    hydration -= jetpackCost;

                    // Slight upward boost
                    spriteY -= 1;
					y_vel = -4.5;
                }
            }
        }

        private void Jump(Controls controls, Physics physics, GameTime gameTime)
        {
            SoundEffectInstance instance = soundList["Jump.wav"].CreateInstance();
            instance.Volume = 0.1f;
            // Jump on button press
            if (controls.isPressed(Keys.S, Buttons.A) && !frozen && grounded)
            {
               if((powerup["jetpack"]==false && !controls.isHeld(Keys.S, Buttons.A))
                   || (powerup["jetpack"] == true && hydration >= jetpackCost && !controls.isHeld(Keys.S, Buttons.A)))
               {                  
                   if(instance.State != SoundState.Playing)
                        instance.Play();           
                   spriteY -= 1;
				   y_vel = -11;
                   jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
               }
            }

            // Cut jump short on button release
            else if (controls.onRelease(Keys.S, Buttons.A) && y_vel < 0)
            {
                y_vel /= 2;
            }
        }

        private void CheckCollisions(Physics physics)
        {
            pushing = false;
            grounded = false;

            // Check enemy collisions
            if (!invulnerable)
            {
                foreach (Enemy e in physics.enemies)
                {
                    if (this.Intersects(e))
                    {
                        Death();
                    }
                }
            }

            // Reached ground (temporary solution for no floor)
            if (spriteY >= physics.ground)
            {
                grounded = true;
                spriteY = physics.ground;
            }


            // Check solid collisions
            foreach (Block b in physics.blocks)
            {
                if (Intersects(b))
                {
                    // Up collision
					if (topWall - Convert.ToInt32(y_vel) > b.bottomWall)
                    {
                        while (topWall < b.bottomWall)
                            spriteY++;
                        y_vel = 0;
                    }

                    // Down collision
                    if (!grounded &&
						(bottomWall - Convert.ToInt32(y_vel)) < b.topWall)
                    {
                        grounded = true;
                        while (bottomWall > b.topWall)
                            spriteY--;
                    }

                    // Collision with right block
                    else if (bottomWall > b.topWall &&
                        rightWall - Convert.ToInt32(x_vel) < b.leftWall &&
                        x_vel > 0)
                    {
                        // Push
                        if (b.pushRight && !b.rCol)
                        {
                            b.x_vel = x_vel;
                            pushing = true;
                        }

                        // Hit the wall
                        else
                        {
                            while (rightWall >= b.leftWall)
                                spriteX--;
                        }
                    }

                    // Push to the left
                    else if (bottomWall > b.topWall &&
                        leftWall - Convert.ToInt32(x_vel) > b.rightWall &&
                        x_vel < 0)
                    {
                        // Push
                        if (b.pushLeft && !b.lCol)
                        {
                            b.x_vel = x_vel;
                            pushing = true;
                        }

                        // Hit the wall
                        else
                        {
                            while (leftWall <= b.rightWall)
                                spriteX++;
                        }
                    }
                }
            }

            foreach (Sprite item in physics.items)
            {
                if (item is PowerUp && Intersects(item))
                {
                    powerup[((PowerUp)item).name] = true;
                    item.destroyed = true;
                    SoundEffectInstance instance = soundList["Powerup.wav"].CreateInstance();
                    instance.Volume = 0.3f;
                    instance.Play();
                   // newMap = "Content/Level2.tmx";
                }
                if (item is Button && Intersects(item))
                {
                    Button but = (Button)item;
                    but.Action(physics);
                }

                if (item is Pipe && Intersects(item) && (puddled && frameIndex == 5 * 32))
                {
                    Pipe p = (Pipe)item;
                    if (p.direction == "down")
                    {
                        p.Action(physics);
                        //Death ();
                    }

                }
            }
        }

        private void HandleCollisions(Physics physics)
        {

        }

        public void Death()
        {
            Console.WriteLine("Death");
            spriteX = checkpointXPos;
            spriteY = checkpointYPos;
            y_vel = 0;
            puddled = false;
            hydration = maxHydration;
        }

        private void Animate(Controls controls, Physics physics, GameTime gameTime)
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
                    //frameIndex = (physics.count / 8 % 4) * 32;
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
                        puddled = false;
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
        }

    }
}
