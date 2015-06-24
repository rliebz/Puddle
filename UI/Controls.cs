using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;


namespace Puddle
{
    class Controls
    {
        public enum Directions { up, down, left, right };

        private KeyboardState kb;
        private KeyboardState kbo;
        private GamePadState gp;
        private GamePadState gpo;

        // UI Inputs
        private Keys confirmKey;
        private Keys backKey;
        private Keys startKey;
        private Buttons confirmButton;
        private Buttons backButton;
        private Buttons startButton;

        // Direction Inputs
        private Keys upKey;
        private Keys downKey;
        private Keys leftKey;
        private Keys rightKey;
        private Buttons upButton;
        private Buttons downButton;
        private Buttons leftButton;
        private Buttons rightButton;

        // Player Inputs
        private Keys primaryShootKey;
        private Keys secondaryShootKey;
        private Keys jumpKey;
        private Buttons primaryShootButton;
        private Buttons secondaryShootButton;
        private Buttons jumpButton;

        // Public Tuples
        public Tuple<Keys, Buttons> Confirm;
        public Tuple<Keys, Buttons> Back;
        public Tuple<Keys, Buttons> Start;
        public Tuple<Keys, Buttons, Directions> Up;
        public Tuple<Keys, Buttons, Directions> Down;
        public Tuple<Keys, Buttons, Directions> Left;
        public Tuple<Keys, Buttons, Directions> Right;
        public Tuple<Keys, Buttons> PrimaryShot;
        public Tuple<Keys, Buttons> SecondaryShot;
        public Tuple<Keys, Buttons> Jump;

        public Controls()
        {
            kb = Keyboard.GetState();
            kbo = Keyboard.GetState();
            gp = GamePad.GetState(PlayerIndex.One);
            gpo = GamePad.GetState(PlayerIndex.One);

            // Default UI Inputs
            confirmKey = Keys.D;
            backKey = Keys.S;
            startKey = Keys.Enter;
            confirmButton = Buttons.A;
            backButton = Buttons.B;
            startButton = Buttons.Start;

            // Default Direction Inputs
            upKey = Keys.Up;
            downKey = Keys.Down;
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            upButton = Buttons.DPadUp;
            downButton = Buttons.DPadDown;
            leftButton = Buttons.DPadLeft;
            rightButton = Buttons.DPadRight;

            // Default Player Inputs
            primaryShootKey = Keys.D;
            secondaryShootKey = Keys.A;
            jumpKey = Keys.S;
            primaryShootButton = Buttons.RightShoulder;
            secondaryShootButton = Buttons.RightTrigger;
            jumpButton = Buttons.A;

            // Public Tuples
            Confirm = Tuple.Create(confirmKey, confirmButton);
            Back = Tuple.Create(backKey, backButton);
            Start = Tuple.Create(startKey, startButton);
            Up = Tuple.Create(upKey, upButton, Directions.up);
            Down = Tuple.Create(downKey, downButton, Directions.down);
            Left = Tuple.Create(leftKey, leftButton, Directions.left);
            Right = Tuple.Create(rightKey, rightButton, Directions.right);
            PrimaryShot = Tuple.Create(primaryShootKey, primaryShootButton);
            SecondaryShot = Tuple.Create(secondaryShootKey, secondaryShootButton);
            Jump = Tuple.Create(jumpKey, jumpButton);
        }

        public void Update(Level level)
        {
            kbo = kb;
            gpo = gp;
            kb = Keyboard.GetState();
            gp = GamePad.GetState(PlayerIndex.One);
        }

        // Private Methods
        private bool isThumbstickHeld(GamePadState gamepad, Directions direction)
        {
            switch (direction)
            {
                case Directions.up:
                    return gamepad.ThumbSticks.Left.Y > 0.5;
                case Directions.down:
                    return gamepad.ThumbSticks.Left.Y < -0.5;
                case Directions.left:
                    return gamepad.ThumbSticks.Left.X < -0.5;
                case Directions.right:
                    return gamepad.ThumbSticks.Left.X > 0.5;
                default:
                    return false;
            }
        }

        private bool isPressed(Keys key, Buttons button)
        { return kb.IsKeyDown(key) || gp.IsButtonDown(button); }

        private bool isPressed(Keys key, Buttons button, Directions dir)
        { return kb.IsKeyDown(key) || gp.IsButtonDown(button) || isThumbstickHeld(gp, dir); }

        private bool onPress(Keys key, Buttons button)
        {
            return (kb.IsKeyDown(key) && kbo.IsKeyUp(key)) ||
                (gp.IsButtonDown(button) && gpo.IsButtonUp(button));
        }

        private bool onPress(Keys key, Buttons button, Directions dir)
        {
            return (kb.IsKeyDown(key) && kbo.IsKeyUp(key)) ||
                (gp.IsButtonDown(button) && gpo.IsButtonUp(button)) ||
                (isThumbstickHeld(gp, dir) && !isThumbstickHeld(gpo, dir));
        }

        private bool onRelease(Keys key, Buttons button)
        {
            return (kb.IsKeyUp(key) && kbo.IsKeyDown(key)) ||
                (gp.IsButtonUp(button) && gpo.IsButtonDown(button));
        }

        private bool onRelease(Keys key, Buttons button, Directions dir)
        {
            return (kb.IsKeyUp(key) && kbo.IsKeyDown(key)) ||
                (gp.IsButtonUp(button) && gpo.IsButtonDown(button)) ||
                (!isThumbstickHeld(gp, dir) && isThumbstickHeld(gpo, dir));
        }

        // Public Methods
        public bool isPressed(Tuple<Keys, Buttons> input)
        { return isPressed(input.Item1, input.Item2); }

        public bool isPressed(Tuple<Keys, Buttons, Directions> input)
        { return isPressed(input.Item1, input.Item2, input.Item3); }

        public bool onPress(Tuple<Keys, Buttons> input)
        { return onPress(input.Item1, input.Item2); }

        public bool onPress(Tuple<Keys, Buttons, Directions> input)
        { return onPress(input.Item1, input.Item2, input.Item3); }

        public bool onRelease(Tuple<Keys, Buttons> input)
        { return onRelease(input.Item1, input.Item2); }

        public bool onRelease(Tuple<Keys, Buttons, Directions> input)
        { return onRelease(input.Item1, input.Item2, input.Item3); }
    }
}
