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
            OnClickFaceButton();
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
        }
        
        private void SetupUIElements()
        {
            FaceB = GameObject.Find("FaceB").GetComponent<Button>();
            BodyB = GameObject.Find("BodyB").GetComponent<Button>();
            HairB = GameObject.Find("HairB").GetComponent<Button>();
            ClothB = GameObject.Find("ClothB").GetComponent<Button>();
            MakeupB = GameObject.Find("MakeupB").GetComponent<Button>();

            FacePanel = GameObject.Find("FacePanel");
            BodyPanel = GameObject.Find("BodyPanel");
            HairPanel = GameObject.Find("HairPanel");
            ClothPanel = GameObject.Find("ClothPanel");
            MakeupPanel = GameObject.Find("MakeupPanel");

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

            GameObject temobj = GameObject.Find("FaceScroller");
            if(temobj != null)
            {
                FaceScroller = temobj.AddComponent<ScrollerController>();
                FaceScroller.name = "FaceScroller";
                FaceScroller.gameObject.SetActive(true);    
                FaceScroller.Setup(BlockItemPrototype);
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

        }
        private void SetupHairUI()
        {

        }

        private void SetupClothUI()
        {

        }

        private void SetupMakeupUI()
        {

        }

        public void OnClickFaceButton()
        {
            FacePanel.SetActive(true);
            BodyPanel.SetActive(false);
            HairPanel.SetActive(false);
            ClothPanel.SetActive(false);
            MakeupPanel.SetActive(false);
        }

        public void OnClickBodyButton()
        {
            FacePanel.SetActive(false);
            BodyPanel.SetActive(true);
            HairPanel.SetActive(false);
            ClothPanel.SetActive(false);
            MakeupPanel.SetActive(false);
        }
        public void OnClickHairButton()
        {
            FacePanel.SetActive(false);
            BodyPanel.SetActive(false);
            HairPanel.SetActive(true);
            ClothPanel.SetActive(false);
            MakeupPanel.SetActive(false);
        }
        public void OnClickClothButton()
        {
            FacePanel.SetActive(false);
            BodyPanel.SetActive(false);
            HairPanel.SetActive(false);
            ClothPanel.SetActive(true);
            MakeupPanel.SetActive(false);
        }
        public void OnClickMakeupButton()
        {
            FacePanel.SetActive(false);
            BodyPanel.SetActive(false);
            HairPanel.SetActive(false);
            ClothPanel.SetActive(false);
            MakeupPanel.SetActive(true);
        }

        public void SetController(ShapingControllerCore c)
        {
            core = c;
        }


        public void ImportData()
        {
            core.ImportData();
        }

        public void ExportData()
        {
            core.ExportData();
        }

        //--------------Public Variable

        //--------------Private Variable
        private ShapingControllerCore core;


        //--------------UI Elements
        Button FaceB;
        Button BodyB;
        Button HairB;
        Button ClothB;
        Button MakeupB;

        GameObject FacePanel;
        GameObject BodyPanel;
        GameObject HairPanel;
        GameObject MakeupPanel;
        GameObject ClothPanel;

        ScrollerController FaceScroller;
        ScrollerController BodyScroller;
        ScrollerController HairScroller;
        ScrollerController ClothScroller;
        ScrollerController MakeupScroller;

        public RectTransform BlockItemPrototype;

    }
}