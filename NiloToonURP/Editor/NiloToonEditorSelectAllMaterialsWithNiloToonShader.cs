using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NiloToon.NiloToonURP
{
    public class NiloToonEditorSelectAllMaterialsWithNiloToonShader
    {
#if UNITY_EDITOR
        [MenuItem("Window/NiloToonURP/Utility/SelectAllMaterialsWithNiloToonShader")]
        static void SelectAllMaterialsWithNiloToonShader()
        {
            // https://forum.unity.com/threads/setting-selection-with-multiple-objects.259130/
            string[] guids = AssetDatabase.FindAssets("t:material");
            List<Object> mList = new List<Object>();
            foreach (string guid in guids)
            {
                Material m = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (m.shader.name == ("Universal Render Pipeline/NiloToon/NiloToon_Character"))
                {
                    mList.Add(m);
                }
            }

            Selection.objects = mList.ToArray();
        }
#endif
    }
}