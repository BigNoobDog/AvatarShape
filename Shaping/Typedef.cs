using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;

namespace ShapingPlayer
{
    public enum CameraPos
    {
        Start,
        FAR,
        MID,
        NEAR,
        CameraPosNum,
    }

    public class TransformLikeUnity
    {
        public TransformLikeUnity()
        {
            localPosition = new Vector3(0, 0, 0);
            rotation = Quaternion.Euler(0, 0, 0);
            localScale = new Vector3(0, 0, 0);
        }

        public Vector3 localPosition;
        public Quaternion rotation;
        public Vector3 localScale;
    }

    //public class OnSliderValueChangeFromUI : UnityEngine.Events.UnityEvent<TYPE, int, float> { }
}