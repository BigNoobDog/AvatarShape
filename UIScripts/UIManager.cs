using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using ShapingController;

namespace ShapingUI
{

    public class UIManager : UIBehaviour
    {
        // Start is called before the first frame update
        override protected void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        //--------------Private Function
        public void Setup()
        {
            SetupUIElements();
            SetupFaceUI();
            SetupBodyUI();
            SetupHairUI();
            SetupClothUI();
            SetupMakeupUI();
            SetupPresetUI();

            OnClickFaceButton();
        }
        
        private void SetupUIElements()
        {
            FaceB   = GameObject.Find("FaceB").GetComponent<Button>();
            BodyB   = GameObject.Find("BodyB").GetComponent<Button>();
            HairB   = GameObject.Find("HairB").GetComponent<Button>();
            ClothB  = GameObject.Find("ClothB").GetComponent<Button>();
            MakeupB = GameObject.Find("MakeupB").GetComponent<Button>();
            PresetB = GameObject.Find("PresetB").GetComponent<Button>();

            Import = GameObject.Find("Btn_Import").GetComponent<Button>();
            Export = GameObject.Find("Btn_Export").GetComponent<Button>();

            FacePanel   = GameObject.Find("FacePanel");
            BodyPanel   = GameObject.Find("BodyPanel");
            HairPanel   = GameObject.Find("HairPanel");
            ClothPanel  = GameObject.Find("ClothPanel");
            MakeupPanel = GameObject.Find("MakeupPanel");
            PresetPanel = GameObject.Find("PresetPanel");

            if (FaceB != null && FacePanel != null)
            {
                FaceB.onClick.AddListener(OnClickFaceButton);
            }

            if (BodyB != null && BodyPanel != null)
            {
                BodyB.onClick.AddListener(OnClickBodyButton);
            }

            if (HairB != null && HairPanel != null)
            {
                HairB.onClick.AddListener(OnClickHairButton);
            }

            if (ClothB != null && ClothPanel != null)
            {
                ClothB.onClick.AddListener(OnClickClothButton);
            }

            if (MakeupB != null && MakeupPanel != null)
            {
                MakeupB.onClick.AddListener(OnClickMakeupButton);
            }

            if (PresetB != null && PresetPanel != null)
            {
                PresetB.onClick.AddListener(OnClickPresetButton);
            }

            if (Import != null)
            {
                Import.onClick.AddListener(OnClickImport);
            }
            if (Export != null)
            {
                Export.onClick.AddListener(OnClickExport);
            }



            GameObject temobj = GameObject.Find("FaceScroller");
            if(temobj != null)
            {
                FaceScroller = temobj.AddComponent<ScrollerController>();
                FaceScroller.name = "FaceScroller";
                FaceScroller.gameObject.SetActive(true);    
                FaceScroller.Setup(TYPE.FACE, BlockItemPrototype);
            }

            temobj = GameObject.Find("BodyScroller");
            if (temobj != null)
            {
                BodyScroller = temobj.AddComponent<ScrollerController>();
                BodyScroller.name = "BodyScroller";
                BodyScroller.gameObject.SetActive(true);
                BodyScroller.Setup(TYPE.BODY, BlockItemPrototype);
            }

            temobj = GameObject.Find("MakeupScroller");
            if (temobj != null)
            {
                MakeupScroller = temobj.AddComponent<ScrollerController>();
                MakeupScroller.name = "MakeupScroller";
                MakeupScroller.gameObject.SetActive(true);
                MakeupScroller.Setup(TYPE.MAKEUP, BlockItemPrototype);
            }


            temobj = GameObject.Find("PresetScroller");
            if (temobj != null)
            {
                PresetScroller = temobj.AddComponent<ScrollerController>();
                PresetScroller.name = "PresetScroller";
                PresetScroller.gameObject.SetActive(true);
                PresetScroller.Setup(TYPE.PRESET, BlockItemPrototype);
            }
        }

        private void SetupFaceUI()
        {
            List<ShapingSkeletonTransConfig> sliderconfigs = core.GetFaceSliderConfig();

            for (int i = 0; i < sliderconfigs.Count; i++)
            {
                ShapingSkeletonTransConfig config = sliderconfigs[i];
                ShapingUIEditorSlider configitem = new ShapingUIEditorSlider();

                configitem.type = TYPE.FACE;
                configitem.index = config.index;
                configitem.firstlevelDesc = config.FirstLevelDesc;
                configitem.firstlevel = config.FirstLevel;
                configitem.thirdlevel = config.ThirdLevel;
                configitem.thirdlevelDesc = config.ThirdLevelDesc;

                FaceScroller.AddSliderItem(configitem);
            }

            FaceScroller.UpdateBlocksSize();

        }

        private void SetupBodyUI()
        {
            List<ShapingSkeletonTransConfig> sliderconfigs = core.GetBodySliderConfig();

            for (int i = 0; i < sliderconfigs.Count; i++)
            {
                ShapingSkeletonTransConfig config = sliderconfigs[i];
                ShapingUIEditorSlider configitem = new ShapingUIEditorSlider();

                configitem.type = TYPE.FACE;
                configitem.index = config.index;
                configitem.firstlevelDesc = config.FirstLevelDesc;
                configitem.firstlevel = config.FirstLevel;
                configitem.thirdlevel = config.ThirdLevel;
                configitem.thirdlevelDesc = config.ThirdLevelDesc;

                BodyScroller.AddSliderItem(configitem);
            }

            BodyScroller.UpdateBlocksSize();

        }
        private void SetupHairUI()
        {
            
        }

        private void SetupClothUI()
        {

        }

        private void SetupMakeupUI()
        {
            List<ShapingImageConfig> ImageConig = core.GetMakeupImageConfig();

            int SliderID = 0;
            int ColorGroupID = 0;
            int ImageGroupID = 0;

            for (int i = 0; i < ImageConig.Count; i++)
            {
                ShapingImageConfig config = ImageConig[i];
                if(config.Slider1Desc != null && config.Slider1Desc != "" && config.Slider1Desc != "None")
                {
                    ShapingUIEditorSlider configitem = new ShapingUIEditorSlider();
                    configitem.firstlevel = config.FirstLevelIndex;
                    configitem.thirdlevel = 0;
                    configitem.firstlevelDesc = config.FirstLevelDesc;
                    configitem.thirdlevelDesc = config.Slider1Desc;
                    configitem.index = SliderID;
                    MakeupScroller.AddGroupSliderItem(configitem);
                    SliderID++;
                }

                if (config.Slider2Desc != null && config.Slider2Desc != "" && config.Slider2Desc != "None")
                {
                    ShapingUIEditorSlider configitem = new ShapingUIEditorSlider();
                    configitem.firstlevel = config.FirstLevelIndex;
                    configitem.thirdlevel = 1;
                    configitem.firstlevelDesc = config.FirstLevelDesc;
                    configitem.thirdlevelDesc = config.Slider1Desc;
                    configitem.index = SliderID;
                    MakeupScroller.AddGroupSliderItem(configitem);
                    SliderID++;
                }

                if (config.Slider3Desc != null && config.Slider3Desc != "" && config.Slider3Desc != "None")
                {
                    ShapingUIEditorSlider configitem = new ShapingUIEditorSlider();
                    configitem.firstlevel = config.FirstLevelIndex;
                    configitem.thirdlevel = 2;
                    configitem.firstlevelDesc = config.FirstLevelDesc;
                    configitem.thirdlevelDesc = config.Slider1Desc;
                    configitem.index = SliderID;
                    MakeupScroller.AddGroupSliderItem(configitem);
                    SliderID++;
                }

                if(config.ColorDesc != null && config.ColorDesc != "" && config.ColorDesc != "None")
                {
                    int colortableindex = config.ColorIndex;
                    List<ShapingColorTableItem> group = core.GetColorGroup(colortableindex);

                    MakeupScroller.AddColorGroup(config.FirstLevelIndex, ColorGroupID,  config.FirstLevelDesc, config.ColorDesc, group);
                    ColorGroupID++;
                }

                
                if (config.TextureDesc != null && config.TextureDesc != "" && config.TextureDesc != "None")
                {
                    int colortableindex = config.TextureIndex;
                    List<ShapingTextureTableItem> group = core.GetTextureGroup(colortableindex);

                    // MakeupScroller.AddColorGroup(config.FirstLevelIndex, ColorGroupID, config.FirstLevelDesc, config.ColorDesc, group);
                    MakeupScroller.AddTextureGroup(config.FirstLevelIndex, ImageGroupID, config.FirstLevelDesc, config.TextureDesc, group);

                    ImageGroupID++;
                }
                
            }

            MakeupScroller.UpdateBlocksSize();
        }
        
        private void SetupPresetUI()
        {
            List<ShapingPresetConfigItem> configs = core.GetPresetConfig();
            PresetScroller.AddPresetTextureGroup(UISize.PresetName, configs);
        }


        public void OnClickFaceButton()
        {
            FacePanel.  SetActive(true);
            BodyPanel.  SetActive(false);
            HairPanel.  SetActive(false);
            ClothPanel. SetActive(false);
            MakeupPanel.SetActive(false);
            PresetPanel.SetActive(false);
        }

        public void OnClickBodyButton()
        {
            FacePanel.  SetActive(false);
            BodyPanel.  SetActive(true);
            HairPanel.  SetActive(false);
            ClothPanel. SetActive(false);
            MakeupPanel.SetActive(false);
            PresetPanel.SetActive(false);
        }
        public void OnClickHairButton()
        {
            FacePanel.  SetActive(false);
            BodyPanel.  SetActive(false);
            HairPanel.  SetActive(true);
            ClothPanel. SetActive(false);
            MakeupPanel.SetActive(false);
            PresetPanel.SetActive(false);
        }
        public void OnClickClothButton()
        {
            FacePanel.  SetActive(false);
            BodyPanel.  SetActive(false);
            HairPanel.  SetActive(false);
            ClothPanel. SetActive(true);
            MakeupPanel.SetActive(false);
            PresetPanel.SetActive(false);
        }
        public void OnClickMakeupButton()
        {
            FacePanel.  SetActive(false);
            BodyPanel.  SetActive(false);
            HairPanel.  SetActive(false);
            ClothPanel. SetActive(false);
            MakeupPanel.SetActive(true);
            PresetPanel.SetActive(false);
        }

        public void OnClickPresetButton()
        {
            FacePanel.  SetActive(false);
            BodyPanel.  SetActive(false);
            HairPanel.  SetActive(false);
            ClothPanel. SetActive(false);
            MakeupPanel.SetActive(false);
            PresetPanel.SetActive(true);
        }


        public void SetController(ShapingControllerCore c)
        {
            core = c;
        }

        public void OnClickImport()
        {
            ImportData();
        }

        public void OnClickExport()
        {
            ExportData();
        }

        public void ImportData()
        {
            
            UIEventManager.OnImportEvent.Invoke("Assets\\AvartarShape\\Shaping\\Config\\aa.dat");
        }

        public void ExportData()
        {
            UIEventManager.OnExportEvent.Invoke("Assets\\AvartarShape\\Shaping\\Config\\aa.dat");
        }

        //Event trigged when importing data
        public UnityEngine.Events.UnityAction<TYPE, int, float> GetMakeSliderValueChangeEventHandle()
        {
            return MakeSldierValueChange;
        }

        public void MakeSldierValueChange(TYPE type, int sliderindex, float value)
        {
            if(type == TYPE.FACE)
            {
                FaceScroller.SetSliderValue(sliderindex, value);
            }
            else if(type == TYPE.BODY)
            {
                BodyScroller.SetSliderValue(sliderindex, value);
            }
            else if(type == TYPE.MAKEUP)
            {
                MakeupScroller.SetSliderValue(sliderindex, value);
            }
        }
        public UnityEngine.Events.UnityAction<TYPE, int, int> GetMakeColorValueChangeEventHandle()
        {
            return MakeColorValueChange;
        }

        public void MakeColorValueChange(TYPE type, int groupindex, int value)
        {
            if(type == TYPE.MAKEUP)
            {
                MakeupScroller.SetColorGroupValue(groupindex, value);
            }
        }

        public UnityEngine.Events.UnityAction<TYPE, int, int> GetMakeImageValueChangeEventHandle()
        {
            return MakeImageValueChange;
        }

        public void MakeImageValueChange(TYPE type, int groupindex, int value)
        {
            if (type == TYPE.MAKEUP)
            {
                MakeupScroller.SetTextureGroupValue(groupindex, value);
            }
        }


        //--------------Public Variable

        //--------------Private Variable
        private ShapingControllerCore core;


        //--------------UI Elements
        private Button FaceB;
        private Button BodyB;
        private Button HairB;
        private Button ClothB;
        private Button MakeupB;
        private Button PresetB;

        private Button Import;
        private Button Export;

        private GameObject FacePanel;
        private GameObject BodyPanel;
        private GameObject HairPanel;
        private GameObject MakeupPanel;
        private GameObject ClothPanel;
        private GameObject PresetPanel;

        private ScrollerController FaceScroller;
        private ScrollerController BodyScroller;
        private ScrollerController HairScroller;
        private ScrollerController ClothScroller;
        private ScrollerController MakeupScroller;
        private ScrollerController PresetScroller;

        public RectTransform BlockItemPrototype;

    }
}