// render a separated shadow map (Not related to URP's shadowmap), 
// only containing NiloToon characters, and shadow map orthographic box will fit/bound to NiloToon character only.
// this pass's shadow camera's orthographic view bound is very tight(because only NiloToon characters are included), 
// so a smaller size RT can still produce sharp shadow.

// Ideally we would want to render stencil Shadow volume to produce 100% sharp shadow without require high resolution RTs, 
// but because we don't know user's character model has crack or not, shadow volume is not a stable method for all users
// so we now use shadowmap to do the job

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NiloToon.NiloToonURP
{
    public class NiloToonCharSelfShadowMapRTPass : ScriptableRenderPass
    {
        // singleton
        public static NiloToonCharSelfShadowMapRTPass Instance => _instance;
        static NiloToonCharSelfShadowMapRTPass _instance;

        // settings
        [Serializable]
        public class Settings
        {
            public bool enableCharSelfShadow = true;

            [Header("Style")]
            [Tooltip("If OFF, will use currently active camera's forward direction(and apply with shadowAngle & shadowLRAngle also) as cast shadow direction.\n" +
                        "If ON, will use scene main directional light's forward direction as cast shadow direction, just like any regular shadowmap system.\n" +
                        "Turn it ON if you don't want this shadow affected by camera rotation")]
            public bool useMainLightAsCastShadowDirection = false;

            [Tooltip("Only useful if Use Main Light As Cast Shadow Direction is OFF")]
            [Range(-45, 45)]
            public float shadowAngle = 30f;
            [Tooltip("Only useful if Use Main Light As Cast Shadow Direction is OFF")]
            [Range(-45, 45)]
            public float shadowLRAngle = 0;

            [Header("Quality")]
            [Tooltip("The higher the better(shadow quality), but larger shadow map size = GPU slower")]
            [Range(512, 8192)]
            public int shadowMapSize = 2048;

            [Header("Fix shadow artifacts options")]
            [Tooltip("The shorter the range, the higher quality the shadow is, but objects outside the range will not have shadows")]
            [Range(10, 100)] // set minimum to 10 can hide some shadow problem
            public float shadowRange = 10;

            [Tooltip("The higher the bias, the less artifact(shadow acne) will appear, but more Peter panning will appear")]
            [Range(0, 4)]
            public float depthBias = 1;

            [Tooltip("Add an additional NdotL cel shading to hide shadowmap's artifact(shadow acne)")]
            public bool useNdotLFix = true;

            [Header("Fix unity crash")]
            [Tooltip("If having a terrain in your scene makes your unity crash, disable this toggle until URP/SRP fix it in the future")]
            public bool perfectCullingForShadowCasters = true;
        }
        public Settings settings { get; }

        [NonSerialized]
        public bool debugShadowCameraVisibleBox = false; // can enable it if debug is needed

        static readonly string _NILOTOON_RECEIVE_SELF_SHADOW_Keyword = "_NILOTOON_RECEIVE_SELF_SHADOW";

        NiloToonRendererFeatureSettings allSettings;
        Plane[] cameraPlanes = new Plane[6];
        RenderTargetHandle shadowMapRTH;
        ProfilingSampler m_ProfilingSampler;

        // constructor
        public NiloToonCharSelfShadowMapRTPass(NiloToonRendererFeatureSettings allSettings)
        {
            this.allSettings = allSettings;
            this.settings = allSettings.charSelfShadowSettings;
            _instance = this;

            shadowMapRTH = new RenderTargetHandle();
            shadowMapRTH.Init("_NiloToonCharSelfShadowMapRT");

            m_ProfilingSampler = new ProfilingSampler("NiloToonCharSelfShadowMapRTPass");
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
            var volumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonShadowControlVolume>();

            if (!getShouldRender(volumeEffect)) return;

            float shadowMapSize = getShadowMapSize(volumeEffect);

            // [TEMP fix]
            // if our shadowMapRTH's shadowMap size is the same as URP's shadow map RT(or any other URP's RT), URP's shadow will be buggy, not sure why.
            // we assume it is because Unity/URP try to share 2 RT that has the same RenderTextureDescriptor.
            // For now, we reduce shadow height by 1 pixel, to avoid our RT having the same RenderTextureDescriptor as URP's RT(which trigger bug) 
            // until we find out what the problem is
            RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor((int)shadowMapSize, (int)shadowMapSize - 1, RenderTextureFormat.Shadowmap, 16);
            cmd.GetTemporaryRT(shadowMapRTH.id, renderTextureDescriptor, FilterMode.Bilinear);
            cmd.SetGlobalTexture("_NiloToonCharSelfShadowMapRT", shadowMapRTH.Identifier());

            ConfigureTarget(shadowMapRTH.Identifier());
            ConfigureClear(ClearFlag.Depth, Color.black); // clearing color doesn't matter? since we will redraw character pixels with depth value (now default clear to far value [DX: near = 1, far = 0])
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderCharacterSelfShadowmapRT(context, renderingData);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
#if UNITY_2020_1_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
            cmd.ReleaseTemporaryRT(shadowMapRTH.id);
        }

        bool getShouldRender(NiloToonShadowControlVolume volumeEffect)
        {
            // let volume override settings if needed
            bool shouldRender = volumeEffect.enableCharSelfShadow.overrideState ? volumeEffect.enableCharSelfShadow.value : settings.enableCharSelfShadow;

            // if XR, don't render until we find a correct way to render
            if (XRSettings.isDeviceActive)
                shouldRender = false;

            return shouldRender;
        }
        float getShadowMapSize(NiloToonShadowControlVolume volumeEffect)
        {
            return volumeEffect.shadowMapSize.overrideState ? volumeEffect.shadowMapSize.value : settings.shadowMapSize;
        }

        private void renderCharacterSelfShadowmapRT(ScriptableRenderContext context, RenderingData renderingData)
        {
            // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
            // Currently there's an issue which results in mismatched markers.
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                var volumeEffect = VolumeManager.instance.stack.GetComponent<NiloToonShadowControlVolume>();

                bool shouldRender = getShouldRender(volumeEffect);

                if (shouldRender)
                {
                    // override settings if user overrided any of them in volume
                    // if user didn't override, we will get the value from renderer feature
                    bool useMainLightCastShadowDirection = volumeEffect.useMainLightAsCastShadowDirection.overrideState ? volumeEffect.useMainLightAsCastShadowDirection.value : settings.useMainLightAsCastShadowDirection;
                    float shadowAngle = volumeEffect.shadowAngle.overrideState ? volumeEffect.shadowAngle.value : settings.shadowAngle;
                    float shadowLRAngle = volumeEffect.shadowLRAngle.overrideState ? volumeEffect.shadowLRAngle.value : settings.shadowLRAngle;
                    float shadowRange = volumeEffect.shadowRange.overrideState ? volumeEffect.shadowRange.value : settings.shadowRange;
                    float shadowMapSize = getShadowMapSize(volumeEffect);
                    float depthBias = volumeEffect.depthBias.overrideState ? volumeEffect.depthBias.value : settings.depthBias;

                    Camera camera = renderingData.cameraData.camera;

                    int mainLightIndex = renderingData.lightData.mainLightIndex;
                    bool isMainLightExist = mainLightIndex != -1;

                    Matrix4x4 viewMatrix;

                    if (isMainLightExist && useMainLightCastShadowDirection)
                    {
                        // if we want to follow regular main light's shadow casting logic (same as URP's mainlight shadow map's light direction)
                        VisibleLight shadowLight = renderingData.lightData.visibleLights[mainLightIndex];
                        Light mainLight = shadowLight.light;
                        viewMatrix = mainLight.transform.worldToLocalMatrix;
                        viewMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0)) * viewMatrix;
                    }
                    else
                    {
                        // if we only care the rotation of camera, or main light doesn't exist in scene
                        viewMatrix = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(shadowAngle, shadowLRAngle, 0))) * camera.worldToCameraMatrix;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // auto close-fit to find "character only" tight bound shadow map ortho projection matrix 
                    // (smallest orthographic box that includes all effective shadow caster characters)
                    //
                    // TODO: 
                    // this section's method will incorrectly cull effective shadow caster that is OUTSIDE of main camera frustum
                    // (shadow caster outside of main camera frustum, can still affect shadow rendering of character INSIDE of main camera frustum,
                    // so should not cull them)
                    // see this: https://docs.microsoft.com/en-us/windows/win32/dxtecharts/common-techniques-to-improve-shadow-depth-maps
                    // 
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    float minX = Mathf.Infinity;
                    float maxX = Mathf.NegativeInfinity;
                    float minY = Mathf.Infinity;
                    float maxY = Mathf.NegativeInfinity;
                    float minZ = Mathf.Infinity;
                    float maxZ = Mathf.NegativeInfinity;

                    var allChar = NiloToonAllInOneRendererFeature.Instance.characterList;

                    int visibleCharCount = 0;
                    foreach (var targetChar in allChar)
                    {
                        // if target is not valid, skip it
                        if (targetChar == null) continue;
                        if (!targetChar.isActiveAndEnabled) continue; // character GameObject not enabled(not rendering) but in list

                        // prepare information of character's bounding sphere in world space(WS) and shadow camera's view space(VS)
                        var centerPosWS = targetChar.GetCharacterBoundCenter();
                        var centerPosShadowCamVS = (Matrix4x4.Scale(new Vector3(1, 1, -1)) * viewMatrix).MultiplyPoint(centerPosWS);
                        var boundRadius = targetChar.GetCharacterBoundRadius();

                        // TODO: this section is not correct, which may incorrectly cull effective shadow caster that is OUTSIDE of main camera frustum
                        {
                            // if character too far (outside of shadowRange), ignore them
                            if (centerPosShadowCamVS.z + boundRadius > shadowRange)
                            {
                                continue;
                            }

                            // if character bounding sphere not visible in main camera Frustum(not shadow camera), ignore them
                            GeometryUtility.CalculateFrustumPlanes(camera, cameraPlanes);
                            if (!GeometryUtility.TestPlanesAABB(cameraPlanes, new Bounds(targetChar.GetCharacterBoundCenter(), Vector3.one * boundRadius)))
                            {
                                continue;
                            }
                        }

                        // expand shadow camera's orthographics box bound to include a visible and valid char
                        minX = Mathf.Min(minX, centerPosShadowCamVS.x - boundRadius);
                        maxX = Mathf.Max(maxX, centerPosShadowCamVS.x + boundRadius);
                        minY = Mathf.Min(minY, centerPosShadowCamVS.y - boundRadius);
                        maxY = Mathf.Max(maxY, centerPosShadowCamVS.y + boundRadius);
                        minZ = Mathf.Min(minZ, centerPosShadowCamVS.z - boundRadius);
                        maxZ = Mathf.Max(maxZ, centerPosShadowCamVS.z + boundRadius);

                        visibleCharCount++;
                    }

                    // if nothing to render, treat it as disabled, early exit
                    if (visibleCharCount == 0)
                    {
                        CoreUtils.SetKeyword(cmd, _NILOTOON_RECEIVE_SELF_SHADOW_Keyword, false);
                        goto END;
                    }

                    Matrix4x4 projectionMatrix = Matrix4x4.Ortho(minX, maxX, minY, maxY, minZ, maxZ);

#if UNITY_EDITOR
                    bool isSceneViewCamera = renderingData.cameraData.isSceneViewCamera;

                    // we only want to draw using game camera's transform data
                    // so we can render a stable white box in scene view
                    if (debugShadowCameraVisibleBox && !isSceneViewCamera)
                    {
                        Matrix4x4 I_V = (Matrix4x4.Scale(new Vector3(1, 1, -1)) * viewMatrix).inverse;
                        Vector3 point1 = I_V.MultiplyPoint(new Vector3(minX, maxY, minZ));
                        Vector3 point2 = I_V.MultiplyPoint(new Vector3(maxX, maxY, minZ));
                        Vector3 point3 = I_V.MultiplyPoint(new Vector3(maxX, minY, minZ));
                        Vector3 point4 = I_V.MultiplyPoint(new Vector3(minX, minY, minZ));
                        Vector3 point5 = I_V.MultiplyPoint(new Vector3(minX, maxY, maxZ));
                        Vector3 point6 = I_V.MultiplyPoint(new Vector3(maxX, maxY, maxZ));
                        Vector3 point7 = I_V.MultiplyPoint(new Vector3(maxX, minY, maxZ));
                        Vector3 point8 = I_V.MultiplyPoint(new Vector3(minX, minY, maxZ));

                        // draw shadow camera visible box
                        Debug.DrawLine(point1, point2, Color.red);
                        Debug.DrawLine(point2, point3, Color.red);
                        Debug.DrawLine(point3, point4, Color.red);
                        Debug.DrawLine(point4, point1, Color.red);

                        Debug.DrawLine(point5, point6, Color.white);
                        Debug.DrawLine(point6, point7, Color.white);
                        Debug.DrawLine(point7, point8, Color.white);
                        Debug.DrawLine(point8, point5, Color.white);

                        Debug.DrawLine(point1, point5, Color.white);
                        Debug.DrawLine(point2, point6, Color.white);
                        Debug.DrawLine(point3, point7, Color.white);
                        Debug.DrawLine(point4, point8, Color.white);
                    }
#endif

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // set culling for shadow camera -> do culling
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////               
                    camera.TryGetCullingParameters(out var cullingParameters);

                    // update culling matrix
                    cullingParameters.cullingMatrix = projectionMatrix * viewMatrix;

                    // update culling planes
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cullingParameters.cullingMatrix);
                    for (int i = 0; i < planes.Length; i++)
                    {
                        cullingParameters.SetCullingPlane(i, planes[i]);
                    }

                    CullingResults cullResults;
                    if (settings.perfectCullingForShadowCasters)
                    {
                        // use the above new cullResults in DrawRenderers() below, 
                        // so even a renderer is not visible in the perspective of main camera,
                        // it can still render correctly in shadow camera's perspective due to this new culling

                        // (2021-07-14) unity will crash if code running this line and terrain exist in scene
                        cullResults = context.Cull(ref cullingParameters); // original working code, but will crash if terrain exist
                    }
                    else
                    {
                        // (2021-07-14) a special temp fix to avoid terrain crashing unity, but will make shadow culling not always correctly if shadow caster is not existing on screen
                        cullResults = renderingData.cullResults;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // override view & Projection matrix
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // draw all char renderer using SRP batching
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ShaderTagId shaderTagId = new ShaderTagId("NiloToonSelfShadowCaster");
                    var drawSetting = CreateDrawingSettings(shaderTagId, ref renderingData, SortingCriteria.CommonOpaque);
                    var filterSetting = new FilteringSettings(RenderQueueRange.opaque);
                    context.DrawRenderers(cullResults, ref drawSetting, ref filterSetting); // using custom cullResults from shadow camera's perspective, instead of main camera's cull result

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // restore camera matrix
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //set global RT
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    cmd.SetGlobalTexture(shadowMapRTH.id, new RenderTargetIdentifier(shadowMapRTH.id));
                    cmd.SetGlobalFloat("_NiloToonSelfShadowRange", shadowRange);

                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    Matrix4x4 GPU_P = GL.GetGPUProjectionMatrix(projectionMatrix, true);

                    Matrix4x4 GPU_worldToClip = GPU_P * viewMatrix;
                    cmd.SetGlobalMatrix("_NiloToonSelfShadowWorldToClip", GPU_worldToClip);
                    cmd.SetGlobalVector("_NiloToonSelfShadowParam", new Vector4(1f / shadowMapSize, 1f / shadowMapSize, shadowMapSize, shadowMapSize));
                    cmd.SetGlobalFloat("_NiloToonGlobalSelfShadowDepthBias", depthBias * 0.005f);
                    cmd.SetGlobalVector("_NiloToonSelfShadowLightDirection", viewMatrix.inverse.MultiplyVector(Vector3.forward));
                    cmd.SetGlobalFloat("_NiloToonSelfShadowUseNdotLFix", settings.useNdotLFix ? 1 : 0);
                }

                CoreUtils.SetKeyword(cmd, _NILOTOON_RECEIVE_SELF_SHADOW_Keyword, shouldRender);         
            }
            
            END:
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}