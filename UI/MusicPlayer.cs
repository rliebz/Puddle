using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TiledSharp;

namespace Puddle
{
    class MusicPlayer
    {
        public string soundDirectory = "Sounds";
        public List<string> songFiles;
        public Dictionary<string, SoundEffectInstance> songDict;

        public MusicPlayer()
        {
            this.songFiles = new List<string> { "InGame", "Menu", "Boss" };
            this.songDict = new Dictionary<string, SoundEffectInstance>();
        }

        public void LoadContent(ContentManager content)
        {
            foreach (string file in songFiles)
            {
                string effectPath = soundDirectory + "/" + file;
                SoundEffect effect = content.Load<SoundEffect>(effectPath);
                SoundEffectInstance instance = effect.CreateInstance();
                instance.IsLooped = true;
                songDict.Add(file, instance);
            }
        }

        public void PlayLevelMusic(Level level)
        {
            switch (level.name)
            {
                case "Select":
                case "Menu":
                    songDict["InGame"].Stop();
                    songDict["Boss"].Stop();
                    songDict["Menu"].Play();
                    break;
                case "Boss":
                    songDict["Boss"].Play();
                    songDict["InGame"].Stop();
                    songDict["Menu"].Stop();
                    break;
                default:
                    songDict["InGame"].Play();
                    songDict["Boss"].Stop();
                    songDict["Menu"].Stop();
                    break;
            }
        }
    }
}