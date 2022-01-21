using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShapingPlayer
{
    public class UVCameraController : MonoBehaviour
    {

        private GameObject camera;
        private Camera cameracomponent;

        private void Start()
        {
            camera = GameObject.Find("Main Camera");
            cameracomponent = camera.GetComponent<Camera>();
            //cameracomponent.usesha
        }

        void Update()
        {

        }

    }
}