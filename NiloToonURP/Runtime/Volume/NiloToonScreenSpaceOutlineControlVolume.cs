using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/ScreenSpaceOutlineControl")]
    public class NiloToonScreenSpaceOutlineControlVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("Require enable ScreenSpace outline in NiloToonAllInOne renderer feature in order for this volume to become effective")]
        [Header("--------------------------")]
        [Header("Master control")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter intensityForCharacter = new ClampedFloatParameter(1, 0, 1);
        public ClampedFloatParameter intensityForEnvironment = new ClampedFloatParameter(1, 0, 1);

        [Header("Global width multiplier")]
        public ClampedFloatParameter widthMultiplier = new ClampedFloatParameter(1, 0, 10f);
        public ClampedFloatParameter widthMultiplierForCharacter = new ClampedFloatParameter(1, 0, 10f);
        public ClampedFloatParameter widthMultiplierForEnvironment = new ClampedFloatParameter(1, 0, 10f);
        public ClampedFloatParameter extraWidthMultiplierForXR = new ClampedFloatParameter(0.2f, 0, 10f);

        [Header("Global normal sensitivityOffset")]
        public ClampedFloatParameter normalsSensitivityOffset = new ClampedFloatParameter(0, -10f, 10f);
        public ClampedFloatParameter normalsSensitivityOffsetForCharacter = new ClampedFloatParameter(0, -10f, 10f);
        public ClampedFloatParameter normalsSensitivityOffsetForEnvironment = new ClampedFloatParameter(0, -10f, 10f);

        [Header("Global depth sensitivityOffset")]
        public ClampedFloatParameter depthSensitivityOffset = new ClampedFloatParameter(0, -10f, 10f);
        public ClampedFloatParameter depthSensitivityOffsetForCharacter = new ClampedFloatParameter(0, -10f, 10f);
        public ClampedFloatParameter depthSensitivityOffsetForEnvironment = new ClampedFloatParameter(0, -10f, 10f);

        [Header("Global depth sensitivity Distance Fadeout Strength")]
        [Tooltip("Fadeout depth sensitivity on far objects, to avoid artifact")]
        public ClampedFloatParameter depthSensitivityDistanceFadeoutStrength = new ClampedFloatParameter(1, 0, 10);
        [Tooltip("Fadeout depth sensitivity on far objects (for character shader), to avoid artifact")]
        public ClampedFloatParameter depthSensitivityDistanceFadeoutStrengthForCharacter = new ClampedFloatParameter(1, 0, 10);
        [Tooltip("Fadeout depth sensitivity on far objects (for environment shader), to avoid artifact")]
        public ClampedFloatParameter depthSensitivityDistanceFadeoutStrengthForEnvironment = new ClampedFloatParameter(1, 0, 10);

        [Header("Outline Tint Color (environment)")]
        public ColorParameter outlineTintColor = new ColorParameter(Color.white, true, false, true);
        public ColorParameter outlineTintColorForChar = new ColorParameter(Color.white, true, false, true);
        public ColorParameter outlineTintColorForEnvi = new ColorParameter(new Color(0.12f, 0.12f, 0.12f), true, false, true);

        public bool IsActive() => intensity.value > 0.0f && widthMultiplier.value > 0.0f;

        public bool IsTileCompatible() => false;
    }
}

