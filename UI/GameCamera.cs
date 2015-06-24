using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;


namespace Puddle
{
    class GameCamera
    {
        // Settings
        private GraphicsDeviceManager graphics;
        public bool useScroll;
        public int scrollSpeed;
        public int gameScale;

        // Current camera positions
        private float currentX;
        private float currentY;

        // Destination camera positions
        private float destinationX;
        private float destinationY;

        public GameCamera(Vector2 position, GraphicsDeviceManager g, int scale)
        {
            graphics = g;
            useScroll = true;
            scrollSpeed = 10;

            gameScale = scale;
            JumpToPosition(position);
        }

        // Update to perform slow scroll
        public void Update() 
        {
            if (useScroll)
            {
                ScrollCamera();
            }
            else
            {
                MoveCamera();
            }
        }

        private void MoveCamera()
        {
            currentX = destinationX;
            currentY = destinationY;
        }

        private void ScrollCamera()
        {
            float xToMove = (destinationX - currentX) / scrollSpeed;
            currentX += xToMove;

            float yToMove = (destinationY - currentY) / scrollSpeed;
            currentY += yToMove;
        }

        public void setGameScale(int scale)
        {
            gameScale = scale;
        }

        public float X
        {
            get
            {
                return Math.Max(
                    Math.Min(0, currentX),
                    // TODO: Pass map size (22) from Game1.cs
                    graphics.PreferredBackBufferWidth - Sprite.SIZE * 22 * gameScale
                );
            }
            set { destinationX = value; }
        }

        public float Y
        {
            get 
            { 
                return Math.Max(
                    Math.Min(0, currentY),
                    // TODO: Pass map size (22) from Game1.cs
                    graphics.PreferredBackBufferHeight - Sprite.SIZE * 22 * gameScale
                );
            }
            set { destinationY = value; }
        }

        // Set destination
        public void GoToPosition(Vector2 position)
        {
            destinationX = position.X;
            destinationY = position.Y;
        }

        // Immediate jump with no slow scrolling
        public void JumpToPosition(Vector2 position)
        {
            currentX = position.X;
            currentY = position.Y;
            destinationX = position.X;
            destinationY = position.Y;
        }

    }
}
