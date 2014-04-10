using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using TiledSharp;

namespace Puddle
{
    class Checkpoint : Sprite
    {

        public bool activated;
        private SoundEffectInstance instance;

        public Checkpoint(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "checkpoint.png";
            name = obj.Name.ToLower();
            activated = false;
            soundFiles.Add("Sounds/Checkpoint.wav");
        }

        public override void Update(Level level)
        {
            CheckCollisions(level);
            Animate(level);
        }



        public void Animate(Level level)
        {
            if (activated && frameIndex < (32 * 7) && (level.count % 2) == 0)
            {
                frameIndex += 32;
            }
        }

        public void CheckCollisions(Level level)
        {
            if (Intersects(level.player))
            {
                Action(level.player);
                if (!activated)
                {
                    instance = soundList["Sounds/Checkpoint.wav"].CreateInstance();
                    instance.Volume = 0.7f;
                    instance.Play();
                }
                foreach (Sprite s in level.items)
                {
                    if (s != this && s is Checkpoint)
                    {
                        Checkpoint c = (Checkpoint)s;
                        c.activated = false;
                        c.frameIndex = 0;
                    }
                }
                activated = true;
            }
        }

        public void Action(Player player)
        {
            player.checkpointXPos = spriteX;
            player.checkpointYPos = spriteY;
        }
    }
}