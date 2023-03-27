using MapleLib;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Maple
{
    public static class WzInfoUtil
    {

        #region Point

        public static Vector2 PointFToSystemPoint(PointF source)
        {
            return new Vector2((int)source.X, (int)source.Y);
        }

        public static System.Drawing.Point VectorToSystemPoint(WzVectorProperty source)
        {
            return new System.Drawing.Point(source.X.Value, source.Y.Value);
        }

        #endregion

        #region Object
        public static string GetMobNameById(string id, WzFileManager fileManager)
        {
            id = RemoveLeadingZeros(id);

            WzImage stringWzDirs = (WzImage)fileManager.FindWzImageByName("string", "Mob.img");
            if (stringWzDirs != null)
            {
                WzObject mobObj = stringWzDirs[id];
                WzStringProperty mobName = (WzStringProperty)mobObj["name"];
                if (mobName == null)
                    return "";

                return mobName.Value;
            }
            return "";
        }

        public static string GetNpcNameById(string id, WzFileManager fileManager)
        {
            id = RemoveLeadingZeros(id);

            WzImage stringWzDirs = (WzImage)fileManager.FindWzImageByName("string", "Npc.img");
            if (stringWzDirs != null)
            {
                WzObject npcObj = stringWzDirs[id];
                WzStringProperty npcName = (WzStringProperty)npcObj["name"];
                if (npcName == null)
                    return "";

                return npcName.Value;
            }
            return "";
        }

        public static WzSubProperty GetMapStringProp(string id, WzFileManager fileManager)
        {
            id = RemoveLeadingZeros(id);

            WzImage mapImg = (WzImage)fileManager.FindWzImageByName("string", "Map.img");
            if (mapImg != null)
            {
                foreach (WzSubProperty mapNameCategory in mapImg.WzProperties)
                {
                    WzSubProperty mapNameDirectory = (WzSubProperty)mapNameCategory[id];
                    if (mapNameDirectory != null)
                    {
                        return mapNameDirectory;
                    }
                }
            }
            return null;
        }

        public static string GetMapName(WzSubProperty mapProp)
        {
            if (mapProp == null)
            {
                return "";
            }
            WzStringProperty mapName = (WzStringProperty)mapProp["mapName"];
            if (mapName == null)
            {
                return "";
            }
            return mapName.Value;
        }

        public static string GetMapStreetName(WzSubProperty mapProp)
        {
            if (mapProp == null)
            {
                return "";
            }
            WzStringProperty streetName = (WzStringProperty)mapProp["streetName"];
            if (streetName == null)
            {
                return "";
            }
            return streetName.Value;
        }

        public static string GetMapCategoryName(WzSubProperty mapProp)
        {
            if (mapProp == null)
            {
                return "";
            }
            return mapProp.Parent.Name;
        }

        public static WzObject GetObjectByRelativePath(WzObject currentObject, string path)
        {
            foreach (string directive in path.Split("/".ToCharArray()))
            {
                if (directive == "..")
                    currentObject = currentObject.Parent;
                else if (currentObject is WzImageProperty)
                    currentObject = ((WzImageProperty)currentObject)[directive];
                else if (currentObject is WzImage)
                    currentObject = ((WzImage)currentObject)[directive];
                else if (currentObject is WzDirectory)
                    currentObject = ((WzDirectory)currentObject)[directive];
                else throw new Exception("invalid type");
            }
            return currentObject;
        }

        public static WzObject ResolveUOL(WzUOLProperty uol)
        {
            WzObject wzObjectInCurrentWz = (WzObject)GetObjectByRelativePath(uol.Parent, uol.Value);

            return wzObjectInCurrentWz;
        }

        public static string RemoveExtension(string source)
        {
            if (source.Substring(source.Length - 4) == ".img")
                return source.Substring(0, source.Length - 4);
            return source;
        }

        public static WzImageProperty GetRealProperty(WzImageProperty prop)
        {
            if (prop is WzUOLProperty)
                return (WzImageProperty)ResolveUOL((WzUOLProperty)prop);
            else
                return prop;
        }

        public static WzCanvasProperty GetMobImage(WzImage parentImage)
        {
            WzSubProperty standParent = (WzSubProperty)parentImage["stand"];
            if (standParent != null)
            {
                WzCanvasProperty frame1 = (WzCanvasProperty)GetRealProperty(standParent["0"]);
                if (frame1 != null) return frame1;
            }
            WzSubProperty flyParent = (WzSubProperty)parentImage["fly"];
            if (flyParent != null)
            {
                WzCanvasProperty frame1 = (WzCanvasProperty)GetRealProperty(flyParent["0"]);
                if (frame1 != null) return frame1;
            }
            return null;
        }

        public static WzCanvasProperty GetNpcImage(WzImage parentImage)
        {
            WzSubProperty standParent = (WzSubProperty)parentImage["stand"];
            if (standParent != null)
            {
                WzCanvasProperty frame1 = (WzCanvasProperty)GetRealProperty(standParent["0"]);
                if (frame1 != null) return frame1;
            }
            return null;
        }

        public static WzCanvasProperty GetReactorImage(WzImage parentImage)
        {
            WzSubProperty action0 = (WzSubProperty)parentImage["0"];
            if (action0 != null)
            {
                WzCanvasProperty frame1 = (WzCanvasProperty)GetRealProperty(action0["0"]);
                if (frame1 != null) return frame1;
            }
            return null;
        }

        /// <summary>
        /// Finds a map image from the list of Map.wz
        /// On pre 64-bit client:
        /// Map.wz/Map/Map1/10000000.img
        /// 
        /// On post 64-bit client:
        /// Map/Map/Map1/Map1_000.wz/10000000.img
        /// </summary>
        /// <param name="mapid"></param>
        /// <returns></returns>
        public static WzImage FindMapImage(string mapid, WzFileManager fileManager)
        {
            string mapIdNamePadded = AddLeadingZeros(mapid, 9) + ".img";
            string mapcat = "Map" + mapIdNamePadded.Substring(0, 1);

            List<WzDirectory> mapWzDirs = fileManager.GetWzDirectoriesFromBase("map");
            foreach (WzDirectory mapWzDir in mapWzDirs)
            {
                WzImage mapImage = (WzImage)mapWzDir?["Map"]?[mapcat]?[mapIdNamePadded];
                if (mapImage != null)
                    return mapImage;
            }
            return null;
        }

        #endregion

        #region String
        public static string GetString(this WzImageProperty source)
        {
            return source == null ? null : source.GetString();
        }

        public static WzStringProperty SetString(string value)
        {
            return new WzStringProperty("", value);
        }

        public static string GetOptionalString(this WzImageProperty source)
        {
            return source == null ? null : source.GetString();
        }

        public static WzStringProperty SetOptionalString(string value)
        {
            return value == null ? null : SetString(value);
        }

        /// <summary>
        /// Add leading zeros to the source string. (pad left)
        /// i.e 550  = 0000550
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string AddLeadingZeros(string source, int maxLength)
        {
            return source.PadLeft(maxLength, '0');
        }

        /// <summary>
        /// remove leading zeros
        /// i.e 0000550 = 550
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveLeadingZeros(string source)
        {
            int firstNonZeroIndex = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != (char)0x30) //char at index i is not 0
                {
                    firstNonZeroIndex = i;
                    break;
                }
                else if (i == source.Length - 1) //all chars are 0, return 0
                    return "0";
            }
            return source.Substring(firstNonZeroIndex);
        }
        #endregion

        #region Double
        public static double GetDouble(this WzImageProperty source)
        {
            return source == null ? 0 : source.GetDouble();
        }

        public static WzDoubleProperty SetDouble(double value)
        {
            return new WzDoubleProperty("", value);
        }
        #endregion

        #region Integer
        public static int GetInt(this WzImageProperty source, int default_ = 0)
        {
            return source == null ? default_ : source.GetInt();
        }

        public static WzIntProperty SetInt(int value)
        {
            return new WzIntProperty("", value);
        }

        public static int? GetOptionalInt(this WzImageProperty source, int? default_ = null)
        {
            return source == null ? (int?)default_ : source.GetInt();
        }

        public static WzIntProperty SetOptionalInt(int? value)
        {
            return value.HasValue ? SetInt(value.Value) : null;
        }
        #endregion

        #region Translated Integer
        public static int? GetOptionalTranslatedInt(this WzImageProperty source)
        {
            string str = WzInfoUtil.GetOptionalString(source);
            if (str == null) return null;
            return int.Parse(str);
        }

        public static WzStringProperty SetOptionalTranslatedInt(int? value)
        {
            if (value.HasValue)
            {
                return SetString(value.Value.ToString());
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Rectangle
        /// <summary>
        /// Gets System.Drawing.Rectangle from "lt" and "rb"
        /// </summary>
        /// <param name="parentSource"></param>
        /// <returns></returns>
        public static Rectangle GetLtRbRectangle(this WzImageProperty parentSource)
        {
            WzVectorProperty lt = WzInfoUtil.GetOptionalVector(parentSource["lt"]);
            WzVectorProperty rb = WzInfoUtil.GetOptionalVector(parentSource["rb"]);

            int width = rb.X.Value - lt.X.Value;
            int height = rb.Y.Value - lt.Y.Value;

            Rectangle rectangle = new Rectangle(
                lt.X.Value,
                lt.Y.Value,
                width,
                height);
            return rectangle;
        }

        /// <summary>
        /// Sets the "lt" and "rb" value in a WzImageProperty parentSource
        /// derived from Rectangle
        /// </summary>
        /// <param name="parentSource"></param>
        /// <param name="rect"></param>
        public static void SetLtRbRectangle(this WzImageProperty parentSource, Rectangle rect)
        {
            parentSource["lt"] = WzInfoUtil.SetVector(rect.X, rect.Y);
            parentSource["rb"] = WzInfoUtil.SetVector(rect.X + rect.Width, rect.Y + rect.Height);
        }
        #endregion

        #region Vector
        /// <summary>
        /// Gets the vector value of the WzImageProperty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static WzVectorProperty GetVector(this WzImageProperty source)
        {
            return (WzVectorProperty)source;
        }

        /// <summary>
        /// Sets vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static WzVectorProperty SetVector(float x, float y)
        {
            return new WzVectorProperty("", x, y);
        }


        /// <summary>
        /// Gets an optional Vector. 
        /// Returns x = 0, and y = 0 if the WzImageProperty is not found.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static WzVectorProperty GetOptionalVector(this WzImageProperty source)
        {
            if (source == null)
                return new WzVectorProperty(String.Empty, 0, 0);

            return GetVector(source);
        }

        #endregion

        #region Long
        public static long GetLong(this WzImageProperty source)
        {
            return source.GetLong();
        }

        public static WzLongProperty SetLong(long value)
        {
            return new WzLongProperty("", value);
        }

        public static long? GetOptionalLong(this WzImageProperty source)
        {
            return source == null ? (long?)null : source.GetLong();
        }

        public static WzLongProperty SetOptionalLong(long? value)
        {
            return value.HasValue ? SetLong(value.Value) : null;
        }
        #endregion

        #region Boolean
        public static bool GetBool(this WzImageProperty source)
        {
            if (source == null)
                return false;
            return source.GetInt() == 1;
        }

        public static WzIntProperty SetBool(bool value)
        {
            return new WzIntProperty("", value ? 1 : 0);
        }

        public static bool? GetOptionalBool(this WzImageProperty source)
        {
            return (source == null ? null : true);
        }

        public static WzIntProperty SetOptionalBool(bool? value)
        {
            // return value.HasValue ? SetBool(value.Value) : null;
            return (value == null ? null : SetBool(value.Value));
        }
        #endregion

        #region Float
        public static float GetFloat(this WzImageProperty source)
        {
            return source == null ? 0 : source.GetFloat();
        }

        public static WzFloatProperty SetFloat(float value)
        {
            return new WzFloatProperty("", value);
        }

        public static float? GetOptionalFloat(this WzImageProperty source)
        {
            return source == null ? (float?)null : source.GetFloat();
        }

        public static WzFloatProperty SetOptionalFloat(float? value)
        {
            return value.HasValue ? SetFloat(value.Value) : null;
        }
        #endregion
    }
}
