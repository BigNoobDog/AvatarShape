// URP' official bloom's logic note
/*
// RT note (all RT = B10G11R11_UFloatPack32)
_CameraColorTexture =	full resolution
_BloomMipDown0 = 	1/2 ressolution
_BloomMipUp1 = 		1/4 ressolution
_BloomMipDown1 = 	1/4 ressolution
_BloomMipUp2 = 		1/8 ressolution
_BloomMipDown2 = 	1/8 ressolution
...

// pass note
Bloom.shader - Bloom Prefilter          = sample _CameraColorTexture(HQ = get more near samples, average) -> user clamp(min()) -> threshold -> EncodeHDR() -> return
Bloom.shader - Bloom Blur Horizontal    = sample 9 taps gaussin blur -> EncodeHDR() -> return
Bloom.shader - Bloom Blur Vertical      = sample 5 taps gaussin blur -> EncodeHDR() -> return
Bloom.shader - Bloom Upsample           = sample _SourceTex and _SourceTexLowMip, then combine them by lerp(a,b,t) -> return

// render step
1.RT _CameraColorTexture render done¡Aresolve
----------------------------------------
2.render RT _BloomMipDown0	(set _CameraColorTexture as _SourceTex)(Bloom.shader - Bloom Prefilter)
----------------------------------------
3.render RT _BloomMipUp1	(set _BloomMipDown0 as _SourceTex)(Bloom.shader - Bloom Blur Horizontal)
4.render RT _BloomMipDown1	(set _BloomMipUp1 as _SourceTex)(Bloom.shader - Bloom Blur Vertical)
5.render RT _BloomMipUp2	(set _BloomMipDown1 as _SourceTex)(Bloom.shader - Bloom Blur Horizontal)
6.render RT _BloomMipDown2	(set _BloomMipUp2 as _SourceTex)(Bloom.shader - Bloom Blur Vertical)
....
7.render RT _BloomMipUp8	(set _BloomMipDown7 as _SourceTex)(Bloom.shader - Bloom Blur Horizontal)
8.render RT _BloomMipDown8	(set _BloomMipUp8 as _SourceTex)(Bloom.shader - Bloom Blur Vertical)
----------------------------------------
9.render RT _BloomMipUp7	(set _BloomMipDown7 as _SourceTex, _BloomMipDown8 as _SourceTexLowMip)(Bloom.shader - Bloom Upsample)
----------------------------------------
10.render RT _BloomMipUp6	(set _BloomMipDown6 as _SourceTex, _BloomMipUp7 as _SourceTexLowMip)(Bloom.shader - Bloom Upsample)
11.render RT _BloomMipUp5	(set _BloomMipDown5 as _SourceTex, _BloomMipUp6 as _SourceTexLowMip)(Bloom.shader - Bloom Upsample)
12.render RT _BloomMipUp4	(set _BloomMipDown4 as _SourceTex, _BloomMipUp5 as _SourceTexLowMip)(Bloom.shader - Bloom Upsample)
...
13.render RT _BloomMipUp0	(set _BloomMipDown0 as _SourceTex, _BloomMipUp1 as _SourceTexLowMip)(Bloom.shader - Bloom Upsample)
----------------------------------------
14.render to <No name>		(set _CameraColorTex as _SourceTex¡A_BloomMipUp0 as _BloomTexture, do LUT) (UberPost.shader - UberPost) (keyword = _BLOOM_LQ only)
(do ApplyColorGrading(_CameraColorTex + _BloomMipUp0 * BloomTint * BloomIntensity))
(so custom postprocess ping pong +bloom result, is correct)
*/

using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace NiloToon.NiloToonURP
{
    public class NiloToonUberPostProcessPass : ScriptableRenderPass
    {
        #region Singleton
        public static NiloToonUberPostProcessPass Instance
        {
            get => _instance;
        }
        static NiloToonUberPostProcessPass _instance;
        #endregion

        ProfilingSampler niloToonBloomProfileSampler;
        ProfilingSampler niloToonUberProfileSampler;
        #region [Copy and edited, from URP10.5.0's PostProcessPass.cs's fields]
        const string k_RenderPostProcessingTag = "Render NiloToon PostProcessing Effects"; // edited string value, original is "Render PostProcessing Effects"
        NiloToonBloomVolume m_Bloom; // edited type, original is Bloom
        #endregion

        #region [Direct copy and NO edit, from URP10.5.0's PostProcessPass.cs's fields]
        RenderTextureDescriptor m_Descriptor;
        RenderTargetHandle m_Source;

        private static readonly ProfilingSampler m_ProfilingRenderPostProcessing = new ProfilingSampler(k_RenderPostProcessingTag);

        MaterialLibrary m_Materials;

        // Misc
        const int k_MaxPyramidSize = 16;
        readonly GraphicsFormat m_DefaultHDRFormat;
        bool m_UseRGBM;

        // Option to use procedural draw instead of cmd.blit
        bool m_UseDrawProcedural;
        #endregion

        public NiloToonUberPostProcessPass(NiloToonRendererFeatureSettings allSettings)
        {
            _instance = this;

            niloToonBloomProfileSampler = new ProfilingSampler("NiloToonBloomProfileSampler");
            niloToonUberProfileSampler = new ProfilingSampler("NiloToonUberProfileSampler");

            Shader bloomShader = Shader.Find("Hidden/Universal Render Pipeline/NiloToonBloom");
            Shader uberShader = Shader.Find("Hidden/Universal Render Pipeline/NiloToonUberPost");
            Shader blitShader = Shader.Find("Shaders/Utils/Blit.shader");

            #region [Copy and edited, from URP10.5.0's PostProcessPass.cs's constructor PostProcessPass(...)]
            // NiloToon add UNITY_2020_1_OR_NEWER macro:
#if UNITY_2020_1_OR_NEWER
            base.profilingSampler = new ProfilingSampler(nameof(NiloToonUberPostProcessPass)); // edited type, original is PostProcessPass
#endif
            m_Materials = new MaterialLibrary(bloomShader, uberShader, blitShader); // edited input param, original is a PostProcessData class instance
            #endregion

            #region [Copy and remove unneeded, keep bloom only, from URP10.5.0's PostProcessPass.cs's constructor PostProcessPass(...)]
            // Texture format pre-lookup
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Linear | FormatUsage.Render))
            {
                m_DefaultHDRFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                m_UseRGBM = false;
            }
            else
            {
                m_DefaultHDRFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
                m_UseRGBM = true;
            }

            // Bloom pyramid shader ids - can't use a simple stackalloc in the bloom function as we
            // unfortunately need to allocate strings
            ShaderConstants._BloomMipUp = new int[k_MaxPyramidSize];
            ShaderConstants._BloomMipDown = new int[k_MaxPyramidSize];
            #endregion

            #region [Copy and edited, from URP10.5.0's PostProcessPass.cs's constructor PostProcessPass(...)]
            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                ShaderConstants._BloomMipUp[i] = Shader.PropertyToID("_NiloToonBloomMipUp" + i); // edited string name, original is _BloomMipUp
                ShaderConstants._BloomMipDown[i] = Shader.PropertyToID("_NiloToonBloomMipDown" + i); // edited string name, original is _BloomMipDown
            }
            #endregion
        }

        #region [Copy and edited, from URP10.5.0's PostProcessPass.cs's Setup(...)]
        public void Setup(in RenderTextureDescriptor baseDescriptor, in RenderTargetHandle source)
        {
            m_Descriptor = baseDescriptor;
            m_Descriptor.useMipMap = false;
            m_Descriptor.autoGenerateMips = false;
            m_Source = source;

            // NiloToonURP removed
            /*
            m_Destination = destination;
            m_Depth = depth;
            m_InternalLut = internalLut;
            m_IsFinalPass = false;
            m_HasFinalPass = hasFinalPass;
            m_EnableSRGBConversionIfNeeded = enableSRGBConversion;
            */
        }
        #endregion

#if UNITY_2020_1_OR_NEWER
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
#else
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
#endif
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            #region [Copy and edited, from URP10.5.0's PostProcessPass.cs's Execute(...)]
            // Start by pre-fetching all builtin effect settings we need
            // Some of the color-grading settings are only used in the color grading lut pass
            var stack = VolumeManager.instance.stack;
            m_Bloom = stack.GetComponent<NiloToonBloomVolume>(); // edited type, original is Bloom

            // NiloToon add UNITY_2020_1_OR_NEWER macro:
#if UNITY_2020_1_OR_NEWER
            m_UseDrawProcedural = renderingData.cameraData.xrRendering; // edited, original is renderingData.cameraData.xr
#endif
            m_UseDrawProcedural = false; // TEMP disabled to make Blit() works
            #endregion

            #region [Direct copy and no edit, from URP10.5.0's PostProcessPass.cs's Execute(...)]

            // Regular render path (not on-tile) - we do everything in a single command buffer as it
            // makes it easier to manage temporary targets' lifetime
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingRenderPostProcessing))
            {
                Render(cmd, ref renderingData);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            #endregion
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            #region [copy and edited, from URP10.5.0's PostProcessPass.cs's Render()]
            ref var cameraData = ref renderingData.cameraData;

            int source = m_Source.id;

            // Utilities to simplify intermediate target management
            int GetSource() => source;

            // Setup projection matrix for cmd.DrawMesh()
            cmd.SetGlobalMatrix(ShaderConstants._FullscreenProjMat, GL.GetGPUProjectionMatrix(Matrix4x4.identity, true));

            // Combined post-processing stack
            using (new ProfilingScope(cmd, niloToonUberProfileSampler)) // NiloToon edit: original = ProfilingSampler.Get(URPProfileId.UberPostProcess)
            {
                // Bloom goes first
                bool bloomActive = m_Bloom.IsActive();
                if (bloomActive)
                {
                    using (new ProfilingScope(cmd, niloToonBloomProfileSampler)) // NiloToon edit: ProfilingSampler.Get(URPProfileId.Bloom)
                        SetupBloom(cmd, GetSource(), m_Materials.uber);
                }

                bool uberNeeded = bloomActive;
                if(uberNeeded)
                {
                    // 1.Blit direct copy _CameraColorTexture to _NiloToonUberTempRT
                    RenderTextureDescriptor rtdTempRT = renderingData.cameraData.cameraTargetDescriptor;
                    rtdTempRT.msaaSamples = 1; // no need MSAA for just a blit
                    cmd.GetTemporaryRT(Shader.PropertyToID("_NiloToonUberTempRT"), rtdTempRT, FilterMode.Point);
                    Blit(cmd, new RenderTargetIdentifier(GetSource()), new RenderTargetIdentifier("_NiloToonUberTempRT"), m_Materials.blit);

                    // 2.Blit _NiloToonUberTempRT to _CameraColorTexture, do uber postprocess
                    Blit(cmd, new RenderTargetIdentifier("_NiloToonUberTempRT"), new RenderTargetIdentifier(GetSource()), m_Materials.uber);

                    // 3.Cleanup
                    cmd.ReleaseTemporaryRT(Shader.PropertyToID("_NiloToonUberTempRT"));
                }

                // 3.Cleanup
                if (bloomActive)
                    cmd.ReleaseTemporaryRT(ShaderConstants._BloomMipUp[0]);
            }
            #endregion
        }
        #region [Copy and edited, from URP10.5.0's PostProcessPass.cs's SetupBloom()]
        // (all changes will be marked by a [NiloToon edited] tag)
        // (if there is no [NiloToon edited] tag, then it is a direct copy
        void SetupBloom(CommandBuffer cmd, int source, Material uberMaterial)
        {
            // [NiloToon edited]
            //===========================================================================================================
            // URP's official bloom will have very different bloom result depending on game window size(height), because blur is applied to constant count of pixels(4+1+4).
            // NiloToon's bloom can override to use a fixed size height instead of URP's "Start at half-res", to make all screen resolution's bloom result consistant.

            int th;
            int tw;
            if(m_Bloom.renderTextureOverridedToFixedHeight.overrideState)
            {
                // NiloToon's bloom code
                th = m_Bloom.renderTextureOverridedToFixedHeight.value;
                tw = (int)(th * ((float)m_Descriptor.width / (float)m_Descriptor.height));
            }
            else
            {
                // URP's official bloom code
                tw = m_Descriptor.width >> 1;
                th = m_Descriptor.height >> 1;
            }
            //===========================================================================================================

            // Determine the iteration count
            int maxSize = Mathf.Max(tw, th);
            int iterations = Mathf.FloorToInt(Mathf.Log(maxSize, 2f) - 1);
            iterations -= m_Bloom.skipIterations.value;
            int mipCount = Mathf.Clamp(iterations, 1, k_MaxPyramidSize);

            // Pre-filtering parameters
            float clamp = m_Bloom.clamp.value;
            float threshold = Mathf.GammaToLinearSpace(m_Bloom.threshold.value);
            float thresholdKnee = threshold * 0.5f; // Hardcoded soft knee

            // Material setup
            float scatter = Mathf.Lerp(0.05f, 0.95f, m_Bloom.scatter.value);
            var bloomMaterial = m_Materials.bloom;
            bloomMaterial.SetVector(ShaderConstants._Params, new Vector4(scatter, clamp, threshold, thresholdKnee));
            CoreUtils.SetKeyword(bloomMaterial, ShaderKeywordStrings.BloomHQ, m_Bloom.highQualityFiltering.value);
            CoreUtils.SetKeyword(bloomMaterial, ShaderKeywordStrings.UseRGBM, m_UseRGBM);

            // [NiloToon added]
            //====================================================================================
            // if not overrided, use generic threshold.
            // if overrided, use characterAreaOverridedThreshold.
            float finalThreshold = Mathf.GammaToLinearSpace(m_Bloom.characterAreaOverridedThreshold.overrideState ? m_Bloom.characterAreaOverridedThreshold.value : m_Bloom.threshold.value);
            float finalThresholdKnee = finalThreshold * 0.5f; // Hardcoded soft knee
            bloomMaterial.SetFloat("_NiloToonBloomCharacterAreaThreshold", finalThreshold);
            bloomMaterial.SetFloat("_NiloToonBloomCharacterAreaThresholdKnee", finalThresholdKnee);
            //====================================================================================

            // Prefilter
            var desc = GetCompatibleDescriptor(tw, th, m_DefaultHDRFormat);
            cmd.GetTemporaryRT(ShaderConstants._BloomMipDown[0], desc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderConstants._BloomMipUp[0], desc, FilterMode.Bilinear);
            Blit(cmd, source, ShaderConstants._BloomMipDown[0], bloomMaterial, 0);

            // Downsample - gaussian pyramid
            int lastDown = ShaderConstants._BloomMipDown[0];
            for (int i = 1; i < mipCount; i++)
            {
                tw = Mathf.Max(1, tw >> 1);
                th = Mathf.Max(1, th >> 1);
                int mipDown = ShaderConstants._BloomMipDown[i];
                int mipUp = ShaderConstants._BloomMipUp[i];

                desc.width = tw;
                desc.height = th;

                cmd.GetTemporaryRT(mipDown, desc, FilterMode.Bilinear);
                cmd.GetTemporaryRT(mipUp, desc, FilterMode.Bilinear);

                // Classic two pass gaussian blur - use mipUp as a temporary target
                //   First pass does 2x downsampling + 9-tap gaussian
                //   Second pass does 9-tap gaussian using a 5-tap filter + bilinear filtering
                Blit(cmd, lastDown, mipUp, bloomMaterial, 1);
                Blit(cmd, mipUp, mipDown, bloomMaterial, 2);

                lastDown = mipDown;
            }

            // Upsample (bilinear by default, HQ filtering does bicubic instead
            for (int i = mipCount - 2; i >= 0; i--)
            {
                int lowMip = (i == mipCount - 2) ? ShaderConstants._BloomMipDown[i + 1] : ShaderConstants._BloomMipUp[i + 1];
                int highMip = ShaderConstants._BloomMipDown[i];
                int dst = ShaderConstants._BloomMipUp[i];

                cmd.SetGlobalTexture(ShaderConstants._SourceTexLowMip, lowMip);
                Blit(cmd, highMip, BlitDstDiscardContent(cmd, dst), bloomMaterial, 3);
            }

            // Cleanup
            for (int i = 0; i < mipCount; i++)
            {
                cmd.ReleaseTemporaryRT(ShaderConstants._BloomMipDown[i]);
                if (i > 0) cmd.ReleaseTemporaryRT(ShaderConstants._BloomMipUp[i]);
            }

            // Setup bloom on uber
            var tint = m_Bloom.tint.value.linear;
            var luma = ColorUtils.Luminance(tint);
            tint = luma > 0f ? tint * (1f / luma) : Color.white;

            var bloomParams = new Vector4(m_Bloom.intensity.value, tint.r, tint.g, tint.b);
            uberMaterial.SetVector(ShaderConstants._Bloom_Params, bloomParams);
            uberMaterial.SetFloat(ShaderConstants._Bloom_RGBM, m_UseRGBM ? 1f : 0f);

            // [NiloToon added]
            //====================================================================================
            // if not overrided, use generic intensity.
            // if overrided, use characterAreaOverridedIntensity.
            uberMaterial.SetFloat("_NiloToonBloomCharacterAreaIntensity", m_Bloom.characterAreaOverridedIntensity.overrideState ? m_Bloom.characterAreaOverridedIntensity.value : m_Bloom.intensity.value);
            //====================================================================================

            cmd.SetGlobalTexture(ShaderConstants._Bloom_Texture, ShaderConstants._BloomMipUp[0]);

            // Setup lens dirtiness on uber
            // Keep the aspect ratio correct & center the dirt texture, we don't want it to be
            // stretched or squashed
            var dirtTexture = m_Bloom.dirtTexture.value == null ? Texture2D.blackTexture : m_Bloom.dirtTexture.value;
            float dirtRatio = dirtTexture.width / (float)dirtTexture.height;
            float screenRatio = m_Descriptor.width / (float)m_Descriptor.height;
            var dirtScaleOffset = new Vector4(1f, 1f, 0f, 0f);
            float dirtIntensity = m_Bloom.dirtIntensity.value;

            if (dirtRatio > screenRatio)
            {
                dirtScaleOffset.x = screenRatio / dirtRatio;
                dirtScaleOffset.z = (1f - dirtScaleOffset.x) * 0.5f;
            }
            else if (screenRatio > dirtRatio)
            {
                dirtScaleOffset.y = dirtRatio / screenRatio;
                dirtScaleOffset.w = (1f - dirtScaleOffset.y) * 0.5f;
            }

            uberMaterial.SetVector(ShaderConstants._LensDirt_Params, dirtScaleOffset);
            uberMaterial.SetFloat(ShaderConstants._LensDirt_Intensity, dirtIntensity);
            uberMaterial.SetTexture(ShaderConstants._LensDirt_Texture, dirtTexture);

            // Keyword setup - a bit convoluted as we're trying to save some variants in Uber...
            if (m_Bloom.highQualityFiltering.value)
                uberMaterial.EnableKeyword(dirtIntensity > 0f ? ShaderKeywordStrings.BloomHQDirt : ShaderKeywordStrings.BloomHQ);
            else
                uberMaterial.EnableKeyword(dirtIntensity > 0f ? ShaderKeywordStrings.BloomLQDirt : ShaderKeywordStrings.BloomLQ);
        }
        #endregion

        #region [Copy and remove unneeded and edited, keep bloom and uber only, add blit, delay material create, from URP10.5.0's PostProcessPass.cs's MaterialLibrary class]
        class MaterialLibrary
        {
            public readonly Material bloom;
            public readonly Material uber;
            public readonly Material blit;

            // NiloToon edited, instead of receiving a PostProcessData class, we pass 2 Shaders directly
            public MaterialLibrary(Shader bloomShader, Shader uberShader, Shader blitShader)
            {
                bloom = Load(bloomShader);
                uber = Load(uberShader);
                blit = Load(blitShader);
            }

            Material Load(Shader shader)
            {
                if (shader == null)
                {
                    //Debug.LogErrorFormat($"Missing shader. {GetType().DeclaringType.Name} render pass will not execute. Check for missing reference in the renderer resources."); // NiloToonURP: removed
                    return null;
                }
                else if (!shader.isSupported)
                {
                    return null;
                }

                return CoreUtils.CreateEngineMaterial(shader);
            }

            internal void Cleanup()
            {
                CoreUtils.Destroy(bloom);
                CoreUtils.Destroy(uber);
                CoreUtils.Destroy(blit);
            }
        }
        #endregion

        #region [Direct copy and no edit, from URP10.5.0's PostProcessPass.cs]
        RenderTextureDescriptor GetCompatibleDescriptor()
            => GetCompatibleDescriptor(m_Descriptor.width, m_Descriptor.height, m_Descriptor.graphicsFormat, m_Descriptor.depthBufferBits);

        RenderTextureDescriptor GetCompatibleDescriptor(int width, int height, GraphicsFormat format, int depthBufferBits = 0)
        {
            var desc = m_Descriptor;
            desc.depthBufferBits = depthBufferBits;
            desc.msaaSamples = 1;
            desc.width = width;
            desc.height = height;
            desc.graphicsFormat = format;
            return desc;
        }

        private BuiltinRenderTextureType BlitDstDiscardContent(CommandBuffer cmd, RenderTargetIdentifier rt)
        {
            // We set depth to DontCare because rt might be the source of PostProcessing used as a temporary target
            // Source typically comes with a depth buffer and right now we don't have a way to only bind the color attachment of a RenderTargetIdentifier
            cmd.SetRenderTarget(new RenderTargetIdentifier(rt, 0, CubemapFace.Unknown, -1),
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
            return BuiltinRenderTextureType.CurrentActive;
        }
        
        private new void Blit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int passIndex = 0)
        {
            cmd.SetGlobalTexture(ShaderPropertyId.sourceTex, source);
            if (m_UseDrawProcedural)
            {
                Vector4 scaleBias = new Vector4(1, 1, 0, 0);
                cmd.SetGlobalVector(ShaderPropertyId.scaleBias, scaleBias);

                cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                    RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
                cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
            }
            else
            {
                cmd.Blit(source, destination, material, passIndex);
            }
        }
        

        // Precomputed shader ids to same some CPU cycles (mostly affects mobile)
        static class ShaderConstants
        {
            public static readonly int _TempTarget = Shader.PropertyToID("_TempTarget");
            public static readonly int _TempTarget2 = Shader.PropertyToID("_TempTarget2");

            public static readonly int _StencilRef = Shader.PropertyToID("_StencilRef");
            public static readonly int _StencilMask = Shader.PropertyToID("_StencilMask");

            public static readonly int _FullCoCTexture = Shader.PropertyToID("_FullCoCTexture");
            public static readonly int _HalfCoCTexture = Shader.PropertyToID("_HalfCoCTexture");
            public static readonly int _DofTexture = Shader.PropertyToID("_DofTexture");
            public static readonly int _CoCParams = Shader.PropertyToID("_CoCParams");
            public static readonly int _BokehKernel = Shader.PropertyToID("_BokehKernel");
            public static readonly int _PongTexture = Shader.PropertyToID("_PongTexture");
            public static readonly int _PingTexture = Shader.PropertyToID("_PingTexture");

            public static readonly int _Metrics = Shader.PropertyToID("_Metrics");
            public static readonly int _AreaTexture = Shader.PropertyToID("_AreaTexture");
            public static readonly int _SearchTexture = Shader.PropertyToID("_SearchTexture");
            public static readonly int _EdgeTexture = Shader.PropertyToID("_EdgeTexture");
            public static readonly int _BlendTexture = Shader.PropertyToID("_BlendTexture");

            public static readonly int _ColorTexture = Shader.PropertyToID("_ColorTexture");
            public static readonly int _Params = Shader.PropertyToID("_Params");
            public static readonly int _SourceTexLowMip = Shader.PropertyToID("_SourceTexLowMip");
            public static readonly int _Bloom_Params = Shader.PropertyToID("_Bloom_Params");
            public static readonly int _Bloom_RGBM = Shader.PropertyToID("_Bloom_RGBM");
            public static readonly int _Bloom_Texture = Shader.PropertyToID("_Bloom_Texture");
            public static readonly int _LensDirt_Texture = Shader.PropertyToID("_LensDirt_Texture");
            public static readonly int _LensDirt_Params = Shader.PropertyToID("_LensDirt_Params");
            public static readonly int _LensDirt_Intensity = Shader.PropertyToID("_LensDirt_Intensity");
            public static readonly int _Distortion_Params1 = Shader.PropertyToID("_Distortion_Params1");
            public static readonly int _Distortion_Params2 = Shader.PropertyToID("_Distortion_Params2");
            public static readonly int _Chroma_Params = Shader.PropertyToID("_Chroma_Params");
            public static readonly int _Vignette_Params1 = Shader.PropertyToID("_Vignette_Params1");
            public static readonly int _Vignette_Params2 = Shader.PropertyToID("_Vignette_Params2");
            public static readonly int _Lut_Params = Shader.PropertyToID("_Lut_Params");
            public static readonly int _UserLut_Params = Shader.PropertyToID("_UserLut_Params");
            public static readonly int _InternalLut = Shader.PropertyToID("_InternalLut");
            public static readonly int _UserLut = Shader.PropertyToID("_UserLut");
            public static readonly int _DownSampleScaleFactor = Shader.PropertyToID("_DownSampleScaleFactor");

            public static readonly int _FullscreenProjMat = Shader.PropertyToID("_FullscreenProjMat");

            public static int[] _BloomMipUp;
            public static int[] _BloomMipDown;
        }
        #endregion

        #region [Direct copy and no edit, from URP10.5.0's UniversalRenderPipelineCore.cs]
        internal static class ShaderPropertyId
        {
            public static readonly int glossyEnvironmentColor = Shader.PropertyToID("_GlossyEnvironmentColor");
            public static readonly int subtractiveShadowColor = Shader.PropertyToID("_SubtractiveShadowColor");

            public static readonly int ambientSkyColor = Shader.PropertyToID("unity_AmbientSky");
            public static readonly int ambientEquatorColor = Shader.PropertyToID("unity_AmbientEquator");
            public static readonly int ambientGroundColor = Shader.PropertyToID("unity_AmbientGround");

            public static readonly int time = Shader.PropertyToID("_Time");
            public static readonly int sinTime = Shader.PropertyToID("_SinTime");
            public static readonly int cosTime = Shader.PropertyToID("_CosTime");
            public static readonly int deltaTime = Shader.PropertyToID("unity_DeltaTime");
            public static readonly int timeParameters = Shader.PropertyToID("_TimeParameters");

            public static readonly int scaledScreenParams = Shader.PropertyToID("_ScaledScreenParams");
            public static readonly int worldSpaceCameraPos = Shader.PropertyToID("_WorldSpaceCameraPos");
            public static readonly int screenParams = Shader.PropertyToID("_ScreenParams");
            public static readonly int projectionParams = Shader.PropertyToID("_ProjectionParams");
            public static readonly int zBufferParams = Shader.PropertyToID("_ZBufferParams");
            public static readonly int orthoParams = Shader.PropertyToID("unity_OrthoParams");

            public static readonly int viewMatrix = Shader.PropertyToID("unity_MatrixV");
            public static readonly int projectionMatrix = Shader.PropertyToID("glstate_matrix_projection");
            public static readonly int viewAndProjectionMatrix = Shader.PropertyToID("unity_MatrixVP");

            public static readonly int inverseViewMatrix = Shader.PropertyToID("unity_MatrixInvV");
            public static readonly int inverseProjectionMatrix = Shader.PropertyToID("unity_MatrixInvP");
            public static readonly int inverseViewAndProjectionMatrix = Shader.PropertyToID("unity_MatrixInvVP");

            public static readonly int cameraProjectionMatrix = Shader.PropertyToID("unity_CameraProjection");
            public static readonly int inverseCameraProjectionMatrix = Shader.PropertyToID("unity_CameraInvProjection");
            public static readonly int worldToCameraMatrix = Shader.PropertyToID("unity_WorldToCamera");
            public static readonly int cameraToWorldMatrix = Shader.PropertyToID("unity_CameraToWorld");

            public static readonly int sourceTex = Shader.PropertyToID("_SourceTex");
            public static readonly int scaleBias = Shader.PropertyToID("_ScaleBias");
            public static readonly int scaleBiasRt = Shader.PropertyToID("_ScaleBiasRt");

            // Required for 2D Unlit Shadergraph master node as it doesn't currently support hidden properties.
            public static readonly int rendererColor = Shader.PropertyToID("_RendererColor");
        }
        #endregion
#if UNITY_2020_1_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
        }
    }
}