using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maple.UI;
using Timers;
using Maple.Map;
using Maple.Map.Info;

namespace Maple
{
    public class MapleGameManager : MonoBehaviour
    {
        /// <summary>
        /// controller of wz files
        /// </summary>
        private WzResourceController wzController_ = null;
        private UIMapleController uiController_ = null;
        private MapleBoard mapBoard_ = null;

        private void Awake()
        {
            wzController_ = new WzResourceController();
            wzController_.Intialize();

            mapBoard_ = new MapleBoard(Vector2Int.zero, Vector2Int.zero);
            mapBoard_.LoadContent("000030000", this);

            uiController_ = new UIMapleController(mapBoard_);
            uiController_.Intialize();
        }

        private void Start()
        {
            TimersManager.SetLoopableTimer(this, 0.01f, UIUpdateTimer);

            mapBoard_.DrawBackgrounds(Time.time);
        }

        private void Update()
        {
            //uiController_.UIEvent();

            // Create Backgrounds
            
        }

        /// <summary>
        /// 0.01s timer for UI update
        /// </summary>
        private void UIUpdateTimer()
        {
            uiController_.UpdateUI();
        }
    }
}
