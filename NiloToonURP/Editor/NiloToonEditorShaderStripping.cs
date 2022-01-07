// useful resources about shader stripping:
// https://docs.unity3d.com/ScriptReference/Build.IPreprocessShaders.OnProcessShader.html
// https://github.com/lujian101/ShaderVariantCollector
// https://blog.unity.com/technology/stripping-scriptable-shader-variants
// see also URP's ShaderPreprocessor.cs

using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace NiloToon.NiloToonURP
{
    class NiloToonEditorShaderStripping : IPreprocessShaders
    {
        List<ShaderKeyword> ignoreList = new List<ShaderKeyword>();

        static readonly ShaderKeyword _NILOTOON_RECEIVE_URP_SHADOWMAPPING = new ShaderKeyword("_NILOTOON_RECEIVE_URP_SHADOWMAPPING"); // if NOT on, can strip the following
        static readonly ShaderKeyword _MAIN_LIGHT_SHADOWS_CASCADE = new ShaderKeyword("_MAIN_LIGHT_SHADOWS_CASCADE");

        static readonly ShaderKeyword _ISFACE = new ShaderKeyword("_ISFACE"); // if NOT on, can strip the following
        static readonly ShaderKeyword _FACE_MASK_ON = new ShaderKeyword("_FACE_MASK_ON");
        static readonly ShaderKeyword _FACE_SHADOW_GRADIENTMAP = new ShaderKeyword("_FACE_SHADOW_GRADIENTMAP");

        static readonly ShaderKeyword _NILOTOON_RECEIVE_SELF_SHADOW = new ShaderKeyword("_NILOTOON_RECEIVE_SELF_SHADOW"); // if NOT on, can strip the following
        static readonly ShaderKeyword _SHADOWS_SOFT = new ShaderKeyword("_SHADOWS_SOFT");
        static readonly ShaderKeyword _NILOTOON_SELFSHADOW_INTENSITY_MAP = new ShaderKeyword("_NILOTOON_SELFSHADOW_INTENSITY_MAP");

        static readonly ShaderKeyword _RAMP_LIGHTING = new ShaderKeyword("_RAMP_LIGHTING"); // if NOT on, can strip the following
        static readonly ShaderKeyword _RAMP_LIGHTING_SAMPLE_UVY_TEX = new ShaderKeyword("_RAMP_LIGHTING_SAMPLE_UVY_TEX");

        static readonly ShaderKeyword _RAMP_SPECULAR = new ShaderKeyword("_RAMP_SPECULAR"); // if NOT on, can strip the following
        static readonly ShaderKeyword _RAMP_SPECULAR_SAMPLE_UVY_TEX = new ShaderKeyword("_RAMP_SPECULAR_SAMPLE_UVY_TEX");

        static readonly ShaderKeyword _SPECULARHIGHLIGHTS = new ShaderKeyword("_SPECULARHIGHLIGHTS"); // if NOT on, can strip the following
        static readonly ShaderKeyword _SPECULARHIGHLIGHTS_TEX_TINT = new ShaderKeyword("_SPECULARHIGHLIGHTS_TEX_TINT");

        public NiloToonEditorShaderStripping()
        {
            // we are going to fill in targetResultSetting by a correct setting of current platform
            NiloToonShaderStrippingSettingSO.Settings targetResultSetting;

            // get Scriptable Object(SO) from active forward renderer's NiloToonAllInOneRendererFeature's shaderStrippingSettingSO slot
            NiloToonShaderStrippingSettingSO perPlatformUserStrippingSetting = NiloToonAllInOneRendererFeature.Instance.settings.shaderStrippingSettingSO;

            // if we can't get any SO (user didn't assign it in active forward renderer's NiloToonAllInOneRendererFeature's shaderStrippingSettingSO slot)
            // spawn a temp SO for this function only, which contains default values
            if (perPlatformUserStrippingSetting == null)
            {
                perPlatformUserStrippingSetting = new NiloToonShaderStrippingSettingSO();
            }

            // assign default setting first
            targetResultSetting = perPlatformUserStrippingSetting.DefaultSettings;

            // then assign per platform overrides
#if UNITY_ANDROID
            targetResultSetting = perPlatformUserStrippingSetting.AndroidSettings;
#endif
#if UNITY_IOS
            targetResultSetting = perPlatformUserStrippingSetting.iOSSettings;
#endif

            // finally, we have a correct targetResultSetting,
            // now add keywords that we want to strip to our ignore list
            if (!targetResultSetting.include_NILOTOON_DEBUG_SHADING)
            {
                ignoreList.Add(new ShaderKeyword("_NILOTOON_DEBUG_SHADING"));
            }
            if (!targetResultSetting.include_NILOTOON_FORCE_MINIMUM_SHADER)
            {
                ignoreList.Add(new ShaderKeyword("_NILOTOON_FORCE_MINIMUM_SHADER"));
            }
            if (!targetResultSetting.include_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE)
            {
                ignoreList.Add(new ShaderKeyword("_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE"));
            }
            if (!targetResultSetting.include_NILOTOON_RECEIVE_URP_SHADOWMAPPING)
            {
                ignoreList.Add(new ShaderKeyword("_NILOTOON_RECEIVE_URP_SHADOWMAPPING"));
            }
            if (!targetResultSetting.include_NILOTOON_RECEIVE_SELF_SHADOW)
            {
                ignoreList.Add(new ShaderKeyword("_NILOTOON_RECEIVE_SELF_SHADOW"));
            }
            if (!targetResultSetting.include_NILOTOON_DITHER_FADEOUT)
            {
                ignoreList.Add(new ShaderKeyword("_NILOTOON_DITHER_FADEOUT"));
            }
            if (!targetResultSetting.include_MAIN_LIGHT_SHADOWS_CASCADE)
            {
                ignoreList.Add(new ShaderKeyword("_MAIN_LIGHT_SHADOWS_CASCADE"));
            }
        }

        public int callbackOrder { get { return 0; } }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            // we only want to affect our character shader
            if (!shader.name.Contains("NiloToon") || !shader.name.Contains("Character")) return;

            for (int i = data.Count - 1; i >= 0; --i)
            {
                var currentKeywordSet = data[i].shaderKeywordSet;
                bool shouldIgnore = false;

                // NiloToon debug keywords
                foreach (var ignoreKeyword in ignoreList)
                {
                    if (currentKeywordSet.IsEnabled(ignoreKeyword))
                    {
                        shouldIgnore = true;
                        break;
                    }
                }

                // strip invalid _MAIN_LIGHT_SHADOWS_CASCADE
                if (!currentKeywordSet.IsEnabled(_NILOTOON_RECEIVE_URP_SHADOWMAPPING))
                {
                    if (currentKeywordSet.IsEnabled(_MAIN_LIGHT_SHADOWS_CASCADE))
                    {
                        shouldIgnore = true;
                    }
                }
                // strip invalid _SHADOWS_SOFT
                if (!currentKeywordSet.IsEnabled(_NILOTOON_RECEIVE_SELF_SHADOW))
                {
                    if (currentKeywordSet.IsEnabled(_SHADOWS_SOFT))
                        shouldIgnore = true;
                    if (currentKeywordSet.IsEnabled(_NILOTOON_SELFSHADOW_INTENSITY_MAP))
                        shouldIgnore = true;
                }

                // strip invalid _FACE_MASK_ON & _FACE_SHADOW_GRADIENTMAP
                if (!currentKeywordSet.IsEnabled(_ISFACE))
                {
                    if (currentKeywordSet.IsEnabled(_FACE_MASK_ON))
                    {
                        shouldIgnore = true;
                    }

                    if (currentKeywordSet.IsEnabled(_FACE_SHADOW_GRADIENTMAP))
                    {
                        shouldIgnore = true;
                    }
                }

                // strip invalid _RAMP_LIGHTING_SAMPLE_UVY_TEX
                if (!currentKeywordSet.IsEnabled(_RAMP_LIGHTING))
                {
                    if (currentKeywordSet.IsEnabled(_RAMP_LIGHTING_SAMPLE_UVY_TEX))
                    {
                        shouldIgnore = true;
                    }
                }

                // strip invalid _RAMP_SPECULAR_SAMPLE_UVY_TEX
                if (!currentKeywordSet.IsEnabled(_RAMP_SPECULAR))
                {
                    if (currentKeywordSet.IsEnabled(_RAMP_SPECULAR_SAMPLE_UVY_TEX))
                    {
                        shouldIgnore = true;
                    }
                }

                // strip invalid _SPECULARHIGHLIGHTS_TEX_TINT
                if (!currentKeywordSet.IsEnabled(_SPECULARHIGHLIGHTS))
                {
                    if (currentKeywordSet.IsEnabled(_SPECULARHIGHLIGHTS_TEX_TINT))
                    {
                        shouldIgnore = true;
                    }
                }

                if (shouldIgnore)
                    data.RemoveAt(i);
            }
        }
    }
}
