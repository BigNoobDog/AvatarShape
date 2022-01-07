using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal;

namespace NiloToon.NiloToonURP
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NiloToonPerCharacterRenderController))]

    public class NiloToonEditorPerCharacterRenderControllerCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var t = (target as NiloToonPerCharacterRenderController);
            ReimportSingleCharIfNeeded(t);

            // brute force: force all character in scene update 
            foreach (var script in FindObjectsOfType<NiloToonPerCharacterRenderController>())
                ReimportSingleCharIfNeeded(script);

            NiloToonEditor_ReimportAllAssetFilteredByLabel.DeleteAllTempMeshAssetCloneWithCanDeletePrefix(); // auto clean up project

            DrawAutoSetupButton(t);
            DrawSelectAllNiloToonCharacterMaterialsButton(t);

            RenderMessage(t);

            ///////////////////////////////////////////////////
            // draw original
            ///////////////////////////////////////////////////
            base.OnInspectorGUI();
        }

        private void ReimportSingleCharIfNeeded(NiloToonPerCharacterRenderController t)
        {
            AutoAssignLabelToAllMeshsOfSingleChar(t);
            AutoReimportIfMeshDontHaveUV8(t);
        }

        private static void AutoReimportIfMeshDontHaveUV8(NiloToonPerCharacterRenderController perCharScript)
        {
            if (perCharScript.reimportDone) return;

            foreach (var renderer in perCharScript.allRenderers)
            {
                if (renderer == null) continue;

                Mesh mesh = null;
                switch (renderer)
                {
                    case MeshRenderer mr: mesh = mr.GetComponent<MeshFilter>().sharedMesh; break;
                    case SkinnedMeshRenderer smr: mesh = smr.sharedMesh; ; break;
                    default:
                        break; // do nothing if not a supported renderer(e.g. particle system's renderer)
                }

                if (mesh == null) continue;

                // if uv8 is not the correct smooth normal data, AddAssetLabelAndReimport
                if (mesh.uv8 == null || mesh.uv8.Length == 0)
                {
                    NiloToonEditor_AssetLabelAssetPostProcessor.AddAssetLabelAndReimport(new UnityEngine.Object[] { mesh }, false); // use false = always reimport
                    perCharScript.reimportDone = true;
                }
            }
        }

        public static void AutoAssignLabelToAllMeshsOfSingleChar(NiloToonPerCharacterRenderController perCharScript, bool shouldSkipIfLabelExist = true)
        {
            foreach (var renderer in perCharScript.allRenderers)
            {
                Mesh mesh = null;
                switch (renderer)
                {
                    case MeshRenderer mr: mesh = mr.GetComponent<MeshFilter>().sharedMesh; break;
                    case SkinnedMeshRenderer smr: mesh = smr.sharedMesh; ; break;
                    default:
                        break; // do nothing if not a supported renderer(e.g. particle system's renderer)
                }
                NiloToonEditor_AssetLabelAssetPostProcessor.AddAssetLabelAndReimport(new UnityEngine.Object[] { mesh }, shouldSkipIfLabelExist);
            }
        }

        private void DrawAutoSetupButton(NiloToonPerCharacterRenderController perCharScript)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Auto setup this character"))
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // collect all materials used by this character(don't care what the shader is)
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                List<Material> allMaterials = new List<Material>();
                foreach (var renderer in perCharScript.GetComponentsInChildren<Renderer>())
                {
                    foreach (var mat in renderer.sharedMaterials)
                        allMaterials.Add(mat);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // if material is not URP's shader, it should be using a built-in RP's shader, switch material's shader to "Standard" 
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                foreach (var mat in allMaterials)
                {
                    // if material is not URP's shader, it won't have _BaseMap(e.g. vrm/mtoon or realtoon)  
                    if (!mat.GetTexture("_BaseMap"))
                    {
                        // switch to built-in RP's standard shader first, prepare for URP's material upgrade
                        mat.shader = Shader.Find("Standard");
                        EditorUtility.SetDirty(mat);
                    }
                }
                AssetDatabase.SaveAssets();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // if material is not a URP's shader, it won't have _BaseMap(e.g. vrm/mtoon or realtoon or any error shader material),  
                // in that case, we similate a click to "Edit/Render Pipeline/Universal Render Pipeline/Upgrade Selected Materials to UniversalRP Materials",
                // which means calling to UniversalRenderPipelineMaterialUpgrader.UpgradeSelectedMaterials().
                // But UniversalRenderPipelineMaterialUpgrader.UpgradeSelectedMaterials() is internal, so we copied it here
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                List<MaterialUpgrader> upgraders = new List<MaterialUpgrader>();
                GetUpgraders(ref upgraders);

                HashSet<string> shaderNamesToIgnore = new HashSet<string>();
                GetShaderNamesToIgnore(ref shaderNamesToIgnore);

                // set selection to materials that we want to upgrade
                UnityEngine.Object[] targets = new UnityEngine.Object[allMaterials.Count];
                for (int i = 0; i < allMaterials.Count; i++)
                {
                    targets[i] = allMaterials[i];
                }
                var originalSelectionObjects = Selection.objects;
                Selection.objects = targets;

                // call SRP's UpgradeSelection()
                MaterialUpgrader.UpgradeSelection(upgraders, shaderNamesToIgnore, "Upgrade to UniversalRP Materials", MaterialUpgrader.UpgradeFlags.LogMessageWhenNoUpgraderFound);

                // restore Selection
                Selection.objects = originalSelectionObjects;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // switch material to use NiloToonURP character shader, set IsFace and IsSkin also
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Shader niloToonCharShader = Shader.Find("Universal Render Pipeline/NiloToon/NiloToon_Character");
                Shader URPLitShader = Shader.Find("Universal Render Pipeline/Lit");
                foreach (var m in allMaterials)
                {
                    // [auto change mateiral's shader (switch URP lit to NiloToon char)]

                    // if the current material is URP's default Lit material(name is "Lit"), skip it, else we will be editing URP's default material
                    if (m.shader == URPLitShader && m.name == "Lit")
                    {
                        continue;
                    }
                    // if the current shader is not a nilotoon's shader, and it is not an Particle/Sprite shader, switch this shader to nilotoon character's shader
                    if (!m.shader.name.Contains("NiloToon") && !m.shader.name.Contains("Particle") && !m.shader.name.Contains("Sprite"))
                    {
                        m.shader = niloToonCharShader;
                        EditorUtility.SetDirty(m);
                    }
                    // if the current shader is nilotoon character's shader(but not sticker shader)
                    if (m.shader == niloToonCharShader)
                    {
                        //auto enable IsFace if material name contain "face","eye" or "mouth"
                        bool isFace = m.name.IndexOf("face", StringComparison.OrdinalIgnoreCase) >= 0;
                        isFace |= m.name.IndexOf("eye", StringComparison.OrdinalIgnoreCase) >= 0;
                        isFace |= m.name.IndexOf("mouth", StringComparison.OrdinalIgnoreCase) >= 0;
                        if (isFace)
                        {
                            // [skin]
                            m.SetFloat("_IsSkin", 1);
                            // no keyword on/off is needed for _IsSkin

                            // [face]
                            m.SetFloat("_IsFace", 1);
                            m.EnableKeyword("_ISFACE");
                        }

                        //auto enable IsSkin if material name contain "skin"
                        bool isSkin = m.name.IndexOf("skin", StringComparison.OrdinalIgnoreCase) >= 0;
                        if (isSkin)
                        {
                            // [skin]
                            m.SetFloat("_IsSkin", 1);
                            // no keyword on/off is needed for _IsSkin
                        }
                        EditorUtility.SetDirty(m);
                    }
                }
                AssetDatabase.SaveAssets();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // misc settings
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Transform depthSearch(Transform parent, string targetName)
                {
                    foreach (Transform child in parent)
                    {
                        if (child.name.IndexOf(targetName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return child;
                        }

                        var result = depthSearch(child, targetName);
                        if (result != null)
                            return result;
                    }

                    // find nothing
                    return null;
                }
                // set character center bone (name = pelvis/hip), depth first search
                if (!perCharScript.customCharacterBoundCenter)
                    perCharScript.customCharacterBoundCenter = depthSearch(perCharScript.transform, "hip");
                if (!perCharScript.customCharacterBoundCenter)
                    perCharScript.customCharacterBoundCenter = depthSearch(perCharScript.transform, "pelvis");
                // set head bone (name = head), depth first search
                if (!perCharScript.headBoneTransform)
                    perCharScript.headBoneTransform = depthSearch(perCharScript.transform, "head");

                // reset allRenderers list to trigger re-find
                perCharScript.allRenderers.Clear();

                // let unity know our edit
                EditorUtility.SetDirty(perCharScript);

                Debug.Log($"Auto setup {perCharScript.gameObject.name} DONE!");
            }
            GUILayout.Space(10);
        }

        private void DrawSelectAllNiloToonCharacterMaterialsButton(NiloToonPerCharacterRenderController t)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Select all NiloToon_Character materials of this character"))
            {
                List<Material> materialList = new List<Material>();
                Shader niloToonCharacterShader = Shader.Find("Universal Render Pipeline/NiloToon/NiloToon_Character");
                foreach (var renderer in t.allRenderers)
                {
                    if (!renderer) continue;

                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (!material) continue;

                        if (material.shader == niloToonCharacterShader)
                        {
                            materialList.Add(material);
                        }
                    }
                }

                Selection.objects = materialList.ToArray();
            }
        }

        private static void RenderMessage(NiloToonPerCharacterRenderController t)
        {
            if (Selection.gameObjects.Length == 1)
            {
                // produce yellow warnings
                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.black;
                EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------------------------------------------------------------", style);

                bool isSetupCorrect = true;
                style.normal.textColor = Color.yellow;

                if (!t.headBoneTransform)
                {
                    EditorGUILayout.LabelField("(recommend action!) ", style);
                    EditorGUILayout.LabelField("You can assign 'Head Bone Transform' with a head bone in 'Set up - Head Bone and Face forward direction' section", style);
                    EditorGUILayout.LabelField("Doing this can make Face lighting fix and perspective removal possible", style);
                    EditorGUILayout.LabelField(""); // empty line
                    isSetupCorrect = false;
                }

                if (!t.customCharacterBoundCenter)
                {
                    EditorGUILayout.LabelField("(recommend action!) ", style);
                    EditorGUILayout.LabelField("You can assign 'Custom Character Bound Center' with a hip / pelvis bone in 'Set up - Override Bounding Sphere' section", style);
                    EditorGUILayout.LabelField("Doing this can make CPU much faster, else this script will rebuild realtime character bound center every frame(slow)", style);
                    EditorGUILayout.LabelField(""); // empty line
                    isSetupCorrect = false;
                }

                // separate line
                style.normal.textColor = Color.black;
                EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------------------------------------------------------------", style);

                // Final Status message
                if (isSetupCorrect)
                {
                    style.normal.textColor = Color.green;
                    EditorGUILayout.LabelField("Status: All setup correct", style);
                }
                else
                {
                    style.normal.textColor = Color.yellow;
                    EditorGUILayout.LabelField("Status: Setup not complete, recommend solving the above warnings", style);
                }

                // separate line
                style.normal.textColor = Color.black;
                EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------------------------------------------------------------", style);
            }
            else
            {
                EditorGUILayout.LabelField("Selecting multi objects, message hidden now");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // copy from UniversalRenderPipelineMaterialUpgrader.cs
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void GetShaderNamesToIgnore(ref HashSet<string> shadersToIgnore)
        {
            shadersToIgnore.Add("Universal Render Pipeline/Baked Lit");
            shadersToIgnore.Add("Universal Render Pipeline/Lit");
            shadersToIgnore.Add("Universal Render Pipeline/Particles/Lit");
            shadersToIgnore.Add("Universal Render Pipeline/Particles/Simple Lit");
            shadersToIgnore.Add("Universal Render Pipeline/Particles/Unlit");
            shadersToIgnore.Add("Universal Render Pipeline/Simple Lit");
            shadersToIgnore.Add("Universal Render Pipeline/Nature/SpeedTree7");
            shadersToIgnore.Add("Universal Render Pipeline/Nature/SpeedTree7 Billboard");
            shadersToIgnore.Add("Universal Render Pipeline/Nature/SpeedTree8");
            shadersToIgnore.Add("Universal Render Pipeline/2D/Sprite-Lit-Default");
            shadersToIgnore.Add("Universal Render Pipeline/Terrain/Lit");
            shadersToIgnore.Add("Universal Render Pipeline/Unlit");
            shadersToIgnore.Add("Sprites/Default");

            // NiloToonURP added all character shader to ignore list:
            shadersToIgnore.Add("Universal Render Pipeline/NiloToon/NiloToon_Character");
            shadersToIgnore.Add("Universal Render Pipeline/NiloToon/NiloToon_Character Sticker(Multiply)");
            shadersToIgnore.Add("Universal Render Pipeline/NiloToon/NiloToon_Character Sticker(Additive)");
        }
        private static void GetUpgraders(ref List<MaterialUpgrader> upgraders)
        {
            /////////////////////////////////////
            //     Unity Standard Upgraders    //
            /////////////////////////////////////
            upgraders.Add(new StandardUpgrader("Standard"));
            upgraders.Add(new StandardUpgrader("Standard (Specular setup)"));

            ////////////////////////////////////
            // Particle Upgraders             //
            ////////////////////////////////////
            upgraders.Add(new ParticleUpgrader("Particles/Standard Surface"));
            upgraders.Add(new ParticleUpgrader("Particles/Standard Unlit"));
            upgraders.Add(new ParticleUpgrader("Particles/VertexLit Blended"));

            ////////////////////////////////////
            // Autodesk Interactive           //
            ////////////////////////////////////
            upgraders.Add(new AutodeskInteractiveUpgrader("Autodesk Interactive"));
        }
    }
}


