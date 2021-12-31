using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;
using ShapingUI;


namespace ShapingPlayer
{

    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SkeMan = gameObject.AddComponent<PlayerSkeletonAnim>();
            MatMan = gameObject.AddComponent<PlayerMaterialAnim>();
            presetMan = gameObject.AddComponent<PlayerPresetController>();
        }

        public void Setup()
        {
            MatMan.Setup(controller);
            SkeMan.Setup(controller, SkeletonRoot);
            presetMan.Setup(controller, this);

            InitMatManMaterial();
        }


        private void InitMatManMaterial()
        {
            SkinnedMeshRenderer FaceSMR = Face.GetComponent<SkinnedMeshRenderer>();
            List<Material> facematerials = new List<Material>();
            FaceSMR.GetMaterials(facematerials);
            if(facematerials.Count > 0) 
            {
                MatMan.SetFaceMaterial(facematerials[0]);
            }

            if (facematerials.Count > 2)
            {
                MatMan.SetEyeMaterial(facematerials[2]);
            }


        }

        // Update is called once per frame
        void Update()
        {

        }


        public void ImportData(string filepath)
        {
            ApplyData(controller.GetBlankUsableData());
            controller.ImportData(filepath);
            controller.ApplyData();
            ApplyData(controller.GetUsableData());

            ApplyDataToUI();
        }

        public void ApplyDataToUI()
        {
            List<float> facedata = new List<float>();
            facedata = controller.GetFaceData();
            for(int index = 0; index < facedata.Count; index ++)
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
            SkeMan.SetScalaParams(data.BodyBones);

            MatMan.SetScalaParams(data.ScalaParams);
            MatMan.SetVectorParams(data.VectorParams);
            MatMan.SetTextureParams(data.TextureParams);
        }

        public void ExportData(string filepath)
        {
            controller.ExportData(filepath);
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
            if(type == TYPE.FACE || type == TYPE.BODY)
            {
                SkeMan.OnSliderValueChangeFromUI(type, index, value);
            }

            if(type == TYPE.MAKEUP)
            {
                MatMan.OnSliderValueChangeFromUI(type, index, value);
            }

        }


        private ShapingControllerCore controller;

        private PlayerMaterialAnim MatMan;
        private PlayerSkeletonAnim SkeMan;
        private PlayerPresetController presetMan;

        public GameObject Face;
        public GameObject Body;

        public GameObject SkeletonRoot;

    }

}