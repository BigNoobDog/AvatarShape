using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using ShapingController;
using ShapingPlayer;

namespace ShapingUI
{
    public class OnSliderValueChange : UnityEngine.Events.UnityEvent<TYPE, int, float> { }
    static public class UIEventManager
    {
        static public void Init()
        {
            if (player != null)
                onUpdateSliderValue.AddListener(player.GetSliderEventHandle());
        }



        static public OnSliderValueChange onUpdateSliderValue = new OnSliderValueChange();


        static public void BindPlayer(Player p)
        {
            player = p;
        }


        static Player player;
    }


    public static class UISize
    {
        public static float sliderwidth = 0;
        public static float sliderheight = 30;
    }

}