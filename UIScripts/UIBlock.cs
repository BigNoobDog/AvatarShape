using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using ShapingController;

namespace ShapingUI
{

    public class UIBlock : UIBehaviour
    {
        public UIBlock()
        {

            sliders = new List<SliderItem>();
        }

        // Start is called before the first frame update
        override protected void Start()
        {

        }

        //Public Function

        public void Setup(TYPE typevalue)
        {
            type = typevalue;
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

        public void AddSliderItem(int sliderindex, string desc)
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
            item.UpdateInfo(type, sliderindex, desc);
            item.UpdateItem();
            sliders.Add(item);
        }

        public void AddGroupSliderItem(int sliderindex, string desc)
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
            item.UpdateInfo(type, sliderindex, desc);
            item.UpdateItem();
            sliders.Add(item);
        }

        public void AddImageGroup(int groupid, string desc, List<ShapingTextureTableItem> group)
        {
            RectTransform newimagegroup = GameObject.Instantiate(ImageGroupPrototype);

            newimagegroup.name = desc;
            ImageGroup item = newimagegroup.GetComponentInChildren<ImageGroup>();

            Vector2 presizeD = newimagegroup.sizeDelta;
            //Vector2 prePos = newslider.anchoredPosition;
            presizeD.y += UISize.imagewidth * (group.Count / UISize.imagenumperrow) + 2 * UISize.imagemarginwidth;

            newimagegroup.SetParent(Content.transform);


            newimagegroup.anchoredPosition = new Vector2(0.0f, 0.0f);
            newimagegroup.sizeDelta = presizeD;


            item.gameObject.SetActive(true);
            item.Setup(type, groupid, desc, group, ImageItemPrototype);
            //item.UpdateSize();
            imagegroup = item;
        }

        public void AddPresetImageGroup(int groupid, string desc, List<ShapingPresetConfigItem> group)
        {
            RectTransform newimagegroup = GameObject.Instantiate(ImageGroupPrototype);

            newimagegroup.name = desc;
            ImageGroup item = newimagegroup.GetComponentInChildren<ImageGroup>();

            Vector2 presizeD = newimagegroup.sizeDelta;
            //Vector2 prePos = newslider.anchoredPosition;
            presizeD.y += UISize.Presetimagewidth * (group.Count / UISize.Presetimagenumperrow) + 2 * UISize.imagemarginwidth;

            newimagegroup.SetParent(Content.transform);


            newimagegroup.anchoredPosition = new Vector2(0.0f, 0.0f);
            newimagegroup.sizeDelta = presizeD;


            item.gameObject.SetActive(true);
            item.Setup(type, groupid, desc, group, ImageItemPrototype);
            //item.UpdateSize();
            imagegroup = item;
        }

        public void AddColorGroup(int groupid, string desc, List<ShapingColorTableItem> group)
        {
            RectTransform newcolorgroup = GameObject.Instantiate(ColorGroupPrototype);
            

            newcolorgroup.name = desc;
            ColorGroup item = newcolorgroup.GetComponentInChildren<ColorGroup>();

            Vector2 presizeD = newcolorgroup.sizeDelta;
            //Vector2 prePos = newslider.anchoredPosition;
            presizeD.y += UISize.imagewidth * (group.Count / UISize.imagenumperrow) + 2 * UISize.imagemarginwidth;

            newcolorgroup.SetParent(Content.transform);


            newcolorgroup.anchoredPosition = new Vector2(0.0f, 0.0f);
            newcolorgroup.sizeDelta = presizeD;

            
            item.gameObject.SetActive(true);
            item.Setup(type, groupid, desc, group, ColorItemPrototype);
            //item.UpdateSize();
            colorgroup = item;
        }

        public void SetUI(RectTransform ui)
        {

        }

        //Set the Inner items' position
        public void UpdateSize()
        {
            height = 0;

            //First: Settle the Image Group
            float imagegroupwidth = 0.0f;
            if (imagegroup != null)
            {
                imagegroupwidth = imagegroup.GetHeight();
                imagegroup.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -height);
            }
            height += imagegroupwidth;

            //Second:Settle the Color Group
            if(colorgroup != null)
            {
                float colorgroupwidth = colorgroup.GetHeight();
                colorgroup.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -height);
                height += colorgroupwidth;
            }

            //Third: Settle the Slider Group


            for(int i = 0; i < sliders.Count; i ++)
            {
                RectTransform srt = sliders[i].gameObject.GetComponent<RectTransform>();

                Vector2 pos = srt.anchoredPosition;
                pos.y = - i * UISize.sliderheight - height;
                srt.anchoredPosition = pos;

                Vector2 sd = srt.sizeDelta;
                sd.y = UISize.sliderheight;
                srt.sizeDelta = sd;
            }

            float sliderswidth = UISize.sliderheight * sliders.Count;
            height += sliderswidth;
            RectTransform brt = gameObject.GetComponent<RectTransform>();
            Vector2 blockSD = brt.sizeDelta;
            blockSD.y = height;
            brt.sizeDelta = blockSD;
        }

        public float UpdatePos(float pos)
        {
            startY = pos;
            RectTransform brt = gameObject.GetComponent<RectTransform>();
            Vector2 tmp_v2 = brt.anchoredPosition;
            tmp_v2.y = startY;
            brt.anchoredPosition = tmp_v2;

            return pos - height;
        }

        //Base Function
        public int GetCurSliderNum()
        {
            return sliders.Count;
        }

        public bool SetSliderValue(int sliderindex, float value)
        {
            foreach(SliderItem item in sliders)
            {
                if (item.SetShowValue(sliderindex, value))
                    return true;
            }
            return false;
        }


        public bool SetColorGroupValue(int groupindex, int value)
        {
            if (colorgroup == null)
                return false;

            return colorgroup.UpdateState(groupindex, value);
        }

        public bool SetTextureGroupValue(int groupindex, int value)
        {
            if (imagegroup == null)
                return false;

            return  imagegroup.UpdateState(groupindex, value);

        }

        //Private Variable
        int index;
        Text Desc;
        GameObject Content;
        private TYPE type;

        public RectTransform SliderItemPrototype;
        public RectTransform ImageItemPrototype;
        public RectTransform ColorItemPrototype;
        public RectTransform ColorGroupPrototype;
        public RectTransform ImageGroupPrototype;

        List<SliderItem> sliders;
        //List<ImageGroup> images;
        //List<ColorGroup> colors;
        ImageGroup imagegroup;
        ColorGroup colorgroup;

        float width;
        float height;

        float startY;
    }
}