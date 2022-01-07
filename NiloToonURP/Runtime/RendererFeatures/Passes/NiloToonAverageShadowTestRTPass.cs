using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    public class NiloToonAverageShadowTestRTPass : ScriptableRenderPass
    {
        // singleton
        public static NiloToonAverageShadowTestRTPass Instance => _instance;
        static NiloToonAverageShadowTestRTPass _instance;

        // for most game type, 128 is a big enough number, but still not affecting performance
        // maximum 127 nilotoon characters can be inside the same scene, the last slot is reserved for camera's anime postprocess 
        const int MAX_SHADOW_SLOT_COUNT = 128;

        RenderTargetHandle shadowTestResultRTH;
        ProfilingSampler m_ProfilingSampler;

        static readonly int _SphereShadowMapRT_SID = Shader.PropertyToID("_SphereShadowMapRT");

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
            // RT width: use MAX_SHADOW_SLOT_COUNT as RT width, each pixel represent 1 nilotton character's average shadow(the last pixel is camera's average shadow)
            // RT height: RT height is 1
            // RTFormat: RT format is RFloat, because we want to store a 0~1 average shadowAttenuation value
            // don't need depthbuffer/stencil/mipmap
            RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(MAX_SHADOW_SLOT_COUNT, 1, RenderTextureFormat.RFloat, 0, 1);

            // it is linear data
            renderTextureDescriptor.sRGB = false;
            // we need each pixel providing a per character average shadow attenuation value when sampling, so FilterMode is Point
            cmd.GetTemporaryRT(shadowTestResultRTH.id, renderTextureDescriptor, FilterMode.Point);
            cmd.SetGlobalTexture(_SphereShadowMapRT_SID, shadowTestResultRTH.Identifier());

            // still need to clear RT to white even average shadow is not enabled 
            // (in character shader we removed average shadow's multi_compile to save memory & build time/size, so character shader is always sampling this RT)
            ConfigureTarget(shadowTestResultRTH.Identifier());
            ConfigureClear(ClearFlag.Color, Color.white); // default white = default no shadow
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderPerCharacterAverageShadowAtlaRT(context, renderingData);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
#if UNITY_2020_1_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
            cmd.ReleaseTemporaryRT(shadowTestResultRTH.id);
        }

        [Serializable]
        public class Settings
        {
            [Tooltip("If you want character to receive average shadow casted by environment objects, turn it on")]
            public bool enableAverageShadow = true;
        }

        public Settings settings;

        Material material;
        float[] shaderDataArray;

        public NiloToonAverageShadowTestRTPass(NiloToonRendererFeatureSettings allSettings)
        {
            this.settings = allSettings.sphereShadowTestSettings;

            shadowTestResultRTH.Init("_NiloToonAverageShadowMapRT");
            shaderDataArray = new float[MAX_SHADOW_SLOT_COUNT * 4];

            _instance = this;

            m_ProfilingSampler = new ProfilingSampler("NiloToonAverageShadowTestRTPass");
        }

        private void renderPerCharacterAverageShadowAtlaRT(ScriptableRenderContext context, RenderingData renderingData)
        {
            var shadowControlVolumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonShadowControlVolume>();

            bool enableAverageShadow = shadowControlVolumeEffect.enableCharAverageShadow.overrideState ? shadowControlVolumeEffect.enableCharAverageShadow.value : settings.enableAverageShadow;
            if (!enableAverageShadow) return;

            // delay CreateEngineMaterial to as late as possible, to make it safe when ReimportAll is running
            if (!material)
                material = CoreUtils.CreateEngineMaterial("Hidden/NiloToon/AverageShadowTestRT");

            // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
            // Currently there's an issue which results in mismatched markers.
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // reserve the right most slot for camera, other slots for each character
                for (int i = 0; i < Mathf.Min(MAX_SHADOW_SLOT_COUNT - 1, NiloToonAllInOneRendererFeature.Instance.characterList.Count); i++)
                {
                    NiloToonPerCharacterRenderController controller = NiloToonAllInOneRendererFeature.Instance.characterList[i];

                    if (controller)
                    {
                        Vector3 centerPosWS = controller.GetCharacterBoundCenter();
                        float radiusWS = controller.GetCharacterBoundRadius();

                        shaderDataArray[i * 4 + 0] = centerPosWS.x;
                        shaderDataArray[i * 4 + 1] = centerPosWS.y;
                        shaderDataArray[i * 4 + 2] = centerPosWS.z;
                        shaderDataArray[i * 4 + 3] = radiusWS;
                    }
                    else
                    {
                        shaderDataArray[i * 4 + 3] = 0;
                    }
                }

                // RT's right most slot(pixel) for camera only
                Camera cam = renderingData.cameraData.camera;
                Vector3 cameraPosForTesting = cam.transform.position + cam.transform.forward * 0.1f; // add offset to not use cam pos center directly, to avoid wrong cascade shadowmap index
                shaderDataArray[(MAX_SHADOW_SLOT_COUNT - 1) * 4 + 0] = cameraPosForTesting.x;
                shaderDataArray[(MAX_SHADOW_SLOT_COUNT - 1) * 4 + 1] = cameraPosForTesting.y;
                shaderDataArray[(MAX_SHADOW_SLOT_COUNT - 1) * 4 + 2] = cameraPosForTesting.z;
                shaderDataArray[(MAX_SHADOW_SLOT_COUNT - 1) * 4 + 3] = 2;

                cmd.SetGlobalFloatArray("_GlobalAverageShadowTestBoundingSphereDataArray", shaderDataArray); // once set, we can't change the size of array in GPU anymore, it is not Unity's fault but graphics API's design.
                cmd.SetGlobalFloat("_GlobalAverageShadowStrength", shadowControlVolumeEffect.charAverageShadowStrength.value);
                cmd.Blit(null, shadowTestResultRTH.Identifier(), material);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}