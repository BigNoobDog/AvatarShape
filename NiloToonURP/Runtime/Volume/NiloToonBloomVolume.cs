using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/Bloom")]
    public class NiloToonBloomVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("NiloToonBloomVolume can be used together with URP's Bloom, or replacing URP's Bloom")]
        [Header("(using NiloToonBloomVolume instead of URP's bloom will cause extra GPU performance cost)")]
        [Header("")]

        [Header("======== Same as URP's Bloom ======================================================================")]

        [Header("Bloom")]
        [Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
        public MinFloatParameter threshold = new MinFloatParameter(0.9f, 0f);

        [Tooltip("Strength of the bloom filter.")]
        public MinFloatParameter intensity = new MinFloatParameter(0f, 0f);

        [Tooltip("Changes the extent of veiling effects.")]
        public ClampedFloatParameter scatter = new ClampedFloatParameter(0.7f, 0f, 1f);

        [Tooltip("Global tint of the bloom filter.")]
        public ColorParameter tint = new ColorParameter(Color.white, false, false, true);

        [Tooltip("Clamps pixels to control the bloom amount.")]
        public MinFloatParameter clamp = new MinFloatParameter(65472f, 0f);

        [Tooltip("Use bicubic sampling instead of bilinear sampling for the upsampling passes. This is slightly more expensive but helps getting smoother visuals.")]
        public BoolParameter highQualityFiltering = new BoolParameter(false);

        [Tooltip("The number of final iterations to skip in the effect processing sequence.")]
        public ClampedIntParameter skipIterations = new ClampedIntParameter(1, 0, 16);

        [Header("Lens Dirt")]

        [Tooltip("Dirtiness texture to add smudges or dust to the bloom effect.")]
        public TextureParameter dirtTexture = new TextureParameter(null);

        [Tooltip("Amount of dirtiness.")]
        public MinFloatParameter dirtIntensity = new MinFloatParameter(0f, 0f);

        [Header("======== NiloToon added ==========================================================================")]
        public ClampedIntParameter renderTextureOverridedToFixedHeight = new ClampedIntParameter(540, 135, 2160);
        public MinFloatParameter characterAreaOverridedThreshold = new MinFloatParameter(0.9f, 0f); // same default value as threshold
        public MinFloatParameter characterAreaOverridedIntensity = new MinFloatParameter(0,0); // same default value as intensity

        public bool IsActive() => intensity.value > 0f || characterAreaOverridedIntensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}
