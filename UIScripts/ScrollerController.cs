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

        public void Setup(TYPE typevalue, RectTransform blockp, ShapingControllerCore corevalue)
        {
            type = typevalue;
            BlockItemPrototype = blockp;
            core = corevalue;

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

        public void AddPresetTextureGroup(string texturegroupdesc, List<ShapingPresetConfigItem> group)
        {

            CheckBlock(0, UISize.PresetName);

            UIBlock block = blocks[0];

            block.UpdateInfo(0, texturegroupdesc);

            block.AddPresetImageGroup(0, ShapingModel.MODELNAME[0], group);
        }

        //TEMP
        public void AddModelBlock()
        {

            AddClothBlock(TYPE.CLOTH_HAIR, UISize.HairName);
            AddClothBlock(TYPE.CLOTH_SHIRT, UISize.ShirtName);
            AddClothBlock(TYPE.CLOTH_DRESS, UISize.DressName);
            AddClothBlock(TYPE.CLOTH_SHOES, UISize.ShoesName);


            UIBlock block = blocks[0];

            //if(type == TYPE.CLOTH_HAIR)
            {
                List<ShapingTextureTableItem> tmp_list = new List<ShapingTextureTableItem>();
                block.UpdateInfo(0, block.transform.name);
                ShapingTextureTableItem item = new ShapingTextureTableItem();
                item.Path = "Xiayu_hair";
                item.ThumbPath = "Hair1";
                ShapingTextureTableItem item1 = new ShapingTextureTableItem();
                //block.UpdateInfo(0, block.transform.name);
                item1.Path = "Base_hair";
                item1.ThumbPath = "Hair2";

                ShapingTextureTableItem item2 = new ShapingTextureTableItem();
                //block.UpdateInfo(2, block.transform.name);
                item2.Path = "Xiayu_hair_02";
                item2.ThumbPath = "Hair3";
                ShapingTextureTableItem item3 = new ShapingTextureTableItem();
                //block.UpdateInfo(3, block.transform.name);
                item3.Path = "Base_hair_02";
                item3.ThumbPath = "Hair4";
                tmp_list.Add(item);
                tmp_list.Add(item1);
                tmp_list.Add(item2);
                tmp_list.Add(item3);
                block.AddModelGroup(0, "模型", tmp_list);
                block.AddColorGroup(0, "颜色", core.GetColorGroup(1));
            }
            //else if(type == TYPE.CLOTH_SHIRT)
            {
                List<ShapingTextureTableItem> tmp_list = new List<ShapingTextureTableItem>();
                block = blocks[1];
                block.UpdateInfo(1, block.transform.name);
                ShapingTextureTableItem item = new ShapingTextureTableItem();
                item.Path = "Xiayu_clothes";
                item.ThumbPath = "shangyi2";
                ShapingTextureTableItem item1 = new ShapingTextureTableItem();
                item1.Path = "Base_clothes";
                item1.ThumbPath = "shangyi1";
                tmp_list.Add(item);
                tmp_list.Add(item1);
                block.AddModelGroup(0, "模型", tmp_list);
                block.AddColorGroup(0, "颜色", core.GetColorGroup(1));
            }
            //else if (type == TYPE.CLOTH_DRESS)
            {
                List<ShapingTextureTableItem> tmp_list = new List<ShapingTextureTableItem>();
                block = blocks[2];
                block.UpdateInfo(2, block.transform.name);
                ShapingTextureTableItem item = new ShapingTextureTableItem();
                item.Path = "Xiayu_kuzi";
                item.ThumbPath = "kuzi2";
                ShapingTextureTableItem item1 = new ShapingTextureTableItem();
                item1.Path = "Base_kuzi";
                item1.ThumbPath = "kuzi1";
                tmp_list.Add(item);
                tmp_list.Add(item1);
                block.AddModelGroup(0, "模型", tmp_list);
                block.AddColorGroup(0, "颜色", core.GetColorGroup(1));
            }
            //else if (type == TYPE.CLOTH_SHOES)
            {
                List<ShapingTextureTableItem> tmp_list = new List<ShapingTextureTableItem>();
                block = blocks[3];
                block.UpdateInfo(3, block.transform.name);
                ShapingTextureTableItem item = new ShapingTextureTableItem();
                item.Path = "Xiayu_shoes";
                item.ThumbPath = "shoes1";
                ShapingTextureTableItem item1 = new ShapingTextureTableItem();
                item1.Path = "Base_shoes";
                item1.ThumbPath = "shoes2";
                tmp_list.Add(item);
                tmp_list.Add(item1);
                block.AddModelGroup(0, "模型", tmp_list);
                block.AddColorGroup(0, "颜色", core.GetColorGroup(1));
            }

            foreach(var item in blocks)
            {
                item.UpdateSize();
            }    
        }

        public void AddClothBlock(TYPE typevalue, string lablename)
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
            blockpanel.name = lablename;
            blockpanel.gameObject.SetActive(true);
            UIBlock blockscript = blockpanel.GetComponent<UIBlock>();
            blockscript.Setup(typevalue);
            blocks.Add(blockscript);
        }

        public void AddColorBlock()
        {
            //CheckBlock(0, UISize.PresetName);
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

        public void UpdateContentSize()
        {
            
            for (int i = 0; i < blocks.Count; i++)
            {
                Height += blocks[i].GetHeight();
            }
            RectTransform rt = Content.GetComponent<RectTransform>();
            if (Height > rt.sizeDelta.y)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, Height);
            }
            //Content.transform.D
        }

        public void SetSliderValue(int sliderindex, float value)
        {
            foreach(UIBlock block in blocks)
            {
                if (block.SetSliderValue(sliderindex, value))
                    return;
            }
        }

        public void SetColorGroupValue(int groupindex, int value)
        {
            foreach (UIBlock block in blocks)
            {
                if (block.SetColorGroupValue(groupindex, value))
                    return;
            }
        }

        public void SetTextureGroupValue(int groupindex, int value)
        {
            foreach (UIBlock block in blocks)
            {
                if (block.SetTextureGroupValue(groupindex, value))
                    return;
            }
        }

        public void SetRandom()
        {

        }
        
        public void SetTYPE(TYPE value)
        {
            type = value;
        }

        public void ShowClothSubCatelog(TYPE typevalue)
        {
            if (blocks.Count != 4)
                return;

            foreach (var item in blocks)
            {
                item.gameObject.SetActive(false);
            }

            if (typevalue == TYPE.CLOTH_HAIR)
            {
                blocks[0].gameObject.SetActive(true);
            }
            else if(typevalue == TYPE.CLOTH_SHIRT)
            {
                blocks[1].gameObject.SetActive(true);
            }
            else if(typevalue == TYPE.CLOTH_DRESS)
            {
                blocks[2].gameObject.SetActive(true);
            }
            else if(typevalue == TYPE.CLOTH_SHOES)
            {
                blocks[3].gameObject.SetActive(true);
            }
        }

        public TYPE GetTYPE()
        {
            return type;
        }


        private TYPE type;
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
        private float Height;

        private ShapingControllerCore core;
    }
}