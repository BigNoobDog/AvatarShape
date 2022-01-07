using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [System.Serializable, VolumeComponentMenu("NiloToon/Rendering Performance Control")]
    public class NiloToonRenderingPerformanceControlVolume : VolumeComponent, IPostProcessComponent
    {
        [Header("NiloToonRenderingPerformanceControlVolume can be used to control performance setting in different scene")]
        [Header("For example, using low quality settings in multi-player scene, or using highest quality in player skin showcase scene")]
        [Header("")]
        public BoolParameter overrideEnableDepthTextureRimLigthAndShadow = new BoolParameter(true);

        public bool IsActive() => true;

        public bool IsTileCompatible() => false;
    }
}
