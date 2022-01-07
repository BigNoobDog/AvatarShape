using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/EnvironmentControlVolume")]
    public class NiloToonEnvironmentControlVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("Global GI edit")]
        public ColorParameter GlobalIlluminationTintColor = new ColorParameter(Color.white, true, false, true);
        public ColorParameter GlobalIlluminationAddColor = new ColorParameter(Color.black, true, false, true);

        [Header("Global GI override")]
        public ColorParameter GlobalIlluminationOverrideColor = new ColorParameter(Color.clear,true,true,true);

        [Header("Global Albedo override")]
        public ColorParameter GlobalAlbedoOverrideColor = new ColorParameter(new Color(1, 1, 1, 0), true, true, true);

        [Header("Global Surface Color Result override")]
        public ColorParameter GlobalSurfaceColorResultOverrideColor = new ColorParameter(new Color(1, 1, 1, 0), true, true, true);

        [Header("Global shadow boader tint color override")]
        public ColorParameter GlobalShadowBoaderTintColorOverrideColor = new ColorParameter(new Color(0, 0, 0, 0), true, true, true);

        public bool IsActive() => true;

        public bool IsTileCompatible() => false;
    }
}

