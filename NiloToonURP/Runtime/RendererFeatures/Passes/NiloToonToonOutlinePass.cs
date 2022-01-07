using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace NiloToon.NiloToonURP
{
    public class NiloToonToonOutlinePass : ScriptableRenderPass
    {
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
#if UNITY_2020_1_OR_NEWER
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
#else
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
#endif
        {
            // We automatically turn on _CameraDepthTexture and _CameraNormalsTexture if needed,
            // so user don't need to turn on _CameraDepthTexture in their Universal Render Pipeline Asset manually.
            // In URP7(Unity 2019.4) we don't have ConfigureInput(), so we will need user to enable depth texture manually.
            // *_CameraNormalsTexture only exists in URP10 or above
#if UNITY_2020_1_OR_NEWER
            ScriptableRenderPassInput input = ScriptableRenderPassInput.None;
            // added isPreviewCamera check, to avoid "material preview window makes outline flicker" bug
            if (ShouldRenderScreenSpaceOutline(renderingData) || renderingData.cameraData.isPreviewCamera)
            {
                input |= ScriptableRenderPassInput.Depth; // screen space outline requires URP's _CameraDepthTexture
                input |= ScriptableRenderPassInput.Normal; // screen space outline requires URP's _CameraNormalsTexture
            }
            ConfigureInput(input);
#endif
        }

        private bool ShouldRenderScreenSpaceOutline(RenderingData renderingData)
        {
            var screenSpaceOutlineVolumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonScreenSpaceOutlineControlVolume>();
            //Debug.Log("screenSpaceOutlineVolumeEffect.IsActive()" + screenSpaceOutlineVolumeEffect.IsActive()); // NOTE: when material preview exist, user drag UI will make this false
            return settings.AllowRenderScreenSpaceOutline && screenSpaceOutlineVolumeEffect && screenSpaceOutlineVolumeEffect.IsActive();
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderOutline(context, renderingData);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
#if UNITY_2020_1_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
            // do nothing
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // for rendering "LightMode"="ToonOutline" Pass (Traditional outline)
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static readonly ShaderTagId toonOutlineLightModeShaderTagId = new ShaderTagId("NiloToonOutline");
        static readonly ShaderTagId toonExtraThickOutlineLightModeShaderTagId = new ShaderTagId("NiloToonExtraThickOutline");

        static readonly int _GlobalShouldRenderOutline_SID = Shader.PropertyToID("_GlobalShouldRenderOutline");
        static readonly int _GlobalOutlineWidthMultiplier_SID = Shader.PropertyToID("_GlobalOutlineWidthMultiplier");
        static readonly int _GlobalOutlineTintColor_SID = Shader.PropertyToID("_GlobalOutlineTintColor");

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // for rendering Screen space outline
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static readonly int _GlobalScreenSpaceOutlineIntensityForChar_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineIntensityForChar");
        static readonly int _GlobalScreenSpaceOutlineIntensityForEnvi_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineIntensityForEnvi");

        static readonly int _GlobalScreenSpaceOutlineWidthMultiplierForChar_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineWidthMultiplierForChar");
        static readonly int _GlobalScreenSpaceOutlineWidthMultiplierForEnvi_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineWidthMultiplierForEnvi");

        static readonly int _GlobalScreenSpaceOutlineNormalsSensitivityOffsetForChar_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineNormalsSensitivityOffsetForChar");
        static readonly int _GlobalScreenSpaceOutlineNormalsSensitivityOffsetForEnvi_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineNormalsSensitivityOffsetForEnvi");

        static readonly int _GlobalScreenSpaceOutlineDepthSensitivityOffsetForChar_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineDepthSensitivityOffsetForChar");
        static readonly int _GlobalScreenSpaceOutlineDepthSensitivityOffsetForEnvi_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineDepthSensitivityOffsetForEnvi");

        static readonly int _GlobalScreenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForChar_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForChar");
        static readonly int _GlobalScreenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForEnvi_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForEnvi");

        static readonly int _GlobalScreenSpaceOutlineTintColorForChar_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineTintColorForChar");
        static readonly int _GlobalScreenSpaceOutlineTintColorForEnvi_SID = Shader.PropertyToID("_GlobalScreenSpaceOutlineTintColorForEnvi");

        [Serializable]
        public class Settings
        {
            [Header("Traditional outline")]
            [Tooltip("Can turn OFF to improve performance")]
            public bool ShouldRenderOutline = true;
            [Range(0, 4)]
            public float outlineWidthMultiplier = 1;
            [Range(0, 4)]
            public float outlineWidthExtraMultiplierForXR = 0.5f; // VR default smaller outline, due to high FOV(90)
            [ColorUsage(false, true)]
            public Color outlineTintColor = Color.white;

            [Header("Screen space outline (require URP10 or above)")]
            [Tooltip("Enable this will allow NiloToon to automatically enable URP's Depth and Normals Texture's rendering (can be slow), similar to URP10's SSAO renderer feature (default is false because it is very slow)")]
            public bool AllowRenderScreenSpaceOutline = false;
        }

        NiloToonRendererFeatureSettings allSettings;
        Settings settings;
        ProfilingSampler m_ProfilingSamplerTraditionalOutline;
        ProfilingSampler m_ProfilingSamplerExtraThickOutline;
        ProfilingSampler m_ProfilingSamplerScreenSpaceOutline;

        // constructor
        public NiloToonToonOutlinePass(NiloToonRendererFeatureSettings allSettings)
        {
            this.allSettings = allSettings;
            this.settings = allSettings.outlineSettings;
            m_ProfilingSamplerTraditionalOutline = new ProfilingSampler("NiloToonToonOutlinePass(Traditional outline)");
            m_ProfilingSamplerExtraThickOutline = new ProfilingSampler("NiloToonToonOutlinePass(Extra thick outline");
            m_ProfilingSamplerScreenSpaceOutline = new ProfilingSampler("NiloToonToonOutlinePass(Screen space outline)");
        }

        private void renderOutline(ScriptableRenderContext context, RenderingData renderingData)
        {
            // NOTE: [how to use ProfilingSampler to correctly]
            /*
		        // [write as class member]
	            ProfilingSampler m_ProfilingSampler;

	            // [call once in constrcutor]
	            m_ProfilingSampler = new ProfilingSampler("NiloToonToonOutlinePass");

	            // [call in execute]
		        // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
		        // Currently there's an issue which results in mismatched markers.
		        CommandBuffer cmd = CommandBufferPool.Get();
		        using (new ProfilingScope(cmd, m_ProfilingSampler))
		        { 
		            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
		            cmd.SetGlobalTexture("_CameraNormalRT", _normalRT.Identifier());
		        }
		        context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);	
            */

            /////////////////////////////////////////////////////////
            // traditional outline
            /////////////////////////////////////////////////////////
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSamplerTraditionalOutline))
            {
                bool shouldRenderTraditionalOutline = settings.ShouldRenderOutline && !allSettings.MiscSettings.ForceNoOutline && !allSettings.MiscSettings.ForceMinimumShader;

                if (shouldRenderTraditionalOutline)
                {
                    var volumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonCharRenderingControlVolume>();

                    // set default value first because volume may not exist in scene
                    float outlineWidthMultiplierResult = settings.outlineWidthMultiplier;
                    // extra outline control if XR
                    if (XRSettings.isDeviceActive)
                    {
                        outlineWidthMultiplierResult *= volumeEffect.charOutlineWidthExtraMultiplierForXR.overrideState ? volumeEffect.charOutlineWidthExtraMultiplierForXR.value : settings.outlineWidthExtraMultiplierForXR;
                    }

                    Color outlineTintColor = settings.outlineTintColor;
                    // then set new value if user overrided
                    if (volumeEffect != null)
                    {
                        outlineWidthMultiplierResult *= volumeEffect.charOutlineWidthMultiplier.value;
                        outlineTintColor *= volumeEffect.charOutlineMulColor.value;
                    }
                    // set
                    cmd.SetGlobalFloat(_GlobalShouldRenderOutline_SID, 1);
                    cmd.SetGlobalFloat(_GlobalOutlineWidthMultiplier_SID, outlineWidthMultiplierResult);
                    cmd.SetGlobalColor(_GlobalOutlineTintColor_SID, outlineTintColor);

                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);

                    // draw self outline
                    DrawingSettings drawingSettings = CreateDrawingSettings(toonOutlineLightModeShaderTagId, ref renderingData, SortingCriteria.CommonOpaque);
                    FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
                }
                else
                {
                    // set
                    cmd.SetGlobalFloat(_GlobalShouldRenderOutline_SID, 0);

                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                    // no draw
                    // (X)
                }
            }


            /////////////////////////////////////////////////////////
            // extra thick outline
            /////////////////////////////////////////////////////////
            CommandBuffer cmdExtraThickOutline = CommandBufferPool.Get();
            using (new ProfilingScope(cmdExtraThickOutline, m_ProfilingSamplerExtraThickOutline))
            {
                // draw extra thick outline, no need to do disable check because it is default off per charcater
                if (Application.isPlaying)
                {
                    DrawingSettings extraThickOutlineDrawingSettings = CreateDrawingSettings(toonExtraThickOutlineLightModeShaderTagId, ref renderingData, SortingCriteria.CommonOpaque);
                    FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
                    context.DrawRenderers(renderingData.cullResults, ref extraThickOutlineDrawingSettings, ref filteringSettings);
                }

            }
            context.ExecuteCommandBuffer(cmdExtraThickOutline);
            CommandBufferPool.Release(cmdExtraThickOutline);


            //////////////////////////////////////////////////////////////////////////////////////////////////
            // global volume (NiloToonScreenSpaceOutlineControlVolume)
            //////////////////////////////////////////////////////////////////////////////////////////////////
            CommandBuffer cmdSS = CommandBufferPool.Get();
            using (new ProfilingScope(cmdSS, m_ProfilingSamplerScreenSpaceOutline))
            {
                // added isPreviewCamera check, to avoid "material preview window makes outline flicker" bug
                if (!renderingData.cameraData.isPreviewCamera)
                {
                    bool shouldRenderScreenSpaceOutline = ShouldRenderScreenSpaceOutline(renderingData);
                    if (shouldRenderScreenSpaceOutline)
                    {
                        // set default value first because volume may not exist in scene
                        // TODO: maybe it is not needed to set default value first, because it seems VolumeManager.instance.stack.GetComponent<XXXVolume>() will never return null, even no volume exist in scene
                        float screenSpaceOutlineIntensityForChar = 0;
                        float screenSpaceOutlineIntensityForEnvi = 0;

                        float screenSpaceOutlineWidthMultiplierForChar = 1;
                        float screenSpaceOutlineWidthMultiplierForEnvi = 1;

                        float screenSpaceOutlineDepthSensitivityGlobalOffsetForChar = 0;
                        float screenSpaceOutlineDepthSensitivityGlobalOffsetForEnvi = 0;

                        float screenSpaceOutlineNormalsSensitivityGlobalOffsetForChar = 0;
                        float screenSpaceOutlineNormalsSensitivityGlobalOffsetForEnvi = 0;

                        float screenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForChar = 1;
                        float screenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForEnvi = 1;

                        Color screenSpaceOutlineTintColorForChar = Color.white;
                        Color screenSpaceOutlineTintColorForEnvi = Color.white * 0.12f;

                        var screenSpaceOutlineVolumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonScreenSpaceOutlineControlVolume>();

                        // then set new value if user overrided any of them by volume
                        if (screenSpaceOutlineVolumeEffect != null)
                        {
                            var v = screenSpaceOutlineVolumeEffect; // just for writing shorter name as (v) in the code below

                            screenSpaceOutlineIntensityForChar = v.intensity.value * v.intensityForCharacter.value;
                            screenSpaceOutlineIntensityForEnvi = v.intensity.value * v.intensityForEnvironment.value;

                            screenSpaceOutlineWidthMultiplierForChar = v.widthMultiplier.value * v.widthMultiplierForCharacter.value;
                            screenSpaceOutlineWidthMultiplierForEnvi = v.widthMultiplier.value * v.widthMultiplierForEnvironment.value;

                            // extra outline control if XR
                            if (XRSettings.isDeviceActive)
                            {
                                screenSpaceOutlineWidthMultiplierForChar *= v.extraWidthMultiplierForXR.value;
                                screenSpaceOutlineWidthMultiplierForEnvi *= v.extraWidthMultiplierForXR.value;
                            }

                            screenSpaceOutlineDepthSensitivityGlobalOffsetForChar = v.depthSensitivityOffset.value + v.depthSensitivityOffsetForCharacter.value;
                            screenSpaceOutlineDepthSensitivityGlobalOffsetForEnvi = v.depthSensitivityOffset.value + v.depthSensitivityOffsetForEnvironment.value;

                            screenSpaceOutlineNormalsSensitivityGlobalOffsetForChar = v.normalsSensitivityOffset.value + v.normalsSensitivityOffsetForCharacter.value;
                            screenSpaceOutlineNormalsSensitivityGlobalOffsetForEnvi = v.normalsSensitivityOffset.value + v.normalsSensitivityOffsetForEnvironment.value;

                            screenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForChar = v.depthSensitivityDistanceFadeoutStrength.value * v.depthSensitivityDistanceFadeoutStrengthForCharacter.value;
                            screenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForEnvi = v.depthSensitivityDistanceFadeoutStrength.value * v.depthSensitivityDistanceFadeoutStrengthForEnvironment.value;

                            screenSpaceOutlineTintColorForChar = v.outlineTintColor.value * v.outlineTintColorForChar.value;
                            screenSpaceOutlineTintColorForEnvi = v.outlineTintColor.value * v.outlineTintColorForEnvi.value;
                        }


                        // values that send to shader is defined here, adjustment is applyied in order to make volume UI's default value always = 0 or 1 (easier for user)
                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineIntensityForChar_SID, screenSpaceOutlineIntensityForChar);
                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineIntensityForEnvi_SID, screenSpaceOutlineIntensityForEnvi);

                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineWidthMultiplierForChar_SID, screenSpaceOutlineWidthMultiplierForChar * 1.875f);
                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineWidthMultiplierForEnvi_SID, screenSpaceOutlineWidthMultiplierForEnvi * 1.875f);

                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineDepthSensitivityOffsetForChar_SID, screenSpaceOutlineDepthSensitivityGlobalOffsetForChar);
                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineDepthSensitivityOffsetForEnvi_SID, screenSpaceOutlineDepthSensitivityGlobalOffsetForEnvi);

                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineNormalsSensitivityOffsetForChar_SID, screenSpaceOutlineNormalsSensitivityGlobalOffsetForChar);
                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineNormalsSensitivityOffsetForEnvi_SID, screenSpaceOutlineNormalsSensitivityGlobalOffsetForEnvi + 0.25f);

                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForChar_SID, screenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForChar);
                        cmdSS.SetGlobalFloat(_GlobalScreenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForEnvi_SID, screenSpaceOutlineDepthSensitivityDistanceFadeoutStrengthForEnvi);

                        cmdSS.SetGlobalColor(_GlobalScreenSpaceOutlineTintColorForChar_SID, screenSpaceOutlineTintColorForChar);
                        cmdSS.SetGlobalColor(_GlobalScreenSpaceOutlineTintColorForEnvi_SID, screenSpaceOutlineTintColorForEnvi);
                    }
                    CoreUtils.SetKeyword(cmdSS, "_NILOTOON_GLOBAL_ENABLE_SCREENSPACE_OUTLINE", shouldRenderScreenSpaceOutline);

                }

            }
            context.ExecuteCommandBuffer(cmdSS);
            CommandBufferPool.Release(cmdSS);
        }
    }
}