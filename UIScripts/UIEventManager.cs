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

    public class OnMakeSliderValueChange : UnityEngine.Events.UnityEvent<TYPE, int, float> { }

    public class OnMakeColorItemChange : UnityEngine.Events.UnityEvent<TYPE, int, int> { }

    public class OnMakeImageItemChange : UnityEngine.Events.UnityEvent<TYPE, int, int> { }

    public class OnExport : UnityEngine.Events.UnityEvent<string> { }
    public class OnImport : UnityEngine.Events.UnityEvent<string> { }

    public class OnRandom : UnityEngine.Events.UnityEvent<TYPE> { }
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


                OnRandomEvent.AddListener(player.RandomData);
            }

            if (UIMan != null)
            {
                OnImportDataMakeSliderValueChange.AddListener(UIMan.GetMakeSliderValueChangeEventHandle());
                OnImportDataMakeColorValueChange.AddListener(UIMan.GetMakeColorValueChangeEventHandle());
                OnImportDataMakeImageValueChange.AddListener(UIMan.GetMakeImageValueChangeEventHandle());
            }
                
        }



        static public OnSliderValueChange onUpdateSliderValue = new OnSliderValueChange();

        static public OnColorItemChange onUpdateColorValue = new OnColorItemChange();

        static public OnImageItemChange onUpdateImageValue = new OnImageItemChange();

        static public OnExport OnExportEvent = new OnExport();

        static public OnImport OnImportEvent = new OnImport();

        static public OnRandom OnRandomEvent = new OnRandom();

        static public OnMakeSliderValueChange OnImportDataMakeSliderValueChange = new OnMakeSliderValueChange();

        static public OnMakeColorItemChange OnImportDataMakeColorValueChange = new OnMakeColorItemChange();

        static public OnMakeImageItemChange OnImportDataMakeImageValueChange = new OnMakeImageItemChange();


        static public void BindPlayer(Player p)
        {
            player = p;
        }
        static public void BindUIMan(UIManager uimanager)
        {
            UIMan = uimanager;
        }


        static UIManager UIMan;
        static Player player;
    }
}