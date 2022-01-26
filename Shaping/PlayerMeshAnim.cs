using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;

namespace ShapingPlayer
{
    public class PlayerMeshAnim : MonoBehaviour
    {



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

            player = this.gameObject;
            MeshDictory = new Dictionary<string, Transform>();
            Dictionary<string, Transform> tmpT = new Dictionary<string, Transform>();
            Transform[] tmp_l = player.GetComponentsInChildren<Transform>();
            foreach(Transform t in tmp_l)
            {
                if(t.name == "Base_clothes")
                {
                    MeshDictory["Base_clothes"] = t;
                }
                if(t.name == "Base_shoes")
                {
                    MeshDictory["Base_shoes"] = t;
                }
                if(t.name == "Base_hair")
                {
                    MeshDictory["Base_hair"] = t;
                }
                if(t.name == "Base_kuzi")
                {
                    MeshDictory["Base_kuzi"] = t;
                }
                if(t.name == "Xiayu_clothes")
                {
                    MeshDictory["Xiayu_clothes"] = t;
                }
                if(t.name == "Xiayu_hair")
                {
                    MeshDictory["Xiayu_hair"] = t;
                }
                if(t.name == "Xiayu_hair_02")
                {
                    MeshDictory["Xiayu_hair_02"] = t;
                }
                if(t.name == "Xiayu_shoes")
                {
                    MeshDictory["Xiayu_shoes"] = t;
                }
                if(t.name == "Xiayu_kuzi")
                {
                    MeshDictory["Xiayu_kuzi"] = t;
                }
                if(t.name == "Base_hair_02")
                {
                    MeshDictory["Base_hair_02"] = t;
                }
            }


            OnImageValueChangedFromUI(TYPE.CLOTH_HAIR, 0, 0, "Base_hair");
            OnImageValueChangedFromUI(TYPE.CLOTH_SHIRT, 0, 0, "Base_clothes");
            OnImageValueChangedFromUI(TYPE.CLOTH_DRESS, 0, 0, "Base_kuzi");
            OnImageValueChangedFromUI(TYPE.CLOTH_SHOES, 0, 0, "Base_shoes");

        }

        public void ImportData()
        {

            OnImageValueChangedFromUI(TYPE.CLOTH_HAIR, 0, 0, "Xiayu_hair");
            OnImageValueChangedFromUI(TYPE.CLOTH_SHIRT, 0, 0, "Xiayu_clothes");
            OnImageValueChangedFromUI(TYPE.CLOTH_DRESS, 0, 0, "Xiayu_kuzi");
            OnImageValueChangedFromUI(TYPE.CLOTH_SHOES, 0, 0, "Xiayu_shoes");
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



        public void OnImageValueChangedFromUI(TYPE type, int index, int value, string path)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + path;
            Debug.Log(logstr);

            if(type == TYPE.CLOTH_HAIR)
            {
                hairmesh = path;
                if(path == "Base_hair")
                {
                    MeshDictory["Base_hair"].gameObject.SetActive(true);
                    MeshDictory["Xiayu_hair"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_hair_02"].gameObject.SetActive(false);
                    MeshDictory["Base_hair_02"].gameObject.SetActive(false);
                }
                if (path == "Xiayu_hair")
                {
                    MeshDictory["Base_hair"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_hair"].gameObject.SetActive(true);
                    MeshDictory["Xiayu_hair_02"].gameObject.SetActive(false);
                    MeshDictory["Base_hair_02"].gameObject.SetActive(false);
                }
                if (path == "Xiayu_hair_02")
                {
                    MeshDictory["Base_hair"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_hair"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_hair_02"].gameObject.SetActive(true);
                    MeshDictory["Base_hair_02"].gameObject.SetActive(false);
                }
                if(path == "Base_hair_02")
                { 
                    MeshDictory["Base_hair"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_hair"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_hair_02"].gameObject.SetActive(false);
                    MeshDictory["Base_hair_02"].gameObject.SetActive(true);
                }
            }

            if(type == TYPE.CLOTH_DRESS)
            {
                kuzimesh = path;
                if(path == "Base_kuzi")
                {
                    MeshDictory["Base_kuzi"].gameObject.SetActive(true);
                    MeshDictory["Xiayu_kuzi"].gameObject.SetActive(false);
                }
                if(path == "Xiayu_kuzi")
                {
                    MeshDictory["Base_kuzi"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_kuzi"].gameObject.SetActive(true);

                }
            }
            if(type == TYPE.CLOTH_SHIRT)
            {
                shirtmesh = path;
                if(path == "Base_clothes")
                {
                    MeshDictory["Base_clothes"].gameObject.SetActive(true);
                    MeshDictory["Xiayu_clothes"].gameObject.SetActive(false);
                }
                if (path == "Xiayu_clothes")
                {
                    MeshDictory["Base_clothes"].gameObject.SetActive(false);
                    MeshDictory["Xiayu_clothes"].gameObject.SetActive(true);
                }
            }
            if(type == TYPE.CLOTH_SHOES)
            {
                shoesmesh = path;
                if(path == "Base_shoes")
                {
                    if (path == "Base_shoes")
                    {
                        MeshDictory["Base_shoes"].gameObject.SetActive(true);
                        MeshDictory["Xiayu_shoes"].gameObject.SetActive(false);
                    }
                    if(path == "Xiayu_shoes")
                    {
                        
                        MeshDictory["Base_shoes"].gameObject.SetActive(false);
                        MeshDictory["Xiayu_shoes"].gameObject.SetActive(true);
                    }

                }
            }
        }

        public Dictionary<string, Transform> MeshDictory;
        private GameObject player;
        private ShapingControllerCore controller;


        public string hairmesh { get; set; }
        public string kuzimesh { get; set; }
        public string shirtmesh { get; set; }
        public string shoesmesh { get; set; }
    }
}