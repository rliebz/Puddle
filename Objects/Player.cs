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

            // Sprite Information
            frameIndex = 0;
        }

        // Property determining if the character can act
        public bool frozen()
        {
            return (puddled);
        }

        // Property determining if the character can be hurt
        public bool invulnerable()
        {
            return (puddled && frameIndex == 5 * 32);
        }

        public void Update(Controls controls, Physics physics, 
            ContentManager content)
        {
            // Move based on controls and physics
            Move(controls, physics);

            // Puddle based on controls
            if (controls.isPressed(Keys.Down, Buttons.DPadDown) &&
                !frozen() && grounded && powerup["puddle"])
            {
                puddled = true;
            }

            // Shoot based on controls
            Shoot(controls, physics, content);

            // Jump based on controls
            Jump(controls, physics);

            // Check for collisions
            CheckCollisions(physics);

            // Animate sprite
            Animate(controls, physics);
        }


        public void CheckCollisions(Physics physics)
        {
            // Check enemy collisions
            if (!invulnerable())
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
                        puddled = false;
                    }
                }
            }
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

            // Determine direction
            if (x_vel > 0.1)
                left = false;
            else if (x_vel < -0.1)
                left = true;

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

        private void Shoot(Controls controls, Physics physics, ContentManager content)
        {
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

                    // Slight upward boost
                    spriteY -= 1;
                    y_vel = -8;
                }
            }
        }

        private void Jump(Controls controls, Physics physics)
        {
            // Jump on press
            if (controls.isPressed(Keys.S, Buttons.A) && !frozen() && grounded)
            {
                spriteY -= 1;
                y_vel = -15;
                grounded = false;
                jump_point = physics.count;
            }

            // Cut jump short on release
            else if (controls.onRelease(Keys.S, Buttons.A) && y_vel < 0)
            {
                y_vel /= 2;
            }
        }

        private void Animate(Controls controls, Physics physics)
        {
            // Determine type of movement
            if (!frozen())
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
                    frameIndex = (physics.count / 8 % 4) * 32;
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
