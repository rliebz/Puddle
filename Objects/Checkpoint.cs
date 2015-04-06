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
    class Checkpoint : Sprite
    {

        public bool activated;

        public Checkpoint(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y)
        {
            imageFile = "Textures/checkpoint";
            name = obj.Name.ToLower();
            activated = false;
            soundFiles.Add("Sounds/Checkpoint");
        }

        public override void Update(Level level)
        {
            CheckCollisions(level);
            Animate(level);
        }



        public void Animate(Level level)
        {
            if (activated && frameIndexX < (32 * 7) && (level.count % 2) == 0)
            {
                frameIndexX += 32;
            }
        }

        public void CheckCollisions(Level level)
        {
            if (Intersects(level.player))
            {
                Action(level.player);
                if (!activated)
                {
                    soundList["Sounds/Checkpoint"].Play(0.7f, 0, 0);
                    foreach (Sprite s in level.items)
                    {
                        if (s != this && s is Checkpoint)
                        {
                            Checkpoint c = (Checkpoint)s;
                            c.activated = false;
                            c.frameIndexX = 0;
                        }
                    }
                    activated = true;
                }
            }
        }

        public void Action(Player player)
        {
            player.checkpointXPos = spriteX;
            player.checkpointYPos = spriteY;
        }
    }
}