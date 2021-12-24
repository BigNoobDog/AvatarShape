using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using ShapingController;

namespace ShapingUI
{


    public class ScrollerController : UIBehaviour
    {

        public ScrollerController()
        {
            blocks = new List<UIBlock>();
        }

        override protected void Start()
        {
            
        }

        public void Setup(TYPE typevalue, RectTransform blockp)
        {
            type = typevalue;
            BlockItemPrototype = blockp;

            Component[] objs = gameObject.GetComponentsInChildren<Component>();
            foreach(Component obj in objs)
            {
                if(obj != null && obj.gameObject != null && obj.gameObject.name == "Content")
                {
                    Content = obj.gameObject;
                }
            }
        }

        public void SetupBlockPrototype(RectTransform Prototype)
        {
            BlockItemPrototype = Prototype;
        }

        public void AddSliderItem(ShapingUIEditorSlider config)
        {
            int blockid = config.firstlevel;
            string desc = config.firstlevelDesc;
            int index = config.index;
            string itemdesc = config.thirdlevelDesc;

            //if the block whose index is blockid is not existed, Create one.
            CheckBlock(blockid, desc);

            UIBlock block = blocks[blockid];
            if (block.GetCurSliderNum() > index)
                return;

            block.UpdateInfo(blockid, desc);
            
            block.AddSliderItem(index, itemdesc);
        }

        private void CheckBlock(int blockid, string blockname)
        {
            if (blockid >= blocks.Count)
            {
                for (int i = blocks.Count; i <= blockid; i++)
                {
                    RectTransform blockpanel = GameObject.Instantiate(BlockItemPrototype);
                    if (blockpanel != null && Content != null)
                    {
                        Vector2 presizeD = blockpanel.sizeDelta;
                        Vector2 prePos = blockpanel.anchoredPosition;
                        prePos.y = 0;
                        blockpanel.SetParent(Content.transform);

                        blockpanel.anchoredPosition = prePos;
                        blockpanel.sizeDelta = presizeD;

                    }
                    blockpanel.name = blockname;
                    blockpanel.gameObject.SetActive(true);
                    UIBlock blockscript = blockpanel.GetComponent<UIBlock>();
                    blockscript.Setup(type);
                    blocks.Add(blockscript);
                }
            }
        }

        public void AddGroupSliderItem(ShapingUIEditorSlider config)
        {
            int blockid = config.firstlevel;
            string desc = config.firstlevelDesc;
            int index = config.index;
            string itemdesc = config.thirdlevelDesc;

            CheckBlock(blockid, desc);

            UIBlock block = blocks[blockid];

            block.UpdateInfo(blockid, desc);

            block.AddGroupSliderItem(index, itemdesc);
        }

        public void AddColorGroup(int blockid, int groupid, string blocdesc, string colorgroupdesc, List<ShapingColorTableItem> group)
        {

            CheckBlock(blockid, blocdesc);

            UIBlock block = blocks[blockid];

            block.UpdateInfo(blockid, blocdesc);

            block.AddColorGroup(groupid, colorgroupdesc, group);
        }


        public void AddTextureGroup(int blockid, int groupid, string blocdesc, string texturegroupdesc, List<ShapingTextureTableItem> group)
        {

            CheckBlock(blockid, blocdesc);

            UIBlock block = blocks[blockid];

            block.UpdateInfo(blockid, blocdesc);

            block.AddImageGroup(groupid, texturegroupdesc, group);
        }


        public void UpdateBlocksSize()
        {
            for(int i = 0; i < blocks.Count; i ++)
            {
                blocks[i].UpdateSize();
            }

            float PosY = 0;
            for(int i = 0; i < blocks.Count; i ++)
            {
                PosY = blocks[i].UpdatePos(PosY);
            }
            
        }


        public TYPE type;
        private RectTransform BlockItemPrototype;

        public GameObject Content;

        private List<UIBlock> blocks;
        private float width;
        private float height;

        private float sliderwidth;
        private float sliderheight;
        private float imagewidth;
        private float imageheight;
        private float colorwidth;
        private float colorheight;
    }
}