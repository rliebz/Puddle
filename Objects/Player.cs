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
    class Player : Sprite
    {
        // Traits
        public bool moving;
        public bool grounded;
        public bool puddled;
        public bool left;
        public bool shooting;
        public Dictionary<string, bool> powerup;

        // Movement
        private int speed;
        private int x_accel;
        private double friction;
        public double x_vel;
        public int y_vel;

        // Internal calculations
        private int shot_point;
        private int jump_point;
        private int jump_delay;
        private int shot_delay;
        private int frameIndex;

        // TODO: Move this

        public Player(int x, int y, int width, int height) : base(x, y, width, height)
        {
            // Objects
            powerup = new Dictionary<string, bool>();

            // Properties
            powerup["puddle"] = true;
            powerup["jetpack"] = true;
            moving = false;
            grounded = false;
            puddled = false;
            left = false;
            shooting = false;

            // Movement
            speed = 7;
            friction = .15;
            x_accel = 0;
            x_vel = 0;
            y_vel = 0;

            // Internal calculations
            shot_delay = 10;
            jump_delay = 17;
            shot_point = 0;
            jump_point = 0;

            // Sprite Business
            frameIndex = 0;
        }

        // TODO: Add all freezing effects here.
        // Since a number of effects may cause the character to be
        // unable to move, a single property can check for all of them.
        public bool frozen()
        {
            return (puddled);
        }

        public void Update(Controls controls, Physics physics, 
            ContentManager content)
        {
            Move(controls, physics);

            // Puddle
            if (controls.isPressed(Keys.Down, Buttons.DPadDown) &&
                !frozen() && grounded && powerup["puddle"])
            {
                puddled = true;
            }

            // New shots
            if (controls.onPress(Keys.D, Buttons.RightShoulder))
            {
                shooting = true;
                shot_point = physics.count;
            }
            else if (controls.onRelease(Keys.D, Buttons.RightShoulder))
            {
                shooting = false;
            }

            if (controls.onPress(Keys.S, Buttons.A) && grounded)
            {
                jump_point = physics.count;
            }
            else if (controls.onRelease(Keys.S, Buttons.A))
            {
                // Cut jump short
                if (y_vel < 0)
                    y_vel /= 2;
            }

            // Deal with shot creation and delay
            if (!frozen())
            {
                // Generate regular shots
                if ((physics.count - shot_point) % shot_delay == 0 && shooting)
                {
                    string dir = controls.isPressed(Keys.Up, Buttons.DPadUp) ? "up" : "none";
                    Shot s = new Shot(this, dir);
                    s.LoadContent(content);
                    physics.shots.Add(s);
                }

                // Generate downward shots
                if ((physics.count - jump_point) % jump_delay == 0 && powerup["jetpack"] &&
                    !grounded && controls.isPressed(Keys.S, Buttons.A))
                {
                    // New shot
                    Shot s = new Shot(this, "down");
                    s.LoadContent(content);
                    physics.shots.Add(s);

                    // Upwards
                    spriteY -= 1;
                    y_vel = -8;
                }
            }

            // Jump logic
            if (controls.isPressed(Keys.S, Buttons.A) && !frozen() && grounded)
            {
                spriteY -= 1;
                y_vel = -15;
                grounded = false;
            }

            Animate(controls, physics);
            
            CheckCollisions(physics);
        }


        public void CheckCollisions(Physics physics)
        {
            // Check enemy collisions
            if (!puddled)
            {
                foreach (Enemy e in physics.enemies)
                {
                    if (Math.Sqrt(Math.Pow(spriteX - e.spriteX, 2) +
                        Math.Pow(spriteY - e.spriteY, 2)) < 32)
                    {
                        spriteX = 400;
                        spriteY = -32;
                        y_vel = 0;
                        grounded = false;
                    }
                }
            }

            // TODO: Check other collisions
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
            x_vel = x_vel * (1 - friction)
                + (frozen() ? 0 : x_accel * .10);
            spriteX += Convert.ToInt32(x_vel);

            // Gravity
            if (spriteY < physics.ground)
            {
                spriteY += y_vel;
                y_vel += physics.gravity;
            }
            else
            {
                spriteY = physics.ground;
                y_vel = 0;
                grounded = true;
            }
        }

        private void Animate(Controls controls, Physics physics)
        {
            // Determine direction
            if (x_vel > 0.1)
                left = false;
            else if (x_vel < -0.1)
                left = true;

            // Determine movement
            if (!frozen())
            {
                // Jumping
                if (!grounded)
                {
                    if (image != images["jump"])
                    {
                        image = images["jump"];
                        frameIndex = 0;
                    }
                }
                // Not Moving
                else if (Math.Abs(x_vel) < .5)
                {
                    if (image != images["stand"])
                    {
                        image = images["stand"];
                        frameIndex = 0;
                    }
                }

                // Yes moving
                else if (image != images["walk"])
                {
                    image = images["walk"];
                    frameIndex = 0;
                }
            }


            // Puddle sprite logic
            if (puddled)
            {
                if (image != images["puddle"])
                {
                    image = images["puddle"];
                    frameIndex = 0;
                }
                
                if (controls.isPressed(Keys.Down, Buttons.DPadDown))
                {
                    if (frameIndex < 5 * 32)
                        frameIndex += 32;
                }
                else
                {
                    frameIndex -= 32;
                    if (frameIndex <= 0)
                        puddled = false;
                }
            }

            // Walking sprite frames
            else if (image == images["walk"])
            {
                frameIndex = (physics.count / 8 % 4) * 32;
            }

            // Jumping sprite frames
            else if (image == images["jump"])
            {
                if (frameIndex < 2 * 32)
                    frameIndex += 32;
            }

            // Standing sprite frames
            else
            {
                frameIndex = 0;
            }
        }

        public new void LoadContent(ContentManager content)
        {
            images["stand"] = content.Load<Texture2D>("PC/stand.png");
            images["jump"] = content.Load<Texture2D>("PC/jump.png");
            images["walk"] = content.Load<Texture2D>("PC/walk.png");
            images["puddle"] = content.Load<Texture2D>("PC/puddle.png");
            image = images["stand"];
        }

        public new void Draw(SpriteBatch sb)
        {
            // Draw the player
            sb.Draw(
                image, 
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), 
                new Rectangle(frameIndex, 0, 32, 32),
                Color.White,
                0,
                new Vector2(16, 16),
                left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
            );

        }

    }
}
