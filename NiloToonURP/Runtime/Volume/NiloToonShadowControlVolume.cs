using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/ShadowControl")]
    public class NiloToonShadowControlVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("If overrided, can override NiloToonAllInOneRendererFeature's settings per volume.")]
        [Header("If not overrided, will use NiloToonAllInOneRendererFeature's settings.")]
        [Header("----------------------------------------------------------")]

        [Header("Character Average shadow")]
        public BoolParameter enableCharAverageShadow = new BoolParameter(true);
        public ClampedFloatParameter charAverageShadowStrength = new ClampedFloatParameter(1, 0, 1);

        [Header("Character self shadow")]
        // all default values copy from NiloToonCharSelfShadowMapRTPass.Settings
        public BoolParameter enableCharSelfShadow = new BoolParameter(true);

        [Tooltip(   "If false, will use camera's forward(with the shadowAngle & shadowLRAngle) as cast shadow direction.\n" +
                    "If true, will use main light's forward as cast shadow direction.\n" +
                    "Turn it ON if you don't want shadow affected by camera rotation")]
        public BoolParameter useMainLightAsCastShadowDirection = new BoolParameter(false, false);

        public ClampedFloatParameter shadowAngle = new ClampedFloatParameter(30f,-45f,45f);
        public ClampedFloatParameter shadowLRAngle = new ClampedFloatParameter(0f, -45f, 45f);

        public ClampedFloatParameter shadowRange = new ClampedFloatParameter(10, 10f, 100f);
        public ClampedFloatParameter shadowMapSize = new ClampedFloatParameter(2048, 512, 8192);
        public ClampedFloatParameter depthBias = new ClampedFloatParameter(1, 0, 4);

        [Header("Character receiving URP shadow")]
        public BoolParameter receiveURPShadow = new BoolParameter(false);
        public ClampedFloatParameter URPShadowIntensity = new ClampedFloatParameter(1, 0, 1);
        [Tooltip("Drag to 0 to produce regular URP shadow result (block all direct light)")]
        public ClampedFloatParameter URPShadowAsDirectLightMultiplier = new ClampedFloatParameter(1, 0, 1);
        public ColorParameter URPShadowAsDirectLightTintColor = new ColorParameter(Color.white,false,false,true);
        public bool IsActive() => true;

        public bool IsTileCompatible() => false;
    }
}

