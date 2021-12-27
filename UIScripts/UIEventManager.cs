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
    public class OnColorItemChange : UnityEngine.Events.UnityEvent<TYPE, int, int, Color> { }
    public class OnImageItemChange : UnityEngine.Events.UnityEvent<TYPE, int, int, string> { }

    public class OnExport : UnityEngine.Events.UnityEvent<string> { }
    public class OnImport : UnityEngine.Events.UnityEvent<string> { }

    static public class UIEventManager
    {
        static public void Init()
        {
            if (player != null)
            {
                onUpdateSliderValue.AddListener(player.GetSliderEventHandle());
                onUpdateColorValue.AddListener(player.GetColorEventHandle());
                onUpdateImageValue.AddListener(player.GetImageEventHandle());
                OnImportEvent.AddListener(player.ImportData);
                OnExportEvent.AddListener(player.ExportData);
            }
                
        }



        static public OnSliderValueChange onUpdateSliderValue = new OnSliderValueChange();

        static public OnColorItemChange onUpdateColorValue = new OnColorItemChange();

        static public OnImageItemChange onUpdateImageValue = new OnImageItemChange();

        static public OnExport OnExportEvent = new OnExport();

        static public OnImport OnImportEvent = new OnImport();


        static public void BindPlayer(Player p)
        {
            player = p;
        }


        static Player player;
    }
}