using UnityEngine;

namespace NiloToon.NiloToonURP
{
    [CreateAssetMenu(fileName = "NiloToonShaderStrippingSettingSO", menuName = "NiloToon/CreateNiloToonShaderStrippingSettingSO", order = 0)]
    public class NiloToonShaderStrippingSettingSO : ScriptableObject
    {
        // if user didn't assign setting in renderer feature, will use C# hardcode per platform settings here
        public Settings DefaultSettings = new Settings();

        // these platform will easily out of memory crash, so we strip more shader keywords
        [Header("Android Override")]
        public bool ShouldOverrideSettingForAndroid = true;
        public Settings AndroidSettings = new Settings(false, false, false, true, true, true, true); // +strip _NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE, since it is not common to use in mobile

        [Header("iOS Override")]
        public bool ShouldOverrideSettingForIOS = true;
        public Settings iOSSettings = new Settings(false, false, false, true, true, true, true); // +strip _NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE, since it is not common to use in mobile

        [System.Serializable]
        public class Settings
        {
            public Settings() { }
            public Settings(
                bool include_NILOTOON_DEBUG_SHADING,
                bool include_NILOTOON_FORCE_MINIMUM_SHADER,
                bool include_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE,
                bool include_NILOTOON_RECEIVE_URP_SHADOWMAPPING,
                bool include_NILOTOON_RECEIVE_SELF_SHADOW,
                bool include_NILOTOON_DITHER_FADEOUT,
                bool include_MAIN_LIGHT_SHADOWS_CASCADE)
            {
                this.include_NILOTOON_DEBUG_SHADING = include_NILOTOON_DEBUG_SHADING;
                this.include_NILOTOON_FORCE_MINIMUM_SHADER = include_NILOTOON_FORCE_MINIMUM_SHADER;
                this.include_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE = include_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE;
                this.include_NILOTOON_RECEIVE_URP_SHADOWMAPPING = include_NILOTOON_RECEIVE_URP_SHADOWMAPPING;
                this.include_NILOTOON_RECEIVE_SELF_SHADOW = include_NILOTOON_RECEIVE_SELF_SHADOW;
                this.include_NILOTOON_DITHER_FADEOUT = include_NILOTOON_DITHER_FADEOUT;
                this.include_MAIN_LIGHT_SHADOWS_CASCADE = include_MAIN_LIGHT_SHADOWS_CASCADE;
            }

            [Header("If you don't need any of these in build, set them to false will reduce build time and runtime shader memory usage a lot")]
            public bool include_NILOTOON_DEBUG_SHADING = false; // usually only for editor only, so default don't include it, if user want to enable it, they can do that by editing ScriptableObject file in project
            public bool include_NILOTOON_FORCE_MINIMUM_SHADER = false; // usually only for editor only, so default don't include it, if user want to enable it, they can do that by editing ScriptableObject file in project
            public bool include_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE = true; // default included, but Android/iOS don't include it to reduce shader memory usage
            public bool include_NILOTOON_RECEIVE_URP_SHADOWMAPPING = true;
            public bool include_NILOTOON_RECEIVE_SELF_SHADOW = true;
            public bool include_NILOTOON_DITHER_FADEOUT = true;
            public bool include_MAIN_LIGHT_SHADOWS_CASCADE = true;
        }
    }
}