using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/CharRenderingControl")]
    public class NiloToonCharRenderingControlVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("Base Color Tint (without affecting rimlight & specular)")]
        [Tooltip("maybe useful to lower the value when bloom on, to keep things not over bright")]
        public MinFloatParameter charBaseColorMultiply = new MinFloatParameter(1, 0);
        public ColorParameter charBaseColorTintColor = new ColorParameter(Color.white, true, false, true);

        [Header("Overall Multiply Color (affecting rimlight & specular)")]
        [Tooltip("maybe useful to lower the value when bloom on, to keep things not over bright")]
        public MinFloatParameter charMulColorIntensity = new MinFloatParameter(1, 0);
        public ColorParameter charMulColor = new ColorParameter(Color.white, true, false, true);

        [Header("Lerp Color")]
        public ClampedFloatParameter charLerpColorUsage = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter charLerpColor = new ColorParameter(new Color(1, 1, 1, 0), true, false, true);

        [Header("Occlusion")]
        public ClampedFloatParameter charOcclusionUsage = new ClampedFloatParameter(1, 0, 1);

        [Header("Indirect Light")]
        public MinFloatParameter charIndirectLightMultiplier = new MinFloatParameter(1, 0);
        [Tooltip("A minimum light color when no active light in scene, to prevent rendering completely black, most of the time this value is useless")]
        public ColorParameter charIndirectLightMinColor = new ColorParameter(new Color(0.01f, 0.01f, 0.01f, 0), false, false, true);

        [Header("Direction Light")]
        public MinFloatParameter mainDirectionalLightIntensityMultiplier = new MinFloatParameter(1f, 0);
        public ColorParameter mainDirectionalLightIntensityMultiplierColor = new ColorParameter(Color.white, true, false, true);
        [Tooltip("Set it lower to avoid character over exposure")]
        public MinFloatParameter mainDirectionalLightMaxContribution = new MinFloatParameter(1f, 0);
        public ColorParameter mainDirectionalLightMaxContributionColor = new ColorParameter(Color.white, true, false, true);
        [Header("Additional Light")]
        public MinFloatParameter additionalLightIntensityMultiplier = new MinFloatParameter(1f, 0);
        public ColorParameter additionalLightIntensityMultiplierColor = new ColorParameter(Color.white, true, false, true);
        [Tooltip("Set it lower to avoid character over exposure")]
        public MinFloatParameter additionalLightMaxContribution = new MinFloatParameter(1f, 0);
        public ColorParameter additionalLightMaxContributionColor = new ColorParameter(Color.white, true, false, true);

        [Header("Specular")]
        public MinFloatParameter specularIntensityMultiplier = new MinFloatParameter(1f, 0);
        public MinFloatParameter specularInShadowMinIntensity = new MinFloatParameter(0.25f, 0);
        public BoolParameter specularReactToLightDirectionChange = new BoolParameter(false); // default is false by design

        [Header("Depth texture Rim Light and Shadow")]
        public MinFloatParameter depthTextureRimLightAndShadowWidthMultiplier = new MinFloatParameter(1f, 0);

        [Header("Rim Light")]
        public MinFloatParameter charRimLightMultiplier = new MinFloatParameter(1, 0);
        public ColorParameter charRimLightTintColor = new ColorParameter(new Color(1, 1, 1, 0), true, false, true);
        public MinFloatParameter charRimLightCameraDistanceFadeoutStartDistance = new MinFloatParameter(50, 0);
        public MinFloatParameter charRimLightCameraDistanceFadeoutEndDistance = new MinFloatParameter(100, 0);

        [Header("Outline")]
        [Range(0, 10)]
        public MinFloatParameter charOutlineWidthMultiplier = new MinFloatParameter(1, 0);
        public MinFloatParameter charOutlineWidthExtraMultiplierForXR = new MinFloatParameter(0.5f, 0); // VR default smaller outline, due to high FOV

        public ColorParameter charOutlineMulColor = new ColorParameter(Color.white, true, false, true);


        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

