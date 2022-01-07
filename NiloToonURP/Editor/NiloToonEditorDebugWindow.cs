// how to create EditorWindow editor script:
// https://docs.unity3d.com/Manual/editor-EditorWindows.html
// https://docs.unity3d.com/ScriptReference/EditorWindow.GetWindow.html

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace NiloToon.NiloToonURP
{
    class NiloToonEditorDebugWindow : EditorWindow
    {
        [MenuItem("Window/NiloToonURP/Debug Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(NiloToonEditorDebugWindow), false, "NiloToon Debug Window");
        }

        // The actual window code goes here
        void OnGUI()
        {
            //////////////////////////////////////////////////////
            // Shading Debug 
            //////////////////////////////////////////////////////
            EditorGUILayout.LabelField("Editor - Shading Debug ", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("- This option only works in editor. In build, it is always OFF.");
            EditorGUILayout.LabelField("- Will reset to false if you enter/exit PlayMode");
            // no need to use PlayerPrefs in editor to hold this bool between edit mode and play mode, because we actually want to reset
            NiloToonSetToonParamPass.Instance.EnableShadingDebug = EditorGUILayout.Toggle("Enable Shading Debug", NiloToonSetToonParamPass.Instance.EnableShadingDebug);
            NiloToonSetToonParamPass.Instance.shadingDebugCase = (NiloToonSetToonParamPass.ShadingDebugCase)EditorGUILayout.EnumPopup("Shading Debug Case", NiloToonSetToonParamPass.Instance.shadingDebugCase);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //////////////////////////////////////////////////////
            // Preserve PlayMode Material Change
            //////////////////////////////////////////////////////
            EditorGUILayout.LabelField("Editor - Need Preserve PlayMode Material Change?", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("- This option only works in editor. In build, it is always OFF, which allows SRP batching = better performance");
            EditorGUILayout.LabelField("- In editor, turn this ON  can   preserve material change in editor playmode, but breaks  SRP batching and some per character feature. Good for material editing in PlayMode.");
            EditorGUILayout.LabelField("- In editor, turn this OFF can't preserve material change in editor playmode, but enables SRP batching and all  per character feature. Good for profiling / check final result.");
            // use PlayerPrefs in editor to hold this bool between edit mode and play mode
            bool preservePlayModeMaterialChange = EditorGUILayout.Toggle("Keep PlayMode mat edit?", NiloToonPerCharacterRenderController.GetNiloToonNeedPreserveEditorPlayModeMaterialChange_EditorOnly());
            NiloToonPerCharacterRenderController.SetNiloToonNeedPreserveEditorPlayModeMaterialChange_EditorOnly(preservePlayModeMaterialChange);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //////////////////////////////////////////////////////
            // force repaint scene view when window exist
            //////////////////////////////////////////////////////
            // disabled due to bug (NiloToon Debug window always wrongly focus project/scene window)
            /*
            if(!Application.isPlaying)
            {
                EditorWindow view = EditorWindow.GetWindow<SceneView>();
                view.Repaint();
            }
            */
        }
    }
}
