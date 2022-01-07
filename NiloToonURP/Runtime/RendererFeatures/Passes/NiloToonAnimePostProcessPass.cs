using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace NiloToon.NiloToonURP
{
    public class NiloToonAnimePostProcessPass : ScriptableRenderPass
    {
        // singleton
        public static NiloToonAnimePostProcessPass Instance
        {
            get => _instance;
        }
        static NiloToonAnimePostProcessPass _instance;

        [Serializable]
        public class Settings
        {
            [Tooltip("can turn off to improve performance for low quality graphics setting")]
            public bool allowRender = true;
        }
        public Settings settings { get; }

        Material material;
        NiloToonRendererFeatureSettings allSettings;
        ProfilingSampler m_ProfilingSampler;

        public NiloToonAnimePostProcessPass(NiloToonRendererFeatureSettings allSettings)
        {
            this.allSettings = allSettings;
            settings = allSettings.animePostProcessSettings;
            _instance = this;
            m_ProfilingSampler = new ProfilingSampler("NiloToonAnimePostProcessPass");
        }

#if UNITY_2020_1_OR_NEWER
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
#else
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
#endif
        {
            var animePP = VolumeManager.instance.stack.GetComponent<NiloToonAnimePostProcessVolume>();
            if (animePP.drawBeforePostProcess.value)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }
            else
            {
                // force render after "everything except UI", make it render correctly even FXAA on, draw on default frame buffer(back buffer) directly
                renderPassEvent = RenderPassEvent.AfterRendering + 2;
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Render(context, ref renderingData);
        }

        private void Render(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // we only want to render anime postprocess to Game window
            bool isNotGameCamera;
#if UNITY_2020_1_OR_NEWER
            isNotGameCamera = renderingData.cameraData.cameraType != CameraType.Game;
#else
            isNotGameCamera = renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera;
#endif
            if (isNotGameCamera) return;
            //===================================================================

            var animePP = VolumeManager.instance.stack.GetComponent<NiloToonAnimePostProcessVolume>();

            if (!settings.allowRender || !animePP.IsActive()) return;

            // if XR, don't render until we find a correct way to render
            bool shouldDraw = !XRSettings.isDeviceActive;

            if (!shouldDraw)
            {
                return;
            }

            // delay CreateEngineMaterial to as late as possible, to make it safe when ReimportAll is running
            if (!material)
                material = CoreUtils.CreateEngineMaterial("Hidden/NiloToon/AnimePostProcess");

            float topLightEffectIntensity = animePP.topLightEffectIntensity.value * animePP.intensity.value;
            float bottomDarkenEffectIntensity = animePP.bottomDarkenEffectIntensity.value * animePP.intensity.value;
            material.SetFloat("_TopLightIntensity", topLightEffectIntensity);
            material.SetFloat("_TopLightDesaturate", animePP.topLightDesaturate.value);
            material.SetColor("_TopLightTintColor", animePP.topLightTintColor.value);
            material.SetFloat("_TopLightDrawAreaHeight", animePP.topLightEffectDrawHeight.value);
            material.SetFloat("_BottomDarkenIntensity", bottomDarkenEffectIntensity);
            material.SetFloat("_BottomDarkenDrawAreaHeight", animePP.bottomDarkenEffectDrawHeight.value);

            // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
            // Currently there's an issue which results in mismatched markers.
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // about how to draw a full screen quad without RT switch:
                // https://gist.github.com/phi-lira/46c98fc67640cda47dcd27e9b3765b85#file-fullscreenquadpass-cs-L23

                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity); // set V,P to identity matrix so we can draw full screen quad (mesh's vertex position used as final NDC position)

                // optimization: only draw if it is affecting result
                if (topLightEffectIntensity > 0)
                {
                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, 0, 0); // pass 0, top light pass
                }
                if (bottomDarkenEffectIntensity > 0)
                {
                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, 0, 1); // pass 1, bottom darken pass
                }

                cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix); // restore
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

#if UNITY_2020_1_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
        }
    }
}