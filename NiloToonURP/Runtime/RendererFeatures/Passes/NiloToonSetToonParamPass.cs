using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    public class NiloToonSetToonParamPass : ScriptableRenderPass
    {
        // this renderer feature's public GUI settings
        [Serializable]
        public class Settings
        {
            [Header("Receive URP Shadow")]
            [Tooltip("Turn OFF to improve performance. Default is OFF because OFF looks better (visually more stable), " +
                        "since we don't know if user's URP shadowmap is high quality/resolution enough or not. " +
                        "Not receive URP shadow is better than receiving jaggy URP shadow map")]
            public bool ShouldReceiveURPShadows = false;
            [Range(0, 1)]
            [Tooltip("Default is 1")]
            public float URPShadowIntensity = 1;
            [Tooltip("Default is 0.1")]
            public float URPShadowDepthBias = 0.1f;
            [Tooltip("Default is 0 by design, to avoid shadow holes when receiving URP shadow map")]
            public float URPShadowNormalBiasMultiplier = 0;

            [Header("Depth Texture Rim Light and Shadow")]
            [Tooltip("Default ON, can turn OFF to fallback to NoV rimlight to improve performance if you are targeting slow mobile (turn ON will auto request URP's depth texture!)")]
            public bool EnableDepthTextureRimLigthAndShadow = true;
            [Tooltip("Default is 1, you can edit it for artistic reason")]
            public float DepthTextureRimLightAndShadowWidthMultiplier = 1;
            [Tooltip("Default is 0, you can edit it for artistic reason")]
            public float DepthTexRimLightDepthDiffThresholdOffset = 0;

            [Header("Debug performance")]
            public bool ForceMinimumShader = false;
            public bool ForceMinimumEnviShader = false;
            public bool ForceNoOutline = false;
        }

        // [Debug shading API]
        // set by user scripting / NiloToon's debug window only, not exposed in renderer feature's GUI
        [NonSerialized] public bool EnableShadingDebug = false;
        [NonSerialized] public ShadingDebugCase shadingDebugCase = ShadingDebugCase.Albedo;

        public enum ShadingDebugCase
        {
            Albedo = 0,
            White = 1,
            Occlusion = 2,
            Emission = 3,
            NormalWS = 4,
            UV = 5,
            VertexColorR = 6,
            VertexColorG = 7,
            VertexColorB = 8,
            VertexColorA = 9,
            VertexColorRGB = 10,
            Specular = 11,
            UV8 = 12,
            SimpleLit = 13,
        }

        // singleton
        public static NiloToonSetToonParamPass Instance => _instance;
        static NiloToonSetToonParamPass _instance;

        // setting
        public Settings Setting => this.allSettings.MiscSettings;
        NiloToonRendererFeatureSettings allSettings;

        // private field and public access
        public bool EnableDepthTextureRimLigthAndShadow => enableDepthTextureRimLigthAndShadow;
        bool enableDepthTextureRimLigthAndShadow;

        // profiling
        ProfilingSampler m_ProfilingSampler;

        // all Shader.PropertyToID
        static readonly int _GlobalReceiveURPShadowAmount_SID = Shader.PropertyToID("_GlobalReceiveShadowMappingAmount");
        static readonly int _GlobalReceiveSelfShadowMappingPosOffset_SID = Shader.PropertyToID("_GlobalReceiveSelfShadowMappingPosOffset");
        static readonly int _GlobalToonShaderNormalBiasMultiplier_SID = Shader.PropertyToID("_GlobalToonShaderNormalBiasMultiplier");
        static readonly int _GlobalMainLightURPShadowAsDirectResultTintColor_SID = Shader.PropertyToID("_GlobalMainLightURPShadowAsDirectResultTintColor");

        static readonly int _GlobalToonShadeDebugCase_SID = Shader.PropertyToID("_GlobalToonShadeDebugCase");
        static readonly int _GlobalOcclusionStrength_SID = Shader.PropertyToID("_GlobalOcclusionStrength");

        static readonly int _GlobalIndirectLightMultiplier_SID = Shader.PropertyToID("_GlobalIndirectLightMultiplier");
        static readonly int _GlobalIndirectLightMinColor_SID = Shader.PropertyToID("_GlobalIndirectLightMinColor");

        static readonly int _GlobalMainDirectionalLightMultiplier_SID = Shader.PropertyToID("_GlobalMainDirectionalLightMultiplier");
        static readonly int _GlobalMainDirectionalLightMaxContribution_SID = Shader.PropertyToID("_GlobalMainDirectionalLightMaxContribution");

        static readonly int _GlobalAdditionalLightMultiplier_SID = Shader.PropertyToID("_GlobalAdditionalLightMultiplier");
        static readonly int _GlobalAdditionalLightMaxContribution_SID = Shader.PropertyToID("_GlobalAdditionalLightMaxContribution");

        static readonly int _GlobalSpecularIntensityMultiplier_SID = Shader.PropertyToID("_GlobalSpecularIntensityMultiplier");
        static readonly int _GlobalSpecularInShadowMinIntensity_SID = Shader.PropertyToID("_GlobalSpecularMinIntensity");
        static readonly int _GlobalSpecularReactToLightDirectionChange_SID = Shader.PropertyToID("_GlobalSpecularReactToLightDirectionChange");

        static readonly int _CurrentCameraFOV_SID = Shader.PropertyToID("_CurrentCameraFOV");
        static readonly int _GlobalAspectFix_SID = Shader.PropertyToID("_GlobalAspectFix");
        static readonly int _GlobalFOVorOrthoSizeFix_SID = Shader.PropertyToID("_GlobalFOVorOrthoSizeFix");
        static readonly int _GlobalDepthTexRimLightAndShadowWidthMultiplier_SID = Shader.PropertyToID("_GlobalDepthTexRimLightAndShadowWidthMultiplier");
        static readonly int _GlobalDepthTexRimLightDepthDiffThresholdOffset_SID = Shader.PropertyToID("_GlobalDepthTexRimLightDepthDiffThresholdOffset");
        static readonly int _GlobalMainLightDirVS_SID = Shader.PropertyToID("_GlobalMainLightDirVS");
        static readonly int _GlobalVolumeBaseColorTintColor_SID = Shader.PropertyToID("_GlobalVolumeBaseColorTintColor");
        static readonly int _GlobalVolumeMulColor_SID = Shader.PropertyToID("_GlobalVolumeMulColor");
        static readonly int _GlobalVolumeLerpColor_SID = Shader.PropertyToID("_GlobalVolumeLerpColor");
        static readonly int _GlobalRimLightMultiplier_SID = Shader.PropertyToID("_GlobalRimLightMultiplier");
        static readonly int _GlobalDepthTexRimLightCameraDistanceFadeoutStartDistance_SID = Shader.PropertyToID("_GlobalDepthTexRimLightCameraDistanceFadeoutStartDistance");
        static readonly int _GlobalDepthTexRimLightCameraDistanceFadeoutEndDistance_SID = Shader.PropertyToID("_GlobalDepthTexRimLightCameraDistanceFadeoutEndDistance");

        static readonly int _NiloToonGlobalEnviGITintColor_SID = Shader.PropertyToID("_NiloToonGlobalEnviGITintColor");
        static readonly int _NiloToonGlobalEnviGIAddColor_SID = Shader.PropertyToID("_NiloToonGlobalEnviGIAddColor");
        static readonly int _NiloToonGlobalEnviGIOverride_SID = Shader.PropertyToID("_NiloToonGlobalEnviGIOverride");
        static readonly int _NiloToonGlobalEnviAlbedoOverrideColor_SID = Shader.PropertyToID("_NiloToonGlobalEnviAlbedoOverrideColor");
        static readonly int _NiloToonGlobalEnviMinimumShader_SID = Shader.PropertyToID("_NiloToonGlobalEnviMinimumShader");
        static readonly int _NiloToonGlobalEnviShadowBorderTintColor_SID = Shader.PropertyToID("_NiloToonGlobalEnviShadowBorderTintColor");
        static readonly int _NiloToonGlobalEnviSurfaceColorResultOverrideColor_SID = Shader.PropertyToID("_NiloToonGlobalEnviSurfaceColorResultOverrideColor");

        // constructor
        public NiloToonSetToonParamPass(NiloToonRendererFeatureSettings allSettings)
        {
            this.allSettings = allSettings;
            _instance = this;
            m_ProfilingSampler = new ProfilingSampler(typeof(NiloToonSetToonParamPass).Name);
        }

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
            // we automatically turn on _CameraDepthTexture if needed
            // so user don't need to turn on depth texture in their Universal Render Pipeline Asset manually.
            // In URP7(Unity 2019.4) we don't have ConfigureInput(), so we will need user to enable depth texture manually
#if UNITY_2020_1_OR_NEWER
            ScriptableRenderPassInput input = ScriptableRenderPassInput.None;

            if (GetEnableDepthTextureRimLigthAndShadow(Setting))
            {
                input |= ScriptableRenderPassInput.Depth;
            }

            ConfigureInput(input);
#endif
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            setParam(context, renderingData);
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

        private bool GetEnableDepthTextureRimLigthAndShadow(Settings settings)
        {
            var performanceSettingVolume = VolumeManager.instance.stack.GetComponent<NiloToonRenderingPerformanceControlVolume>();

            if(performanceSettingVolume.overrideEnableDepthTextureRimLigthAndShadow.overrideState)
            {
                // if overrided in volume, use overrided setting from volume
                return performanceSettingVolume.overrideEnableDepthTextureRimLigthAndShadow.value;
            }
            else
            {
                // if not overrided in volume, use NiloToon all in one renderer feature's setting
                return settings.EnableDepthTextureRimLigthAndShadow;
            }
        }
        private void setParam(ScriptableRenderContext context, RenderingData renderingData)
        {
            var charRenderingControlVolumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonCharRenderingControlVolume>();
            var shadowControlVolumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonShadowControlVolume>();
            var environmentControlVolumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonEnvironmentControlVolume>();

            // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
            // Currently there's an issue which results in mismatched markers.
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                // additional light keyword on/off
                //////////////////////////////////////////////////////////////////////////////////////////////////
                // see URP's ForwardLights.cs -> SetUp(), where URP enable additional light keywords
                // here we combine URP's per vertex / per pixel keywords into 1 keyword, to reduce multi_compile shader variant count, since we always need vertex light only by design
                CoreUtils.SetKeyword(cmd, "_NILOTOON_ADDITIONAL_LIGHTS", renderingData.lightData.additionalLightsCount > 0);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // URP shadow related global settings
                //////////////////////////////////////////////////////////////////////////////////////////////////
                // [findout main light's shadow is enabled or not]
                // see URP's MainLightShadowCasterPass.cs -> RenderMainLightCascadeShadowmap(), where URP enable main light shadow keyword
                int shadowLightIndex = renderingData.lightData.mainLightIndex;
                bool mainLightEnabledShadow = shadowLightIndex != -1;

                // volume override URP shadow related params
                bool receiveURPShadow = shadowControlVolumeEffect.receiveURPShadow.overrideState ? shadowControlVolumeEffect.receiveURPShadow.value : Setting.ShouldReceiveURPShadows;
                float URPShadowIntensity = shadowControlVolumeEffect.URPShadowIntensity.overrideState ? shadowControlVolumeEffect.URPShadowIntensity.value : Setting.URPShadowIntensity;
                Color URPShadowAsDirectLightTintColor = shadowControlVolumeEffect.URPShadowAsDirectLightTintColor.value;
                URPShadowAsDirectLightTintColor *= shadowControlVolumeEffect.URPShadowAsDirectLightMultiplier.value;

                // here we combine URP's main light shadow keyword and our ShouldReceiveURPShadows keyword into 1, to reduce the number of multi_compile shader variant 
                CoreUtils.SetKeyword(cmd, "_NILOTOON_RECEIVE_URP_SHADOWMAPPING", receiveURPShadow && mainLightEnabledShadow);
                cmd.SetGlobalFloat(_GlobalReceiveURPShadowAmount_SID, URPShadowIntensity);
                cmd.SetGlobalFloat(_GlobalToonShaderNormalBiasMultiplier_SID, Setting.URPShadowNormalBiasMultiplier);
                cmd.SetGlobalFloat(_GlobalReceiveSelfShadowMappingPosOffset_SID, Setting.URPShadowDepthBias);
                cmd.SetGlobalColor(_GlobalMainLightURPShadowAsDirectResultTintColor_SID, URPShadowAsDirectLightTintColor);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // RT(screen) aspect
                //////////////////////////////////////////////////////////////////////////////////////////////////
                cmd.SetGlobalVector(_GlobalAspectFix_SID, new Vector2((float)renderingData.cameraData.camera.pixelHeight / (float)renderingData.cameraData.camera.pixelWidth, 1f));

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // camera fov
                //////////////////////////////////////////////////////////////////////////////////////////////////
                Camera camera = renderingData.cameraData.camera;
                cmd.SetGlobalFloat(_GlobalFOVorOrthoSizeFix_SID, 1f / (camera.orthographic ? camera.orthographicSize * 100f : camera.fieldOfView));
                cmd.SetGlobalFloat(_CurrentCameraFOV_SID, camera.fieldOfView);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // main light direction (view space)
                //////////////////////////////////////////////////////////////////////////////////////////////////
                int mainLightIndex = renderingData.lightData.mainLightIndex;
                if (mainLightIndex >= 0)
                {
                    VisibleLight mainLight = renderingData.lightData.visibleLights[mainLightIndex];
                    Vector3 mainLightDirWS = mainLight.light.transform.forward;
                    Vector3 mainLightDirVS = camera.worldToCameraMatrix.MultiplyVector(mainLightDirWS);
                    cmd.SetGlobalVector(_GlobalMainLightDirVS_SID, -mainLightDirVS);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // 2D depth texture rim light (set global uniform)
                //////////////////////////////////////////////////////////////////////////////////////////////////
                cmd.SetGlobalFloat(_GlobalDepthTexRimLightAndShadowWidthMultiplier_SID, Setting.DepthTextureRimLightAndShadowWidthMultiplier * charRenderingControlVolumeEffect.depthTextureRimLightAndShadowWidthMultiplier.value * 1.25f); // 1.25 to make default == 1 on user's side GUI
                cmd.SetGlobalFloat(_GlobalDepthTexRimLightDepthDiffThresholdOffset_SID, Setting.DepthTexRimLightDepthDiffThresholdOffset);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // 2D depth texture rim light and shadow (let NiloToonPerCharacterRenderController knows enableDepthTextureRimLigthAndShadow's value)
                //////////////////////////////////////////////////////////////////////////////////////////////////
                enableDepthTextureRimLigthAndShadow = GetEnableDepthTextureRimLigthAndShadow(Setting);

                // fix for Unity 2019
#if !UNITY_2020_1_OR_NEWER
                bool isNotGameCamera;

                isNotGameCamera = renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera;

                // if 2019.4 LTS, we disable depth texture rim light if MSAA is OFF, because _CameraDepthTexture depth prepass will not rendered by URP
                // if 2020.3 LTS, URP always render _CameraDepthTexture depth prepass, so we don't care MSAA is on or off
                if (enableDepthTextureRimLigthAndShadow && renderingData.cameraData.cameraTargetDescriptor.msaaSamples == 1 && !isNotGameCamera)
                {
                    enableDepthTextureRimLigthAndShadow = false;
                    Debug.LogWarning("[NiloToon] In Unity 2019, EnableDepthTextureRimLigthAndShadow and not turning on MSAA is NOT supported, will fallback to turn OFF EnableDepthTextureRimLigthAndShadow");
                }
#endif

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // debug case
                //////////////////////////////////////////////////////////////////////////////////////////////////
                cmd.SetGlobalFloat(_GlobalToonShadeDebugCase_SID, (int)shadingDebugCase);
                CoreUtils.SetKeyword(cmd, "_NILOTOON_DEBUG_SHADING", EnableShadingDebug);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // force minium shader
                //////////////////////////////////////////////////////////////////////////////////////////////////
                CoreUtils.SetKeyword(cmd, "_NILOTOON_FORCE_MINIMUM_SHADER", Setting.ForceMinimumShader);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // global volume (NiloToonCharRenderingControlVolume)
                //////////////////////////////////////////////////////////////////////////////////////////////////

                // set default value first because volume may not exist in scene
                Color baseColorTintResult = Color.white;
                Color mulColorResult = Color.white;
                Color lerpColorResult = Color.clear;
                float occlusionUsage = 1;
                float indirectLightMultiplier = 1;
                Color indirectLightMinColor = new Color(0.01f, 0.01f, 0.01f);

                Color rimLightMultiplier = Color.white;
                float rimLightCameraDistanceFadeoutStartDistance = 50;
                float rimLightCameraDistanceFadeoutEndDistance = 100;

                Color mainDirectionalLightIntensityMultiplierColor = Color.white;
                float mainDirectionalLightMaxContribution = 1;
                Color mainDirectionalLightMaxContributionColor = Color.white;

                Color additionalLightIntensityMultiplierColor = Color.white;
                float additionalLightMaxContribution = 1;
                Color additionalLightMaxContributionColor = Color.white;

                float specularIntensityMultiplier = 1;
                float specularInShadowMinIntensity = 0.25f;
                bool specularReactToLightDirectionChange = false; // default is false by design

                // then set new value if user overrided
                if (charRenderingControlVolumeEffect != null)
                {
                    // base color tint
                    baseColorTintResult *= charRenderingControlVolumeEffect.charBaseColorMultiply.value;
                    baseColorTintResult *= charRenderingControlVolumeEffect.charBaseColorTintColor.value;

                    // mul
                    mulColorResult *= charRenderingControlVolumeEffect.charMulColor.value;
                    mulColorResult *= charRenderingControlVolumeEffect.charMulColorIntensity.value;

                    // lerp
                    lerpColorResult = charRenderingControlVolumeEffect.charLerpColor.value;
                    lerpColorResult.a = charRenderingControlVolumeEffect.charLerpColorUsage.value;

                    // occlusion
                    occlusionUsage = charRenderingControlVolumeEffect.charOcclusionUsage.value;

                    // indirect light
                    indirectLightMultiplier = charRenderingControlVolumeEffect.charIndirectLightMultiplier.value;
                    indirectLightMinColor = charRenderingControlVolumeEffect.charIndirectLightMinColor.value;

                    // direct light
                    mainDirectionalLightIntensityMultiplierColor = charRenderingControlVolumeEffect.mainDirectionalLightIntensityMultiplier.value * charRenderingControlVolumeEffect.mainDirectionalLightIntensityMultiplierColor.value;
                    mainDirectionalLightMaxContribution = charRenderingControlVolumeEffect.mainDirectionalLightMaxContribution.value;
                    mainDirectionalLightMaxContributionColor = charRenderingControlVolumeEffect.mainDirectionalLightMaxContributionColor.value;

                    // additional light
                    additionalLightIntensityMultiplierColor = charRenderingControlVolumeEffect.additionalLightIntensityMultiplier.value * charRenderingControlVolumeEffect.additionalLightIntensityMultiplierColor.value;
                    additionalLightMaxContribution = charRenderingControlVolumeEffect.additionalLightMaxContribution.value;
                    additionalLightMaxContributionColor = charRenderingControlVolumeEffect.additionalLightMaxContributionColor.value;

                    // specular
                    specularIntensityMultiplier = charRenderingControlVolumeEffect.specularIntensityMultiplier.value;
                    specularInShadowMinIntensity = charRenderingControlVolumeEffect.specularInShadowMinIntensity.value;
                    specularReactToLightDirectionChange = charRenderingControlVolumeEffect.specularReactToLightDirectionChange.value;

                    // rim light
                    rimLightMultiplier *= charRenderingControlVolumeEffect.charRimLightMultiplier.value * charRenderingControlVolumeEffect.charRimLightTintColor.value;
                    rimLightCameraDistanceFadeoutStartDistance = charRenderingControlVolumeEffect.charRimLightCameraDistanceFadeoutStartDistance.value;
                    rimLightCameraDistanceFadeoutEndDistance = charRenderingControlVolumeEffect.charRimLightCameraDistanceFadeoutEndDistance.value;
                }
                cmd.SetGlobalColor(_GlobalVolumeBaseColorTintColor_SID, baseColorTintResult);
                cmd.SetGlobalColor(_GlobalVolumeMulColor_SID, mulColorResult);
                cmd.SetGlobalColor(_GlobalVolumeLerpColor_SID, lerpColorResult);
                cmd.SetGlobalFloat(_GlobalOcclusionStrength_SID, occlusionUsage);
                cmd.SetGlobalFloat(_GlobalIndirectLightMultiplier_SID, indirectLightMultiplier);
                cmd.SetGlobalColor(_GlobalMainDirectionalLightMultiplier_SID, mainDirectionalLightIntensityMultiplierColor);
                cmd.SetGlobalColor(_GlobalMainDirectionalLightMaxContribution_SID, mainDirectionalLightMaxContribution * mainDirectionalLightMaxContributionColor);
                cmd.SetGlobalColor(_GlobalAdditionalLightMultiplier_SID, additionalLightIntensityMultiplierColor);
                cmd.SetGlobalColor(_GlobalAdditionalLightMaxContribution_SID, additionalLightMaxContribution * additionalLightMaxContributionColor);
                cmd.SetGlobalColor(_GlobalIndirectLightMinColor_SID, indirectLightMinColor);
                cmd.SetGlobalColor(_GlobalRimLightMultiplier_SID, rimLightMultiplier);
                cmd.SetGlobalFloat(_GlobalDepthTexRimLightCameraDistanceFadeoutStartDistance_SID, rimLightCameraDistanceFadeoutStartDistance);
                cmd.SetGlobalFloat(_GlobalDepthTexRimLightCameraDistanceFadeoutEndDistance_SID, rimLightCameraDistanceFadeoutEndDistance);
                cmd.SetGlobalFloat(_GlobalSpecularIntensityMultiplier_SID, specularIntensityMultiplier);
                cmd.SetGlobalFloat(_GlobalSpecularInShadowMinIntensity_SID, specularInShadowMinIntensity);
                cmd.SetGlobalFloat(_GlobalSpecularReactToLightDirectionChange_SID, specularReactToLightDirectionChange ? 1 : 0);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // global volume (NiloToonEnvironmentControlVolume)
                //////////////////////////////////////////////////////////////////////////////////////////////////
                cmd.SetGlobalColor(_NiloToonGlobalEnviGITintColor_SID, environmentControlVolumeEffect.GlobalIlluminationTintColor.value);
                cmd.SetGlobalColor(_NiloToonGlobalEnviGIAddColor_SID, environmentControlVolumeEffect.GlobalIlluminationAddColor.value);
                cmd.SetGlobalColor(_NiloToonGlobalEnviGIOverride_SID, environmentControlVolumeEffect.GlobalIlluminationOverrideColor.value);
                cmd.SetGlobalColor(_NiloToonGlobalEnviAlbedoOverrideColor_SID, environmentControlVolumeEffect.GlobalAlbedoOverrideColor.value);
                cmd.SetGlobalFloat(_NiloToonGlobalEnviMinimumShader_SID, Setting.ForceMinimumEnviShader ? 1 : 0);
                cmd.SetGlobalColor(_NiloToonGlobalEnviShadowBorderTintColor_SID, environmentControlVolumeEffect.GlobalShadowBoaderTintColorOverrideColor.value);
                cmd.SetGlobalColor(_NiloToonGlobalEnviSurfaceColorResultOverrideColor_SID, environmentControlVolumeEffect.GlobalSurfaceColorResultOverrideColor.value);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // END
                //////////////////////////////////////////////////////////////////////////////////////////////////
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}