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
        }

        // Update is called once per frame
        void Update()
        {

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

        public UnityEngine.Events.UnityAction<TYPE, int, float> GetSliderEventHandle()
        {
            return SkeMan.OnSliderValueChangeFromUI;
        }


        private ShapingControllerCore controller;

        private PlayerMaterialAnim MatMan;
        private PlayerSkeletonAnim SkeMan;

        public GameObject Face;
        public GameObject Body;

        public GameObject SkeletonRoot;

    }

}