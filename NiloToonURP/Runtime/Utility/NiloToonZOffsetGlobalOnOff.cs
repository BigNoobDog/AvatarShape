using UnityEngine;

namespace NiloToon.NiloToonURP
{
    public static class NiloToonZOffsetGlobalOnOff
    {
        public static void SetEnable(bool shouldEnable)
        {
            // originally created to solve planar reflection problems
            // uniform's value default is 0 (all bit 0 in GPU), so default is allow ZOffset if no user call this function
            Shader.SetGlobalFloat("_GlobalShouldDisableNiloToonZOffset", shouldEnable ? 0 : 1);
        }
    }
}


