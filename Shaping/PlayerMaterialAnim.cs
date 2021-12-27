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

        public void OnColorValueChangedFromUI(TYPE type, int index, int value, Color color)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + color.ToString();
            Debug.Log(logstr);

            ShapingMaterialColorItem configitem = controller.GetMaterialColorConfigItem(type, index);
            controller.SetMaterialVectorParam(type, index, value);

            if (configitem.part == PART.HEAD)
            {
                FaceMaterial.SetColor(configitem.name, color);
            }
            else if(configitem.part == PART.EYE)
            {
                EyeMaterial.SetColor(configitem.name, color);
            }
        }


        public void OnImageValueChangedFromUI(TYPE type, int index, int value, string path)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + path;
            Debug.Log(logstr);

            ShapingMaterialTextureItem configitem = controller.GetMaterialImageConfigItem(type, index);
            controller.SetMaterialImageParam(type, index, value);

            Texture t = Resources.Load<Texture>(path);
            if (t == null)
                return;

            if (configitem.part == PART.HEAD)
            {
                FaceMaterial.SetTexture(configitem.name, t);
            }
            else if (configitem.part == PART.EYE)
            {
                EyeMaterial.SetTexture(configitem.name, t);
            }
        }

        public void OnSliderValueChangeFromUI(TYPE type, int index, float value)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + value.ToString();
            Debug.Log(logstr);

            ShapingMaterialScalaItem configitem = controller.GetMaterialScalaConfigItem(type, index);
            controller.SetOneBoneSliderValue(type, index, value);

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

        public void SetScalaParams(Dictionary<PART, List<ShapingMaterialScalaParam>> dict)
        {
            foreach(PART part in dict.Keys)
            {
                List<ShapingMaterialScalaParam> l = dict[part];
                foreach(ShapingMaterialScalaParam param in l)
                {
                    if (part == PART.HEAD)
                    {
                        FaceMaterial.SetFloat(param.ParamName, param.Value);
                    }
                    else if (part == PART.EYE)
                    {
                        EyeMaterial.SetFloat(param.ParamName, param.Value);
                    }
                }

            }
        }

        public void SetTextureParams(Dictionary<PART, List<ShapingMaterialTextureParam>> dict)
        {
            foreach (PART part in dict.Keys)
            {
                List<ShapingMaterialTextureParam> l = dict[part];
                foreach (ShapingMaterialTextureParam param in l)
                {
                    Texture t = Resources.Load<Texture>(param.Value);
                    if (t == null)
                        return;
                    if (part == PART.HEAD)
                    {
                        FaceMaterial.SetTexture(param.ParamName, t);
                    }
                    else if (part == PART.EYE)
                    {
                        EyeMaterial.SetTexture(param.ParamName, t);
                    }
                }

            }
        }

        public void SetVectorParams(Dictionary<PART, List<ShapingMaterialVectorParam>> dict)
        {
            foreach (PART part in dict.Keys)
            {
                List<ShapingMaterialVectorParam> l = dict[part];
                foreach (ShapingMaterialVectorParam param in l)
                {
                    Color color = new Color(param.r, param.g, param.b);

                    if (part == PART.HEAD)
                    {
                        FaceMaterial.SetColor(param.ParamName, color);
                    }
                    else if (part == PART.EYE)
                    {
                        EyeMaterial.SetColor(param.ParamName, color);
                    }
                }

            }
        }

        private ShapingControllerCore controller;
    }
}