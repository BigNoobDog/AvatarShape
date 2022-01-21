#define USE_METHOD_LATEUPDATE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;
using ShapingUI;


namespace ShapingPlayer
{

    public static class XXX
    {
        public static List<string> xx;
        public static void Setup()
        {
            xx = new List<string>();
            xx.Add("cf_J_MayuTip_top_s_L");
			xx.Add("cf_J_MayuTip_top_s_R");
			xx.Add("cf_J_Forehead_tz");
			xx.Add("cf_J_NoseBase_rx");
			xx.Add("cf_J_MayuTip_s_L");
			xx.Add("cf_J_MayuTip_s_R");
			xx.Add("cf_J_FaceUp_tz");
			xx.Add("cf_J_CheekLow_s_R");
			xx.Add("cf_J_CheekLow_s_L");
			xx.Add("cf_J_Chin_Base");
			xx.Add("cf_J_ChinTip_Base");
			xx.Add("cf_J_Mayumoto_L");
			xx.Add("cf_J_Mayumoto_R");
			xx.Add("cf_J_Mayu_L");
			xx.Add("cf_J_Mayu_R");
			xx.Add("cf_J_Eye_tz");
			xx.Add("cf_J_Eye04_s_L");
			xx.Add("cf_J_Eye04_s_R");
			xx.Add("cf_J_Eye07_s_L");
			xx.Add("cf_J_Eye07_s_R");
			xx.Add("cf_J_Eye06_s_R");
			xx.Add("cf_J_Eye06_s_L");
			xx.Add("cf_J_Eye05_s_L");
			xx.Add("cf_J_Eye05_s_R");
			xx.Add("cf_J_MouthBase_ty");
			xx.Add("cf_J_Mouth_L");
			xx.Add("cf_J_Mouth_R");
			xx.Add("cf_J_Mouthup");
			xx.Add("cf_J_MouthMove");
			xx.Add("cf_J_MouthLow");
			xx.Add("cf_J_Eye_rz_L");
			xx.Add("cf_J_Eye_rz_R");
			xx.Add("cf_J_FaceUp_ty");
			xx.Add("cf_J_FaceLow_tz");
			xx.Add("cf_J_Mayu_ty");
			xx.Add("cf_J_ChinLow");
			xx.Add("cf_J_NoseBase");
			xx.Add("cf_J_NoseBridge_rx");
			xx.Add("cf_J_Eye_tx_L");
			xx.Add("cf_J_Eye_tx_R");
			xx.Add("cf_J_Eye02_s_L");
			xx.Add("cf_J_Eye02_s_R");
			xx.Add("cf_J_Eye03_s_L");
			xx.Add("cf_J_Eye03_s_R");
			xx.Add("cf_J_Eye08_s_L");
			xx.Add("cf_J_Eye08_s_R");
            //xx.Add();
        }
    }

    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SkeMan = gameObject.AddComponent<PlayerSkeletonAnim>();
            MatMan = gameObject.AddComponent<PlayerMaterialAnim>();
            ImportPhotoMan = gameObject.AddComponent<PlayerImportPhoto>();
            presetMan = gameObject.AddComponent<PlayerPresetController>();
        }

        public void Setup(GameController gc)
        {
            MatMan.Setup(controller);
            SkeMan.Setup(controller, SkeletonRoot);
            ImportPhotoMan.Setup(controller, this);
            presetMan.Setup(controller, this);

            gamecontroller = gc;

            InitMatManMaterial();
        }


        private void InitMatManMaterial()
        {
            SkinnedMeshRenderer FaceSMR = Face.GetComponent<SkinnedMeshRenderer>();
            List<Material> facematerials = new List<Material>();
            FaceSMR.GetMaterials(facematerials);
            if (facematerials.Count > 0)
            {
                MatMan.SetFaceMaterial(facematerials[0]);
            }

            //if (facematerials.Count > 2)
            //{
            //    MatMan.SetEyeMaterial(facematerials[2]);
            //}

            SkinnedMeshRenderer LeftEyeSMR = LeftEye.GetComponent<SkinnedMeshRenderer>();
            List<Material> lefteyesmaterials = new List<Material>();
            LeftEyeSMR.GetMaterials(lefteyesmaterials);
            if (lefteyesmaterials.Count > 0)
            {
                MatMan.SetLeftEyeMaterial(lefteyesmaterials[0]);
            }

            SkinnedMeshRenderer RightEyeSMR = RightEye.GetComponent<SkinnedMeshRenderer>();
            List<Material> Righteyesmaterials = new List<Material>();
            RightEyeSMR.GetMaterials(Righteyesmaterials);
            if (Righteyesmaterials.Count > 0)
            {
                MatMan.SetRightEyeMaterial(Righteyesmaterials[0]);
            }


        }

        // Update is called once per frame
        void Update()
        {

        }


        public void ImportData(string filepath)
        {
#if (USE_METHOD_LATEUPDATE)
#else
            ApplyData(controller.GetBlankUsableData());
#endif
            controller.ImportData(filepath);
            controller.ApplyData();
            ApplyData(controller.GetUsableData());

            ApplyDataToUI();
        }

        public void ImportPhotoData()
        {
            
            ApplyData(controller.GetUsableData());
            ApplyDataToUI();
        }

        public void ApplyDataToUI()
        {
            List<float> facedata = new List<float>();
            facedata = controller.GetFaceData();
            for (int index = 0; index < facedata.Count; index++)
            {
                UIEventManager.OnImportDataMakeSliderValueChange.Invoke(TYPE.FACE, index, facedata[index]);
            }

            List<float> bodydata = new List<float>();
            bodydata = controller.GetFaceData();
            for (int index = 0; index < bodydata.Count; index++)
            {
                UIEventManager.OnImportDataMakeSliderValueChange.Invoke(TYPE.BODY, index, bodydata[index]);
            }

            List<float> makeupdata = new List<float>();
            makeupdata = controller.GetMakeupSliderData();
            for (int index = 0; index < makeupdata.Count; index++)
            {
                UIEventManager.OnImportDataMakeSliderValueChange.Invoke(TYPE.MAKEUP, index, makeupdata[index]);
            }

            List<int> makeupcolordata = new List<int>();
            makeupcolordata = controller.GetMakeupColorData();
            for (int index = 0; index < makeupcolordata.Count; index++)
            {
                UIEventManager.OnImportDataMakeColorValueChange.Invoke(TYPE.MAKEUP, index, makeupcolordata[index]);
            }

            List<int> makeuptexturedata = new List<int>();
            makeuptexturedata = controller.GetMakeupTextureData();
            for (int index = 0; index < makeuptexturedata.Count; index++)
            {
                UIEventManager.OnImportDataMakeImageValueChange.Invoke(TYPE.MAKEUP, index, makeuptexturedata[index]);
            }
        }

        public void ApplyData(ShapingUsableData data)
        {
            SkeMan.SetScalaParams(data.FaceBones);
            //SkeMan.SetScalaParams(data.BodyBones);

            MatMan.SetScalaParams(data.ScalaParams);
            MatMan.SetVectorParams(data.VectorParams);
            MatMan.SetTextureParams(data.TextureParams);
        }

        public void ExportData(string filepath)
        {
            controller.ExportData(filepath);
        }

        public void RandomData(TYPE type)
        {
#if (USE_METHOD_LATEUPDATE)
            ApplyData(controller.GetBlankUsableData());
#else
            ApplyData(controller.GetBlankUsableData());
#endif

            controller.RandomData(type);
            controller.ApplyData();

            ApplyData(controller.GetUsableData());

            ApplyDataToUI();
        }

        public void ImportJPG(string filename)
        {
            ImportPhotoMan.ImportJPG(filename);
        }


        public void OnImportJPG(string filename)
        {

        }
        public void RevertData(TYPE type)
        {
            ApplyData(controller.GetBlankUsableData());
            controller.RevertData(type);
            controller.ApplyData();
            ApplyData(controller.GetUsableData());

            ApplyDataToUI();
        }

        public void SetShapingController(ShapingControllerCore core)
        {
            controller = core;
        }

        public UnityEngine.Events.UnityAction<TYPE, int, float> GetSliderEventHandle()
        {
            return OnSliderValueChangeFromUI;
        }

        public UnityEngine.Events.UnityAction<TYPE, int, int, Color> GetColorEventHandle()
        {
            return MatMan.OnColorValueChangedFromUI;
        }


        public UnityEngine.Events.UnityAction<TYPE, int, int, string> GetImageEventHandle()
        {
            return GetImageChangeEvent;
        }

        public void GetImageChangeEvent(TYPE type, int index, int value, string path)
        {
            if (type == TYPE.PRESET)
                presetMan.OnImageValueChangedFromUI(type, index, value, FileNames.DefaultPath + path);
            else
                MatMan.OnImageValueChangedFromUI(type, index, value, path);
        }

        public void OnSliderValueChangeFromUI(TYPE type, int index, float value)
        {
            if (type == TYPE.FACE || type == TYPE.BODY)
            {
                SkeMan.OnSliderValueChangeFromUI(type, index, value);
            }

            if (type == TYPE.MAKEUP)
            {
                MatMan.OnSliderValueChangeFromUI(type, index, value);
            }

        }

        void OnMouseDown()
        {
            gamecontroller.ClickPlayer();
        }

        private ShapingControllerCore controller;

        private PlayerMaterialAnim MatMan;
        private PlayerSkeletonAnim SkeMan;
        private PlayerPresetController presetMan;
        private PlayerImportPhoto ImportPhotoMan;

        public GameObject Face;
        public GameObject Body;
        public GameObject LeftEye;
        public GameObject RightEye;

        public GameObject SkeletonRoot;

        public Transform RotatePoint;

        private GameController gamecontroller;


    }

}