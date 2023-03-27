using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maple.Map
{
    public class BaseDrawItem
    {
        // multi frames
        private readonly List<DrawObject> frames_;
        private int currFrame_ = 0;
        private float lastFrameSwitchTime_ = 0f;

        // one frame
        protected bool flip_;
        protected readonly bool notAnimated_;
        private readonly DrawObject frame0_;

        // last frame
        private DrawObject lastFrameDrawn_;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="flip"></param>
        public BaseDrawItem(List<DrawObject> frames, bool flip)
        {
            if (frames.Count == 1) // not animated if its just 1 frame
            {
                this.frame0_ = frames[0];
                notAnimated_ = true;
                this.flip_ = flip;
            }
            else
            {
                this.frames_ = frames;
                notAnimated_ = false;
                this.flip_ = flip;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame0"></param>
        /// <param name="flip"></param>
        public BaseDrawItem(DrawObject frame0, bool flip)
        {
            this.frame0_ = frame0;
            notAnimated_ = true;
            this.flip_ = flip;
        }

        /// <summary>
        /// mircosec to sec
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DrawObject GetCurrentFrame(float time)
        {
            if(notAnimated_)
            {
                return frame0_;
            }

            float delaySec = frames_[currFrame_].Delay / 1000f;
            if (time - lastFrameSwitchTime_ > delaySec)
            {
                currFrame_++;
                if(currFrame_ == frames_.Count)
                {
                    currFrame_ = 0;
                }
                lastFrameSwitchTime_ = time;
            }
            return frames_[currFrame_];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapShiftX"></param>
        /// <param name="mapShiftY"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="RenderObjectScaling"></param>
        /// <param name="mapRenderResolution"></param>
        /// <param name="time"></param>
        public virtual void Draw(GameObject instance, int mapShiftX, int mapShiftY, int centerX, int centerY,
            int width, int height, float RenderObjectScaling, RenderResolution mapRenderResolution,
            float time)
        {
            int shiftCenteredX = mapShiftX - centerX;
            int shiftCenteredY = mapShiftY - centerY;

            DrawObject frame = GetCurrentFrame(time);

            if (IsFrameWithinView(frame, shiftCenteredX, shiftCenteredY, width, height))
            {
                frame.Draw();

                this.lastFrameDrawn_ = frame;
            }
            else
            {
                this.lastFrameDrawn_ = null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="shiftCenteredX"></param>
        /// <param name="shiftCenteredY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private bool IsFrameWithinView(DrawObject frame, int shiftCenteredX, int shiftCenteredY, int width, int height)
        {
            return (frame.X - shiftCenteredX + frame.Width > 0 &&
                frame.Y - shiftCenteredY + frame.Height > 0 &&
                frame.X - shiftCenteredX < width &&
                frame.Y - shiftCenteredY < height);
        }

        /// <summary>
        /// Get last frame
        /// </summary>
        public DrawObject LastFrameDrawn
        {
            get { return this.lastFrameDrawn_; }
            private set { }
        }



    }

}

