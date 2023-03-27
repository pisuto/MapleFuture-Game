using Maple.Map.Info;
using MapleLib.WzLib.WzStructure.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maple.Map.Instance
{
    public class BackgroundInstance : MapleBoardItem
    {
        private BaseDrawItem drawItem_;
        private GameObject instance_;
        private readonly BackgroundInfo baseInfo_;
        
        private int a_;
        private int cx_;
        private int cy_;
        private int rx_;
        private int ry_;
        private bool front_;

        private bool flip_;
        private int screenMode_;
        private string spineAni_;
        private bool spineRandomStart_;
        private BackgroundType type_;

        public BackgroundInstance(BackgroundInfo baseInfo, MapleBoard board, int x, int y, int z, int rx, int ry, int cx, int cy, BackgroundType type, int a, bool front, 
            bool flip, int screenMode, string spineAni_, bool spineRandomStart_)
            : base(board, x, y, z)
        {
            this.baseInfo_ = baseInfo;
            this.flip_ = flip;
            this.rx_ = rx;
            this.ry_ = ry;
            this.cx_ = cx;
            this.cy_ = cy;
            this.a_ = a;
            this.front_ = front;
            this.type_ = type;
            this.screenMode_ = screenMode;
            this.spineAni_ = spineAni_;
            this.spineRandomStart_ = spineRandomStart_;

            if(this.flip_)
            {
                BaseX -= Width - 2 * (int)Origin.x;
            }

            instance_ = new GameObject(baseInfo_.bS);
            instance_.AddComponent<SpriteRenderer>();
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Backgrounds; }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo_; }
        }

        public override void Draw(int mapShiftX, int mapShiftY, int centerX, int centerY,
            int renderWidth, int renderHeight, float RenderObjectScaling, RenderResolution mapRenderResolution,
            float time)
        {
            DrawObject frame = drawItem_.GetCurrentFrame(time);
            int x = CalculateBackgroundPosX(frame, mapShiftX, centerX, renderWidth, RenderObjectScaling);
            int y = CalculateBackgroundPosY(frame, mapShiftY, centerY, renderHeight, RenderObjectScaling);
            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_DEBUG, "back x:" + x + " y:" + y + " fx:" + frame.X + " fy:" + frame.Y + " rw:" + renderWidth + " rh:" + renderHeight);
            switch(type_)
            {
                case BackgroundType.Regular:
                    Draw2D(instance_, x, y, frame, time);
                    break;
                default:
                    break;
            }
        }

        public int CalculateBackgroundPosX(DrawObject frame, int mapShiftX, int centerX, int RenderWidth, float RenderObjectScaling)
        {
            int width = (int)((RenderWidth / 2) / RenderObjectScaling);
            //int width = RenderWidth / 2;

            return (rx * (mapShiftX - centerX + width) / 100) + frame.X + width;
        }

        public int CalculateBackgroundPosY(DrawObject frame, int mapShiftY, int centerY, int RenderHeight, float RenderObjectScaling)
        {
            int height = (int)((RenderHeight / 2) / RenderObjectScaling);
            //int height = RenderHeight / 2;

            return (ry * (mapShiftY - centerY + height) / 100) + frame.Y + height;
        }

        public void Draw2D(GameObject gameObject, int x, int y, DrawObject frame, float time)
        {
            frame.DrawBackGround(gameObject, x, y, new Color(255, 255, 255, 255), Flip, time);
        }

        #region Members

        public bool Flip
        {
            get
            {
                return flip_;
            }
            set
            {
                if (flip_ == value) return;
                flip_ = value;
                int xFlipShift = Width - 2 * (int)Origin.x;
                if (flip_) BaseX -= xFlipShift;
                else BaseX += xFlipShift;
            }
        }

        public override Texture2D Image
        {
            get
            {
                return baseInfo_.Image;
            }
        }

        public override int Width
        {
            get { return baseInfo_.Width; }
        }

        public override int Height
        {
            get { return baseInfo_.Height; }
        }

        public override Vector2 Origin
        {
            get
            {
                return baseInfo_.Origin;
            }
        }

        public int BaseX { get { return (int)base.pos_.x; } set { base.pos_.x = value; } }
        public int BaseY { get { return (int)base.pos_.y; } set { base.pos_.y = value; } }

        public int rx
        {
            get { return rx_; }
            set { rx_ = value; }
        }

        public int ry
        {
            get { return ry_; }
            set { ry_ = value; }
        }

        public int cx
        {
            get { return cx_; }
            set { cx_ = value; }
        }

        public int cy
        {
            get { return cy_; }
            set { cy_ = value; }
        }

        public int a
        {
            get { return a_; }
            set { a_ = value; }
        }

        public bool front
        {
            get { return front_; }
            set { front_ = value; }
        }

        /// <summary>
        /// The screen resolution to display this background object. (0 = all res)
        /// </summary>
        public int screenMode
        {
            get { return screenMode_; }
            set { screenMode_ = value; }
        }

        /// <summary>
        /// Spine animation path 
        /// </summary>
        public string SpineAni
        {
            get { return spineAni_; }
            set { this.spineAni_ = value; }
        }

        public bool SpineRandomStart
        {
            get { return spineRandomStart_; }
            set { this.spineRandomStart_ = value; }
        }

        public BaseDrawItem DrawItem
        {
            get { return drawItem_;}
            set { drawItem_ = value; }
        }

        #endregion
    }
}
