using Maple.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Maple.Map
{
    public class DrawObject
    {
        private Texture2D texture_;
        private Sprite sprite_;
        private readonly int x_;
        private readonly int y_;
        private readonly int delay_;
        private readonly int alpha_;
        private readonly string name_;

        private object tag_;

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="name"></param>
        /// <param name="delay"></param>
        /// <param name="alpha"></param>
        public DrawObject(Texture2D texture, int x, int y, string name, int delay = 0, int alpha = 255)        
        {
            this.texture_ = texture;
            this.x_ = x;
            this.y_ = y;
            this.delay_ = delay;
            this.alpha_ = alpha;
            this.name_ = name;

            sprite_ = Sprite.Create(texture, new Rect(0, 0, texture.width, texture_.height), UnityEngine.Vector2.zero, MapleConstants.SpritePixelsPerUint);
            sprite_.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="pos"></param>
        /// <param name="delay"></param>
        /// <param name="alpha"></param>
        public DrawObject(Texture2D texture, UnityEngine.Vector2 pos, string name, int delay = 0, int alpha = 255)
        {
            this.x_ = (int)pos.x;
            this.y_ = (int)pos.y;
            this.texture_ = texture;
            this.delay_ = delay;
            this.alpha_ = alpha;

            sprite_ = Sprite.Create(texture, new Rect(0, 0, texture.width, texture_.height), UnityEngine.Vector2.zero, MapleConstants.SpritePixelsPerUint);
            sprite_.name = name;
        }
        #endregion

        #region Functions

        public virtual void Draw()
        {
            
        }

        public void DrawBackGround(GameObject instance, int x, int y, Color color, bool flip, float time)
        {
            var render = instance.GetComponent<SpriteRenderer>();
            if(render == null)
            {
                return;
            }

            render.sprite = sprite_;
            instance.transform.position = new UnityEngine.Vector3(x, -(y - Height), 0);
        }

        public int Delay
        {
            get { return delay_; }
        }

        public int X { get { return x_; } }
        public int Y { get { return y_; } }

        public int Width { get { return texture_.width; } }
        public int Height { get { return texture_.height; } }

        public object Tag { get { return tag_; } set { this.tag_ = value; } }

        #endregion

    }
}

