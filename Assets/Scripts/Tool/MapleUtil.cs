using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Maple.Util
{
    public class MapleUtil
    {
        public static Texture2D BitMapToTex(Bitmap bitMap)
        {
            Texture2D tex = new Texture2D(bitMap.Width, bitMap.Height, TextureFormat.RGBA32, false);
#if UNITY_EDITOR
            tex.alphaIsTransparency = true;
#endif
            using (MemoryStream stream = new MemoryStream())
            {
                bitMap.Save(stream, ImageFormat.Png);
                var buffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buffer, 0, buffer.Length);
                tex.LoadImage(buffer);
            }

            return tex;
        }

        public static string EnumToString(object value)
        {
            int? num = (int)value;
            return num?.ToString();
        }

        public static RectInt? GetRect(Rectangle rect)
        {
            if (rect == null) return null;
            return new RectInt(rect.X, rect.Y, rect.Width, rect.Height);
        }

        //public static int GetNegativeInt(int num)
        //{       
        //    return -num;
        //}

        public static float GetNegativeFloat(float num)
        {
            return -num;
        }

        public static Vector2Int GetVectorInt(PointF point)
        {
            return new Vector2Int((int)point.X, (int)point.Y);
        }
    }
}
