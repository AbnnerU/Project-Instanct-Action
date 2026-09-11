using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scripts.Status;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Scripts.Editor
{
    // One-time migration of the inspected legacy content, not a runtime unit heuristic.
    public static class PercentageConventionMigration
    {
        private const string ReportPath = "Assets/MDs/Migracao_Percentuais.json";
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private static readonly List<Change> _list_Changes = new List<Change>();

        [Serializable]
        private class Change
        {
            public string Asset;
            public string ObjectName;
            public string Property;
            public float Before;
            public float After;
        }

        [Serializable]
        private class Report
        {
            public string Status;
            public List<Change> Changes;
        }

        #region Menu

        [MenuItem("Tools/Project Instanct Action/Percentages/Preview Legacy Migration")]
        public static void Preview() => Run(false);

        [MenuItem("Tools/Project Instanct Action/Percentages/Apply Legacy Migration")]
        public static void Apply() => Run(true);

        [MenuItem("Tools/Project Instanct Action/Percentages/Verify Migrated Values")]
        public static void Verify()
        {
            if (!File.Exists(ReportPath))
                throw new InvalidOperationException("Missing migration report: " + ReportPath);
            Report report = JsonUtility.FromJson<Report>(File.ReadAllText(ReportPath));
            if (report == null || report.Status != "COMPLETE")
                throw new InvalidOperationException("Migration is not complete.");

            foreach (var group in report.Changes.GroupBy(change => change.Asset))
            {
                GameObject prefabRoot = null;
                List<Object> list_Targets = new List<Object>();
                try
                {
                    if (group.Key == ScenePath)
                    {
                        Scene scene = SceneManager.GetActiveScene();
                        if (scene.path != ScenePath)
                            throw new InvalidOperationException("Open " + ScenePath);
                        foreach (GameObject root in scene.GetRootGameObjects())
                            list_Targets.AddRange(root.GetComponentsInChildren<MonoBehaviour>(true));
                    }
                    else if (group.Key.EndsWith(".prefab"))
                    {
                        prefabRoot = PrefabUtility.LoadPrefabContents(group.Key);
                        if (prefabRoot == null)
                            throw new InvalidOperationException("Missing prefab: " + group.Key);
                        list_Targets.AddRange(prefabRoot.GetComponentsInChildren<MonoBehaviour>(true));
                    }
                    else
                    {
                        list_Targets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(group.Key));
                    }

                    foreach (Change change in group)
                    {
                        int matches = 0;
                        foreach (Object target in list_Targets)
                        {
                            if (target == null || target.name != change.ObjectName)
                                continue;
                            SerializedProperty property = new SerializedObject(target).FindProperty(change.Property);
                            if (property == null)
                                continue;
                            if (!Mathf.Approximately(property.floatValue, change.After))
                                throw new InvalidOperationException($"Mismatch: {change.Asset} / {change.Property}: {property.floatValue}, expected {change.After}");
                            matches++;
                        }
                        if (matches != 1)
                            throw new InvalidOperationException($"Expected one target for {change.Asset} / {change.Property}, found {matches}.");
                    }
                }
                finally
                {
                    if (prefabRoot != null)
                        PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
            Debug.Log($"Percentage migration readback PASSED: {report.Changes.Count} values verified.");
        }

        private static void Run(bool apply)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before migrating percentages.");

            if (File.Exists(ReportPath))
                throw new InvalidOperationException("Migration report already exists. Refusing to convert values twice: " + ReportPath);

            Scene scene = SceneManager.GetActiveScene();
            if (scene.path != ScenePath || scene.isDirty)
                throw new InvalidOperationException("Open the saved SampleScene before migrating percentages.");

            _list_Changes.Clear();
            if (apply)
                WriteReport("IN_PROGRESS");

            // Scene overrides are handled before changing their prefab source values.
            bool sceneChanged = false;
            foreach (GameObject root in scene.GetRootGameObjects())
                sceneChanged |= ProcessHierarchy(root, ScenePath, apply, true);

            if (apply && sceneChanged)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene))
                    throw new InvalidOperationException("Could not save " + ScenePath);
            }

            foreach (string guid in AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/Scriptable Object" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset == null)
                    throw new InvalidOperationException("Missing ScriptableObject: " + path);
                if (ProcessObject(asset, path, apply, false) && apply)
                    AssetDatabase.SaveAssetIfDirty(asset);
            }

            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefab" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject root = PrefabUtility.LoadPrefabContents(path);
                if (root == null)
                    throw new InvalidOperationException("Missing prefab: " + path);
                try
                {
                    if (ProcessHierarchy(root, path, apply, true) && apply)
                    {
                        PrefabUtility.SaveAsPrefabAsset(root, path, out bool saved);
                        if (!saved)
                            throw new InvalidOperationException("Could not save prefab: " + path);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            if (apply)
            {
                WriteReport("COMPLETE");
                AssetDatabase.ImportAsset(ReportPath);
            }

            Debug.Log($"Percentage migration {(apply ? "applied" : "preview")}: {_list_Changes.Count} values in {_list_Changes.Select(c => c.Asset).Distinct().Count()} files.\n" +
                      JsonUtility.ToJson(new Report { Status = apply ? "COMPLETE" : "PREVIEW", Changes = _list_Changes }, true));
        }

        #endregion

        #region Conversion

        private static bool ProcessHierarchy(GameObject root, string path, bool apply, bool skipInherited)
        {
            bool changed = false;
            foreach (MonoBehaviour component in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component != null && component.GetType().Namespace != null && component.GetType().Namespace.StartsWith("Scripts."))
                    changed |= ProcessObject(component, path, apply, skipInherited);
            }
            return changed;
        }

        private static bool ProcessObject(Object target, string path, bool apply, bool skipInherited)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty iterator = serialized.GetIterator();
            List<string> list_Properties = new List<string>();
            while (iterator.Next(true))
            {
                if (iterator.propertyType == SerializedPropertyType.Float &&
                    (iterator.name == "_physicForceScaling" || iterator.name == "_magicForceScaling" ||
                     target is AttackSpeedByStatus && (iterator.name == "_defaultAttackSpeed" || iterator.name == "_minimumAttackSpeed")))
                    list_Properties.Add(iterator.propertyPath);

                if (iterator.propertyType != SerializedPropertyType.Generic || iterator.isArray)
                    continue;

                if (iterator.type == "StatusData")
                {
                    SerializedProperty status = iterator.FindPropertyRelative("StatusType");
                    if (status != null && (status.intValue == (int)StatusType.ATTACK_SPEED || status.intValue == (int)StatusType.CRITICAL_DAMAGE))
                    {
                        list_Properties.Add(iterator.propertyPath + ".BaseValue");
                        list_Properties.Add(iterator.propertyPath + ".CurrentValue");
                    }
                }

                if (iterator.type == "StatusEnchantmentModifier")
                {
                    SerializedProperty status = iterator.FindPropertyRelative("StatusType");
                    SerializedProperty mode = iterator.FindPropertyRelative("ValueType");
                    if (status != null && mode != null && status.intValue == (int)StatusType.ATTACK_SPEED && mode.intValue == 0)
                        list_Properties.Add(iterator.propertyPath + ".Value");
                    // CriticalImpact (+12) and CriticalWeight (+8) already use percentage points.
                }

                if (iterator.type == "BuffData")
                {
                    SerializedProperty status = iterator.FindPropertyRelative("_statusType");
                    SerializedProperty mode = iterator.FindPropertyRelative("_valueType");
                    if (status != null && mode != null && status.intValue == (int)StatusType.ATTACK_SPEED && mode.intValue == 0)
                        list_Properties.Add(iterator.propertyPath + "._value");
                }
            }

            bool changed = false;
            foreach (string propertyPath in list_Properties.Distinct())
            {
                SerializedProperty property = serialized.FindProperty(propertyPath);
                if (property == null)
                    throw new InvalidOperationException("Missing property: " + path + " / " + propertyPath);
                if (skipInherited && PrefabUtility.IsPartOfPrefabInstance(target) && !property.prefabOverride)
                    continue;
                if (property.floatValue == 0f)
                    continue;

                float before = property.floatValue;
                _list_Changes.Add(new Change { Asset = path, ObjectName = target.name, Property = propertyPath, Before = before, After = before * 100f });
                changed = true;
                if (apply)
                    property.floatValue = before * 100f;
            }

            if (apply && changed)
            {
                Undo.RecordObject(target, "Convert legacy percentages");
                serialized.ApplyModifiedProperties();
                if (PrefabUtility.IsPartOfPrefabInstance(target))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                EditorUtility.SetDirty(target);
                WriteReport("IN_PROGRESS");
            }
            return changed;
        }

        private static void WriteReport(string status)
        {
            File.WriteAllText(ReportPath, JsonUtility.ToJson(new Report { Status = status, Changes = _list_Changes }, true));
        }

        #endregion
    }
}
