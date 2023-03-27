using MapleLib.WzLib.WzStructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maple.Map.Info
{
    public abstract class MapleBoardItem
    {
        protected Vector3 pos_;
        protected MapleBoard board_;
        protected GameObject obj_;

        public MapleBoardItem(MapleBoard board, int x, int y, int z)
        {
            pos_ = new Vector3(x, y, z);
            this.board_ = board;
        }

        public abstract MapleDrawableInfo BaseInfo { get; }
        public abstract ItemTypes Type { get; }
        public abstract Texture2D Image { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract Vector2 Origin { get; }

        public abstract void Draw(int mapShiftX, int mapShiftY, int centerX, int centerY,
            int renderWidth, int renderHeight, float RenderObjectScaling, RenderResolution mapRenderResolution, float time);

        public virtual int X
        {
            get
            {
                return (int)pos_.x;
            }
            set
            {
                pos_.x = value;
            }
        }

        public virtual int Y
        {
            get
            {
                return (int)pos_.y;
            }
            set
            {
                pos_.y = value;
            }
        }

        public virtual int Z
        {
            get
            {
                return (int)pos_.z;
            }
            set
            {
                pos_.z = Math.Max(0, value);
                /*if (this is LayeredItem || this is BackgroundInstance)
                    board.BoardItems.Sort();*/
            }
        }
    }
}
