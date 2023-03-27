using Maple.Map.Info;
using Maple.Util;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maple.Map
{
    public static class MapleMapLoader
    {

        public static void LoadMap(string mapNameId, MapleBoard board)
        {
            // current map id, 9 length
            string mapId_ = null;
            string mapName_ = null;
            string streetName_ = "";
            string categoryName_ = "";

            int mapId = -1;
            WzImage mapImage = null;
            WzSubProperty strMapProp = null;

            if (mapNameId.StartsWith("MapLogin"))
            {
                List<WzDirectory> uiWzDirs = WzResourceController.wzFileManager_.GetWzDirectoriesFromBase("ui");
                foreach (WzDirectory uiWzDir in uiWzDirs)
                {
                    mapImage = (WzImage)uiWzDir?[mapNameId + ".img"];
                    if (mapImage != null)
                    {
                        break;
                    }
                }
         
                mapId_ = mapName_ = streetName_ = categoryName_ = mapNameId;
            }
            else if (mapNameId == "CashShopPreview")
            {
                List<WzDirectory> uiWzDirs = WzResourceController.wzFileManager_.GetWzDirectoriesFromBase("ui");
                foreach (WzDirectory uiWzDir in uiWzDirs)
                {
                    mapImage = (WzImage)uiWzDir?["CashShopPreview.img"];
                    if (mapImage != null)
                    {
                        break;
                    }
                }
                
                mapId_ = mapName_ = streetName_ = categoryName_ = "CashShopPreview";
            }
            else
            {
                mapId_ = mapNameId;
                int.TryParse(mapId_, out mapId);

                mapImage = WzInfoUtil.FindMapImage(mapId.ToString(), WzResourceController.wzFileManager_);
                board.MiniMap = GetMiniMapAvaliable(mapImage);

                strMapProp = WzInfoUtil.GetMapStringProp(mapId_, WzResourceController.wzFileManager_);
                mapName_ = WzInfoUtil.GetMapName(strMapProp);
                streetName_ = WzInfoUtil.GetMapStreetName(strMapProp);
                categoryName_ = WzInfoUtil.GetMapCategoryName(strMapProp);
            }

            MapleCreator.CreateMapFromImage(mapId, mapImage, mapName_, streetName_, categoryName_, strMapProp, ref board);
        }

        private static Texture2D GetMiniMapAvaliable(WzImage mapImage)
        {
            if (mapImage == null)
            {
                return null;
            }

            WzImageResource rsrc = new WzImageResource(mapImage);

            if (mapImage["info"]["link"] == null)
            {
                WzCanvasProperty mini = (WzCanvasProperty)mapImage.GetFromPath("miniMap/canvas");
                if (mini != null)
                {
                    return MapleUtil.BitMapToTex(mini.GetLinkedWzCanvasBitmap());
                }
            }

            return null;
        }

        
    }
}
