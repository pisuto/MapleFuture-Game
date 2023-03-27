using Maple.Util;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maple.UI;
using MapleLib.WzLib.WzStructure;
using MapleLib.WzLib.WzStructure.Data;
using Maple.Map.Info;
using System.Drawing;
using Maple.Map.Instance;

namespace Maple.Map
{
    class MapleCreator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="mapImage"></param>
        /// <param name="mapName"></param>
        /// <param name="streetName"></param>
        /// <param name="categoryName"></param>
        /// <param name="strMapProp"></param>
        /// <param name="board"></param>
        public static void CreateMapFromImage(int mapId, WzImage mapImage, 
            string mapName, string streetName, string categoryName, 
            WzSubProperty strMapProp, ref MapleBoard board)
        {
            if(!mapImage.Parsed)
            {
                mapImage.ParseImage();
            }

            List<string> copyPropNames = VerifyMapPropsKnown(mapImage, false);

            MapInfo info = new MapInfo(mapImage, mapName, streetName, categoryName);
            foreach (string copyPropName in copyPropNames)
            {
                info.additionalNonInfoProps.Add(mapImage[copyPropName]);
            }

            MapType type = GetMapType(mapImage);
            if (type == MapType.RegularMap)
            {
                info.id = int.Parse(WzInfoUtil.RemoveLeadingZeros(
                    WzInfoUtil.RemoveExtension(mapImage.Name)));
            }
            info.mapType = type;


            // Create board
            board.Loading = true;
            board.MapInfo = info;

            if(!GetMapDimensions(mapImage, ref board))
            {
                Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, "map doesn't contain size info");
                return;
            }

            // Load Map Resources
            LoadBackgrounds(mapImage, ref board);
            // ...

            board.BoardItems.Sort();
            board.Loading = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="bgInstance"></param>
        /// <param name="usedProps"></param>
        /// <param name="flip"></param>
        public static void CreateBackgroundFromProperty(MapleTexturePool pool, WzImageProperty source, BackgroundInstance bgInstance, ref List<WzObject> usedProps, bool flip)
        {
            var list = LoadDrawFrames(pool, source, bgInstance.BaseX, bgInstance.BaseY, ref usedProps);
            if(list.Count == 0)
            {
                return;
            }
            
            if(list.Count == 1)
            {
                bgInstance.DrawItem = new BaseDrawItem(list[0], flip);
            }
            else
            {
                bgInstance.DrawItem = new BaseDrawItem(list, flip);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="property"></param>
        /// <param name="usedProps"></param>
        /// <returns></returns>
        public static CursorItem CreateCursorFromProperty(MapleTexturePool pool, WzImageProperty property, ref List<WzObject> usedProps)
        {
            CursorItem cursor = new CursorItem();

            for (int i = 0; i < UIMapleController.MAX_WZ_RESOURCE_CURSOR_NUM; i++)
            {
                WzSubProperty cursorCanvas = (WzSubProperty)property?[i.ToString()];
                
            }

            return cursor;
        }

        public static void LoadBackgrounds(WzImage mapImage, ref MapleBoard mapBoard)
        {
            WzSubProperty bgParent = (WzSubProperty)mapImage["back"];
            WzSubProperty bgProp = null;
            int i = 0;

            while(null != (bgProp = (WzSubProperty)bgParent[(i++).ToString()]))
            {
                int x = InfoTool.GetInt(bgProp["x"]);
                int y = InfoTool.GetInt(bgProp["y"]); //MapleUtil.GetNegativeInt(InfoTool.GetInt(bgProp["y"]));
                int rx = InfoTool.GetInt(bgProp["rx"]);
                int ry = InfoTool.GetInt(bgProp["ry"]);
                int cx = InfoTool.GetInt(bgProp["cx"]);
                int cy = InfoTool.GetInt(bgProp["cy"]);
                int a = InfoTool.GetInt(bgProp["a"]);
                BackgroundType type = (BackgroundType)InfoTool.GetInt(bgProp["type"]);
                bool front = InfoTool.GetBool(bgProp["front"]);
                int screenMode = InfoTool.GetInt(bgProp["screenMode"], (int)RenderResolution.Res_All);
                string spineAni = InfoTool.GetString(bgProp["spineAni"]);
                bool spineRandomStart = InfoTool.GetBool(bgProp["spineRandomStart"]);
                bool? flip_t = InfoTool.GetOptionalBool(bgProp["f"]);
                bool flip = flip_t.HasValue ? flip_t.Value : false;
                string bS = InfoTool.GetString(bgProp["bS"]);
                bool ani = InfoTool.GetBool(bgProp["ani"]);
                string no = InfoTool.GetInt(bgProp["no"]).ToString();

                BackgroundInfoType infoType = BackgroundInfoType.Background;
                if (spineAni != null)
                {
                    infoType = BackgroundInfoType.Spine;
                }
                else if (ani)
                {
                    infoType = BackgroundInfoType.Animation;
                }

                BackgroundInfo bgInfo = BackgroundInfo.Get(bS, infoType, no);
                if(null == bgInfo)
                {
                    continue;
                }

                IList list = front ? mapBoard.BoardItems.FrontBackgrounds_ : mapBoard.BoardItems.BackBackgrounds_;
                list.Add((BackgroundInstance)bgInfo.CreateInstance(mapBoard, x, y, i, rx, ry, cx, cy, type, a, front, flip, screenMode,
                    spineAni, spineRandomStart));
            }
        }

        /// <summary>
        /// not all gif, may some other's like ui ...
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="usedProps"></param>
        /// <returns></returns>
        private static List<DrawObject> LoadDrawFrames(MapleTexturePool pool, WzImageProperty source, int x, int y, ref List<WzObject> usedProps)
        {
            List<DrawObject> loaded = new List<DrawObject>();
            // ?
            source = WzInfoUtil.GetRealProperty(source);
        
            if(source is WzSubProperty property0 && property0.WzProperties.Count == 1)
            {
                source = property0.WzProperties[0];
            }

            if(source is WzCanvasProperty property1)
            {
                bool bLoadedSpine = LoadSpineMapObjectItem();
                if(!bLoadedSpine)
                {
                    string canvasBitMapPath = property1.FullPath;
                    var texFromCache = pool.GetTexture(canvasBitMapPath);
                    if(null != texFromCache)
                    {
                        source.MSTag = texFromCache;
                    }
                    else
                    {
                        source.MSTag = MapleUtil.BitMapToTex(property1.GetLinkedWzCanvasBitmap());
                        pool.AddTextureToPool(canvasBitMapPath, (Texture2D)source.MSTag);
                    }
                }
                usedProps.Add(source);

                Vector2Int origin = MapleUtil.GetVectorInt(property1.GetCanvasOriginPosition());
                //origin.y = MapleUtil.GetNegativeInt(origin.y);
                if (source.MSTagSpine != null)
                {
                    /// TODO ...
                }
                else if (source.MSTag != null)
                {
                    Texture2D texture = (Texture2D)source.MSTag;
                    int delay = (int)WzInfoUtil.GetOptionalInt(source[WzCanvasProperty.AnimationDelayPropertyName], 100);
                    loaded.Add(new DrawObject(texture, x - origin.x, y - origin.y, source.Name, delay));
                }
                else // fallback
                {
                    Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, "no texture exists");
                }
            }
            else if(source is WzSubProperty) // animated
            {
                WzImageProperty property;
                int i = 0;

                while ((property = WzInfoUtil.GetRealProperty(source[(i++).ToString()])) != null)
                {
                    if (property is WzSubProperty) // issue with 867119250
                    {
                        loaded.AddRange(LoadDrawFrames(pool, property, x, y, ref usedProps));
                    }
                    else
                    {
                        WzCanvasProperty canvas;

                        if (property is WzUOLProperty) // some could be UOL. Ex: 321100000 Mirror world: [Mirror World] Leafre
                        {
                            WzObject linkVal = ((WzUOLProperty)property).LinkValue;
                            if (linkVal is WzCanvasProperty linkCanvas)
                            {
                                canvas = linkCanvas;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            canvas = (WzCanvasProperty)property;
                        }

                        int delay = (int)WzInfoUtil.GetOptionalInt(canvas[WzCanvasProperty.AnimationDelayPropertyName], 100);

                        bool bLoadedSpine = LoadSpineMapObjectItem();
                        if (!bLoadedSpine)
                        {
                            if (canvas.MSTag == null)
                            {
                                string canvasBitmapPath = canvas.FullPath;
                                Texture2D textureFromCache = pool.GetTexture(canvasBitmapPath);
                                if (textureFromCache != null)
                                {
                                    canvas.MSTag = textureFromCache;
                                }
                                else
                                {
                                    canvas.MSTag = MapleUtil.BitMapToTex(canvas.GetLinkedWzCanvasBitmap());
                                    // add to cache
                                    pool.AddTextureToPool(canvasBitmapPath, (Texture2D)canvas.MSTag);
                                }
                            }
                        }
                        usedProps.Add(canvas);


                        Vector2Int origin = MapleUtil.GetVectorInt(canvas.GetCanvasOriginPosition());
                        //origin.y = MapleUtil.GetNegativeInt(origin.y);

                        if (canvas.MSTagSpine != null)
                        {
                            /// TODO ...
                        }
                        else if (canvas.MSTag != null)
                        {
                            Texture2D texture = (Texture2D)canvas.MSTag;

                            loaded.Add(new DrawObject(texture, x - origin.x, y - origin.y, source.Name, delay));
                        }
                        else
                        {
                            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, "no texture exists");
                        }
                    }
                }
            }
            return loaded;
        }

        private static bool LoadSpineMapObjectItem()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapImage"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private static bool GetMapDimensions(WzImage mapImage, ref MapleBoard board)
        {
            RectInt? vr = MapleUtil.GetRect((Rectangle)MapInfo.GetVR(mapImage));
            Vector2Int mapCenter = new Vector2Int();
            Vector2Int mapSize = new Vector2Int();
            Vector2Int minimapSize = new Vector2Int();
            Vector2Int minimapCenter = new Vector2Int();

            // calculate map & mini map size, including vr  
            if (null == board.MiniMap)
            {
                if(null == vr)
                {
                    if(!GetMapVR(mapImage, ref vr))
                    {
                        return false;
                    }
                }
                minimapSize = new Vector2Int(vr.Value.width + 10, vr.Value.height + 10); //leave 5 pixels on each side
                minimapCenter = new Vector2Int(5 - vr.Value.x, 5 - vr.Value.y);
                mapSize = new Vector2Int(minimapSize.x, minimapSize.y);
                mapCenter = new Vector2Int(minimapCenter.x, minimapCenter.y);
            }
            else
            {
                WzImageProperty miniMap = mapImage["miniMap"];
                minimapSize = new Vector2Int(InfoTool.GetInt(miniMap["width"]), InfoTool.GetInt(miniMap["height"]));
                minimapCenter = new Vector2Int(InfoTool.GetInt(miniMap["centerX"]), InfoTool.GetInt(miniMap["centerY"])); //MapleUtil.GetNegativeInt
                int topOffs = 0, botOffs = 0, leftOffs = 0, rightOffs = 0;
                int leftTarget = 69 - minimapCenter.x, topTarget = 86 - minimapCenter.y, rightTarget = minimapSize.x - 69 - 69, botTarget = minimapSize.y - 86 - 86;
                if (vr == null)
                {
                    // We have no VR info, so set all VRs according to their target
                    vr = new RectInt(leftTarget, topTarget, rightTarget, botTarget);
                }
                else
                {
                    if (vr.Value.x < leftTarget)
                    {
                        leftOffs = leftTarget - vr.Value.x;
                    }
                    if (vr.Value.y < topTarget)
                    {
                        topOffs = topTarget - vr.Value.y;
                    }
                    if (vr.Value.x + vr.Value.width > rightTarget)
                    {
                        rightOffs = vr.Value.x + vr.Value.width - rightTarget;
                    }
                    if (vr.Value.y + vr.Value.height > botTarget)
                    {
                        botOffs = vr.Value.y + vr.Value.height - botTarget;
                    }
                }
                mapSize = new Vector2Int(minimapSize.x + leftOffs + rightOffs, minimapSize.y + topOffs + botOffs);
                mapCenter = new Vector2Int(minimapCenter.x + leftOffs, minimapCenter.y + topOffs);
            }
            vr = new RectInt(vr.Value.x, vr.Value.y, vr.Value.width, vr.Value.height);
        
            if(null != board.MiniMap)
            {
                board.MiniMapPosition = -mapCenter;
                // board.MinimapRectangle = new MinimapRectangle(board, new Rectangle(mmPos.X, mmPos.Y, minimapSize.X, minimapSize.Y));
            }
            // mapBoard.VRRectangle = new VRRectangle(mapBoard, VR);
            
            // Initialize map size
            board.MapSize = mapSize;
            board.CenterPoint = mapCenter;
            board.VRRectangle = vr.Value;
            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_DEBUG,
                "Load vr x:" + vr.Value.x + " y:" + vr.Value.y + " w:" + vr.Value.width + " h:" + vr.Value.height);
            return true;
        }

        private static bool GetMapVR(WzImage mapImage, ref RectInt? VR)
        {
            WzSubProperty fhParent = (WzSubProperty)mapImage["foothold"];
            if (fhParent == null) { VR = null; return false; }
            int mostRight = int.MinValue, mostLeft = int.MaxValue, mostTop = int.MaxValue, mostBottom = int.MinValue;
            foreach (WzSubProperty fhLayer in fhParent.WzProperties)
            {
                foreach (WzSubProperty fhCat in fhLayer.WzProperties)
                {
                    foreach (WzSubProperty fh in fhCat.WzProperties)
                    {
                        int x1 = InfoTool.GetInt(fh["x1"]);
                        int x2 = InfoTool.GetInt(fh["x2"]);
                        int y1 = InfoTool.GetInt(fh["y1"]); // GetNegativeInt
                        int y2 = InfoTool.GetInt(fh["y2"]);

                        if (x1 > mostRight) mostRight = x1;
                        if (x1 < mostLeft) mostLeft = x1;
                        if (x2 > mostRight) mostRight = x2;
                        if (x2 < mostLeft) mostLeft = x2;
                        if (y1 > mostBottom) mostBottom = y1;
                        if (y1 < mostTop) mostTop = y1;
                        if (y2 > mostBottom) mostBottom = y2;
                        if (y2 < mostTop) mostTop = y2;
                    }
                }
            }
            if (mostRight == int.MinValue || mostLeft == int.MaxValue || mostTop == int.MaxValue || mostBottom == int.MinValue)
            {
                VR = null; return false;
            }
            int VRLeft = mostLeft - 10;
            int VRRight = mostRight + 10;
            int VRBottom = mostBottom + 110;
            int VRTop = Math.Min(mostBottom - 600, mostTop - 360);
            VR = new RectInt(VRLeft, VRTop, VRRight - VRLeft, VRBottom - VRTop);
            return true;
        }

        public static MapType GetMapType(WzImage mapImage)
        {
            switch (mapImage.Name)
            {
                case "MapLogin.img":
                case "MapLogin1.img":
                case "MapLogin2.img":
                case "MapLogin3.img":
                    return MapType.MapLogin;
                case "CashShopPreview.img":
                    return MapType.CashShopPreview;
                default:
                    return MapType.RegularMap;
            }
        }

        public static List<string> VerifyMapPropsKnown(WzImage mapImage, bool userless)
        {
            List<string> copyPropNames = new List<string>();
            foreach (WzImageProperty prop in mapImage.WzProperties)
            {
                switch (prop.Name)
                {
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8": // what? 749080500.img
                    case "info":
                    case "life":
                    case "ladderRope":
                    case "reactor":
                    case "back":
                    case "foothold":
                    case "miniMap":
                    case "portal":
                    case "seat":
                    case "ToolTip":
                    case "clock":
                    case "shipObj":
                    case "area":
                    case "healer":
                    case "pulley":
                    case "BuffZone":
                    case "swimArea":
                        continue;
                    case "coconut": // The coconut event. Prop is copied but not edit-supported, we don't need to notify the user since it has no stateful objects. (e.g. 109080002)
                    case "user": // A map prop that dresses the user with predefined items according to his job. No stateful objects. (e.g. 930000010)
                    case "noSkill": // Preset in Monster Carnival maps, can only guess by its name that it blocks skills. Nothing stateful. (e.g. 980031100)
                    case "snowMan": // I don't even know what is this for; it seems to only have 1 prop with a path to the snowman, which points to a nonexistant image. (e.g. 889100001)
                    case "weather": // This has something to do with cash weather items, and exists in some nautlius maps (e.g. 108000500)
                    case "mobMassacre": // This is the Mu Lung Dojo header property (e.g. 926021200)
                    case "battleField": // The sheep vs wolf event and other useless maps (e.g. 109090300, 910040100)
                        copyPropNames.Add(prop.Name);
                        continue;
                    case "snowBall": // The snowball/snowman event. It has the snowman itself, which is a stateful object (somewhat of a mob), but we do not support it.
                    case "monsterCarnival": // The Monster Carnival. It has an immense amount of info and stateful objects, including the mobs and guardians. We do not support it. (e.g. 980000201)
                        copyPropNames.Add(prop.Name);
                        if (!userless)
                        {
                            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_WARNING, 
                                "The map you are opening has the feature \"" + prop.Name + "\", which is purposely not supported in the editor.\r\nTo get around this, HaCreator will copy the original feature's data byte-to-byte. This might cause the feature to stop working if it depends on map objects, such as footholds or mobs.");
                        }
                        continue;
                    case "tokyoBossParty": // Neo Tokyo 802000801.img
                    case "skyWhale":
                    case "rectInfo":
                    case "directionInfo":
                    case "particle":
                    case "respawn":
                    case "enterUI":
                    case "mobTeleport":
                    case "climbArea":
                    case "stigma":
                    case "monsterDefense":
                    case "oxQuiz":
                    case "nodeInfo":
                    case "onlyUseSkill":
                    case "replaceUI":
                    case "rapidStream":
                    case "areaCtrl":
                    case "swimArea_Moment":
                    case "reactorRemove":
                    case "objectVisibleLevel":
                    case "bonusRewards":
                    case "incHealRate":
                    case "triggersTW":
                    case "climbArea_Moment":
                    case "crawlArea":
                    case "checkPoint":
                    case "mobKillCountExp":
                    case "ghostPark":
                    case "courtshipDance":
                    case "fishingZone":
                    case "remoteCharacterEffect":
                    case "publicTaggedObjectVisible":
                    case "MirrorFieldData":
                    case "defenseMob":
                    case "randomMobGen":
                    case "unusableSkillArea":
                    case "flyingAreaData":
                    case "extinctMO":
                    case "permittedSkill":
                    case "WindArea":
                    case "pocketdrop":
                    case "footprintData":
                    case "illuminantCluster": // 450016030.img
                    case "property": // 450016110.img
                    case "languageSchool": // 702090101.img
                    case "languageSchoolQuizTime":
                    case "languageSchoolMobSummonItemID":
                        continue;

                    default:
                        string error = string.Format("[MapLoader] Unknown field property '{0}', {1}", prop.Name, mapImage.ToString() /*overrides see WzImage.ToString()*/);

                        Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, error);
                        copyPropNames.Add(prop.Name);
                        break;
                }
            }
            return copyPropNames;
        }
    }
}
