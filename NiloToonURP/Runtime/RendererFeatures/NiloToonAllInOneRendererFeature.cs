// An all in one high-level RendererFeature, user only need to add this RendererFeature in their renderer
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [Serializable]
    public class NiloToonRendererFeatureSettings
    {
        [Header("Outline settings")]
        public NiloToonToonOutlinePass.Settings outlineSettings = new NiloToonToonOutlinePass.Settings();
        [Header("Misc settings")]
        public NiloToonSetToonParamPass.Settings MiscSettings = new NiloToonSetToonParamPass.Settings();
        [Header("Sphere shadow test RT")]
        public NiloToonAverageShadowTestRTPass.Settings sphereShadowTestSettings = new NiloToonAverageShadowTestRTPass.Settings();
        [Header("Anime PostProcess")]
        public NiloToonAnimePostProcessPass.Settings animePostProcessSettings = new NiloToonAnimePostProcessPass.Settings();
        [Header("Char SelfShadow")]
        public NiloToonCharSelfShadowMapRTPass.Settings charSelfShadowSettings = new NiloToonCharSelfShadowMapRTPass.Settings();

        [Header("Override Shader stripping (optional, only useful if you need to reduce shader memory usage)")]
        public NiloToonShaderStrippingSettingSO shaderStrippingSettingSO;
    }

    // URP doesn't allow using [DisallowMultipleRendererFeature] below URP 11
    //[DisallowMultipleRendererFeature]
    public class NiloToonAllInOneRendererFeature : ScriptableRendererFeature
    {
        public NiloToonRendererFeatureSettings settings = new NiloToonRendererFeatureSettings();

        NiloToonSetToonParamPass SetToonParamPass;
        NiloToonAverageShadowTestRTPass SphereShadowTestRTPass;
        NiloToonCharSelfShadowMapRTPass CharSelfShadowMapRTRenderPass;
        NiloToonToonOutlinePass ToonOutlinePass;
        NiloToonAnimePostProcessPass AnimePostProcessPass;
        NiloToonPrepassBufferRTPass PrepassBufferRTPass;
        NiloToonUberPostProcessPass UberPostProcessPass;

        public override void Create()
        {
            SetToonParamPass = new NiloToonSetToonParamPass(settings);
            SphereShadowTestRTPass = new NiloToonAverageShadowTestRTPass(settings);
            CharSelfShadowMapRTRenderPass = new NiloToonCharSelfShadowMapRTPass(settings);
            ToonOutlinePass = new NiloToonToonOutlinePass(settings);
            AnimePostProcessPass = new NiloToonAnimePostProcessPass(settings);
            PrepassBufferRTPass = new NiloToonPrepassBufferRTPass(settings);
            UberPostProcessPass = new NiloToonUberPostProcessPass(settings);

            // Configures where the render pass should be injected.
            // sorted by RenderPassEvent order here
            SetToonParamPass.renderPassEvent = RenderPassEvent.BeforeRendering;
            SphereShadowTestRTPass.renderPassEvent = RenderPassEvent.AfterRenderingShadows;
            CharSelfShadowMapRTRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
            ToonOutlinePass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox; // use AfterRenderingSkybox instead of BeforeRenderingSkybox, to make semi-transparent outline blend with skybox correctly
            // AnimePostProcessPass's renderPassEvent will be decided by the pass itslef
            PrepassBufferRTPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses; // require after _CameraDepthTexture, since character shader's NiloToonPrepassBuffer pass needs _CameraDepthTexture
            UberPostProcessPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor; // copy from URP's ForwardRenderer.cs's SetUp()
            RenderTargetHandle cameraTarget = new RenderTargetHandle(); // to support Unity2019 (URP7), we can't pass in renderingData.cameraData.targetTexture
            cameraTarget.Init("_CameraColorTexture");
            UberPostProcessPass.Setup(cameraTargetDescriptor, cameraTarget);

            renderer.EnqueuePass(SetToonParamPass);
            renderer.EnqueuePass(SphereShadowTestRTPass);
            renderer.EnqueuePass(CharSelfShadowMapRTRenderPass);
            renderer.EnqueuePass(ToonOutlinePass);
            renderer.EnqueuePass(AnimePostProcessPass);

            var bloomEffect = VolumeManager.instance.stack.GetComponent<NiloToonBloomVolume>();
            if (bloomEffect.IsActive())
                renderer.EnqueuePass(PrepassBufferRTPass);

            renderer.EnqueuePass(UberPostProcessPass);
        }


        #region [Singleton to store list of active char (no matter visible by camera or not)]
        public NiloToonAllInOneRendererFeature()
        {
            CheckInit();
            _instance = this;
        }
        public static NiloToonAllInOneRendererFeature Instance
        {
            get => _instance;
        }
        static NiloToonAllInOneRendererFeature _instance;

        [NonSerialized]
        public List<NiloToonPerCharacterRenderController> characterList;
        [NonSerialized]
        public HashSet<NiloToonPerCharacterRenderController> characterHashSet;

        public void AddCharIfNotExist(NiloToonPerCharacterRenderController controller)
        {
            CheckInit();

            // optimize .Contains() call, now use HashSet instead of List: https://stackoverflow.com/questions/823860/listt-contains-is-very-slow
            if (!characterHashSet.Contains(controller))
            {
                characterHashSet.Add(controller);
                characterList.Add(controller);
                UpdateCharacterControllerIndex();
            }
        }
        public void Remove(NiloToonPerCharacterRenderController controller)
        {
            CheckInit();

            // optimize .Contains() call, now use HashSet instead of List: https://stackoverflow.com/questions/823860/listt-contains-is-very-slow
            if (characterHashSet.Contains(controller))
            {
                characterHashSet.Remove(controller);
                characterList.Remove(controller);
                UpdateCharacterControllerIndex();
            }
        }
        void UpdateCharacterControllerIndex()
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                var c = characterList[i];
                if (c == null)
                    continue;

               c.shadowTestIndex = i;
            }
        }
        private void CheckInit()
        {
            if (characterList == null)
                characterList = new List<NiloToonPerCharacterRenderController>(); // for UpdateCharacterControllerIndex()
            if (characterHashSet == null)
                characterHashSet = new HashSet<NiloToonPerCharacterRenderController>(); // for .Contains() checks
        }
        #endregion
    }
}

