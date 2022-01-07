// very similar to URP's DepthOnlyPass.cs, but only draw NiloToon character shader's renderer, rendering shader's Lightmode = "NiloToonPrepassBuffer"
// this pass's on/off is handled by NiloToonAllInOneRendererFeature.cs's AddRenderPasses() function
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    public class NiloToonPrepassBufferRTPass : ScriptableRenderPass
    {
        NiloToonRendererFeatureSettings allSettings;

        RenderTargetHandle prepassBufferRTH;
        ProfilingSampler m_ProfilingSampler;

        // constructor
        public NiloToonPrepassBufferRTPass(NiloToonRendererFeatureSettings allSettings)
        {
            this.allSettings = allSettings;

            prepassBufferRTH.Init("_NiloToonPrepassBufferRT"); // this is RT's name, differrent to the texture name used in shader

            m_ProfilingSampler = new ProfilingSampler("NiloToonPrepassBufferRTPass");
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
#if UNITY_2020_1_OR_NEWER
            int width = renderingData.cameraData.cameraTargetDescriptor.width;
            int height = renderingData.cameraData.cameraTargetDescriptor.height;
#else
            int width = cameraTextureDescriptor.width;
            int height = cameraTextureDescriptor.height;
#endif
            RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 16);
            cmd.GetTemporaryRT(prepassBufferRTH.id, renderTextureDescriptor, FilterMode.Point);

            //set global RT
            cmd.SetGlobalTexture("_NiloToonPrepassBufferTex", prepassBufferRTH.Identifier()); // RT's name and global texture property name can't be the same?

            ConfigureTarget(new RenderTargetIdentifier(prepassBufferRTH.Identifier(),0, CubemapFace.Unknown, -1));
            ConfigureClear(ClearFlag.All, Color.black);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderCharacterPrepassBufferRT(context, renderingData);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
#if UNITY_2020_1_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
            cmd.ReleaseTemporaryRT(prepassBufferRTH.id);
        }

        bool shouldRender(NiloToonBloomVolume volumeEffect)
        {
            return volumeEffect.IsActive();
        }

        private void renderCharacterPrepassBufferRT(ScriptableRenderContext context, RenderingData renderingData)
        {
            // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
            // Currently there's an issue which results in mismatched markers.
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                var volumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonBloomVolume>();

                bool shouldRender = this.shouldRender(volumeEffect);

                if (shouldRender)
                {
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // draw all char renderer using SRP batching
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ShaderTagId shaderTagId = new ShaderTagId("NiloToonPrepassBuffer");
                    var drawSetting = CreateDrawingSettings(shaderTagId, ref renderingData, SortingCriteria.CommonOpaque);
                    var filterSetting = new FilteringSettings(RenderQueueRange.opaque);
                    context.DrawRenderers(renderingData.cullResults, ref drawSetting, ref filterSetting); // using custom cullResults from shadow camera's perspective, instead of main camera's cull result
                }

            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}