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
        private Material LeftEyeMaterial;
        private Material RightEyeMaterial;


        // Start is called before the first frame update
        void Start()
        {



        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(ShapingControllerCore core, PlayerMeshAnim meshman)
        {
            controller = core;
            meshMan = meshman;


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

        public void SetLeftEyeMaterial(Material m)
        {
            LeftEyeMaterial = m;
        }

        public void SetRightEyeMaterial(Material m)
        {
            RightEyeMaterial = m;
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
                LeftEyeMaterial.SetColor(configitem.name, color);
                RightEyeMaterial.SetColor(configitem.name, color);
            }
            else if(configitem.part == PART.HAIR || configitem.part == PART.DOWNCLOTH || 
                configitem.part == PART.UPPERCLOTH || configitem.part == PART.SHOE)
            {
                GetMaterialByPart(configitem.part).SetColor(configitem.name, color);
            }
        }

        public Material GetMaterialByPart(PART part)
        {
            GameObject gotmp;
            if (part == PART.HAIR)
                gotmp = meshMan.MeshDictory[meshMan.hairmesh].gameObject;
            else if(part == PART.DOWNCLOTH)
            {
                gotmp = meshMan.MeshDictory[meshMan.kuzimesh].gameObject;
            }
            else if(part == PART.UPPERCLOTH)
            {
                gotmp = meshMan.MeshDictory[meshMan.shirtmesh].gameObject;
            }
            else if(part == PART.SHOE)
            {
                gotmp = meshMan.MeshDictory[meshMan.shoesmesh].gameObject;
            }
            else
            {
                return null;
            }

            SkinnedMeshRenderer SMR = gotmp.GetComponent<SkinnedMeshRenderer>();
            if (SMR == null)
                return null;

            List<Material> materials = new List<Material>();
            SMR.GetMaterials(materials);
            if (materials.Count > 0)
                return materials[0];

            return null;
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
                LeftEyeMaterial.SetTexture(configitem.name, t);
                RightEyeMaterial.SetTexture(configitem.name, t);
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
                LeftEyeMaterial.SetFloat(configitem.name, lastvalue);
                RightEyeMaterial.SetFloat(configitem.name, lastvalue);
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
                        LeftEyeMaterial.SetFloat(param.ParamName, param.Value);
                        RightEyeMaterial.SetFloat(param.ParamName, param.Value);
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
                        LeftEyeMaterial.SetTexture(param.ParamName, t);
                        RightEyeMaterial.SetTexture(param.ParamName, t);
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
                        LeftEyeMaterial.SetColor(param.ParamName, color);
                        RightEyeMaterial.SetColor(param.ParamName, color);
                    }
                }

            }
        }

        private ShapingControllerCore controller;
        private PlayerMeshAnim meshMan;
    }
}