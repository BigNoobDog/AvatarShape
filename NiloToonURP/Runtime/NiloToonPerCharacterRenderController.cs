using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace NiloToon.NiloToonURP
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class NiloToonPerCharacterRenderController : MonoBehaviour
    {
#if UNITY_EDITOR
        static readonly string PlayerPrefsKey_NiloToonNeedPreserveEditorPlayModeMaterialChange = "NiloToonPreserveEditorPlayModeMaterialChange";
        public static bool GetNiloToonNeedPreserveEditorPlayModeMaterialChange_EditorOnly()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKey_NiloToonNeedPreserveEditorPlayModeMaterialChange) == 1;
        }
        public static void SetNiloToonNeedPreserveEditorPlayModeMaterialChange_EditorOnly(bool preserveEditorPlayModeMaterialChange)
        {
            PlayerPrefs.SetInt(PlayerPrefsKey_NiloToonNeedPreserveEditorPlayModeMaterialChange, preserveEditorPlayModeMaterialChange ? 1 : 0);
        }
#endif

        public enum TransformDirection
        {
            X,
            Y,
            Z,
            negX,
            negY,
            negZ
        }

        [Header("Make attachment renderers sync with this script's setting (e.g. drag weapon/microphone/stencil effect...renderer here)")]
        [Tooltip(
            "Drag any number of attachments(e.g. weapon/microphone/stencil effect...renderer) here, to make these renderers render using this script's settings.\n" +
            "(these renderer's shader need to support perspective removal)")]
        public List<Renderer> attachmentRendererList = new List<Renderer>();

        [Header("Gameplay per character effects - Tint")]
        [ColorUsage(false, true)]
        public Color perCharacterTintColor = Color.white; // default tint white = do nothing
        [Header("Gameplay per character effects - Add")]
        [ColorUsage(false, true)]
        public Color perCharacterAddColor = Color.black; // default add black = do nothing
        [Header("Gameplay per character effects - Desaturate")]
        [Range(0, 1f)]
        public float perCharacterDesaturationUsage = 0;
        [Header("Gameplay per character effects - Lerp")]
        [Range(0, 1)]
        public float perCharacterLerpUsage = 0;
        [ColorUsage(false, true)]
        public Color perCharacterLerpColor = new Color(1, 1, 0); // default yellow, 0 alpha
        [Header("Gameplay per character effects - Rim Light")]
        public bool usePerCharacterRimLightIntensity = false;
        [Range(0, 100)]
        public float perCharacterRimLightIntensity = 2;
        [ColorUsage(false, true)]
        public Color perCharacterRimLightColor = new Color(1, 0, 0);
        [Header("Gameplay per character effects - Outline")]
        [ColorUsage(false, true)]
        public Color perCharacterOutlineEffectTintColor = new Color(1, 1, 1);
        [ColorUsage(true, true)]
        public Color perCharacterOutlineEffectLerpColor = new Color(1, 1, 1, 0);


        [Header("Dither fadeout (only works if in playmode and DebugWindow SRP Batching = on)")]
        [Tooltip("Keep it at 1 will improve performance a lot, since we can turn it off completely in shader.\n" +
            "This slider will apply dither fadeout to URP's shadowmap also, which we recommend enabling URP's softshadow toggle to improve the final shadow result.")]
        [Range(0, 1)] public float ditherOpacity = 1;

        [Header("Outline (only works if in playmode and DebugWindow SRP Batching = on)")]
        public bool shouldRenderSelfOutline = true;

        [Header("Extra Outline (only works if in playmode and DebugWindow SRP Batching = on)")]
        [Tooltip("Usually for selection outline in gameplay, or for any artistic need like a thick white outline")]
        public bool shouldRenderExtraThickOutline = false;
        [ColorUsage(true, true)]
        public Color extraThickOutlineColor = Color.white;
        [Range(0, 100)]
        public float extraThickOutlineWidth = 3;
        [Range(0, 100)]
        public float extraThickOutlineMaximumFinalWidth = 100;

        public Vector3 extraThickOutlineViewSpacePosOffset = Vector3.zero;
        public bool extraThickOutlineRendersBlockedArea = false;
        [Tooltip("usually for render better outline when outline is occluded by opaque ground, set it to 0 and you will see the difference")]
        public float extraThickOutlineZOffset = -0.1f;
        [Range(0, 255)]
        [Tooltip("can be any number, but 199 is not that common, so it will lead to less conflict by default")]
        public int extraThickOutlineStencilID = 199;

        [Header("Perspective removal")]
        [Range(0, 1)]
        public float perspectiveRemovalAmount = 0;
        [Header("Perspective removal (Sphere, using head transform as sphere center)")]
        public float perspectiveRemovalRadius = 1;
        [Header("Perspective removal (world height)")]
        public float perspectiveRemovalStartHeight = 0;
        public float perspectiveRemovalEndHeight = 1;
        [Header("Perspective removal (XR)")]
        public bool disablePerspectiveRemovalInXR = true;

        [Header("Optimization (only works if in playmode and DebugWindow SRP Batching = on)")]
        [Tooltip("If turn this on, will boost CPU performance a lot, but changing material in playmode will require you to call RequestForceMaterialUpdateOnce() in C#")]
        public bool allowCacheSystem = true;
        public bool allowRenderShadowCasterPass = true;
        public bool allowRenderDepthOnlyAndDepthNormalsPass = true;
        public bool allowRenderNiloToonSelfShadowCasterPass = true;
        public bool allowRenderNiloToonPrepassBufferPass = true;

        [Header("Setup - Override Bounding Sphere (affect shadow result)")]
        [Tooltip("You can assign hip / Pelvis bone (any transform that is center of character\nuseful if default realtime bound from all renderers is not working for your character, or realtime bound is too slow")]
        public Transform customCharacterBoundCenter;
        public float characterBoundRadius = 1;
        public bool showBoundingSphereGizmo = false;

        [Header("Setup - Head Bone and Face forward direction (affect face lighting and perspective removal)")]
        public Transform headBoneTransform;
        public TransformDirection faceForwardDirection = TransformDirection.Z;
        public TransformDirection faceUpDirection = TransformDirection.Y;
        [Tooltip("If headBoneTransform is none, this will treat as 0")]
        [Range(0, 1)]
        public float faceNormalFixAmount = 1;
        public bool showFaceForwardDirectionGizmo = true;
        public bool showFaceUpDirectionGizmo = true;

        [Header("Setup - Renderers")]
        public List<Renderer> allRenderers = new List<Renderer>();

        [Header("Setup - BaseColor control")]
        public float perCharacterBaseColorMultiply = 1;
        [ColorUsage(false, true)]
        public Color perCharacterBaseColorTint = Color.white;

        [Header("Setup - Outline control")]
        public float perCharacterOutlineWidthMultiply = 1;
        [ColorUsage(false, true)]
        public Color perCharacterOutlineColorTint = Color.white;

        // NiloToon renderer feature will set this, not allow user see/set this
        [NonSerialized]
        public int shadowTestIndex;

        [NonSerialized]
        public bool reimportDone = false; // to prevent reimport Mesh(not .fbx) dead loop

        // originally created for perspective removal override by an external script(NiloToonCharacterRenderOverrider)
        // to allow character's hand holding weapons or microphone, and everything having the same perspective removal settings from a character
        [NonSerialized]
        public NiloToonCharacterRenderOverrider ExternalRenderOverrider;

        // API to allow user force update material
        public void RequestForceMaterialUpdateOnce()
        {
            requestForceMaterialUpdate = true;
        }

        private MaterialPropertyBlock materialPropertyBlock;
        List<Material> tempMaterialList = new List<Material>();
        bool? lastFrameShouldEditMaterial;
        bool forceMaterialIgnoreCacheAndUpdate;
        bool requestForceMaterialUpdate;

        // optimization: cache of last frame's value, for remove useless material.SetXXX() calls
        int? shadowTestIndex_Cache;
        Color? perCharacterTintColor_Cache;
        Color? perCharacterAddColor_Cache;
        float? perCharacterDesaturationUsage_Cache;
        Vector4? PerCharEffectLerpColor_Cache;
        Vector4? PerCharEffectRimColor_Cache;
        Vector3? FinalFaceForwardDirectionWS_Cache;
        Vector3? FinalFaceUpDirectionWS_Cache;
        float? faceNormalFixAmount_Cache;
        Color? extraThickOutlineColor_Cache;
        float? extraThickOutlineWidth_Cache;
        float? extraThickOutlineMaximumFinalWidth_Cache;
        Vector3? extraThickOutlineViewSpacePosOffset_Cache;
        int? ExtraThickOutlineZTest_Cache;
        bool? extraThickOutlineRendersBlockedArea_Cache;
        int? extraThickOutlineStencilID_Cache;
        float? extraThickOutlineZOffset_Cache;
        Vector3? CharacterBoundCenter_Cache;
        float? PerspectiveRemovalAmount_Cache;
        float? PerspectiveRemovalRadius_Cache;
        float? PerspectiveRemovalStartHeight_Cache;
        float? PerspectiveRemovalEndHeight_Cache;
        Vector3? PerspectiveRemovalCenter_Cache;
        Color? PerCharBaseColorTint_Cache;
        float? perCharacterOutlineWidthMultiply_Cache;
        Color? PerCharOutlineColorTint_Cache;
        Color? perCharacterOutlineEffectLerpColor_Cache;
        float? DitherFadeoutAmount_Cache;
        bool? shouldRenderSelfOutline_Cache;
        bool? ShouldRenderNiloToonExtraThickOutlinePass_Cache;
        bool? ShouldEnableDitherFadeOut_Cache;
        bool? ShouldEnableDepthTextureRimLightAndShadow_Cache;
        bool? allowRenderShadowCasterPass_Cache;
        bool? allowRenderDepthOnlyAndDepthNormalsPass_Cache;
        bool? allowRenderNiloToonSelfShadowCasterPass_Cache;
        bool? allowRenderNiloToonPrepassBufferPass_Cache;

        // RequireMaterialSet or RequireSetShaderPassEnabledCall
        bool shadowTestIndex_RequireMaterialSet;
        bool perCharacterTintColor_RequireMaterialSet;
        bool perCharacterAddColor_RequireMaterialSet;
        bool perCharacterDesaturationUsage_RequireMaterialSet;
        bool PerCharEffectLerpColor_RequireMaterialSet;
        bool PerCharEffectRimColor_RequireMaterialSet;
        bool FinalFaceForwardDirectionWS_RequireMaterialSet;
        bool FinalFaceUpDirectionWS_RequireMaterialSet;
        bool faceNormalFixAmount_RequireMaterialSet;
        bool extraThickOutlineColor_RequireMaterialSet;
        bool extraThickOutlineWidth_RequireMaterialSet;
        bool extraThickOutlineMaximumFinalWidth_RequireMaterialSet;
        bool extraThickOutlineViewSpacePosOffset_RequireMaterialSet;
        bool ExtraThickOutlineZTest_RequireMaterialSet;
        bool extraThickOutlineRendersBlockedArea_RequireMaterialSet;
        bool extraThickOutlineStencilID_RequireMaterialSet;
        bool extraThickOutlineZOffset_RequireMaterialSet;
        bool CharacterBoundCenter_RequireMaterialSet;
        bool PerspectiveRemovalAmount_RequireMaterialSet;
        bool PerspectiveRemovalRadius_RequireMaterialSet;
        bool PerspectiveRemovalStartHeight_RequireMaterialSet;
        bool PerspectiveRemovalEndHeight_RequireMaterialSet;
        bool PerspectiveRemovalCenter_RequireMaterialSet;
        bool PerCharBaseColorTint_RequireMaterialSet;
        bool perCharacterOutlineWidthMultiply_RequireMaterialSet;
        bool PerCharOutlineColorTint_RequireMaterialSet;
        bool perCharacterOutlineEffectLerpColor_RequireMaterialSet;
        bool DitherFadeoutAmount_RequireMaterialSet;
        bool shouldRenderSelfOutline_RequireSetShaderPassEnabledCall;
        bool ShouldRenderNiloToonExtraThickOutlinePass_RequireSetShaderPassEnabledCall;
        bool ShouldEnableDitherFadeOut_RequireKeywordChangeCall;
        bool ShouldEnableDepthTextureRimLightAndShadow_RequireKeywordChangeCall;
        bool allowRenderShadowCasterPass_RequireSetShaderPassEnabledCall;
        bool allowRenderDepthOnlyAndDepthNormalsPass_RequireSetShaderPassEnabledCall;
        bool allowRenderNiloToonSelfShadowCasterPass_RequireSetShaderPassEnabledCall;
        bool allowRenderNiloToonPrepassBufferPass_RequireSetShaderPassEnabledCall;

        // build current frame data cache (for avoid duplicate calls within the same frame)
        bool isHeadBoneTransformExist;
        Vector3 finalFaceDirectionWS_Forward;
        Vector3 finalFaceDirectionWS_Up;
        Vector3 characterBoundCenter;
        Vector3 perspectiveRemovealCenter;
        Vector4 perCharacterTintColorAsVector;
        Vector4 perCharacterAddColorAsVector;
        Vector4 PerCharEffectLerpColorAsVector;
        Vector4 PerCharEffectRimColorAsVector;
        Vector4 extraThickOutlineColorAsVector;
        Vector4 PerCharBaseColorTintAsVector;
        Vector4 PerCharOutlineColorTintAsVector;
        Vector4 perCharacterOutlineEffectLerpColorAsVector;

#if UNITY_EDITOR
        void OnValidate()
        {
            // group: prevent inspector setting negative number
            characterBoundRadius = Mathf.Max(0, characterBoundRadius);
            perspectiveRemovalRadius = Mathf.Max(0, perspectiveRemovalRadius);
            perCharacterBaseColorMultiply = Mathf.Max(0, perCharacterBaseColorMultiply);
            perCharacterOutlineWidthMultiply = Mathf.Max(0, perCharacterOutlineWidthMultiply);

            // make editing inspector value possible when game paused in play mode
            if (Application.isPlaying)
            {
                if (EditorApplication.isPaused)
                {
                    LateUpdate();
                }
            }
        }
#endif

        private void LateUpdate()
        {
            // must call this first before others, because others rely on this
            buildCurrentFrameDataCache();

            checkRequireMaterialSet();

            // auto clear ExternalRenderOverrider to null, if not needed anymore
            if (ExternalRenderOverrider)
            {
                // if ExternalRenderOverrider removed this controller already
                if (!ExternalRenderOverrider.targets.Contains(this))
                    ExternalRenderOverrider = null;
            }

            NiloToonAllInOneRendererFeature allInOneRendererFeature = NiloToonAllInOneRendererFeature.Instance;
            if (allInOneRendererFeature)
            {
                // if user added NiloToonAllInOneRendererFeature already, add this character to list
                allInOneRendererFeature.AddCharIfNotExist(this);
            }
            else
            {
                // user may forget to add NiloToonAllInOneRendererFeature, handle it
                Debug.LogError("You need to add a NiloToonAllInOneRendererFeature to all your ForwardRenderer.asset!");
            }

            // if user didn't assign allRenderers (count == 0), trigger auto find
            bool autoFindRequired = allRenderers.Count == 0;
            // if user updated the fbx, which makes some renderer becomes null, also trigger auto find
            foreach (var r in allRenderers)
            {
                if (r == null)
                {
                    autoFindRequired = true;
                    break;
                }
            }

            if (autoFindRequired)
            {
                DoAutoFindMaterial();
            }

            bool shouldEditMaterial = Application.isPlaying;
#if UNITY_EDITOR
            // this can be overrided if user enable UseSRPBatchingInEditor in [MenuItem("Window/NiloToonURP/Debug Window")]
            // In build, we will always use SRPBatching, because there is almost no reason to not use it

            //- This option only works in editor. In build, it is always ON, which enables SRP batching = better performance.
            //- In editor, turn this OFF can   preserve material change in editor playmode, but breaks  SRP batching. Good for material editing (for artist).
            //- In editor, turn this ON  can't preserve material change in editor playmode, but enables SRP batching. Good for profiling (for programmer/QA).
            if (NiloToonSetToonParamPass.Instance != null)
            {
                // if user don't want to preserve play mode material change, go clone and edit material directly
                shouldEditMaterial &= !GetNiloToonNeedPreserveEditorPlayModeMaterialChange_EditorOnly();
            }
#endif

            if (shouldEditMaterial)
            {
                ////////////////////////////////////////////////////////////////////////////////
                // play mode NOT using material property block to make SRP batching works
                // wiil edit material directly
                ////////////////////////////////////////////////////////////////////////////////
                void workPerRenderer(Renderer renderer)
                {
                    if (!renderer) return;

                    if (lastFrameShouldEditMaterial.HasValue && (lastFrameShouldEditMaterial != shouldEditMaterial))
                        renderer.SetPropertyBlock(null); // only clear our old nilotoon Material property block setting for the first frame since "shouldEditMaterial" change, to let other asset like cloth dynamics to set material value via Material property block correctly

                    renderer.GetMaterials(tempMaterialList);
                    foreach (var material in tempMaterialList)
                    {
                        UpdateMaterial(material);
                    }
                }

                foreach (var renderer in allRenderers)
                    workPerRenderer(renderer);
                foreach (var renderer in attachmentRendererList)
                    workPerRenderer(renderer);
            }
            else
            {
                ////////////////////////////////////////////////////////////////////////////////
                // edit mode using material property block to NOT edit renderer's material
                ////////////////////////////////////////////////////////////////////////////////
                if (materialPropertyBlock == null)
                    materialPropertyBlock = new MaterialPropertyBlock();

                void workPerRenderer(Renderer renderer)
                {
                    if (!renderer) return;

                    // get old block as base first, prevent destroy user's MPB
                    if (renderer.HasPropertyBlock())
                        renderer.GetPropertyBlock(materialPropertyBlock);

                    UpdateMaterialPropertyBlock(materialPropertyBlock);

                    // apply to renderer
                    renderer.SetPropertyBlock(materialPropertyBlock); // this will break SRP batching, don't use it in SRP's playmode!
                }

                foreach (var renderer in allRenderers)
                    workPerRenderer(renderer);
                foreach (var renderer in attachmentRendererList)
                    workPerRenderer(renderer);
            }

            forceMaterialIgnoreCacheAndUpdate = false; // each frame reset, will turn on again when needed

            // if user edit nilotoon debug window's settings, force update once to ensure all material set
            if (lastFrameShouldEditMaterial != shouldEditMaterial)
                forceMaterialIgnoreCacheAndUpdate = true;

            lastFrameShouldEditMaterial = shouldEditMaterial;

            cacheProprtiesForNextFrameOptimizationCheck();

            // when allRenderers list changed, force update once to ensure all material set
            if (autoFindRequired || requestForceMaterialUpdate || !allowCacheSystem)
            {
                forceMaterialIgnoreCacheAndUpdate = true;
            }

            requestForceMaterialUpdate = false; // each frame reset, will turn on by user when needed
        }

        private void DoAutoFindMaterial()
        {
            GetComponentsInChildren<Renderer>(true, allRenderers);
            allRenderers = allRenderers.FindAll(x =>
            {
                foreach (var mat in x.sharedMaterials)
                {
                    // prevents null if shader fails to conpile
                    if (mat)
                        if (mat.shader)
                            if (mat.shader.name.Contains("NiloToon"))
                                return true;
                }

                return false;
            });
        }

        private void OnEnable()
        {
            RequestForceMaterialUpdateOnce();
        }
        private void OnDisable()
        {
            // maybe RenderAverageShadowTestRTRenderPass.Instance doesn't exist anymore (usually happens when switching scene), so add a null check
            NiloToonAllInOneRendererFeature.Instance?.Remove(this);
        }

        private void buildCurrentFrameDataCache()
        {
            //////////////////////////////////////////////////////////////////////////////////////
            // calculate and cache value for current frame's repeated API calls
            //////////////////////////////////////////////////////////////////////////////////////
            isHeadBoneTransformExist = headBoneTransform;
            if (isHeadBoneTransformExist)
            {
                finalFaceDirectionWS_Forward = GetFinalFaceDirectionWS(faceForwardDirection);
                finalFaceDirectionWS_Up = GetFinalFaceDirectionWS(faceUpDirection);
            }
            characterBoundCenter = GetCharacterBoundCenter();
            perspectiveRemovealCenter = GetPerspectiveRemovalCenter();

            perCharacterTintColorAsVector = perCharacterTintColor;
            perCharacterAddColorAsVector = perCharacterAddColor;
            PerCharEffectLerpColorAsVector = GetPerCharEffectLerpColor();
            PerCharEffectRimColorAsVector = GetPerCharEffectRimColor();

            extraThickOutlineColorAsVector = extraThickOutlineColor;
            PerCharBaseColorTintAsVector = GetPerCharBaseColorTint();
            PerCharOutlineColorTintAsVector = GetPerCharOutlineColorTint();
            perCharacterOutlineEffectLerpColorAsVector = perCharacterOutlineEffectLerpColor;
        }
        private void checkRequireMaterialSet()
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // check if UpdateMaterial() or UpdateMaterialPropertyBlock() require to call expensive Material.SetXXX(), Material.EnableKeyword() or Material.SetShaderPassEnabled()
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (forceMaterialIgnoreCacheAndUpdate)
            {
                shadowTestIndex_RequireMaterialSet = true;
                perCharacterTintColor_RequireMaterialSet = true;
                perCharacterAddColor_RequireMaterialSet = true;
                perCharacterDesaturationUsage_RequireMaterialSet = true;
                PerCharEffectLerpColor_RequireMaterialSet = true;
                PerCharEffectRimColor_RequireMaterialSet = true;
                FinalFaceForwardDirectionWS_RequireMaterialSet = true;
                FinalFaceUpDirectionWS_RequireMaterialSet = true;
                faceNormalFixAmount_RequireMaterialSet = true;
                extraThickOutlineColor_RequireMaterialSet = true;
                extraThickOutlineWidth_RequireMaterialSet = true;
                extraThickOutlineMaximumFinalWidth_RequireMaterialSet = true;
                extraThickOutlineViewSpacePosOffset_RequireMaterialSet = true;
                ExtraThickOutlineZTest_RequireMaterialSet = true;
                extraThickOutlineRendersBlockedArea_RequireMaterialSet = true;
                extraThickOutlineStencilID_RequireMaterialSet = true;
                extraThickOutlineZOffset_RequireMaterialSet = true;
                CharacterBoundCenter_RequireMaterialSet = true;
                PerspectiveRemovalAmount_RequireMaterialSet = true;
                PerspectiveRemovalRadius_RequireMaterialSet = true;
                PerspectiveRemovalStartHeight_RequireMaterialSet = true;
                PerspectiveRemovalEndHeight_RequireMaterialSet = true;
                PerspectiveRemovalCenter_RequireMaterialSet = true;
                PerCharBaseColorTint_RequireMaterialSet = true;
                perCharacterOutlineWidthMultiply_RequireMaterialSet = true;
                PerCharOutlineColorTint_RequireMaterialSet = true;
                perCharacterOutlineEffectLerpColor_RequireMaterialSet = true;
                DitherFadeoutAmount_RequireMaterialSet = true;

                shouldRenderSelfOutline_RequireSetShaderPassEnabledCall = true;
                ShouldRenderNiloToonExtraThickOutlinePass_RequireSetShaderPassEnabledCall = true;
                ShouldEnableDitherFadeOut_RequireKeywordChangeCall = true;
                ShouldEnableDepthTextureRimLightAndShadow_RequireKeywordChangeCall = true;
                allowRenderShadowCasterPass_RequireSetShaderPassEnabledCall = true;
                allowRenderDepthOnlyAndDepthNormalsPass_RequireSetShaderPassEnabledCall = true;
                allowRenderNiloToonSelfShadowCasterPass_RequireSetShaderPassEnabledCall = true;
                allowRenderNiloToonPrepassBufferPass_RequireSetShaderPassEnabledCall = true;

                return;
            }

            shadowTestIndex_RequireMaterialSet = shadowTestIndex_Cache != shadowTestIndex;
            perCharacterTintColor_RequireMaterialSet = perCharacterTintColor_Cache != perCharacterTintColor;
            perCharacterAddColor_RequireMaterialSet = perCharacterAddColor_Cache != perCharacterAddColor;
            perCharacterDesaturationUsage_RequireMaterialSet = perCharacterDesaturationUsage_Cache != perCharacterDesaturationUsage;
            PerCharEffectLerpColor_RequireMaterialSet = PerCharEffectLerpColor_Cache != PerCharEffectLerpColorAsVector;
            PerCharEffectRimColor_RequireMaterialSet = PerCharEffectRimColor_Cache != PerCharEffectRimColorAsVector;
            if (isHeadBoneTransformExist)
            {
                FinalFaceForwardDirectionWS_RequireMaterialSet = FinalFaceForwardDirectionWS_Cache != finalFaceDirectionWS_Forward;
                FinalFaceUpDirectionWS_RequireMaterialSet = FinalFaceUpDirectionWS_Cache != finalFaceDirectionWS_Up;
            }
            else
            {
                FinalFaceForwardDirectionWS_RequireMaterialSet = false;
                FinalFaceUpDirectionWS_RequireMaterialSet = false;
            }
            faceNormalFixAmount_RequireMaterialSet = faceNormalFixAmount_Cache != faceNormalFixAmount;
            extraThickOutlineColor_RequireMaterialSet = extraThickOutlineColor_Cache != extraThickOutlineColor;
            extraThickOutlineWidth_RequireMaterialSet = extraThickOutlineWidth_Cache != extraThickOutlineWidth;
            extraThickOutlineMaximumFinalWidth_RequireMaterialSet = extraThickOutlineMaximumFinalWidth_Cache != extraThickOutlineMaximumFinalWidth;
            extraThickOutlineViewSpacePosOffset_RequireMaterialSet = extraThickOutlineViewSpacePosOffset_Cache != extraThickOutlineViewSpacePosOffset;
            ExtraThickOutlineZTest_RequireMaterialSet = ExtraThickOutlineZTest_Cache != GetExtraThickOutlineZTest();
            extraThickOutlineRendersBlockedArea_RequireMaterialSet = extraThickOutlineRendersBlockedArea_Cache != extraThickOutlineRendersBlockedArea;
            extraThickOutlineStencilID_RequireMaterialSet = extraThickOutlineStencilID_Cache != extraThickOutlineStencilID;
            extraThickOutlineZOffset_RequireMaterialSet = extraThickOutlineZOffset_Cache != extraThickOutlineZOffset;
            CharacterBoundCenter_RequireMaterialSet = CharacterBoundCenter_Cache != characterBoundCenter;
            PerspectiveRemovalAmount_RequireMaterialSet = PerspectiveRemovalAmount_Cache != GetPerspectiveRemovalAmount();
            PerspectiveRemovalRadius_RequireMaterialSet = PerspectiveRemovalRadius_Cache != GetPerspectiveRemovalRadius();
            PerspectiveRemovalStartHeight_RequireMaterialSet = PerspectiveRemovalStartHeight_Cache != GetPerspectiveRemovalStartHeight();
            PerspectiveRemovalEndHeight_RequireMaterialSet = PerspectiveRemovalEndHeight_Cache != GetPerspectiveRemovalEndHeight();
            PerspectiveRemovalCenter_RequireMaterialSet = PerspectiveRemovalCenter_Cache != perspectiveRemovealCenter;
            PerCharBaseColorTint_RequireMaterialSet = PerCharBaseColorTint_Cache != GetPerCharBaseColorTint();
            perCharacterOutlineWidthMultiply_RequireMaterialSet = perCharacterOutlineWidthMultiply_Cache != perCharacterOutlineWidthMultiply;
            PerCharOutlineColorTint_RequireMaterialSet = PerCharOutlineColorTint_Cache != GetPerCharOutlineColorTint();
            perCharacterOutlineEffectLerpColor_RequireMaterialSet = perCharacterOutlineEffectLerpColor_Cache != perCharacterOutlineEffectLerpColor;
            DitherFadeoutAmount_RequireMaterialSet = DitherFadeoutAmount_Cache != GetDitherFadeoutAmount();

            shouldRenderSelfOutline_RequireSetShaderPassEnabledCall = shouldRenderSelfOutline_Cache != shouldRenderSelfOutline;
            ShouldRenderNiloToonExtraThickOutlinePass_RequireSetShaderPassEnabledCall = ShouldRenderNiloToonExtraThickOutlinePass_Cache != GetShouldRenderNiloToonExtraThickOutlinePass();
            ShouldEnableDitherFadeOut_RequireKeywordChangeCall = ShouldEnableDitherFadeOut_Cache != GetShouldEnableDitherFadeOut();
            ShouldEnableDepthTextureRimLightAndShadow_RequireKeywordChangeCall = ShouldEnableDepthTextureRimLightAndShadow_Cache != GetShouldEnableDepthTextureRimLightAndShadow();
            allowRenderShadowCasterPass_RequireSetShaderPassEnabledCall = allowRenderShadowCasterPass_Cache != allowRenderShadowCasterPass;
            allowRenderDepthOnlyAndDepthNormalsPass_RequireSetShaderPassEnabledCall = allowRenderDepthOnlyAndDepthNormalsPass_Cache != allowRenderDepthOnlyAndDepthNormalsPass;
            allowRenderNiloToonSelfShadowCasterPass_RequireSetShaderPassEnabledCall = allowRenderNiloToonSelfShadowCasterPass_Cache != allowRenderNiloToonSelfShadowCasterPass;
            allowRenderNiloToonPrepassBufferPass_RequireSetShaderPassEnabledCall = allowRenderNiloToonPrepassBufferPass_Cache != allowRenderNiloToonPrepassBufferPass;
        }
        private void cacheProprtiesForNextFrameOptimizationCheck()
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // save this frame's value in cache, for next frame's optimization
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            shadowTestIndex_Cache = shadowTestIndex;
            perCharacterTintColor_Cache = perCharacterTintColor;
            perCharacterAddColor_Cache = perCharacterAddColor;
            perCharacterDesaturationUsage_Cache = perCharacterDesaturationUsage;
            PerCharEffectLerpColor_Cache = GetPerCharEffectLerpColor();
            PerCharEffectRimColor_Cache = GetPerCharEffectRimColor();
            if (isHeadBoneTransformExist)
            {
                FinalFaceForwardDirectionWS_Cache = finalFaceDirectionWS_Forward;
                FinalFaceUpDirectionWS_Cache = finalFaceDirectionWS_Up;
            }

            faceNormalFixAmount_Cache = faceNormalFixAmount;
            extraThickOutlineColor_Cache = extraThickOutlineColor;
            extraThickOutlineWidth_Cache = extraThickOutlineWidth;
            extraThickOutlineMaximumFinalWidth_Cache = extraThickOutlineMaximumFinalWidth;
            extraThickOutlineViewSpacePosOffset_Cache = extraThickOutlineViewSpacePosOffset;
            ExtraThickOutlineZTest_Cache = GetExtraThickOutlineZTest();
            extraThickOutlineRendersBlockedArea_Cache = extraThickOutlineRendersBlockedArea;
            extraThickOutlineStencilID_Cache = extraThickOutlineStencilID;
            extraThickOutlineZOffset_Cache = extraThickOutlineZOffset;
            CharacterBoundCenter_Cache = characterBoundCenter;
            PerspectiveRemovalAmount_Cache = GetPerspectiveRemovalAmount();
            PerspectiveRemovalRadius_Cache = GetPerspectiveRemovalRadius();
            PerspectiveRemovalStartHeight_Cache = GetPerspectiveRemovalStartHeight();
            PerspectiveRemovalEndHeight_Cache = GetPerspectiveRemovalEndHeight();
            PerspectiveRemovalCenter_Cache = perspectiveRemovealCenter;
            PerCharBaseColorTint_Cache = GetPerCharBaseColorTint();
            perCharacterOutlineWidthMultiply_Cache = perCharacterOutlineWidthMultiply;
            PerCharOutlineColorTint_Cache = GetPerCharOutlineColorTint();
            perCharacterOutlineEffectLerpColor_Cache = perCharacterOutlineEffectLerpColor;
            DitherFadeoutAmount_Cache = GetDitherFadeoutAmount();

            shouldRenderSelfOutline_Cache = shouldRenderSelfOutline;
            ShouldRenderNiloToonExtraThickOutlinePass_Cache = GetShouldRenderNiloToonExtraThickOutlinePass();
            ShouldEnableDitherFadeOut_Cache = GetShouldEnableDitherFadeOut();
            ShouldEnableDepthTextureRimLightAndShadow_Cache = GetShouldEnableDepthTextureRimLightAndShadow();
            allowRenderShadowCasterPass_Cache = allowRenderShadowCasterPass;
            allowRenderDepthOnlyAndDepthNormalsPass_Cache = allowRenderDepthOnlyAndDepthNormalsPass;
            allowRenderNiloToonSelfShadowCasterPass_Cache = allowRenderNiloToonSelfShadowCasterPass;
            allowRenderNiloToonPrepassBufferPass_Cache = allowRenderNiloToonPrepassBufferPass;
        }
        public float GetCharacterBoundRadius()
        {
            // static radius sphere is better than dyanmic calculated radius sphere
            // because it is visually more stable
            return characterBoundRadius;
        }

        public Vector3 GetPerspectiveRemovalCenter()
        {
            // overrider
            if (ExternalRenderOverrider && ExternalRenderOverrider.ShouldOverridePerspectiveRemoval())
            {
                return ExternalRenderOverrider.GetPerspectiveRemovalOverridedCenterPosWS();
            }

            return GetSelfPerspectiveRemovalCenter();
        }
        public Vector3 GetSelfPerspectiveRemovalCenter()
        {
            // we mainly want to remove face's perspective, so use head as center is the preferred choice
            if (isHeadBoneTransformExist)
                return headBoneTransform.position;

            // fallback
            return characterBoundCenter;
        }
        public Vector3 GetCharacterBoundCenter()
        {
            if (customCharacterBoundCenter)
            {
                return customCharacterBoundCenter.position;
            }
            else
            {
                // when user click auto setup button, allRenderers will become empty for that frame, we call this to ensure allRenderers is correct within the same frame
                if (allRenderers.Count == 0)
                    DoAutoFindMaterial();

                // find center in realtime as fallback method (very slow)
                Bounds? bound = null;
                foreach (var renderer in allRenderers)
                {
                    if (!bound.HasValue)
                    {
                        bound = renderer.bounds;
                    }
                    else
                    {
                        bound.Value.Encapsulate(renderer.bounds);
                    }
                }

                if (bound == null)
                {
                    Debug.LogWarning($"No NiloToon shader detected inside {this.gameObject.name}, Did you forget to change character's material to NiloToon's character shader?", this);
                    return transform.position + Vector3.up;
                }
                return bound.Value.center;
            }
        }

        // copy and edit of https://forum.unity.com/threads/debug-drawarrow.85980/
        private static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.05f, float arrowHeadAngle = 22.5f)
        {
            direction *= 0.5f;
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);

            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(+arrowHeadAngle, 180, 0) * new Vector3(0, 0, 1);
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 180, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
        }

        private void OnDrawGizmosSelected()
        {
            // bounding sphere
            if (showBoundingSphereGizmo)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(characterBoundCenter, GetCharacterBoundRadius());
            }

            // face forward direction arrow
            if (showFaceForwardDirectionGizmo && isHeadBoneTransformExist)
            {
                DrawArrow(headBoneTransform.position, finalFaceDirectionWS_Forward, new Color(0, 0, 1, 1f)); // blue Z, same as unity's scene window
            }

            // face up direction arrow
            if (showFaceUpDirectionGizmo && isHeadBoneTransformExist)
            {
                DrawArrow(headBoneTransform.position, finalFaceDirectionWS_Up, new Color(0, 1, 0, 1f)); // green Y, same as unity's scene window
            }
        }

        static readonly int _AverageShadowMapRTSampleIndex = Shader.PropertyToID("_AverageShadowMapRTSampleIndex");
        static readonly int _PerCharEffectTintColor = Shader.PropertyToID("_PerCharEffectTintColor");
        static readonly int _PerCharEffectAddColor = Shader.PropertyToID("_PerCharEffectAddColor");
        static readonly int _PerCharEffectDesaturatePercentage = Shader.PropertyToID("_PerCharEffectDesaturatePercentage");
        static readonly int _PerCharEffectLerpColor = Shader.PropertyToID("_PerCharEffectLerpColor");
        static readonly int _PerCharEffectRimColor = Shader.PropertyToID("_PerCharEffectRimColor");
        static readonly int _FaceForwardDirection = Shader.PropertyToID("_FaceForwardDirection");
        static readonly int _FaceUpDirection = Shader.PropertyToID("_FaceUpDirection");
        static readonly int _FixFaceNormalAmount = Shader.PropertyToID("_FixFaceNormalAmount");
        static readonly int _ExtraThickOutlineColor = Shader.PropertyToID("_ExtraThickOutlineColor");
        static readonly int _ExtraThickOutlineWidth = Shader.PropertyToID("_ExtraThickOutlineWidth");
        static readonly int _ExtraThickOutlineMaxFinalWidth = Shader.PropertyToID("_ExtraThickOutlineMaxFinalWidth");
        static readonly int _ExtraThickOutlineViewSpacePosOffset = Shader.PropertyToID("_ExtraThickOutlineViewSpacePosOffset");
        static readonly int _ExtraThickOutlineZTest = Shader.PropertyToID("_ExtraThickOutlineZTest");
        static readonly int _ExtraThickOutlineStencilID = Shader.PropertyToID("_ExtraThickOutlineStencilID");
        static readonly int _ExtraThickOutlineZOffset = Shader.PropertyToID("_ExtraThickOutlineZOffset");
        static readonly int _CharacterBoundCenterPosWS = Shader.PropertyToID("_CharacterBoundCenterPosWS");
        static readonly int _PerspectiveRemovalAmount = Shader.PropertyToID("_PerspectiveRemovalAmount");
        static readonly int _PerspectiveRemovalRadius = Shader.PropertyToID("_PerspectiveRemovalRadius");
        static readonly int _PerspectiveRemovalStartHeight = Shader.PropertyToID("_PerspectiveRemovalStartHeight");
        static readonly int _PerspectiveRemovalEndHeight = Shader.PropertyToID("_PerspectiveRemovalEndHeight");
        static readonly int _HeadBonePositionWS = Shader.PropertyToID("_HeadBonePositionWS");
        static readonly int _PerCharacterBaseColorTint = Shader.PropertyToID("_PerCharacterBaseColorTint");
        static readonly int _PerCharacterOutlineWidthMultiply = Shader.PropertyToID("_PerCharacterOutlineWidthMultiply");
        static readonly int _PerCharacterOutlineColorTint = Shader.PropertyToID("_PerCharacterOutlineColorTint");
        static readonly int _PerCharacterOutlineColorLerp = Shader.PropertyToID("_PerCharacterOutlineColorLerp");
        static readonly int _DitherFadeoutAmount = Shader.PropertyToID("_DitherFadeoutAmount");

        Vector3 GetFinalFaceDirectionWS(TransformDirection direction)
        {
            switch (direction)
            {
                case TransformDirection.X:
                    return headBoneTransform.right;
                case TransformDirection.Y:
                    return headBoneTransform.up;
                case TransformDirection.Z:
                    return headBoneTransform.forward;
                case TransformDirection.negX:
                    return -headBoneTransform.right;
                case TransformDirection.negY:
                    return -headBoneTransform.up;
                case TransformDirection.negZ:
                    return -headBoneTransform.forward;
                default:
                    throw new System.NotImplementedException();
            }
        }

        float GetPerspectiveRemovalAmount()
        {
            // overrider
            if (ExternalRenderOverrider && ExternalRenderOverrider.ShouldOverridePerspectiveRemoval())
            {
                // overrider settings
                return ExternalRenderOverrider.GetPerspectiveRemovalOverridedAmount();
            }

            // self (XR check)
            if (disablePerspectiveRemovalInXR && XRSettings.isDeviceActive)
            {
                return 0; // disable in VR, because PerspectiveRemoval looks weird in VR when camera rotate a lot
            }

            // self
            return perspectiveRemovalAmount;
        }
        float GetPerspectiveRemovalRadius()
        {
            // overrider
            if (ExternalRenderOverrider && ExternalRenderOverrider.ShouldOverridePerspectiveRemoval())
                return Mathf.Max(0.01f, ExternalRenderOverrider.GetPerspectiveRemovalOverridedRadius()); // prevent /0

            // self
            return Mathf.Max(0.01f, perspectiveRemovalRadius); // prevent /0
        }
        float GetPerspectiveRemovalStartHeight()
        {
            // overrider
            if (ExternalRenderOverrider && ExternalRenderOverrider.ShouldOverridePerspectiveRemoval())
                return ExternalRenderOverrider.GetPerspectiveRemovalOverridedStartHeight();

            // self
            return perspectiveRemovalStartHeight;
        }
        float GetPerspectiveRemovalEndHeight()
        {
            // overrider
            if (ExternalRenderOverrider && ExternalRenderOverrider.ShouldOverridePerspectiveRemoval())
                return ExternalRenderOverrider.GetPerspectiveRemovalOverridedEndHeight();

            // self
            return perspectiveRemovalEndHeight;
        }

        Color GetPerCharEffectLerpColor()
        {
            return new Color(perCharacterLerpColor.r, perCharacterLerpColor.g, perCharacterLerpColor.b, perCharacterLerpUsage);
        }
        Color GetPerCharEffectRimColor()
        {
            return usePerCharacterRimLightIntensity ? perCharacterRimLightIntensity * perCharacterRimLightColor : Color.clear;
        }
        Color GetPerCharBaseColorTint()
        {
            return perCharacterBaseColorTint * perCharacterBaseColorMultiply;
        }
        Color GetPerCharOutlineColorTint()
        {
            return perCharacterOutlineColorTint * perCharacterOutlineEffectTintColor;
        }
        float GetDitherFadeoutAmount()
        {
            return 1 - ditherOpacity;
        }
        bool GetShouldRenderNiloToonExtraThickOutlinePass()
        {
            return extraThickOutlineColor.a > 0 ? shouldRenderExtraThickOutline : false;
        }
        bool GetShouldEnableDitherFadeOut()
        {
            return ditherOpacity < 0.99f;
        }
        bool GetShouldEnableDepthTextureRimLightAndShadow()
        {
            return NiloToonSetToonParamPass.Instance.EnableDepthTextureRimLigthAndShadow && allowRenderDepthOnlyAndDepthNormalsPass;
        }

        int GetExtraThickOutlineZTest()
        {
            return extraThickOutlineRendersBlockedArea ? (int)UnityEngine.Rendering.CompareFunction.Always : (int)UnityEngine.Rendering.CompareFunction.LessEqual;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UpdateMaterial() & UpdateMaterialPropertyBlock() are two almost identical functions,
        // Basicly We want something like this:
        //
        // void UpdateMaterialOrPropertyBlock<T>(T input) where T : Material or MaterialPropertyBlock
        //
        // but we failed to find a way to do it in C#,
        // So now we duplicate the function to make it works first.
        // If anyone know a method that doesn't require duplicating the function
        // Please let us know, thanks!
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateMaterial(Material input)
        {
            // Please copy this section to UpdateMaterialPropertyBlock()
            //================================================================================================================================================
            // DONT IGNORE THIS: for material but not MPB, use SetVector instead of SetColor to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (shadowTestIndex_RequireMaterialSet) input.SetInt(_AverageShadowMapRTSampleIndex, shadowTestIndex);
            if (perCharacterTintColor_RequireMaterialSet) input.SetVector(_PerCharEffectTintColor, perCharacterTintColorAsVector);
            if (perCharacterAddColor_RequireMaterialSet) input.SetVector(_PerCharEffectAddColor, perCharacterAddColorAsVector);
            if (perCharacterDesaturationUsage_RequireMaterialSet) input.SetFloat(_PerCharEffectDesaturatePercentage, perCharacterDesaturationUsage);
            if (PerCharEffectLerpColor_RequireMaterialSet) input.SetVector(_PerCharEffectLerpColor, PerCharEffectLerpColorAsVector);
            if (PerCharEffectRimColor_RequireMaterialSet) input.SetVector(_PerCharEffectRimColor, PerCharEffectRimColorAsVector);
            if (isHeadBoneTransformExist)
            {
                if (FinalFaceForwardDirectionWS_RequireMaterialSet) input.SetVector(_FaceForwardDirection, finalFaceDirectionWS_Forward);
                if (FinalFaceUpDirectionWS_RequireMaterialSet) input.SetVector(_FaceUpDirection, finalFaceDirectionWS_Up);
                if (faceNormalFixAmount_RequireMaterialSet) input.SetFloat(_FixFaceNormalAmount, faceNormalFixAmount);
            }
            else
            {
                input.SetFloat(_FixFaceNormalAmount, 0);
            }
            if (extraThickOutlineColor_RequireMaterialSet) input.SetVector(_ExtraThickOutlineColor, extraThickOutlineColorAsVector);
            if (extraThickOutlineWidth_RequireMaterialSet) input.SetFloat(_ExtraThickOutlineWidth, extraThickOutlineWidth);
            if (extraThickOutlineMaximumFinalWidth_RequireMaterialSet) input.SetFloat(_ExtraThickOutlineMaxFinalWidth, extraThickOutlineMaximumFinalWidth);
            if (extraThickOutlineViewSpacePosOffset_RequireMaterialSet) input.SetVector(_ExtraThickOutlineViewSpacePosOffset, extraThickOutlineViewSpacePosOffset);
            if (ExtraThickOutlineZTest_RequireMaterialSet) input.SetInt(_ExtraThickOutlineZTest, GetExtraThickOutlineZTest());
            if (extraThickOutlineStencilID_RequireMaterialSet) input.SetInt(_ExtraThickOutlineStencilID, extraThickOutlineStencilID);
            if (extraThickOutlineZOffset_RequireMaterialSet) input.SetFloat(_ExtraThickOutlineZOffset, extraThickOutlineZOffset);
            if (CharacterBoundCenter_RequireMaterialSet) input.SetVector(_CharacterBoundCenterPosWS, characterBoundCenter);
            if (PerspectiveRemovalAmount_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalAmount, GetPerspectiveRemovalAmount());
            if (PerspectiveRemovalRadius_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalRadius, GetPerspectiveRemovalRadius());
            if (PerspectiveRemovalStartHeight_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalStartHeight, GetPerspectiveRemovalStartHeight());
            if (PerspectiveRemovalEndHeight_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalEndHeight, GetPerspectiveRemovalEndHeight());
            if (PerspectiveRemovalCenter_RequireMaterialSet) input.SetVector(_HeadBonePositionWS, perspectiveRemovealCenter);
            if (PerCharBaseColorTint_RequireMaterialSet) input.SetVector(_PerCharacterBaseColorTint, PerCharBaseColorTintAsVector); // DONT IGNORE THIS: for material, use SetVector instead of SetColor to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (perCharacterOutlineWidthMultiply_RequireMaterialSet) input.SetFloat(_PerCharacterOutlineWidthMultiply, perCharacterOutlineWidthMultiply);
            if (PerCharOutlineColorTint_RequireMaterialSet) input.SetVector(_PerCharacterOutlineColorTint, PerCharOutlineColorTintAsVector);
            if (perCharacterOutlineEffectLerpColor_RequireMaterialSet) input.SetVector(_PerCharacterOutlineColorLerp, perCharacterOutlineEffectLerpColorAsVector);
            if (DitherFadeoutAmount_RequireMaterialSet) input.SetFloat(_DitherFadeoutAmount, GetDitherFadeoutAmount()); // need to always set, no matter keyword "_NILOTOON_DITHER_FADEOUT" is on or off (pass 1-x to shader to make preview window still render when shader is using 0 as _DitherFadeoutAmount's default value)
            if (ShouldEnableDepthTextureRimLightAndShadow_RequireKeywordChangeCall) input.SetFloat("_NiloToonEnableDepthTextureRimLightAndShadow", GetShouldEnableDepthTextureRimLightAndShadow() ? 1 : 0);
            //================================================================================================================================================


            // API SetShaderPassEnabled(...) only exists in Material class, but not MaterialPropertyBlock class
            // so the following section will NOT exist in UpdateMaterialPropertyBlock(...) function
            //-------------------------------------------------------------------------------------------------------------
            // What is SetShaderPassEnabled()?
            // this API will find "LightMode" = "XXX" in your shader, and disable that pass if param "enabled" is false
            // https://docs.unity3d.com/ScriptReference/Material.SetShaderPassEnabled.html
            //-------------------------------------------------------------------------------------------------------------
            if (shouldRenderSelfOutline_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("NiloToonOutline", shouldRenderSelfOutline);

            // optimization: auto skip rendering if we know that it will not affecting rendering result
            if (ShouldRenderNiloToonExtraThickOutlinePass_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("NiloToonExtraThickOutline", GetShouldRenderNiloToonExtraThickOutlinePass());
            if (ShouldEnableDitherFadeOut_RequireKeywordChangeCall) CoreUtils.SetKeyword(input, "_NILOTOON_DITHER_FADEOUT", GetShouldEnableDitherFadeOut());



            // optimization: allow user to control render these pass or not, so user can turn these off for non-important/far-away characters
            // the first param equals to "LightMode" = "XXX"
            if (allowRenderShadowCasterPass_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("ShadowCaster", allowRenderShadowCasterPass);
            if (allowRenderDepthOnlyAndDepthNormalsPass_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("DepthOnly", allowRenderDepthOnlyAndDepthNormalsPass);
            if (allowRenderDepthOnlyAndDepthNormalsPass_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("DepthNormals", allowRenderDepthOnlyAndDepthNormalsPass);
            if (allowRenderNiloToonSelfShadowCasterPass_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("NiloToonSelfShadowCaster", allowRenderNiloToonSelfShadowCasterPass);
            if (allowRenderNiloToonPrepassBufferPass_RequireSetShaderPassEnabledCall) input.SetShaderPassEnabled("NiloToonPrepassBuffer", allowRenderNiloToonPrepassBufferPass);
        }
        void UpdateMaterialPropertyBlock(MaterialPropertyBlock input)
        {
            // Please copy from UpdateMaterial(), but keep "SetVector() for material" and "SetColor() for MPB" difference
            //================================================================================================================================================
            if (shadowTestIndex_RequireMaterialSet) input.SetInt(_AverageShadowMapRTSampleIndex, shadowTestIndex);
            if (perCharacterTintColor_RequireMaterialSet) input.SetColor(_PerCharEffectTintColor, perCharacterTintColorAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (perCharacterAddColor_RequireMaterialSet) input.SetColor(_PerCharEffectAddColor, perCharacterAddColorAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (perCharacterDesaturationUsage_RequireMaterialSet) input.SetFloat(_PerCharEffectDesaturatePercentage, perCharacterDesaturationUsage);
            if (PerCharEffectLerpColor_RequireMaterialSet) input.SetColor(_PerCharEffectLerpColor, PerCharEffectLerpColorAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (PerCharEffectRimColor_RequireMaterialSet) input.SetColor(_PerCharEffectRimColor, PerCharEffectRimColorAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (isHeadBoneTransformExist)
            {
                if (FinalFaceForwardDirectionWS_RequireMaterialSet) input.SetVector(_FaceForwardDirection, finalFaceDirectionWS_Forward);
                if (FinalFaceUpDirectionWS_RequireMaterialSet) input.SetVector(_FaceUpDirection, finalFaceDirectionWS_Up);
                if (faceNormalFixAmount_RequireMaterialSet) input.SetFloat(_FixFaceNormalAmount, faceNormalFixAmount);
            }
            else
            {
                input.SetFloat(_FixFaceNormalAmount, 0);
            }
            if (extraThickOutlineColor_RequireMaterialSet) input.SetColor(_ExtraThickOutlineColor, extraThickOutlineColorAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (extraThickOutlineWidth_RequireMaterialSet) input.SetFloat(_ExtraThickOutlineWidth, extraThickOutlineWidth);
            if (extraThickOutlineMaximumFinalWidth_RequireMaterialSet) input.SetFloat(_ExtraThickOutlineMaxFinalWidth, extraThickOutlineMaximumFinalWidth);
            if (extraThickOutlineViewSpacePosOffset_RequireMaterialSet) input.SetVector(_ExtraThickOutlineViewSpacePosOffset, extraThickOutlineViewSpacePosOffset);
            if (ExtraThickOutlineZTest_RequireMaterialSet) input.SetInt(_ExtraThickOutlineZTest, GetExtraThickOutlineZTest());
            if (extraThickOutlineStencilID_RequireMaterialSet) input.SetInt(_ExtraThickOutlineStencilID, extraThickOutlineStencilID);
            if (extraThickOutlineZOffset_RequireMaterialSet) input.SetFloat(_ExtraThickOutlineZOffset, extraThickOutlineZOffset);
            if (CharacterBoundCenter_RequireMaterialSet) input.SetVector(_CharacterBoundCenterPosWS, characterBoundCenter);
            if (PerspectiveRemovalAmount_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalAmount, GetPerspectiveRemovalAmount());
            if (PerspectiveRemovalRadius_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalRadius, GetPerspectiveRemovalRadius());
            if (PerspectiveRemovalStartHeight_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalStartHeight, GetPerspectiveRemovalStartHeight());
            if (PerspectiveRemovalEndHeight_RequireMaterialSet) input.SetFloat(_PerspectiveRemovalEndHeight, GetPerspectiveRemovalEndHeight());
            if (PerspectiveRemovalCenter_RequireMaterialSet) input.SetVector(_HeadBonePositionWS, perspectiveRemovealCenter);
            if (PerCharBaseColorTint_RequireMaterialSet) input.SetColor(_PerCharacterBaseColorTint, PerCharBaseColorTintAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (perCharacterOutlineWidthMultiply_RequireMaterialSet) input.SetFloat(_PerCharacterOutlineWidthMultiply, perCharacterOutlineWidthMultiply);
            if (PerCharOutlineColorTint_RequireMaterialSet) input.SetColor(_PerCharacterOutlineColorTint, PerCharOutlineColorTintAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (perCharacterOutlineEffectLerpColor_RequireMaterialSet) input.SetColor(_PerCharacterOutlineColorLerp, perCharacterOutlineEffectLerpColorAsVector); // DONT IGNORE THIS: for MPB, use SetColor instead of SetVector to make edit/play mode the same (don't want unity's auto gamma correction different for this Color)
            if (DitherFadeoutAmount_RequireMaterialSet) input.SetFloat(_DitherFadeoutAmount, GetDitherFadeoutAmount()); // need to always set, no matter keyword "_NILOTOON_DITHER_FADEOUT" is on or off (pass 1-x to shader to make preview window still render when shader is using 0 as _DitherFadeoutAmount's default value)
            if (ShouldEnableDepthTextureRimLightAndShadow_RequireKeywordChangeCall) input.SetFloat("_NiloToonEnableDepthTextureRimLightAndShadow", GetShouldEnableDepthTextureRimLightAndShadow() ? 1 : 0);
            //================================================================================================================================================

            // API EnableKeyword(...)/SetShaderPassEnabled(...) only works for Material class, so it only works in play mode
            //(X)
        }
    }
}


