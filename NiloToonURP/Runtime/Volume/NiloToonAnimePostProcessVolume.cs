using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/AnimePostProcess")]
    public class NiloToonAnimePostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("Master control")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0, 0, 1);

        [Header("Draw Timing")]
        [Tooltip(   "If false, will draw after other rendering.\n" +
                    "If true, will draw before postprocess.\n" +
                    "Should turn on if you are rendering to a custom RenderTexture.")]
        public BoolParameter drawBeforePostProcess = new BoolParameter(false);

        [Header("Top Light")]
        [Tooltip("Default 0.5")]
        public ClampedFloatParameter topLightEffectDrawHeight = new ClampedFloatParameter(0.5f,0,1);
        public MinFloatParameter topLightEffectIntensity = new MinFloatParameter(1, 0);
        public ClampedFloatParameter topLightDesaturate = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter topLightTintColor = new ColorParameter(Color.white, true, false, true);

        [Header("Bottom Darken")]
        [Tooltip("Default 0.5")]
        public ClampedFloatParameter bottomDarkenEffectDrawHeight = new ClampedFloatParameter(0.5f, 0, 1);
        public MinFloatParameter bottomDarkenEffectIntensity = new MinFloatParameter(1, 0);

        public bool IsActive() => intensity.value > 0f && (topLightEffectIntensity.value > 0 || bottomDarkenEffectIntensity.value > 0);

        public bool IsTileCompatible() => false;
    }
}

