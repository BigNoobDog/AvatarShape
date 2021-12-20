using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace ShapingUI
{

    public class UIBlock : UIBehaviour
    {
        public UIBlock()
        {

            sliders = new List<SliderItem>();
            images = new List<ImageItem>();
            colors = new List<ColorItem>();
        }

        // Start is called before the first frame update
        override protected void Start()
        {

        }

        //Public Function

        public void Setup()
        {
            Text[] items = gameObject.GetComponentsInChildren<Text>();
            foreach(Text item in items)
            {
                if(item.name == "Desc")
                {
                    Desc = item;
                }
            }

            Component[] cs = GetComponentsInChildren<Component>();
            foreach(Component c in cs)
            {
                if(c.name == "Content")
                {
                    Content = c.gameObject;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateInfo(int value, string desc)
        {
            index = value;
            Desc.text = desc;
        }

        public void AddSliderItem(int index, string desc)
        {
            RectTransform newslider = GameObject.Instantiate(SliderItemPrototype);
            newslider.name = desc;
            SliderItem item = newslider.GetComponent<SliderItem>();

            Vector2 presizeD = newslider.sizeDelta;
            //Vector2 prePos = newslider.anchoredPosition;
            presizeD.y += UISize.sliderheight;
            
            newslider.SetParent(Content.transform);


            newslider.anchoredPosition = new Vector2(0.0f, 0.0f);
            newslider.sizeDelta = presizeD;

            item.gameObject.SetActive(true);
            item.UpdateInfo(index, desc);
            item.UpdateItem();
            sliders.Add(item);
        }

        public void AddImageGroup()
        {

        }

        public void AddColorGroup()
        {

        }

        public void SetUI(RectTransform ui)
        {

        }

        public void UpdateSize()
        {
            float sliderswidth = UISize.sliderheight * sliders.Count;
            for(int i = 0; i < sliders.Count; i ++)
            {
                RectTransform srt = sliders[i].gameObject.GetComponent<RectTransform>();
                Vector2 pos = srt.anchoredPosition;
                pos.y = - i * UISize.sliderheight;
                Vector2 sd = srt.sizeDelta;
                sd.y = UISize.sliderheight;
                srt.anchoredPosition = pos;
                srt.sizeDelta = sd;
            }

            RectTransform brt = gameObject.GetComponent<RectTransform>();
            Vector2 blockSD = brt.sizeDelta;
            blockSD.y = sliderswidth;
            brt.sizeDelta = blockSD;
        }

        public float UpdatePos(float pos)
        {
            startY = pos;
            RectTransform brt = gameObject.GetComponent<RectTransform>();
            Vector2 tmp_v2 = brt.anchoredPosition;
            tmp_v2.y = startY;
            brt.anchoredPosition = tmp_v2;

            return pos - sliders.Count * UISize.sliderheight;
        }

        //Base Function
        public int GetCurSliderNum()
        {
            return sliders.Count;
        }


        //Private Variable
        int index;
        Text Desc;
        GameObject Content;

        public RectTransform SliderItemPrototype;
        public RectTransform ImageItemPrototype;
        public RectTransform ColorItemPrototype;

        List<SliderItem> sliders;
        List<ImageItem> images;
        List<ColorItem> colors;

        float width;
        float height;

        float startY;
    }
}