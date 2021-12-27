using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;

namespace ShapingPlayer
{

    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SkeMan = gameObject.AddComponent<PlayerSkeletonAnim>();
            MatMan = gameObject.AddComponent<PlayerMaterialAnim>();
        }

        public void Setup()
        {
            MatMan.Setup(controller);
            SkeMan.Setup(controller, SkeletonRoot);

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
            controller.ImportData(filepath);
            controller.ApplyData();
            ApplyData(controller.GetUsableData());
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
            return MatMan.OnImageValueChangedFromUI;
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

        public GameObject Face;
        public GameObject Body;

        public GameObject SkeletonRoot;

    }

}