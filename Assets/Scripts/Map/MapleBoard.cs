using Maple.Map.Instance;
using Maple.Util;
using MapleLib.WzLib;
using MapleLib.WzLib.WzStructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maple.Map.Info
{
    public class MapleBoard
    {
        #region Map

        private int width_;
        private int height_;
        private int renderWidth_;
        private int renderHeight_;
        private RenderResolution mapRenderResolution_;

        private int mapShiftX_ = 0;
        private int mapShiftY_ = 0;
        private float renderObjectScaling_ = 1.0f;
        private float userScreenScaleFactor_ = 1.0f;

        private Vector2Int mapSize_;
        private Vector2Int center_;
        private Vector2Int miniMapPos_;
        private RectInt miniMapArea_;
        private Texture2D miniMap_;
        private readonly MapleBoardItemManager boardItems_;

        private bool loading_ = false;
        private MapInfo mapInfo_ = new MapInfo();

        private readonly MapleTexturePool texturePool = new MapleTexturePool();

        #endregion

        #region Boundary

        private const int VR_BORDER_WIDTH_HEIGHT_ = 600;
        private RectInt vrFieldBoundary_;
        private RectInt vrRect_;

        #endregion

        #region Audio

        private MapleMp3Streamer audio_;

        #endregion

        #region Initialization

        public MapleBoard(Vector2Int mapSize, Vector2Int center)
        {
            this.mapSize_ = mapSize;
            this.center_ = center;

            this.boardItems_ = new MapleBoardItemManager(this);
        }

        public void Dispose()
        {
            boardItems_.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Load all resource of the chosen map
        /// </summary>
        /// <param name="mapImgName"></param>
        public void LoadContent(string mapImgName, MapleGameManager gameManager)
        {
            MapleMapLoader.LoadMap(mapImgName, this);

            // initialize scale
            this.mapRenderResolution_ = RenderResolution.Res_1280x720;
            InitialiseWindowAndMapSize();

            LoadMusic(gameManager);

            LoadVR();

            // default positioning for character
            SetCameraMoveX(true, true, 0);
            SetCameraMoveY(true, true, 0);

            CreateInstances();
        }

        /// <summary>
        /// Play Background Bgm
        /// </summary>
        /// <param name="gameManager"></param>
        private void LoadMusic(MapleGameManager gameManager)
        {
            if (WzResourceController.wzInfoManager_.BGMs.ContainsKey(this.MapInfo.bgm))
            {
                audio_ = new MapleMp3Streamer(WzResourceController.wzInfoManager_.BGMs[this.MapInfo.bgm], true);
                if (audio_ != null)
                {
                    var source = gameManager.GetComponent<AudioSource>();
                    if (null != source)
                    {
                        source.clip = audio_.Audio;
                        source.Play();
                    }
                    else
                    {
                        Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, "no audio exist!");
                    }
                }
            }
        }

        /// <summary>
        /// Fill vr data
        /// </summary>
        private void LoadVR()
        {
            if (this.mapInfo_.VRLeft == null)
            {
                this.vrFieldBoundary_ = new RectInt(0, 0, this.MapSize.x, this.MapSize.y);
            }
            else
            {
                this.vrFieldBoundary_ = new RectInt(this.VRRectangle.xMin + this.CenterPoint.x, 
                    this.VRRectangle.yMin + this.CenterPoint.y, this.VRRectangle.width, this.VRRectangle.height);
            }
        }

        /// <summary>
        /// Create instances
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CreateInstances()
        {
            if (null == WzResourceController.wzFileManager_)
            {
                throw new Exception("wz file not loaded!");
            }

            // Background and objects
            List<WzObject> usedProps = new List<WzObject>();

            WzImage uiBasicImage = (WzImage)WzResourceController.wzFileManager_.FindWzImageByName("ui", "Basic.img");
            WzImage mapHelperImage = (WzImage)WzResourceController.wzFileManager_.FindWzImageByName("map", "MapHelper.img");
            WzImage soundUIImage = (WzImage)WzResourceController.wzFileManager_.FindWzImageByName("sound", "UI.img");
            WzImage uiToolTipImage = (WzImage)WzResourceController.wzFileManager_.FindWzImageByName("ui", "UIToolTip.img"); // UI_003.wz
            WzImage uiWindowImage = (WzImage)WzResourceController.wzFileManager_.FindWzImageByName("ui", "UIWindow.img");

            // cursor
            //this.cursor_ = MapleCreator.CreateCursorFromProperty(texturePool,
            //    (WzImageProperty)uiBasicImage["Cursor"], ref usedProps);
            //this.cursor_.SetActiveCursorType(MAPLE_CUROR_STATUS.MAPLE_CURSOR_NORMAL);

            //background
            foreach (BackgroundInstance background in this.BoardItems.BackBackgrounds_)
            {
                WzImageProperty bgParent = (WzImageProperty)background.BaseInfo.Parent;
                MapleCreator.CreateBackgroundFromProperty(texturePool, bgParent, background, ref usedProps, background.Flip);
            }
            foreach (BackgroundInstance background in this.BoardItems.FrontBackgrounds_)
            {
                WzImageProperty bgParent = (WzImageProperty)background.BaseInfo.Parent;
                MapleCreator.CreateBackgroundFromProperty(texturePool, bgParent, background, ref usedProps, background.Flip);
            }

            // clear used items
            foreach (WzObject obj in usedProps)
            {
                obj.MSTag = null;
                obj.MSTagSpine = null; // cleanup
            }
            usedProps.Clear();
        }

        #endregion

        #region Draw

        public void DrawBackgrounds(float time)
        {
            int mapCenterX = this.CenterPoint.x;
            int mapCenterY = this.CenterPoint.y;
            int shiftCenteredX = this.mapShiftX_ - mapCenterX;
            int shiftCenteredY = this.mapShiftY_ - mapCenterY;

            // background
            this.boardItems_.BackBackgrounds_.ForEach(bg =>
            {
                bg.Draw(mapShiftX_, mapShiftY_, mapCenterX, mapCenterY,
                    renderWidth_, renderHeight_, renderObjectScaling_, mapRenderResolution_,
                    time);
            });

            this.boardItems_.FrontBackgrounds_.ForEach(bg =>
            {
                bg.Draw(mapShiftX_, mapShiftY_, mapCenterX, mapCenterY,
                    renderWidth_, renderHeight_, renderObjectScaling_, mapRenderResolution_,
                    time);
            });

        }

        #endregion

        #region Boundaries

        /// <summary>
        /// Move the camera X viewing range by a specific offset, & centering if needed.
        /// </summary>
        /// <param name="bIsLeftKeyPressed"></param>
        /// <param name="bIsRightKeyPressed"></param>
        /// <param name="moveOffset"></param>
        private void SetCameraMoveX(bool bIsLeftKeyPressed, bool bIsRightKeyPressed, int moveOffset)
        {
            int leftRightVRDifference = (int)(vrFieldBoundary_.width * renderObjectScaling_);
            if (leftRightVRDifference < renderWidth_) // viewing range is smaller than the render width.. keep the rendering position at the center instead (starts from left to right)
            {
                /*
                 * Orbis Tower <20th Floor>
                 *  |____________|
                 *  |____________|
                 *  |____________|
                 *  |____________|
                 *  |____________|
                 *  |____________|
                 *  |____________|
                 *  |____________|
                 *  
                 * vr.Left = 87
                 * vr.Right = 827
                 * Difference = 740px
                 * vr.Center = ((vr.Right - vr.Left) / 2) + vr.Left
                 * 
                 * Viewing width_ = 1024 
                 * Relative viewing center = vr.Center - (Viewing width_ / 2)
                 */
                this.mapShiftX_ = ((leftRightVRDifference / 2) + (int)(vrFieldBoundary_.x * renderObjectScaling_)) - (renderWidth_ / 2);
            }
            else
            {
                // System.Diagnostics.Debug.WriteLine("[{4}] VR.Right {0}, width_ {1}, Relative {2}. [Scaling {3}]", 
                //      vr.Right, RenderWidth, (int)(vr.Right - RenderWidth), (int)((vr.Right - (RenderWidth * renderObjectScaling_)) * renderObjectScaling_),
                //     mapShiftX_ + offset);

                if (bIsLeftKeyPressed)
                {
                    this.mapShiftX_ = Math.Max((int)(vrFieldBoundary_.x * renderObjectScaling_), mapShiftX_ - moveOffset);

                }
                else if (bIsRightKeyPressed)
                {
                    this.mapShiftX_ = Math.Min((int)((vrFieldBoundary_.x + vrFieldBoundary_.width - (renderWidth_ / renderObjectScaling_))), mapShiftX_ + moveOffset);
                }
            }
        }

        /// <summary>
        /// Move the camera Y viewing range by a specific offset, & centering if needed.
        /// </summary>
        /// <param name="bIsUpKeyPressed"></param>
        /// <param name="bIsDownKeyPressed"></param>
        /// <param name="moveOffset"></param>
        private void SetCameraMoveY(bool bIsUpKeyPressed, bool bIsDownKeyPressed, int moveOffset)
        {
            int topDownVRDifference = (int)(vrFieldBoundary_.height * renderObjectScaling_);
            if (topDownVRDifference < renderHeight_)
            {
                this.mapShiftY_ = ((topDownVRDifference / 2) + (int)(vrFieldBoundary_.y * renderObjectScaling_)) - (renderHeight_ / 2);
            }
            else
            {
                /*System.Diagnostics.Debug.WriteLine("[{0}] VR.Bottom {1}, height_ {2}, Relative {3}. [Scaling {4}]",
                    (int)((vr.Bottom - (renderHeight_))),
                    vr.Bottom, renderHeight_, (int)(vr.Bottom - renderHeight_),
                    mapShiftX + offset);*/


                if (bIsUpKeyPressed)
                {
                    this.mapShiftY_ = Math.Max((int)(vrFieldBoundary_.y), mapShiftY_ - moveOffset);
                }
                else if (bIsDownKeyPressed)
                {
                    this.mapShiftY_ = Math.Min((int)((vrFieldBoundary_.y + vrFieldBoundary_.height - (renderHeight_ / renderObjectScaling_))), mapShiftY_ + moveOffset);
                }
            }
        }

        private void InitialiseWindowAndMapSize()
        {
            this.renderObjectScaling_ = 1.0f;
            switch (this.mapRenderResolution_)
            {
                case RenderResolution.Res_1024x768:  // 1024x768
                    height_ = 768;
                    width_ = 1024;
                    break;
                case RenderResolution.Res_1280x720: // 1280x720
                    height_ = 720;
                    width_ = 1280;
                    break;
                case RenderResolution.Res_1366x768:  // 1366x768
                    height_ = 768;
                    width_ = 1366;
                    break;
                case RenderResolution.Res_1920x1080: // 1920x1080
                    height_ = 1080;
                    width_ = 1920;
                    break;
                case RenderResolution.Res_1920x1080_120PercScaled: // 1920x1080
                    height_ = 1080;
                    width_ = 1920;
                    renderObjectScaling_ = 1.2f;
                    break;
                case RenderResolution.Res_1920x1080_150PercScaled: // 1920x1080
                    height_ = 1080;
                    width_ = 1920;
                    renderObjectScaling_ = 1.5f;
                    this.mapRenderResolution_ |= RenderResolution.Res_1366x768; // 1920x1080 is just 1366x768 with 150% scale.
                    break;


                case RenderResolution.Res_1920x1200: // 1920x1200
                    height_ = 1200;
                    width_ = 1920;
                    break;
                case RenderResolution.Res_1920x1200_120PercScaled: // 1920x1200
                    height_ = 1200;
                    width_ = 1920;
                    renderObjectScaling_ = 1.2f;
                    break;
                case RenderResolution.Res_1920x1200_150PercScaled: // 1920x1200
                    height_ = 1200;
                    width_ = 1920;
                    renderObjectScaling_ = 1.5f;
                    break;

                case RenderResolution.Res_All:
                case RenderResolution.Res_800x600: // 800x600
                default:
                    height_ = 600;
                    width_ = 800;
                    break;
            }
            this.userScreenScaleFactor_ = GetDpiScale();

            this.renderHeight_ = (int)(height_ * userScreenScaleFactor_);
            this.renderWidth_ = (int)(width_ * userScreenScaleFactor_);
            this.renderObjectScaling_ = (renderObjectScaling_ * userScreenScaleFactor_);
            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_WARNING, "factor:" + userScreenScaleFactor_);
        }

        /// <summary>
        /// 
        /// </summary>
        private float GetDpiScale()
        {
            float dpi = Screen.dpi;
            if(dpi == 0)
            {
                return 1f;
            }

            return dpi / 96f;
        }

        #endregion

        #region Params

        public MapInfo MapInfo
        {
            get { return mapInfo_; }
            set { mapInfo_ = value; }
        }

        public Texture2D MiniMap
        {
            get { return miniMap_; }
            set { miniMap_ = value; }
        }

        public Vector2Int MiniMapPosition
        {
            get { return miniMapPos_; }
            set { miniMapPos_ = value; }
        }

        public Vector2Int CenterPoint
        {
            get { return center_; }
            internal set { center_ = value; }
        }

        public Vector2Int MapSize
        {
            get
            {
                return mapSize_;
            }
            set
            {
                mapSize_ = value;
                miniMapArea_ = new RectInt(0, 0, mapSize_.x / 16, mapSize_.y / 16);
            }
        }

        public RectInt MiniMapArea
        {
            get { return miniMapArea_; }
        }

        public MapleBoardItemManager BoardItems
        {
            get
            {
                return boardItems_;
            }
        }

        public bool Loading { get { return loading_; } set { loading_ = value; } }

        public RectInt VRRectangle { get { return vrRect_; } set { vrRect_ = value; } }
        
        /// <summary>
        /// Map layers
        /// </summary>
        //public void CreateMapLayers()
        //{
        //    for (int i = 0; i <= MapConstants.MaxMapLayers; i++)
        //    {
        //        AddMapLayer(new Layer(this));
        //    }
        //}

        //public void AddMapLayer(Layer layer)
        //{
        //    lock (parent)
        //        mapLayers.Add(layer);
        //}



        #endregion
    }
}
