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

            //onUpdateSliderValue.AddListener(OnSliderValueChangeFromUI);
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

                    bones[key].localPosition += (locoffset - bonespretran[key].localPosition);
                    bones[key].rotation = bones[key].rotation * quaoffset * Quaternion.Inverse(bonespretran[key].rotation);
                    bones[key].localScale += (scaoffset - bonespretran[key].localScale);

                    bonespretran[key].localPosition = locoffset;
                    bonespretran[key].rotation = quaoffset;
                    bonespretran[key].localScale = scaoffset;
                }
            }
        }



        private ShapingControllerCore controller;
        public GameObject SkeletonRoot;

        private Dictionary<string, Transform> bones;
        private Dictionary<string, TransformLikeUnity> bonespretran;

        //public OnSliderValueChangeFromUI onUpdateSliderValue = new OnSliderValueChangeFromUI();

    }
}