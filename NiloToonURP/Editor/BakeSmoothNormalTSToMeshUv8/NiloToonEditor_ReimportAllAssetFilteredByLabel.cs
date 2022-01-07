using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NiloToon.NiloToonURP
{
    public class NiloToonEditor_ReimportAllAssetFilteredByLabel
    {
        [MenuItem("Window/NiloToonURP/Model label/Remove all labeled models's label")]
        public static void RemoveAllLabeledLabel()
        {
            RemoveAllNiloToonAssetLabel();
            DeleteAllTempMeshAssetCloneWithCanDeletePrefix();
        }


        [MenuItem("Window/NiloToonURP/Model label/Re-fix whole project!")]
        public static void ReFixAll()
        {
            DeleteAllTempMeshAssetCloneWithCanDeletePrefix();
            RemoveAllNiloToonAssetLabel();
            ReAddAssetLabelToAllPrefabWithNiloPerCharScript();
            ReimportAllMeshAssetWithNiloToonAssetLabel();
            DeleteAllTempMeshAssetCloneWithCanDeletePrefix();
        }

        ////////////////////////////////////////////////
        // Core
        ////////////////////////////////////////////////
        public static void DeleteAllTempMeshAssetCloneWithCanDeletePrefix()
        {
            // search the whole project -> delete .fbx if prefix match
            string[] guids = AssetDatabase.FindAssets(NiloToonEditor_AssetLabelAssetPostProcessor.CAN_DELETE_PREFIX);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrWhiteSpace(assetPath) && !string.IsNullOrEmpty(assetPath))
                    AssetDatabase.DeleteAsset(assetPath);
            }

            // this function get called everyframe, so only Log if change exist
            if (guids.Length > 0)
                Debug.Log($"DeleteAllTempMeshAssetCloneWithCanDeletePrefix done! ({guids.Length})");
        }

        public static void RemoveAllNiloToonAssetLabel()
        {
            string[] guids = AssetDatabase.FindAssets($"l:{NiloToonEditor_AssetLabelAssetPostProcessor.ASSET_LABEL}");
            foreach (string guid in guids)
            {
                RemoveSingleAssetNiloToonAssetLabel(guid);
            }
            Debug.Log($"RemoveAllNiloToonAssetLabel done! ({guids.Length})");
        }
        public static void RemoveSingleAssetNiloToonAssetLabel(string guid)
        {
            // To support Unity 2019.4, here we use a compatable API
            // string[] allLabels = AssetDatabase.GetLabels(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GUIDToAssetPath(guid))); //only exists in 2020.2
            string[] allLabels = AssetDatabase.GetLabels(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));

            List<string> allLabelsList = new List<string>(allLabels);
            allLabelsList.RemoveAll(x => x == NiloToonEditor_AssetLabelAssetPostProcessor.ASSET_LABEL);
            allLabels = allLabelsList.ToArray();
            AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)), allLabels);
            AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid));
        }

        public static void ReAddAssetLabelToAllPrefabWithNiloPerCharScript()
        {
            string[] guids = AssetDatabase.FindAssets("t:prefab", null);
            foreach (string guid in guids)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                var script = go.GetComponentInChildren<NiloToonPerCharacterRenderController>(); //GetComponentInChildren<>() will include prefab go's root also
                if (script)
                {
                    NiloToonEditorPerCharacterRenderControllerCustomEditor.AutoAssignLabelToAllMeshsOfSingleChar(script);
                }
            }
            Debug.Log($"ReAddLabelToAllPrefabWithNiloPerCharScript done! ({guids.Length})");
        }

        public static void ReimportAllMeshAssetWithNiloToonAssetLabel()
        {
            string[] guids = AssetDatabase.FindAssets($"l:{NiloToonEditor_AssetLabelAssetPostProcessor.ASSET_LABEL}");
            foreach (string guid in guids)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid));
            }
            Debug.Log($"ReimportAllMeshAssetWithNiloToonAssetLabel done! ({guids.Length})");
        }
    }
}