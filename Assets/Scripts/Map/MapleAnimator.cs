using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Maple.UI;
using Maple.Util;
using UnityEngine;

namespace Maple.Map
{
    public class MapleGif
    {
        /// <summary>
        /// Handle gif specialized method 
        /// </summary>
        /// <param name="sprite"></param>
        public delegate void SetImageDelegate(Texture2D sprite);
        public event SetImageDelegate SetImage;

        private List<Texture2D> gif_ = new List<Texture2D>();
        private List<int> delay_ = new List<int>();
        private int gifTimer_ = 0;
        private int GIF_MAX_TIME_ = 0;
        private int curIndex = 0;

        /// <summary>
        /// Load BitMaps
        /// </summary>
        /// <param name="bitMaps"></param>
        public MapleGif(List<Bitmap> bitMaps, List<int> delays)
        {
            foreach (Bitmap bitmap in bitMaps)
            {
                this.gif_.Add(MapleUtil.BitMapToTex(bitmap));
            }

            this.delay_ = delays;

            foreach(int delay in delays)
            {
                this.GIF_MAX_TIME_ += delay;
            }
        }

        /// <summary>
        /// Load Texture
        /// </summary>
        /// <param name="imgs"></param>
        public MapleGif(List<Texture2D> imgs, List<int> delays)
        {
            this.gif_ = imgs;
            this.delay_ = delays;

            foreach (int delay in delays)
            {
                this.GIF_MAX_TIME_ += delay;
            }
        }

        /// <summary>
        /// Initial Images with const delay time
        /// </summary>
        /// <param name="imgs"></param>
        public MapleGif(List<Texture2D> imgs)
        {
            this.gif_ = imgs;
            this.delay_ = new List<int>(imgs.Count) { UIMapleController.INITIAL_GIF_UPDATE_TIME };

            this.GIF_MAX_TIME_ = imgs.Count * UIMapleController.INITIAL_GIF_UPDATE_TIME;
        }

        public MapleGif(List<Tuple<Texture2D, int>> tuples)
        {
            foreach(Tuple<Texture2D, int> tuple in tuples)
            {
                this.gif_.Add(tuple.Item1);
                this.delay_.Add(tuple.Item2);
                this.GIF_MAX_TIME_ += tuple.Item2;
            }
        }

        public int GetImageCount()
        {
            return gif_ != null ? gif_.Count : 0;
        }

        public Tuple<Texture2D, int> this[int index]
        {
            get { return new Tuple<Texture2D, int>(gif_[index], delay_[index]); }
        }

        /// <summary>
        /// Play animation 0.01s 
        /// </summary>
        public void UpdateGif()
        {
            gifTimer_ += 10;

            int curTotalTime = 0;
            for(int i = 0; i <= curIndex; i++)
            {
                curTotalTime += delay_[curIndex];
            }

            if(gifTimer_ > curTotalTime)
            {
                curIndex = (curIndex + 1) % gif_.Count;
                SetImage(gif_[curIndex]);
            }

            if(gifTimer_ > GIF_MAX_TIME_)
            {
                gifTimer_ = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAnimated() 
        {
            return GetImageCount() != 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Texture2D GetStaticImage()
        {
            return gif_.Count > 0 ? gif_[0] : null;
        }

    }
}
