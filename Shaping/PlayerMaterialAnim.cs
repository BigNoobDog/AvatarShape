using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;

namespace ShapingPlayer
{
    public class PlayerMaterialAnim : MonoBehaviour
    {
        private Material FaceMaterial;
        private Material EyeMaterial;


        // Start is called before the first frame update
        void Start()
        {



        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(ShapingControllerCore core)
        {
            controller = core;



        }

        public void ImportData()
        {
            controller.ImportData();
        }

        public void ExportData()
        {
            controller.ExportData();
        }

        public void SetShapingController(ShapingControllerCore core)
        {
            controller = core;
        }


        //Public Function
        public void SetFaceMaterial(Material m)
        {
            FaceMaterial = m;
        }

        public void SetEyeMaterial(Material m)
        {
            EyeMaterial = m;
        }

        public void OnColorValueChangedFromUI(TYPE type, int index, Color color)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + color.ToString();
            Debug.Log(logstr);

            ShapingMaterialColorItem configitem = controller.GetMaterialColorConfigItem(type, index);

            if(configitem.part == PART.HEAD)
            {
                FaceMaterial.SetColor(configitem.name, color);
            }
            else if(configitem.part == PART.EYE)
            {
                EyeMaterial.SetColor(configitem.name, color);
            }
        }


        public void OnSliderValueChangeFromUI(TYPE type, int index, float value)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + value.ToString();
            Debug.Log(logstr);

            ShapingMaterialScalaItem configitem = controller.GetMaterialScalaConfigItem(type, index);

            float lastvalue = (value - 0.5f) * configitem.limit * 2.0f + 0.5f;
            if (configitem.part == PART.HEAD)
            {
                FaceMaterial.SetFloat(configitem.name, lastvalue);
            }
            else if (configitem.part == PART.EYE)
            {
                EyeMaterial.SetFloat(configitem.name, lastvalue);
            }
        }

        private ShapingControllerCore controller;
    }
}