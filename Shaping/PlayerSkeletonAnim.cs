#define USE_METHOD_LATEUPDATE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;


namespace ShapingPlayer
{

    public class PlayerSkeletonAnim : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (SkeletonRoot != null)
            {
                bones = new Dictionary<string, Transform>();
                bonespretran = new Dictionary<string, TransformLikeUnity>();
                Transform[] trans = SkeletonRoot.GetComponentsInChildren<Transform>();
                foreach (Transform tran in trans)
                {
                    bones[tran.name] = tran;
                    bonespretran[tran.name] = new TransformLikeUnity();
                }

            }
            InputTrans = new List<ShapingSkeletonTrans>();
            //onUpdateSliderValue.AddListener(OnSliderValueChangeFromUI);
            XXX.Setup();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(ShapingControllerCore core, GameObject root)
        {
            controller = core;
            SkeletonRoot = root;
        }

        public void OnSliderValueChangeFromUI(TYPE type, int index, float value)
        {
            string logstr = GlobalFunAndVar.TYPE2Str(type) + " " + index.ToString() + " " + value.ToString();
            Debug.Log(logstr);

            if (type == TYPE.FACE)
            {
#if (USE_METHOD_LATEUPDATE)
                List<ShapingSkeletonTrans> trans = controller.SetOneBoneSliderValue(type, index, value);

                foreach (ShapingSkeletonTrans tran in trans)
                {
                    string key = tran.bonename;
                    //int mask = tran.Mask;
                    //if(mask)
                    Vector3 locoffset = new Vector3(tran.Location.x, tran.Location.y, tran.Location.z);
                    Vector3 rotoffset = new Vector3(tran.Rotation.x, tran.Rotation.y, tran.Rotation.z);
                    Vector3 scaoffset = new Vector3(tran.Scale.x, tran.Scale.y, tran.Scale.z);
                    Quaternion quaoffset = Quaternion.Euler(rotoffset);

                    foreach(string item in XXX.xx)
                    {
                        if(item == key)
                        {
                            bones[key].localPosition += locoffset;
                            bones[key].rotation *= quaoffset;
                        }
                    }
                    //bones[key].localPosition += locoffset;
                    //bones[key].rotation *= quaoffset;
                    bones[key].localScale += scaoffset;

                }

                InputTrans = controller.SetOneBoneSliderValue_Pure(type, index, value);

                //foreach (ShapingSkeletonTrans tran in InputTrans)
                //{
                //    tran.Scale.x = 0.0f;
                //    tran.Scale.y = 0.0f;
                //    tran.Scale.z = 0.0f;
                //}




#else
                List<ShapingSkeletonTrans> trans = controller.SetOneBoneSliderValue(type, index, value);

                foreach (ShapingSkeletonTrans tran in trans)
                {
                    string key = tran.bonename;
                    //int mask = tran.Mask;
                    //if(mask)
                    Vector3 locoffset = new Vector3(tran.Location.x, tran.Location.y, tran.Location.z);
                    Vector3 rotoffset = new Vector3(tran.Rotation.x, tran.Rotation.y, tran.Rotation.z);
                    Vector3 scaoffset = new Vector3(tran.Scale.x, tran.Scale.y, tran.Scale.z);
                    Quaternion quaoffset = Quaternion.Euler(rotoffset);


                    bones[key].localPosition += locoffset;
                    bones[key].rotation *= quaoffset;
                    bones[key].localScale += scaoffset;

                 }
#endif
            }
            else if (type == TYPE.BODY)
            {
                List<ShapingSkeletonTrans> trans = controller.SetOneBoneSliderValue(type, index, value);

                foreach (ShapingSkeletonTrans tran in trans)
                {
                    string key = tran.bonename;
                    //int mask = tran.Mask;
                    //if(mask)
                    Vector3 locoffset = new Vector3(tran.Location.x, tran.Location.y, tran.Location.z);
                    Vector3 rotoffset = new Vector3(tran.Rotation.x, tran.Rotation.y, tran.Rotation.z);
                    Vector3 scaoffset = new Vector3(tran.Scale.x, tran.Scale.y, tran.Scale.z);
                    Quaternion quaoffset = Quaternion.Euler(rotoffset);


                    bones[key].localPosition += locoffset;
                    bones[key].rotation *= quaoffset;
                    bones[key].localScale += scaoffset;
                }
            }
        }

        public void SetScalaParams(List<ShapingSkeletonTrans> trans)
        {
#if (USE_METHOD_LATEUPDATE)
            //List<ShapingSkeletonTrans> trans = controller.SetOneBoneSliderValue(type, index, value);

            foreach (ShapingSkeletonTrans tran in trans)
            {
                string key = tran.bonename;
                //int mask = tran.Mask;
                //if(mask)
                Vector3 locoffset = new Vector3(tran.Location.x, tran.Location.y, tran.Location.z);
                Vector3 rotoffset = new Vector3(tran.Rotation.x, tran.Rotation.y, tran.Rotation.z);
                Vector3 scaoffset = new Vector3(tran.Scale.x, tran.Scale.y, tran.Scale.z);
                Quaternion quaoffset = Quaternion.Euler(rotoffset);

                foreach (string item in XXX.xx)
                {
                    if (item == key)
                    {
                        bones[key].localPosition += locoffset;
                        bones[key].rotation *= quaoffset;
                    }
                }
                //bones[key].localPosition += locoffset;
                //bones[key].rotation *= quaoffset;
                bones[key].localScale += scaoffset;

            }

            InputTrans = trans;
#else
            foreach (ShapingSkeletonTrans tran in trans)
            {
                string key = tran.bonename;
                //int mask = tran.Mask;
                //if(mask)
                Vector3 locoffset = new Vector3(tran.Location.x, tran.Location.y, tran.Location.z);
                Vector3 rotoffset = new Vector3(tran.Rotation.x, tran.Rotation.y, tran.Rotation.z);
                Vector3 scaoffset = new Vector3(tran.Scale.x, tran.Scale.y, tran.Scale.z);
                Quaternion quaoffset = Quaternion.Euler(rotoffset);

                if(bones.ContainsKey(key) == true)
                {
                    bones[key].localPosition += locoffset;
                    bones[key].rotation *= quaoffset;
                    bones[key].localScale += scaoffset;
                }
            }
#endif
        }

        private void LateUpdate()
        {
#if (USE_METHOD_LATEUPDATE)
            foreach (ShapingSkeletonTrans tran in InputTrans)
            {
                string key = tran.bonename;
                //int mask = tran.Mask;
                //if(mask)
                Vector3 locoffset = new Vector3(tran.Location.x, tran.Location.y, tran.Location.z);
                Vector3 rotoffset = new Vector3(tran.Rotation.x, tran.Rotation.y, tran.Rotation.z);
                Vector3 scaoffset = new Vector3(tran.Scale.x, tran.Scale.y, tran.Scale.z);
                Quaternion quaoffset = Quaternion.Euler(rotoffset);

                bool binXXX = false;
                foreach(string item in XXX.xx)
                {
                    if(item == key)
                    {
                        binXXX = true;
                    }
                }

                if(!binXXX)
                {
                    bones[key].localPosition += locoffset;
                    bones[key].rotation *= quaoffset;
                }

                //bones[key].localScale += scaoffset;
                //if(key == "cf_J_MayuTip_s_L")
                //{
                //    tran.Rotation = new Vector3d(0, 0, 0);
                //}

            }
#else
#endif
        }

        private ShapingControllerCore controller;
        private GameObject SkeletonRoot;

        private Dictionary<string, Transform> bones;
        private Dictionary<string, TransformLikeUnity> bonespretran;

        //public OnSliderValueChangeFromUI onUpdateSliderValue = new OnSliderValueChangeFromUI();
        private List<ShapingSkeletonTrans> InputTrans;
    }
}