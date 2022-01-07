namespace NiloToon.NiloToonURP
{
    public static class NiloToonPlanarReflectionHelper
    {
        public static void BeginPlanarReflectionCameraRender()
        {
            NiloToonZOffsetGlobalOnOff.SetEnable(false);
            NiloToonPerspectiveRemovalGlobalOnOff.SetEnable(false);
        }
        public static void EndPlanarReflectionCameraRender()
        {
            NiloToonZOffsetGlobalOnOff.SetEnable(true);
            NiloToonPerspectiveRemovalGlobalOnOff.SetEnable(true);
        }
    }
}

