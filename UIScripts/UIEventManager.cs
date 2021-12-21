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
    public class OnColorItemChange : UnityEngine.Events.UnityEvent<TYPE, int, Color> { }
    static public class UIEventManager
    {
        static public void Init()
        {
            if (player != null)
            {
                onUpdateSliderValue.AddListener(player.GetSliderEventHandle());
                onUpdateColorValue.AddListener(player.GetColorEventHandle());
            }
                
        }



        static public OnSliderValueChange onUpdateSliderValue = new OnSliderValueChange();

        static public OnColorItemChange onUpdateColorValue = new OnColorItemChange();


        static public void BindPlayer(Player p)
        {
            player = p;
        }


        static Player player;
    }
}