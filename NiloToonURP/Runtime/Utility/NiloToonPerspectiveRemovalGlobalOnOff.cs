using UnityEngine;

namespace NiloToon.NiloToonURP
{
    public static class NiloToonPerspectiveRemovalGlobalOnOff
    {
        public static void SetEnable(bool shouldEnable)
        {
            // originally created to solve planar reflection problems
            // uniform's value default is 0 (all bit 0 in GPU), so default is allow PerspectiveRemoval if no user call this function
            Shader.SetGlobalFloat("_GlobalShouldDisableNiloToonPerspectiveRemoval", shouldEnable ? 0 : 1);
        }
    }
}


