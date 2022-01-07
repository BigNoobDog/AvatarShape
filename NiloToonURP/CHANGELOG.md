# Changelog
All notable changes to NiloToonURP & demo project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

- (Core) means the change is part of NiloToonURP_[version].unitypackage (files inside NiloToonURP folder)
- (Demo) means the change is only changing the demo project, without changing NiloToonURP.unitypackage
- (Doc) means the change is only affecting any .pdf documents
- (InternalCore) means Core, but if you are only using NiloToonURP as a tool, and don't need to read/edit NiloToonURP's source code/files, you can ignore this change.
----------------------------------------------
## [0.8.10] - 2021-10-13

### Changed
- (Demo) manifest.json: remove com.unity.toolchain.win-x86_64-linux-x86_64
----------------------------------------------
## [0.8.9] - 2021-10-11

### Added
- (Core) NiloToonCharacter.shader: added _FaceShadowGradientMapUVScaleOffset & _DebugFaceShadowGradientMap for "Face Shadow Gradient Map" section
----------------------------------------------
## [0.8.8] - 2021-10-10

### Added
- (Core) NiloToonCharacter.shader: add more tips and information for "Face Shadow Gradient Map" section

### Fixed
- (Core) NiloToonToonOutlinePass.cs: added isPreviewCamera check for screen space outline, to solve "material preview window makes outline flicker" bug
----------------------------------------------
## [0.8.7] - 2021-10-8

### Breaking Changes
- (Core) NiloToonCharacter.shader: fix a bug "normalmap can not affect lighting result"
----------------------------------------------
## [0.8.6] - 2021-10-4

### Breaking Changes
- (Core) NiloToonPerCharacterRenderController.cs: fix a gamma/linear color bug that cause play/edit mode color is not the same
----------------------------------------------
## [0.8.5] - 2021-9-29

### Added
- (Core) NiloToonCharacter.shader: added _DepthTexRimLightAndShadowWidthTexChannelMask and _OutlineWidthTexChannelMask option

### Fixed
- (Core) NiloToonEditorPerCharacterRenderControllerCustomEditor.cs: + null check
----------------------------------------------
## [0.8.4] - 2021-9-28

### Added
- (Core) NiloToonPerCharacterRenderController.cs: added allowCacheSystem, to let user control CPU optimization option
- (Core) NiloToonPerCharacterRenderController.cs: added API RequestForceMaterialUpdateOnce(), for user to call after changing material in playmode
- (Core) NiloToonPerCharacterRenderController.cs: OnEnable will now call RequestForceMaterialUpdateOnce()
----------------------------------------------
## [0.8.3] - 2021-9-27

### Breaking Changes
- (Core) NiloToonPerCharacterRenderController.cs: combine allowRenderDepthOnlyPass and allowRenderDepthNormalsPass into 1 public bool allowRenderDepthOnlyAndDepthNormalsPass

### Added
- (Core) NiloToonCharacter.shader: BaseMap Alpha Blending Layer 1 added UVScaleOffset, UVAnimSpeed, and optional mask texture. If it is useful, we will add these feature to other layers

### Changed
- (Core) remove keyword _NILOTOON_ENABLE_DEPTH_TEXTURE_RIMLIGHT_AND_SHADOW in C# and shader, replaced by a new material float _NiloToonEnableDepthTextureRimLightAndShadow. In order to trade a small amount of fragment performance for 50% lower shader memory usage
- (Core) NiloToonPerCharacterRenderController.cs: _NiloToonEnableDepthTextureRimLightAndShadow will not be enabled if allowRenderDepthOnlyAndDepthNormalsPass is false, to prevent wrong rim light result when user disabled allowRenderDepthOnlyAndDepthNormalsPass
- (Core) NiloToonPerCharacterRenderController.cs: auto set up button will not upgrade particle and sprite material anymore

### Fixed
- (Core) NiloToonPerCharacterRenderController.cs: now script will not miss material update(shadowTestIndex_RequireMaterialSet) due to cache logic when setting up a new character 
- (Core) fixed a bug that triggers useless Debug.warning when setting up a new character using auto button
----------------------------------------------
## [0.8.2] - 2021-9-23

### Added
- (Core) NiloToonCharacter.shader: added _DepthTexRimLightThresholdOffset and _DepthTexRimLightFadeoutRange, to let user control rim light area
----------------------------------------------
## [0.8.1] - 2021-9-21

### Added
- (Core) NiloToonCharacter.shader: added "Face shadow gradient map" section, allow user to control face shadow's visual by a special threshold gradient grayscale texture
- (Core) NiloToonPerCharacterRenderController.cs: added "Face Up Direction", for user to setup. (will affect material's "Face shadow gradient map" result)
- (Core) NiloToonEditorShaderStripping.cs will strip _FACE_SHADOW_GRADIENTMAP if _ISFACE is off also
- (Core) NiloToonPerCharacterRenderController: added allowRenderShadowCasterPass,allowRenderDepthOnlyPass,allowRenderDepthNormalsPass,allowRenderNiloToonSelfShadowCasterPass,allowRenderNiloToonPrepassBufferPass for user side optimization option
- (Demo) Update Ganyu.prefab and Klee.prefab's face rendering(using new "Face shadow gradient map" feature)

### Changed
- (Demo) upgrade URP from 10.5.0 to 10.5.1
- (Demo) optimize close shot scene's camera near far

### Fixed
- (Core) optimize NiloToonPerCharacterRenderController.LateUpdate()'s CPU time cost (only call material.SetXXX when value changed) and "IEnumerable<Renderer> AllRenderersIncludeAttachments()"'s GC
- (Demo) fix bug: "multiple incorrect renderer feature data in forward renderers"
----------------------------------------------
## [0.7.3] - 2021-9-12

### Changed
- (Core) NiloToonPerCharacterRenderController will not clear material property block every frame now

### Fixed
- (Core) correctly support 'ClothDynamics' asset
----------------------------------------------
## [0.7.2] - 2021-9-10

### Added
- (Core) NiloToonCharacter.shader: Emission section added _EmissionMapUseSingleChannelOnly and _EmissionMapSingleChannelMask
- (Core) NiloToonCharacter.shader: added "Support 'ClothDynamics' asset" section(just an on/off toggle)
- (InternalCore) Added NiloToonFullscreen_Shared.hlsl, to provide FullscreenVert() function for fullscreen shaders for URP9 or lower

### Changed
- (Core) When EnableDepthTextureRimLightAndShadow is off, low quality fallback rim light will NOT consider normalmap contribution anymore, which can produce rim light that is much more similar to depth texture rim light

### Fixed
- (Core) Fix shader bug to correctly support Unity 2019  
----------------------------------------------
## [0.7.1] - 2021-9-6

### Added
- (Core) Added NiloToonBloomVolume.cs, a bloom volume that is similar to URP's offical Bloom, but with more controls
- (Core) Added NiloToonRenderingPerformanceControlVolume .cs, a control volume that let you control performance per volume
- (Core) NiloToonCharacter.shader: Added "Allow NiloToonBloom Override?" section, and all required functions
- (InternalCore) NiloToonCharacter_Shared.shader: Added "Allow NiloToonBloom Override?" section required MACRO, uniforms and functions
- (InternalCore) Added new files NiloToonPrepassBufferRTPass.cs, NiloToonUberPostProcessPass.cs, NiloToonBloom.shader & NiloToonUberPost.shader
- (InternalCore) NiloToonAllInOneRendererFeature.cs now enqueue NiloToonPrepassBufferRTPass and NiloToonUberPostProcessPass
- (InternalCore) NiloToonCharacter.shader: Added NiloToonPrepassBuffer pass

### Changed
- (Core) NiloToonCharacter.shader: _CelShadeSoftness minimum value is now 0.001, instead of 0, to avoid lighting direction flipped.
- (Core) Remove large amount of useless Debug.Log of NiloToonEditor_AssetLabelAssetPostProcessor.cs
- (InternalCore) Delete files that are not currently using: MathUtility.cs and NiloToonShaderStrippingSettingSO.cs(a duplicated file)

### Fixed
- (Core) all character shader: all _DitherOpacity are now correctly replaced by_DitherFadeoutAmount, shader can compile correctly(material not pink anymore)
- (Core) fix NiloToonEditor_AssetLabelAssetPostProcessor's memory leak, which will produce fatal error when importing large amount of character
- (InternalCore) add namespace to NiloToonEditor_EditorLoopCleanUpTempAssetsGenerated.cs and NiloToonEditorSelectAllMaterialsWithNiloToonShader.cs

### TODO
- (Core) Optimize NiloToonPerCharacterRenderController.LateUpdate() (only call material.SetXXX when value changed)
----------------------------------------------
## [0.6.3] - 2021-8-27

### Added
- (Core) NiloToonCharacter.shader: Added a "BaseMap Stacking Layer 4-6" section
----------------------------------------------
## [0.6.2] - 2021-8-25

### Changed
- (Core) change _DitherOpacity to _DitherFadeoutAmount (not just rename, but with logic change). This will fix a bug -> preview window can't render nilotoon character shader
----------------------------------------------
## [0.6.1] - 2021-8-24

### Added
- (Core) NiloToonPerCharacterRenderController: Added a "Select all NiloToon_Character materials of this character" button
- (Core) Added NiloToonShaderStrippingSettingSO.cs, a scriptable object storing per platform NiloToon shader stripping settings
- (Core) NiloToonAllInOneRendererFeature: Added shaderStrippingSettingSO slot, user can assign a NiloToonShaderStrippingSettingSO to control NiloToon's shader stripping per platform
- (Core) NiloToonCharacter.shader: Added BaseMapStackingLayer section (1-3)
- (Core) NiloToonCharRenderingControlVolume: Added bool specularReactToLightDirectionChange
- (Core) NiloToonCharacter.shader: Added _NiloToonSelfShadowIntensityMultiplierTex in "Can Receive NiloToon Shadow?" section, useful if you want to control nilo self shadow intensity by a texture
- (Demo) Added NiloToonShaderStrippingSettingSO to project, which reduce all platform's shader memory usage by 50%, and android/iOS shader memory usage by 75%, this change will let demo apk run on low memory phones(1-2GB).
- (Demo) Added "Close Shot" scene, showing a close shot male character.
- (DOC) Added "How to separate IsFace, because I combine everything into 1 renderer/material" section
- (DOC) Added "Some DepthTexture shadow is missing if shadow caster is very close to shadow receiving surface"
- (DOC) Added a few section's about how to use volume to edit visual globally
- (DOC) Added "How to make specular results react to light direction change?" section

### Changed
- (Core) delete all NiloToonURPFirstTimeOpenProject folder, scripts and files
- (Doc) rewrite "NiloToon shader is using too much memory, how to reduce it?" section, to explain how to use NiloToonShaderStrippingSettingSO 

### Fixed
- (Core) Fixed a bug -> when dither fadeout is 0% (completely invisible), depth/depthNormal texture pass will not render
----------------------------------------------
## [0.5.1] - 2021-8-16

### Added
- (Core) NiloToonPerCharacterRenderController's dither fade out will now affect URP's shadowmap, will rely on URP's soft shadow filter to improve the final result.

### Changed
- (Demo) Now demo project will dynamic load character from Resources folder, instead of just placing every character in scene. This will reduce memory usage a lot, which helps android build(.apk) to prevent out of memory crash.
- (Demo) keepLoadedShadersAlive in PlayerSetting is now false, to save some memory usage on scene change

### Fixed
- (Core) Fix an important bug which NiloToonPerCharacterRenderController wrongly edit URP's default Lit material's shader. If any user were affected by version 0.4.1, please update to 0.5.1 and delete URP's Lit.material in the project's Library folder, doing this will trigger URP's regenerate default asset, which reset URP's Lit material and fixes the problem.
- (Core) Environment shader will NOT receive screen space outline if SurfaceType is Transparent
----------------------------------------------
## [0.4.1] - 2021-8-10

### Breaking Changes
- (Core) NiloToonPerCharacterRenderController.cs: rename extraThickOutlineMaxiumuFinalWidth -> extraThickOutlineMaximumFinalWidth

### Added
- (Core) NiloToonPerCharacterRenderController: Added "Auto setup this character" button, user can use click this button to quickly setup any character that is not using nilotoon
- (Demo) Added 4 new demo model, set up using NiloToon
- (Doc) Added section about "Auto setup this character" button
----------------------------------------------
## [0.3.5] - 2021-8-04

### Added
- (Core) NiloToonCharRenderingControlVolume: +rim light distance fadeout params, you can use it if you want to hide rim light flicker artifact due to not enough pixel count for character(resolution low / character far away from camera)

### Fixed
- (Core) now NiloToon will NOT leave any generated character .fbx in project (in older versions, NiloToon will generate .fbx that has (you can ignore or delete safely) prefix, polluting version control history)
----------------------------------------------
## [0.3.4] - 2021-8-02

### Added
- (Core) character shader: Occlusion section added _OcclusionStrengthIndirectMultiplier
- (Core) character shader: depthtex rim light and shadow width can now optionally controlled by texture and vertex color
- (Doc) added "How to enable character shader’s screen space outline? section

### Changed
- (Core) Now FrameDebugger will show NiloToon's passes correctly (Profiling scope)
- (Core) Now screen space outline can run on mobile platforms(android/iOS)
- (Core) screen space outline now support OpenGLES

### Fixed
- (Core) NiloToonPerCharacterRenderController will now auto re-find allRenderers, if any null renderer is detected in allRenderers list
- (Core) LWGUI will now correctly not saving _ keyword in material, which will also fix screen space outline constantly flicking in scene window unless user's mouse is moving on GUI
- (Core) fix _GlobalIndirectLightMinColor not applied correctly, which ignored occlusion map
----------------------------------------------
## [0.3.3] - 2021-7-19

### Added
- (Core) Environment shader/volume: +_NiloToonGlobalEnviSurfaceColorResultOverrideColor
- (Core) provide a new option in NiloToonAllInOneRendererFeature, "Perfect Culling For Shadow Casters", to fix terrain crash Unity bug
- (Doc) added section for "Perfect Culling For Shadow Casters (how to prevent terrain crash)"

### Changed
- (Core) character shader: rewrite "Ramp texture (specular)" section
- (Core) update SpecularRampTexForEroSkin.psd
----------------------------------------------
## [0.3.2] - 2021-7-11

### Added
- (Core) Improve XR rendering a lot! (by adding correct depth texture rim light and shadow in XR)
- (Core) NiloToonScreenSpaceOutlineControlVolume: add an extra outline width multiplier for XR
- (Core) NiloToonScreenSpaceOutlineControlVolume: add separated control for environment and character
- (Core) NiloToonCharacter shader: _LowSaturationFallbackColor's alpha can now control the intensity of _LowSaturationFallbackColor
- (Core) NiloToonCharacter shader: rewrite specular ramp method
- (Core) NiloToonPerCharacterRenderController: + _PerCharacterOutlineColorLerp
- (Core) NiloToonEnvironmentControlVolume: +_NiloToonGlobalEnviGITintColor , _NiloToonGlobalEnviGIAddColor , _NiloToonGlobalEnviShadowBorderTintColor
- (Demo) NiloToonEnviJPStreetScene now support playing in XR
- (Demo) Some example character models + MatCap(Additive) for metal reflection
- (Demo) Add version number on OnGUI (bottom right of the screen when playing)
- (Demo) Add UI text background transparent black quad, to make OnGUI text easier to read

### Fixed
- (Core) Fixed an environmment shader bug, NiloToonURP can support 2019.4 (URP 7.6.0) now
- (Core) Fixed a bug "After switching platform or reimport character model .fbx assets, baked smooth normal outline data in character model's uv8 will sometimes disappear"
- (Core) Fixed a bug "focusing on NiloToonPerCharacterRenderController is very slow due to wrongly call to AssetDataBase.Refresh()""
- (Core) Fixed a bug "NiloToonPerCharacterRenderController running very slow and allocate huge GC"
----------------------------------------------
## [0.3.1] - 2021-7-05

### Breaking Changes
- (Core) NiloToonCharacter_Shared.hlsl: rename struct LigthingData to ToonLightingData, so now NiloToonURP can also run on Unity2021.1 (URP11) or Unity2021.2 (URP12)
- (Core) NiloToonPerCharacterRenderController.cs: remove useCustomCharacterBoundCenter, now assigning customCharacterBoundCenter transform will treat as enable
- (Core) All screen space outline related feature, will be auto-disabled if running on mobile platform(android / iOS) due to performance and memory impact is high

### Known Issues
- (Core) when inspector is focusing on any NiloToon_Character materials, screen space outline in editor will always keep flickering in scene/game window
- [Done in 0.3.2] (Core) (this bug already exist in 0.2.4) After switching platform or reimport character model assets, baked smooth outline data in character model's uv8 will sometimes disappear. You will need to click on that character's NiloToonPerCharacterRenderController script to trigger a rebake, or click "Windows/NiloToonUrp/Model Label/Re-fix whole project!" button to rebake every character
- (Core) (this bug already exist in 0.2.4) Sometime (you can ignore or delete it safely)xxx.fbx will be generated in project, and not correctly deleted by NiloToonURP automatically    

### Added
- (Core) Added NiloToonEnvironment.shader (Universal Render Pipeline/NiloToon/NiloToon_Environment), you can switch any URP's Lit shader to this shader, they are early stage proof of concept toon shader for environment (Don't use it for production! still WIP/in proof of concept stage, will change a lot in future). Added these envi shader because lots of customers request to include it first(even the shader is far from complete)
- (Core) Added NiloToonEnvironmentControlVolume.cs, to provide extra per volume control for NiloToonEnvironment.shader
- (Core) Added NiloToonScreenSpaceOutlineControlVolume.cs, for controlling NiloToonCharacter.shader and NiloToonEnvironment.shader's screen space outline's settings
- (Core) NiloToonToonOutlinePass.cs added "AllowRenderScreenSpaceOutline" option, you can enable it in NiloToonAllInOne renderer feature if you need to render any screen space outline(default is off)
- (Core) Character shader: Added a WIP and experimental "Ramp Texture (Specular) section"
- (Demo) Add NiloToonEnviJPStreetScene.scene (and all related assets), it is a new scene to demo the new environment shader
- (Doc) Added CustomCharacterBoundCenter section
- (Doc) Added a simple environment shader's document

### Changed
- (Core) now support URP11.0 and URP12.0, because all NiloToon shader renamed struct LightingData to ToonLightingData (solving Shader error in URP11 -> 'Universal Render Pipeline/NiloToon/NiloToon_Character': redefinition of 'LightingData')
- (Core) Character shader: screen space outline(experimental) section completely rewrite (new algorithm, adding depthSensitifity texture, normalsSensitifity, normalsSensitifity texture, extra control for face....)
- (Core) Character shader: screen space outline use multi_compile now(to allow on off by different quality setting using renderer feature)
- (Core) Character shader: rename Outline to Traditional Outline, to better separate it from Screen space outline
- (Core) make screen space outline not affected by RenderScale,screen resolution (scale outline size to match resoltion)
- (Core) global uniform "_GlobalAspectFix" now use camera RT width height as input, instead of screen width height
- (Core) extract screen space outline's code to a new .hlsl (now shared by character shader and environment shader)
- (Core) NiloToonPerCharacterRenderController: optimize shader memory and build time for mobile, but increase GPU shading pressure a bit
- (Core) NiloToonPerCharacterRenderController: optimize C# cpu time spent, and reduce GC 
- (Core) NiloToonPerCharacterRenderController: better warning in Status section, if something not set up correctly 
- (Core) NiloToonEditorShaderStripping.cs: screen space outline will get stripped when building for mobile(android / iOS)
- (Demo) upgrade demo project to Unity 2020.3.12f1
- (Demo) Upgrade demo project to URP 10.5

### Fixed
- (Core) NiloToonEditor_AssetLabelAssetPostProcessor: handle KeyNotFoundException: The given key was not present in the dictionary. when reimporting a model. now will produce a warning message for tracking which model will produce this error
----------------------------------------------
## [0.2.4] - 2021-6-23

### Added
- (Core)NiloToonPerCharacterRenderController: Add "attachmentRendererList" for user to attach any other renderer to the current character, these attachment renderers will use that NiloToonPerCharacterRenderController script's setting(e.g. sync weapon/microphone's perspective removal with a character) 

### Changed
- (Core)NiloToonPerCharacterRenderController: better ToolTip
- (Core)GenericStencilUnlit shader: rewrite to support attachmentRendererList and SRP batching

### Fixed
- (Core)NiloToonPerCharacterRenderController: handle null renderer

----------------------------------------------
## [0.2.3] - 2021-6-21

### Added
- (Core)Character shader: Add "Override Shadow Color by texture" section
- (Core)Character shader: Add "Override Outline Color by texture" section

### Changed
- (Core)Character shader: better material GUI

### Fixed
- (Core)Character shader: GGX specular use float instead of half to avoid precision problem on mobile platform
- GenericStencilUnlit.shader support SRP batching and VR
----------------------------------------------
## [0.2.2] - 2021-6-16

### Changed
- (Core)Character shader: better material GUI's note
### Fixed
- (Core)Character shader: fixed a bug which makes SRP batching not working correctly
- (Core)Character shader: fixed a bug which makes Detail albedo not used by specular amd emission

----------------------------------------------
## [0.2.1] - 2021-6-15

### Breaking Changes
- (Core)(IMPORTANT)Character shader: not using all roughness related settings in "Specular" section anymore, add a new shared "Smoothness" section. This change is made to make NiloToon character shader matches URP Lit.shader's smoothness data convention, and now "Smoothness" section's data can be shared/used by multiple features such as "Environment reflection" and "Specular(GGX)". ***If you are using "Specular" section's roughness setting in your project's materials already, after this update you will have to set up it again in the new "Smoothness" section***
- (Core)(IMPORTANT)Character shader: "Matcap Mask" section merged into "MatCap(alpha blend)" and "MatCap(additive)" section. This change is made due to the old design will force user to combine 2 mask textures into 1 texture, which is not flexible enough. This change will break old materials's "Matcap Mask" section. ***If you are using "MatCap mask" section's settings in your project's materials already, you will have to set up it again in the "MatCap(alpha blend)" and "MatCap(additive)" section's Optional mask setting***
- (Core)rename class NiloToonPerCharacterRenderControllerOverrider to NiloToonCharacterRenderOverrider, to avoid confusion when adding a NiloToonCharacterRenderOverrider script to GameObject
- (Core)rename shader internal MACRO NiloToonIsOutlinePass to NiloToonIsAnyOutlinePass, you can ignore this change if you didn't edit NiloToon's shader code
- (Core)rename shader internal MACRO NiloToonIsColorPass to NiloToonIsAnyLitColorPass, you can ignore this change if you didn't edit NiloToon's shader code

### Added
- (Core)(IMPORTANT): NiloToonAllInOneRendererFeature and NiloToonShadowControlVolume added a new toggle "useMainLightAsCastShadowDirection", you can enable it if you want NiloToon's self shadow system use scene's MainLight direction to cast shadow(same shadow casting direction as regular URP main light shadow, which means shadow result will NOT be affected by camera rotation/movement) 
- (Core)Add GenericStencilUnlit.shader, useful if you want to apply stencil effects that uses drawn character pixels as a stencil mask
- (Core)Character shader: in "Specular" section, add _MultiplyBaseColorToSpecularColor slider, useful if you want to mix base color into specular result
- (Core)Character shader: in "Specular" section, add "Extra Tint by Texture" option, useful if you want to mix any texture into specular result
- (Core)Character shader: in "Emission" section, add _EmissionIntensity, _MultiplyBaseColorToEmissionColor, to allow more Emission color control
- (Core)Character shader: add ColorMask option (RGBA or RGB_), useful if you don't want to pollute RenderTexture's alpha channel for semi-transparent materials
- (Core)Character shader: MatCap(additive) add _MatCapAdditiveMaskMapChannelMask and _MatCapAdditiveMaskMap's remap minmax slider
- (Core)Character shader: MatCap(alpha blend) add _MatCapAlphaBlendMaskMapChannelMask and _MatCapAlphaBlendMaskMap's remap minmax slider
- (Core)Character shader: MatCap(alpha blend) add _MatCapAlphaBlendTintColor and _MatCapAlphaBlendMapAlphaAsMask
- (Core)Character shader: add a new "Environment Reflections" section
- (Core)Character shader: in "Lighting Style" section,added _IndirectLightFlatten, to allow more control on how to display lightprobe result
- (Core)Character shader: in "Outline" section, added _UnityCameraDepthTextureWriteOutlineExtrudedPosition, you can disable it to help removing weird 2D white line artifact on material
- (Core)Character shader: in "Can receive NiloToon Shadow?" section, add _NiloToonSelfShadowIntensityForNonFace(Default 1) and _NiloToonSelfShadowIntensityForFace(Default 0). Face can receive NiloToon's self shadow map now.
- (DEMO)Add MMD4Mecanim folder
- (DEMO)Add some MMD model for testing(only exist in project window)
- (DEMO)Bike Mb1MotorMd000001 add a prefab variant for showing "Environment Reflection" material

### Changed
- (Core)Character shader: hide stencil option (stencil options are not being used in all versions)
- (Core)Sticker shader: use ColorMask RGB now, to not pollute RT's alpha channel
- (Core)NiloToonAnimePostProcessPass set ScriptableRenderPass.renderPassEvent = XXX directly, internally NiloToonAllInOneRendererFeature don't need to create 2 renderpass now 
- (Core)Internal shader code big refactor without changing visual result, if you didn't edit NiloToon's shader source code, you can ignore this change
- (DEMO)Update model materials (IsSkin and Smoothness)
- (DEMO)CRS scene use main light as NiloToon self shadow casting direction (enable useMainLightAsCastShadowDirection in NiloToonSelfShadowVolume)

### Fixed
- (Core)Character shader: fixed a bug where _OcclusionStrength and _GlobalOcclusionStrength is not applied in a correct order
- (Core)Debug window: Fix a bug where NiloToon Debug window always wrongly focus project/scene window
- (Core)Anime postProcess shader: Fix Hidden/NiloToon/AnimePostProcess produce "framgent shader output doesn't have enough component" error in Metal graphics API
----------------------------------------------
## [0.1.3] - 2021-5-19

### Breaking Changes
- (Core)Change namespace of all NiloToonURP C# script to "using NiloToon.NiloToonURP;", please delete your old NiloToonURP folder first before importing the updated NiloToonURP.unitypackage
- (Core)Change shader path of Character shader to "Universal Render Pipeline/NiloToon/NiloToon_Character"
- (Core)Change shader path of Sticker shader to "Universal Render Pipeline/NiloToon/xxx"

### Known Issues
- (Demo)Planar reflection in CRS scene (VR mode) is not correct

### Added
- (Core)Character Shader: Add "IsSkin?" toggle in material, you can enable this toggle if a material is skin(hand/leg/body...), enable it will make the shader use an optional overrided shadow color for skin, optional skin mask can be enabled also if your material is a mix of skin and cloth
- (Core)Character Shader: Add _OverrideByFaceShadowTintColor,_OverrideBySkinShadowTintColor,_OutlineWidthExtraMultiplier
- (Core)Character Shader: Add _ZOffsetEnable to allow on off ZOffset by a toggle
- (Core)Character Shader: Add _EditFinalOutputAlphaEnable to allow on off EditFinalAlphaOuput by a toggle
- (Core)Character Shader: Add _EnableNiloToonSelfShadowMapping to allow on off NiloToonSelfShadowMapping by a toggle
- (Core)Character Shader: Add _NiloToonSelfShadowMappingDepthBias to allow edit NiloToonSelfShadowMapping's depth bias per material
- (Core)Character Shader: Add RGBAverage and RGBLuminance to RGBAChannelMaskToVec4Drawer, you will see it if you click the RGBA channel drop list in material UI
- (Core)Sticker Shader: Add override alpha by a texture (optional)
- (Core)Correctly support Unity 2019.4.0f1 or above (need to use URP 7.4.1 or above, NOT URP 7.3.1, you can upgrade URP to 7.4.1 in the package manager)
- (Core)Add NiloToonPlanarReflectionHelper.cs, for planar reflection camera support, you need to call NiloToonPlanarReflectionHelper.cs's function in C# when rendering your planar reflection camera (see MirrorReflection.cs in CRS demo scene), user document has a new section about it, see "When rendering NiloToon shader in planar reflection camera, some part of the model disappeared"
- (Core)Add NiloToonPerCharacterRenderControllerOverrider.cs, to sync perspective removal result from a source to a group of characters
- (Core)Add SimpleLit debug option in NiloToon debug window
- (Core)NiloToonAllInOneRendererFeature: Add useNdotLFix to hide self shadowmap artifact, you can turn it off if you don't like it
- (Core)NiloToonCharRenderingControlVolume: + depthTextureRimLightAndShadowWidthMultiplier
- (Core)NiloToonPerCharacterRenderController: + perCharacterOutlineWidthMultiply , perCharacterOutlineColorTint
- (Demo)Add CRS(Candy rock star) demo scene, with a planar reflection script(MirrorReflection.cs)
- (Demo)Add more demo models
- (Demo)Add .vrm file auto prefab generation editor script, inside ThirdParty(VRM) folder. You can drag .vrm files into demo project and prefab will be generated
- (Demo)Add 4ExtremeHighQuality and 5HighestQuality quality settings, PC build now default use 4ExtremeHighQuality
- (Doc)Add section for how to correctly use NiloToonURP in 2019.4 (need to enable URP's depth texture manually and install URP 7.4.1 or above) 
- (Doc)Add section for setting up semi-transparent alpha blending material
- (Doc)Add section for changing VRM(MToon)/RealToon material to NiloToon material

### Changed
- (Core)remove _vertex and _fragment suffixes in multi_compile and shader_compile, in order to support 2019.4 correctly
- (Core)Character Shader: Now the minimum supported OpenGLES version is 3.1, not 3.0, due to support SRP Batching correctly
- (Core)package.json: Now the minimum supported URP version is 7.4.1, not 7.6.0
- (Core)package.json: Now the minimum supported editor version is 2019.4.0f1, not 2019.4.25f1
- (Core)NiloToonPerCharacterRenderController.cs: now auto disable perspective removal in XR
- (Core)Smooth normal editor baking: Will skip baking if model don't have correct tangent, and better import error message if model don't have tangent data
- (Core)if _ZWrite = 0, disable all _CameraDepthTexture related effects (e.g.auto disable depth texture 2D rim light of a ZOffset enabled eyebrow material)
- (Core)now use global keyword SHOULD_STRIP_FORCE_MINIMUM_SHADER in demo's enable NiloToon toggle
- (Demo)change demo script to use namespace using NiloToon.NiloToonURP;
- (Demo)now limit Screen height to maximum 1080, to increase fps in 4k monitors
- (Demo)optimize some model's texture max resolution for android, to avoid using too much memory in .apk
- (Doc)improve user document with more FAQ

### Fixed
- (Core)fix SRP batcher mode linear gamma bug
- (Core)fixed NiloToonCharacterSticker shaders return alpha not equals 1 problem, these shader will not pollute RT's alpha channel anymore (always return alpha == 1) 
- (Core)fixed some shader and C# warning (no harm)
- (Core)fixed a bug that make SRP batcher not working (add _PerCharacterBaseColorTint in NiloToonCharacter.shader's Properties section)
----------------------------------------------
## [0.1.2] - 2021-5-3

### Added
- (Core)Add NiloToonShadowControlVolume for Volume, you can use it to control and override shadow settings per volume instead of editing NiloToonAllInOneRendererFeature directly
- (Core)All shaders add basic XR support, but many shader features are now auto disabled in XR in this version temporarily, because we are fixing them
- (Core)In XR, due to high fov, outline width is default 50% (only in XR), you can change this number in NiloToonCharRenderingControlVolume or NiloToonAllInOneRendererFeature
- (Core)character shader: Add DepthNormal pass, URP's SSAO renderer feature(DepthNormals mode) will work correctly now
- (Core)character shader: Add "screen space outline" feature in material (experimental, need URP's _CameraDepthTexture enabled), enable it in material UI will add more detail outline to character, useful for alphaclip materials where the default outline looks bad
- (Core)character shader: Add "Dynamic eye" feature in material, for users who need circular dynamic eye pupil control
- (Core)character shader: Add VertExmotion support, you can enable it in NiloToonCharacter_ExtendDefinesForExternalAsset.hlsl
- (Core)character shader: Add _ZOffsetMaskMapChannelMask,_ExtraThickOutlineMaxFinalWidth,_DepthTexShadowThresholdOffset,_DepthTexShadowFadeoutRange,_GlobalMainLightURPShadowAsDirectResultTintColor in material
- (Core)NiloToonAnimePostProcessVolume: add anime postprocess effect draw height and draw timing control
- (Core)Add per character and per volume extra BaseColor tint
- (Core)Can install from PackageManager(install from disk) using NiloToonURP folder's package.json file
- (Core)Add auto reimport and message box when using NiloToonURP the first time
- (Demo)All demo shader add XR support
- (Demo)All scene add steam XR support, see user document pdf for instruction on how to try it in editor play mode (steam PCVR)
- (Demo)Add NiloToonDemoDanceScene_UI.cs, adding more user control in NiloToonDancingDemo.unity

### Changed
- (Core)NiloToonAnimePostProcess: Reduce default top light intensity from 100% to 75%
- (Core)improve UI display and tooltips
- (Demo)player settings remove Vulkan API for Android, now always use OpenGLES3 to support testing on  more devices
- (Demo)UnityChan edit skin material to have better shadow color(in 0.1.1 shadow color is too dirty and grey) 
- (Doc)improve user document with more FAQ

### Fixed
- (Core) Fixed bake smooth normal UV8 index out of bound exception if .fbx has no tangent, will now produce error log only
- (Core) Fixed NoV rimlight V matrix not corerct bug (now use UNITY_MATRIX_V instead of unity_CameraToWorld)
- (Core) Fixed a missing abs() bug in NiloZOffsetUtil.hlsl
- (Demo) Fixed multi AdvanceFPS script in scene bug
----------------------------------------------
## [0.1.1] - 2021-4-19

### Added
- (Core)Add change log file (this file)
- (Core)character shader: Add "MatCap (blend,add and mask)" section, which mix MatCap textures to Base map
- (Core)character shader: Add "Ramp lighting texture" section, which override most lighting params by a ramp texture
- (Core)character shader: Add "Final output alpha" section, to allow user render opaque character to custom RT without alpha problem
- (Core)character shader: "Detail Maps" section add uv2 toggle and albedo texture white point slider
- (Core)character shader: "Calculate Shadow Color" section add _FaceShadowTintColor
- (Core)character shader: "Calculate Shadow Color" section add _LitToShadowTransitionAreaTintColor, LitToShadowTransitionArea HSV edit 
- (Core)character shader: "Outline" section add _OutlineOcclusionAreaTintColor, _OutlineReplaceColor
- (Core)character shader: "Depth texture shadow" section add _DepthTexShadowUsage
- (Core)character shader: NiloToonCharacter_ExtendFunctionsForUserCustomLogic.hlsl add more empty functions for user
- (Core)character shader: Add support to VertExmotion asset (no change to NiloToon shader name) (add NiloToonCharacter_ExtendDefinesForExternalAsset.hlsl for user to enable VertExmotion support if they need it)
- (Core)volume: NiloToonCharRenderingControlVolume.cs add Directional Light, Additional Light, Specular's volume control to allow better control light intensity on character
- (Core)character root script: NiloToonPerCharacterRenderController.cs Add perCharacterDesaturation 
- (Core)character root script: NiloToonPerCharacterRenderController.cs Add extra think outline view space pos offset, usually for stylized color 2D drop shadow
- (Demo)Add GBVS Narmaya model and setup her using NiloToon (face material not ready for 360 light rotation)
- (Demo)Add NiloToonCutinScene.unity to show GBVS Narmaya model
- (Demo)Add bike (using NiloToonURP) in NiloToonSampleScene.unity
- (Demo)Include XR related files, preparing for future XR support
- (Demo)Add 4HighestQuality, it will now ignore performance limit, using the best possible setting (only for PC build, WebGL and editor)

### Changed
- (Core)NiloToonAllInOneRendererFeature: Unlock self shadow's shadow map resolution limit from 4096 to 8192
- (Core)NiloToonAverageShadowTestRT.shader: add sampling pos offset to prevent over generating average shadow due to near by objects/characters
- (Core)revert indirectLightMultiplier's default setting from 2 to 1
- (Demo)Upgraded dmeo project to 2020.3.4f1
- (Demo)Optimize 0~3 quality settings for mobile, while allow best quality(4) for PC/Editor/WebGL
- (Demo)Allow every scene to switch to another in play mode, no matter user start playing in which scene
- (Demo)Refactor and rename some files and folder

### Fixed
- (Core)character root script: NiloToonPerCharacterRenderController.cs fix can't edit material in pause play mode bug

### Know Issues
- (Core)Depth texture rim light is not correct in PCVR Editor
- (Core)Shader variant(memory usage) too much in mobile build
- (Core)Outline render incorrectly if camera is too close
- [DONE in 0.3.2] (Core)per character scirpt's editor script is too slow 
- (Demo)Switching scene in editor play mode takes a very long time, due to editor script

### TODO
- (Core)Finish detail document on material properties, NiloToon UI params
- (Core)Charcter shader can't receive point or spot light shadow (need to support URP11 first, since point light shadow only exist in URP11)
- [DONE in 0.1.2] (Core)depth texture sobel outline shader_feature
- [DONE in 0.1.2] (Core)control URP shadow intensity remove directional light contribution (as volume)
- (Demo)Add moving tree shadowmap demo scene
- (Demo)GBVS Narmaya's face material not yet setup correctly to support all light direction
----------------------------------------------
## [0.0.3] - 2021-4-06
### Added
- (Demo)Add MaGirlHeadJsonTransformSetter.cs
- (Demo)Add random face & ear animation for MaGirl using MaGirlHeadJsonTransformSetter.cs 
----------------------------------------------
## [0.0.2] - 2021-4-03
### Added
- (Demo)Add MaGirl3.prefab and it's variants, setup her using NiloToon
- (Demo)Add MaGirl2.prefab and it's variants, setup her using NiloToon
- (Demo)Add NiloToonDancingDemo.unity scene
----------------------------------------------
## [0.0.1] - 2021-3-28
### Added
- (Core)First version to record in change log.
- (Demo)First version to record in change log.