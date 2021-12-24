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
        GameObject Content;

        private List<ImageItem> buttons = new List<ImageItem>();


        private string desc;
        public float value;

        private float height;

        protected override void Start()
        {
        }

        protected override void Awake()
        {
            Desc = transform.gameObject.GetComponentInChildren<Text>();

        }

        //protected override void Setup()
        //{
        //    Revert();
        //}

        public void Setup(TYPE typevalue, int value, string desc, List<ShapingTextureTableItem> group, RectTransform imageitem)
        {
            type = typevalue;
            index = value;
            Desc.text = desc;

            Component[] objs = gameObject.GetComponentsInChildren<Component>();
            foreach (Component obj in objs)
            {
                if (obj != null && obj.gameObject != null && obj.gameObject.name == "Content")
                {
                    Content = obj.gameObject;
                }
            }

            for (int i = 0; i < group.Count; i++)
            {
                RectTransform imageiteminstance = GameObject.Instantiate(imageitem);
                imageiteminstance.gameObject.SetActive(true);
                imageiteminstance.name = i.ToString();
                buttons.Add(imageiteminstance.GetComponentInChildren<ImageItem>());
                imageiteminstance.GetComponentInChildren<ImageItem>().Setup(type, index, i, group[i].ThumbPath, group[i].Path, this);
                RectTransform rt = imageiteminstance.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(UISize.imagewidth, UISize.imagewidth);
                imageiteminstance.transform.SetParent(Content.transform);

                int row = i / UISize.imagenumperrow;
                int col = i % UISize.imagenumperrow;
                float tmp_x = col * (UISize.imagewidth + UISize.imagemarginwidth) + UISize.imagemarginwidth;
                float tmp_y = -row * (UISize.imagewidth + UISize.imagemarginwidth) - UISize.imagemarginwidth;
                rt.anchoredPosition = new Vector2(tmp_x, tmp_y);
            }

            RectTransform mainrt = GetComponent<RectTransform>();

            //Ceiling
            int rows = (buttons.Count + (UISize.imagenumperrow - 1)) / UISize.imagenumperrow;

            height = rows * (UISize.imagemarginwidth + UISize.imagewidth) + UISize.imagemarginwidth;
            mainrt.sizeDelta = new Vector2(0, height);

        }

        public void UpdateState(int groupindex, int itemindex)
        {
            if (groupindex != index)
                return;

            for (int i = 0; i < buttons.Count; i++)
            {
                ImageItem item = buttons[i];
                if (item.GetIndex() == itemindex)
                {
                    item.SetOutlineSelected(true);
                }
                else
                {
                    item.SetOutlineSelected(false);
                }
            }
        }

        public float GetHeight()
        {
            return height;
        }
    }

}