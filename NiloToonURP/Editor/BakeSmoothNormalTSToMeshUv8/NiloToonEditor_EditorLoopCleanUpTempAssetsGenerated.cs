// https://answers.unity.com/questions/39313/how-do-i-get-a-callback-every-frame-in-edit-mode.html

using UnityEngine;
using UnityEditor;
using NiloToon.NiloToonURP;

namespace NiloToon.NiloToonURP
{
    [InitializeOnLoad]
    class NiloToonEditor_EditorLoopCleanUpTempAssetsGenerated
    {
        // will be set to true by NiloToonEditor_AssetLabelAssetPostProcessor, after model fbx reimported
        public static bool requireCleanUp = false;

        static NiloToonEditor_EditorLoopCleanUpTempAssetsGenerated()
        {
            // https://docs.unity3d.com/ScriptReference/EditorApplication-update.html
            EditorApplication.update += Update;
        }

        static void Update()
        {
            if (requireCleanUp)
            {
                NiloToonEditor_ReimportAllAssetFilteredByLabel.DeleteAllTempMeshAssetCloneWithCanDeletePrefix(); // auto clean up project
                Debug.Log("Reimport detected, delete all temp generated NiloToon assets");
                requireCleanUp = false; // reset, wait for next clean up request
            }
        }
    }
}