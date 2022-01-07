// this script is an edited version of this article's example code: 
// 【Job/Toon Shading Workflow】自动生成硬表面模型Outline Normal
// https://zhuanlan.zhihu.com/p/107664564

// Credits to 喵刀Hime

// changes to the original script are:
// -use Vector instead of Color for Jobs
// -bake smooth normal into uv8 instead of vertex color
// -use AssetLabel("NiloToonBakeSmoothNormalTSIntoUV8") instead of asset name to decide if bake is needed or not, because using AssetLabel is more user friendly & flexible

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using Unity.Jobs;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;

namespace NiloToon.NiloToonURP
{
    public class NiloToonEditor_AssetLabelAssetPostProcessor : AssetPostprocessor
    {
        public const string ASSET_LABEL = "NiloToonBakeSmoothNormalTSIntoUV8";
        public const string CAN_DELETE_PREFIX = "(you can ignore or delete it safely)";

        [MenuItem("Assets/NiloToon/Add NiloToon Postprocessor Label")]
        static void SelectionObjectsAddAssetLabel()
        {
            AddAssetLabelAndReimport(Selection.objects);
        }
        public static void AddAssetLabelAndReimport(Object[] targetAssets, bool shouldSkipIfLabelExist = true)
        {
            bool isDirty = false;
            foreach (var o in targetAssets)
            {
                if (o == null) continue;

                // but prevent adding label to Unity's default mesh asset, since Unity's default mesh asset always reset which enters [add->reset] loop
                string assetPath = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrWhiteSpace(assetPath) || string.IsNullOrEmpty(assetPath)) continue;

                bool shouldProcess = !assetPath.Contains("Library/unity default resources");
                shouldProcess &= !o.name.Contains("(Clone)"); // some plugin like MagicaCloth will edit character's mesh, we don't want to add label to non-asset/dynamic created mesh
                if (!shouldProcess) continue;

                string[] tags = AssetDatabase.GetLabels(o);

                //if label already exist, early skip to avoid useless reimport time spent
                bool shouldSkip = false;
                foreach (string s in tags)
                {
                    if (s == ASSET_LABEL)
                        shouldSkip = true;
                }
                if (shouldSkipIfLabelExist && shouldSkip) continue;

                if (!shouldSkip)
                {
                    string[] newLabels = new string[tags.Length + 1];
                    for (int i = 0; i < tags.Length; i++)
                    {
                        newLabels[i] = tags[i];
                    }
                    newLabels[newLabels.Length - 1] = ASSET_LABEL; // append our new label
                    AssetDatabase.SetLabels(o, newLabels);
                }


                AssetDatabase.SaveAssets(); // save labels to disk
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(o));
                isDirty = true;
            }
            if (isDirty)
            {
                AssetDatabase.SaveAssets(); // save labels to disk
                AssetDatabase.Refresh();

                ////////////////////////////////////////////////////////////
                // will make outline disappear! so disabled
                //////////////////////////////////////////////////////////// 
                //NiloToonEditor_ReimportAllAssetFilteredByLabel.DeleteAllTempMeshAssetCloneWithCanDeletePrefix();
            }
        }
        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        // https://docs.unity3d.com/ScriptReference/MenuItem.html
        [MenuItem("Assets/NiloToon/Add NiloToon Postprocessor Label", true)]
        static bool ValidateAddAssetLabel()
        {
            return isValidSelection();
        }

        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        // https://docs.unity3d.com/ScriptReference/MenuItem.html
        [MenuItem("Assets/NiloToon/Remove NiloToon Postprocessor Label", true)]
        static bool ValidateRemoveAssetLabel()
        {
            return isValidSelection();
        }

        [MenuItem("Assets/NiloToon/Remove NiloToon Postprocessor Label")]
        static void RemoveAssetLabel()
        {
            foreach (var o in Selection.objects)
            {
                string[] tags = AssetDatabase.GetLabels(o);
                tags = tags.Where(x => x != ASSET_LABEL).ToArray();
                AssetDatabase.SetLabels(o, tags);
            }
            AssetDatabase.Refresh();
        }


        static bool isValidSelection()
        {
            return Selection.objects.Length == Selection.gameObjects.Length; // at least limit valid target to game object only
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Core
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static int extraReimportCount = 0;

        // will call this BEFORE creating fbx GameObject
        void OnPreprocessModel()
        {
            // will enter here only if model is a copied .fbx
            if (assetPath.Contains(CAN_DELETE_PREFIX))
            {
                // edit setting, use Unity's auto smooth normal, merge vertices
                ModelImporter model = assetImporter as ModelImporter;
                //model.isReadable = true; //no need to edit isReadable 
                model.importNormals = ModelImporterNormals.Calculate;
                model.normalCalculationMode = ModelImporterNormalCalculationMode.AngleWeighted; //AngleWeighted, not area weighted
                model.normalSmoothingAngle = 180.0f;
                model.importAnimation = false;
                model.materialImportMode = ModelImporterMaterialImportMode.None;
            }
        }
        bool ShouldCare(AssetImporter assetImporter)
        {
            bool shouldCare = false;

            foreach (var label in AssetDatabase.GetLabels(assetImporter))
            {
                if (label == ASSET_LABEL)
                    shouldCare = true;
            }

            return shouldCare;
        }

        // will call this AFTER created fbx GameObject, we store smooth normal in uv8 here
        void OnPostprocessModel(GameObject go)
        {
            // if is copiedFBX, use copiedFBX's OnPostprocessModel() to trigger srcFBX's second import.
            // then exit.
            if (isCopiedFBX(assetPath))
            {
                AssetDatabase.ImportAsset(ConvertCopiedFBXPathToSrcFBXPath(assetPath));
                return;
            }

            //we only care the source fbx which has "NiloToonBakeSmoothNormalTSIntoUV8" asset label
            if (!ShouldCare(assetImporter))
                return;

            // [only the source fbx which has "NiloToonBakeSmoothNormalTSIntoUV8" asset label will enter this section]
            ModelImporter model = assetImporter as ModelImporter;
            string srcFBXPath = model.assetPath;
            string copiedFBXPath = ConvertSrcFBXPathToCopiedFBXPath(srcFBXPath);

            if (AssetDatabase.LoadAssetAtPath<GameObject>(copiedFBXPath) == null)
            {
                // copy a fbx for unity to generate smooth normal
                AssetDatabase.CopyAsset(srcFBXPath, copiedFBXPath);

                extraReimportCount = 1;

                // then trigger copiedFBX's reimport
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(copiedFBXPath);
            }
            else
            {
                var copyFBXGO = AssetDatabase.LoadAssetAtPath<GameObject>(copiedFBXPath);

                Dictionary<string, Mesh> originalMesh = GetMesh(go), smoothedMesh = GetMesh(copyFBXGO);

                foreach (var item in originalMesh)
                {
                    var mesh = item.Value;
                    smoothedMesh.TryGetValue(item.Key, out Mesh outSmoothedMesh);
                    if (outSmoothedMesh == null)
                    {
                        Debug.LogWarning($"{srcFBXPath}: will produce KeyNotFoundException: The given key was not present in the dictionary.");
                        continue; // avoid KeyNotFoundException: The given key was not present in the dictionary.
                    }
                    Vector4[] bakeData = ComputeSmoothedNormalByJob(outSmoothedMesh, mesh, srcFBXPath);
                    if (bakeData == null) continue;

                    //bake data store in uv8 (use Vector3 to reduce data size) (can use Vector2 to reduce more, but you need to pay the cost of normal reconstruct in shader)
                    Vector3[] uv8Array = new Vector3[bakeData.Length];
                    for (int i = 0; i < bakeData.Length; i++)
                    {
                        uv8Array[i].x = bakeData[i].x;
                        uv8Array[i].y = bakeData[i].y;
                        uv8Array[i].z = bakeData[i].z;
                        //uv8Array[i].w = 0; //can save any extra data here, like Curvature
                    }

                    //choose uv8 because user usually don't use it
                    mesh.SetUVs(7, uv8Array);
                }

                extraReimportCount--;
            }

            AssetDatabase.Refresh();

            // we should delete all copied fbx generated, but adding this line will make the outline disappear
            //AssetDatabase.DeleteAsset(copyFBXPath);

            // so we request the delete by an external script "NiloToonEditor_EditorLoopCleanUpTempAssetsGenerated" instead
            NiloToonEditor_EditorLoopCleanUpTempAssetsGenerated.requireCleanUp = true;
        }

        // https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessAllAssets.html
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var s in importedAssets)
            {
                if (isCopiedFBX(s))
                {
                    string copyFBXPath = s;
                    foreach (var path in importedAssets)
                    {
                        foreach (var label in GetLabelsFromPath(path))
                        {
                            if (label == ASSET_LABEL && !path.Contains(CAN_DELETE_PREFIX))
                            {
                                string srcFBXPath = path;

                                // only reimport 1 more time, to prevent unlimited import dead loop
                                if (extraReimportCount > 0)
                                {
                                    extraReimportCount--;
                                    AssetDatabase.ImportAsset(srcFBXPath);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var s in importedAssets)
            {
                foreach (var label in GetLabelsFromPath(s))
                {
                    if (label == ASSET_LABEL && !isCopiedFBX(s))
                    {
                        string srcFBXPath = s;
                        if (extraReimportCount <= -1)
                        {
                            string copyFBXPath = ConvertSrcFBXPathToCopiedFBXPath(srcFBXPath);

                            if (!string.IsNullOrEmpty(copyFBXPath))
                            {
                                AssetDatabase.DeleteAsset(copyFBXPath); //delete after all work finished, but in project window the deleted .fbx will still visible(although it is not in disk already)
                                AssetDatabase.Refresh();
                            }
                            else
                            {
                                Debug.LogError("ERROR");
                            }
                        }
                    }
                }
            }
        }

        static bool isCopiedFBX(string fbxPath) => fbxPath.Contains(CAN_DELETE_PREFIX);

        // To support Unity 2019.4, here we use a compatable API
        // static string[] GetLabelsFromPath(string fbxPath) => AssetDatabase.GetLabels(AssetDatabase.GUIDFromAssetPath(fbxPath));  //only exists in 2020.2
        static string[] GetLabelsFromPath(string fbxPath) => AssetDatabase.GetLabels(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(fbxPath)));
        static string ConvertSrcFBXPathToCopiedFBXPath(string srcFBXPath) => Path.GetDirectoryName(srcFBXPath) + "/" + CAN_DELETE_PREFIX + Path.GetFileName(srcFBXPath);
        static string ConvertCopiedFBXPathToSrcFBXPath(string copiedFBXPath) => copiedFBXPath.Replace(CAN_DELETE_PREFIX, "");

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Jobs
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public struct CollectNormalJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector3> normals, vertrx;
            [NativeDisableContainerSafetyRestriction]
            public NativeArray<UnsafeHashMap<Vector3, Vector3>.ParallelWriter> result;

            public CollectNormalJob(NativeArray<Vector3> normals, NativeArray<Vector3> vertrx, NativeArray<UnsafeHashMap<Vector3, Vector3>.ParallelWriter> result)
            {
                this.normals = normals;
                this.vertrx = vertrx;
                this.result = result;
            }

            void IJobParallelFor.Execute(int index)
            {
                for (int i = 0; i < result.Length + 1; i++)
                {
                    if (i == result.Length)
                    {
                        Debug.LogWarning($"merge vertex count（{i}）overflow");
                        break;
                    }

                    //Debug.Log($"import{result[i]}");//(Not required anymore after we increased each resultParallel's element's HashMap capacity to 16x) ??????????注释将抛出异常 (need to call this line, else "HashMap is full" error)

                    if (result[i].TryAdd(vertrx[index], normals[index]))
                    {
                        break;
                    }
                }
            }
        }

        public struct BakeNormalJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector3> vertrx, normals;
            [ReadOnly] public NativeArray<Vector4> tangents;
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<UnsafeHashMap<Vector3, Vector3>> result;
            [ReadOnly] public bool existVector4s;
            public NativeArray<Vector4> Vector4s;

            public BakeNormalJob(NativeArray<Vector3> vertrx, NativeArray<Vector3> normals, NativeArray<Vector4> tangents, NativeArray<UnsafeHashMap<Vector3, Vector3>> result, bool existVector4s, NativeArray<Vector4> Vector4s)
            {
                this.vertrx = vertrx;
                this.normals = normals;
                this.tangents = tangents;
                this.result = result;
                this.existVector4s = existVector4s;
                this.Vector4s = Vector4s;
            }

            void IJobParallelFor.Execute(int index)
            {
                Vector3 smoothedNormals = Vector3.zero;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i][vertrx[index]] != Vector3.zero)
                        smoothedNormals += result[i][vertrx[index]];
                    else
                        break;
                }
                smoothedNormals = smoothedNormals.normalized;

                var binormal = (Vector3.Cross(normals[index], tangents[index]) * tangents[index].w).normalized;

                var tbn = new Matrix4x4(
                    tangents[index],
                    binormal,
                    normals[index],
                    Vector3.zero);
                tbn = tbn.transpose;

                var bakedNormal = tbn.MultiplyVector(smoothedNormals).normalized;

                Vector4 Vector4 = new Vector4();
                Vector4.x = bakedNormal.x;
                Vector4.y = bakedNormal.y;
                Vector4.z = bakedNormal.z;

                Vector4s[index] = Vector4;
            }
        }
        Dictionary<string, Mesh> GetMesh(GameObject go)
        {
            Dictionary<string, Mesh> dic = new Dictionary<string, Mesh>();
            foreach (var item in go.GetComponentsInChildren<MeshFilter>())
                dic.Add(item.name, item.sharedMesh);
            //if (dic.Count == 0) //why need this?
            foreach (var item in go.GetComponentsInChildren<SkinnedMeshRenderer>())
                dic.Add(item.name, item.sharedMesh);
            return dic;
        }

        Vector4[] ComputeSmoothedNormalByJob(Mesh smoothedMesh, Mesh originalMesh, string fbxPath, int maxOverlapvertices = 10)
        {
            int svc = smoothedMesh.vertexCount, ovc = originalMesh.vertexCount;
            // CollectNormalJob Data
            NativeArray<Vector3> normals = new NativeArray<Vector3>(smoothedMesh.normals, Allocator.Persistent),
                vertrx = new NativeArray<Vector3>(smoothedMesh.vertices, Allocator.Persistent),
                smoothedNormals = new NativeArray<Vector3>(svc, Allocator.Persistent);
            var result = new NativeArray<UnsafeHashMap<Vector3, Vector3>>(maxOverlapvertices, Allocator.Persistent);
            var resultParallel = new NativeArray<UnsafeHashMap<Vector3, Vector3>.ParallelWriter>(result.Length, Allocator.Persistent);
            // NormalBakeJob Data
            NativeArray<Vector3> normalsO = new NativeArray<Vector3>(originalMesh.normals, Allocator.Persistent),
                vertrxO = new NativeArray<Vector3>(originalMesh.vertices, Allocator.Persistent);
            var tangents = new NativeArray<Vector4>(originalMesh.tangents, Allocator.Persistent);
            var Vector4s = new NativeArray<Vector4>(ovc, Allocator.Persistent);

            void DisposeAll()
            {
                // remove this forloop will cause Persistent memory leak, leaked memory can only be released when Unity shut down
                for (int i = 0; i < result.Length; i++)
                {
                    result[i].Dispose();
                }

                normals.Dispose();
                vertrx.Dispose();
                result.Dispose();
                smoothedNormals.Dispose();
                resultParallel.Dispose();
                normalsO.Dispose();
                vertrxO.Dispose();
                tangents.Dispose();
                Vector4s.Dispose();
            }

            // safe check before baking
            if (originalMesh.normals.Length != 0 && originalMesh.tangents.Length == 0)
            {
                Debug.LogError($"NiloToon can't bake smooth normal, because {originalMesh}({fbxPath}) don't have tangents, " +
                    $"you can let Unity calculate tangents for you (click .fbx->Model tab->Tangents = Calculate ...).");
                DisposeAll();
                return null; // return null to notice caller's program to stop
            }

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new UnsafeHashMap<Vector3, Vector3>(ovc*8, Allocator.Persistent); // NiloToon edit: original capacity is src, we changed to ovc * 8 to avoid "HashMap is full" problem
                resultParallel[i] = result[i].AsParallelWriter();
            }

            CollectNormalJob collectNormalJob = new CollectNormalJob(normals, vertrx, resultParallel);
            BakeNormalJob normalBakeJob = new BakeNormalJob(vertrxO, normalsO, tangents, result, false, Vector4s);

            normalBakeJob.Schedule(ovc, 100, collectNormalJob.Schedule(svc, 100)).Complete();

            Vector4[] resultVector4s = new Vector4[Vector4s.Length];
            Vector4s.CopyTo(resultVector4s);

            DisposeAll();

            return resultVector4s;
        }
    }
}