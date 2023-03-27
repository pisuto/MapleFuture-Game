using Maple.Map.Instance;
using Maple.Util;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maple.Map.Info
{
    public enum BackgroundInfoType
    {
        Animation = 1,
        Background = 2,
        Spine = 3
    }

    public class BackgroundInfo : MapleDrawableInfo
    {
        private string bS_; // background name, i.e grassySoil
        private string no_; // background index, i.e 0, 1, 2, ...
        private BackgroundInfoType type_;
        private readonly WzImageProperty imageProperty_;
        // private WzSpineAnimationItem wzSpineAnimationItem; // only applicable if its a spine item, otherwise null.

        public BackgroundInfo(WzImageProperty imageProperty, 
            string bS,
            string no,
            BackgroundInfoType type,
            Texture2D texture, 
            Vector2 origin,     
            WzObject parent = null) : base(texture, origin, parent)
        {
            this.imageProperty_ = imageProperty;
            this.bS_ = bS;
            this.no_ = no;
            this.type_ = type;
            //this.wzSpineAnimationItem = wzSpineAnimationItem;
        }

        public static BackgroundInfo Get(string bS, BackgroundInfoType type, string no)
        {
            if(!WzResourceController.wzInfoManager_.BackgroundSets.ContainsKey(bS))
            {
                return null;
            }

            WzImage bsImg = WzResourceController.wzInfoManager_.BackgroundSets[bS];

            WzImageProperty bgInfoProp = bsImg[GetBgTypeName(type)]?[no];

            if (bgInfoProp == null)
            {
                string logError = string.Format("Background image {0}/{1} is null, {2}", bS, no, bsImg.ToString());
                Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, logError);
                return null;
            }

            if(type == BackgroundInfoType.Spine)
            {
                /// TODO ...
                return null;
            }
            else
            {
                if(bgInfoProp.HCTag == null)
                {
                    bgInfoProp.HCTag = CreateBackGroundInfo(bgInfoProp, bS, type, no);
                }

                return (BackgroundInfo)bgInfoProp.HCTag;
            }
        }

        private static BackgroundInfo CreateBackGroundInfo(WzImageProperty parentObject, string bS, BackgroundInfoType type, string no)
        {
            WzCanvasProperty frame0;
            if (type == BackgroundInfoType.Animation)
            {
                frame0 = (WzCanvasProperty)WzInfoUtil.GetRealProperty(parentObject["0"]);
            }
            else if (type == BackgroundInfoType.Spine)
            {
                // TODO: make a preview of the spine image ffs
                WzCanvasProperty spineCanvas = (WzCanvasProperty)parentObject["0"];
                if (spineCanvas != null)
                {
                    return null;
                    //// Load spine
                    //WzSpineAnimationItem wzSpineAnimationItem = null;
                    //if (graphicsDevice != null) // graphicsdevice needed to work.. assuming that it is loaded by now before BackgroundPanel
                    //{
                    //    WzImageProperty spineAtlasProp = ((WzSubProperty)parentObject).WzProperties.FirstOrDefault(
                    //        wzprop => wzprop is WzStringProperty property && property.IsSpineAtlasResources);
                    //    if (spineAtlasProp != null)
                    //    {
                    //        WzStringProperty stringObj = (WzStringProperty)spineAtlasProp;
                    //        wzSpineAnimationItem = new WzSpineAnimationItem(stringObj);

                    //        wzSpineAnimationItem.LoadResources(graphicsDevice);
                    //    }
                    //}

                    //// Preview Image
                    //Bitmap bitmap = spineCanvas.GetLinkedWzCanvasBitmap();

                    //// Origin
                    //PointF origin__ = spineCanvas.GetCanvasOriginPosition();

                    //return new BackgroundInfo(parentObject, bitmap, WzInfoTools.PointFToSystemPoint(origin__), bS, type, no, parentObject, wzSpineAnimationItem);
                }
                else
                {
                    return new BackgroundInfo(parentObject, bS, no, type, null, Vector2.zero, parentObject);
                }
            }
            else
            {
                frame0 = (WzCanvasProperty)WzInfoUtil.GetRealProperty(parentObject);
            };

            var origin = frame0.GetCanvasOriginPosition();
            origin.Y = MapleUtil.GetNegativeFloat(origin.Y);
            return new BackgroundInfo(frame0, bS, no, type, MapleUtil.BitMapToTex(frame0.GetLinkedWzCanvasBitmap()), WzInfoUtil.PointFToSystemPoint(origin), parentObject);
        }

        public MapleBoardItem CreateInstance(MapleBoard board, int x, int y, int z, int rx, int ry, int cx, int cy, BackgroundType type, int a, bool front, bool flip, int screenMode,
            string spineAni, bool spineRandomStart)
        {
            if (spineAni == null) // if one isnt set already, via pre-existing object in map. It probably means its created via BackgroundPanel
            {
                // attempt to get one
                //if (wzSpineAnimationItem != null && wzSpineAnimationItem.SkeletonData.Animations.Count > 0)
                //{
                //    spineAni = wzSpineAnimationItem.SkeletonData.Animations[0].Name; // actually we should allow the user to select, but nexon only places 1 animation for now
                //}
            }
            return new BackgroundInstance(this, board, x, y, z, rx, ry, cx, cy, type, a, front, flip, screenMode,
                spineAni, spineRandomStart);
        }

        public static string GetBgTypeName(BackgroundInfoType type)
        {
            switch (type)
            {
                case BackgroundInfoType.Animation:
                    {
                        return "ani";
                    }
                case BackgroundInfoType.Spine:
                    {
                        return "spine";
                    }
                default:
                    {
                        return "back";
                    }
            }
        }

        #region Member
        public string bS
        {
            get { return bS_; }
            set { this.bS_ = value; }
        }

        public BackgroundInfoType Type
        {
            get { return type_; }
            set { this.type_ = value; }
        }

        public string no
        {
            get { return no_; }
            set { this.no_ = value; }
        }

        public WzImageProperty WzImageProperty
        {
            get { return imageProperty_; }
            private set { }
        }

        //public WzSpineAnimationItem WzSpineAnimationItem
        //{
        //    get { return wzSpineAnimationItem; }
        //    set { this.wzSpineAnimationItem = value; }
        //}
        #endregion
    }
}
