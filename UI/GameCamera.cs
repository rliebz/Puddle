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
        // Current camera positions
        private float currentX;
        private float currentY;

        // Destination camera positions
        private float destinationX;
        private float destinationY;

        public GameCamera(Vector2 position)
        {
            JumpToPosition(position);
        }

        // Update to perform slow scroll
        public void Update() 
        {
            float xToMove = (destinationX - currentX) / 10;
            currentX += xToMove;

            float yToMove = (destinationY - currentY) / 10;
            currentY += yToMove;
        }

        public float X
        {
            get { return currentX; }
            set { destinationX = value; }
        }

        public float Y
        {
            get { return currentY; }
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
