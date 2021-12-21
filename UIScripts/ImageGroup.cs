using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ShapingController;
using System.Collections.Generic;

namespace ShapingUI
{

    public class ImageGroup : UIBehaviour
    {
        TYPE type;
        int index;
        public Text Desc;

        private List<Button> buttons;


        private string desc;
        public float value;

        protected override void Awake()
        {
            Desc = transform.gameObject.GetComponentInChildren<Text>();

        }

        //protected override void Setup()
        //{
        //    Revert();
        //}

        public void UpdateInfo(int value, string desc)
        {
            index = value;
            Desc.text = desc;
        }
    }
}