using MapleLib.WzLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maple.Map.Info
{
    public class MapleDrawableInfo
    {
        private Texture2D texture_;
        private Vector2 origin_;
        private WzObject parent_;
        private int width_;
        private int height_;

        public MapleDrawableInfo(Texture2D texture, Vector2 origin, WzObject parent = null)
        {
            this.origin_ = origin;
            this.parent_ = parent;

            this.texture_ = texture;
            this.width_ = texture.width;
            this.height_ = texture.height;
        }

        public virtual Texture2D GetTexture()
        {
            return texture_;
        }

        public virtual WzObject Parent
        {
            get
            {
                return parent_;
            }
            set
            {
                parent_ = value;
            }
        }

        public virtual Texture2D Image
        {
            get
            {
                return texture_;
            }

            set
            {
                texture_ = value;
                if(texture_ != null)
                {
                    width_ = texture_.width;
                    height_ = texture_.height;
                }
            }
        }

        public virtual int Width { get { return width_; } }

        public virtual int Height { get { return height_; } }

        public virtual Vector2 Origin { get { return origin_; } set { origin_ = value; } }
    }

}