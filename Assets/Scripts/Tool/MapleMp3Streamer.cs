using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleLib;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NAudio.Wave;
using System.IO;
using UnityEngine;

namespace Maple.Util
{
    public class MapleMp3Streamer
    {
        private AudioClip clip_;
        private bool repeat_;

        public MapleMp3Streamer(WzBinaryProperty sound, bool repeat)
        {
            this.repeat_ = repeat;
            this.clip_ = UWL.NAudioPlayer.FromMp3Data(sound.Name, sound.GetBytes(false));
        }

        public AudioClip Audio { get { return clip_; } }

        public bool Repeat { get { return repeat_; } set { repeat_ = value; } }
    }
}
