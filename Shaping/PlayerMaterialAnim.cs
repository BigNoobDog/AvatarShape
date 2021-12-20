using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;

namespace ShapingPlayer
{
    public class PlayerMaterialAnim : MonoBehaviour
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


        private ShapingControllerCore controller;
    }
}