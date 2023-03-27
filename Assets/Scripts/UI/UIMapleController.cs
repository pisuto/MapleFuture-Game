using MapleLib.WzLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maple.Map;
using Maple.Map.Info;
using Maple.Map.Instance;

namespace Maple.UI
{
    public class UIMapleController
    {
        public int mapShiftX_ = 0;
        public int mapShiftY_ = 0;
        public Vector2Int miniMapPos_;

        private int width_;
        private int height_;
        private int renderWidth_;
        private int renderHeight_;
        private float renderObjectScaling = 1.0f;
        private float UserScreenScaleFactor = 1.0f;

        private readonly MapleTexturePool texturePool = new MapleTexturePool();

        public const int MIN_UI_UPDATE_TIME = 10; // ms
        public const int MAX_WZ_RESOURCE_CURSOR_NUM = 13;
        public const int INITIAL_GIF_UPDATE_TIME = 150;

        // Cursor
        private CursorItem cursor_;

        // Board
        private MapleBoard board_;

        public UIMapleController(MapleBoard board)
        {
            this.board_ = board;
        }

        public void Intialize()
        {
            
        }

        public void UpdateUI()
        {
            //cursor_.UpdateCursorState();

            

            
        }

        public void UIEvent()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    cursor_.SetActiveCursorType(MAPLE_CUROR_STATUS.MAPLE_CURSOR_CLICKED);
            //}

            //if (Input.GetMouseButtonUp(0))
            //{
            //    cursor_.SetActiveCursorType(MAPLE_CUROR_STATUS.MAPLE_CURSOR_NORMAL);
            //}
        }
    }
}
