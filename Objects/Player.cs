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
        public bool faceLeft;
        public bool shooting;
        public Dictionary<string, bool> powerup;

        bool pushing;

        // Movement
        private int speed;
        private int x_accel;
        private double friction;
        public double x_vel; // Changes before movement
        public int y_vel;
        public int y_vel_old; // Changes after movement

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
            powerup["jetpack"] = false;
            moving = false;
            grounded = false;
            puddled = false;
            faceLeft = false;
            shooting = false;
            sizeX = 16;

            pushing = false;

            // Movement
            speed = 6;
            friction = .15;
            x_accel = 0;
            x_vel = 0;
            y_vel = 0;
            y_vel_old = 0;

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

            // Handle those collisions
            HandleCollisions(physics);

            // Animate sprite
            Animate(controls, physics);
        }


        private void CheckCollisions(Physics physics)
        {
            pushing = false;
            grounded = false;

            // Check enemy collisions
            if (!invulnerable())
            {
                foreach (Enemy e in physics.enemies)
                {
                    if (this.Intersects(e))
                    {
                        spriteX = 400;
                        spriteY = -32;
                        y_vel = 0;
                        puddled = false;
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
                    if (topWall - y_vel_old > b.bottomWall)
                    {
                        while (topWall < b.bottomWall)
                            spriteY++;
                        y_vel = 0;
                    }

                    // Down collision
                    if ( !grounded && 
                        (bottomWall - y_vel_old) < b.topWall )
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
                        if (b.right && !b.rCol)
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
                        if (b.left && !b.lCol)
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
                }
                if (item is Button && Intersects(item))
                {
                    Button but = (Button)item;
                    but.Action(physics);
                }
            }
        }

        private void HandleCollisions(Physics physics)
        {

        }

        private void Move(Controls controls, Physics physics)
        {
            y_vel_old = y_vel;
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
                + (frozen() ? 0 : x_accel * .10);
            spriteX += Convert.ToInt32(x_vel);

            // Determine direction
            if (x_vel > 0.1)
                faceLeft = false;
            else if (x_vel < -0.1)
                faceLeft = true;

            // Gravity
            if (!grounded)
            {
                spriteY += y_vel;
                y_vel += physics.gravity;
            }
            else
            {
                y_vel = 1;
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

    /*    public new void Draw(SpriteBatch sb)
        {
            // Draw the player
            sb.Draw(
                image, 
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), 
                new Rectangle(frameIndex, 0, 32, 32),
                Color.White,
                0,
                new Vector2(spriteWidth / 2, spriteHeight / 2),
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
            );

        }
     */

    }
}
