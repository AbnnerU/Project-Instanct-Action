using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scripts.Combat;
using Scripts.Weapon;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Scripts.Editor.CombatWorkshop
{
    public enum CombatMarkerType { HIT, OPEN_COMBO, CLOSE_COMBO, FINISH }

    [Serializable]
    public class CombatMarker
    {
        public CombatMarkerType Type;
        public float Time;
    }

    [Serializable]
    public class WorkshopAttack
    {
        public AnimationClip Clip;
        public List<CombatMarker> List_Markers = new List<CombatMarker>();
    }

    public class CombatWorkshopDocument : ScriptableObject
    {
        public WeaponLoadoutSO Source;
        public WeaponLoadoutSO Loadout;
        public ComboAttackSetSO Combo;
        public AnimatorController BaseController;
        public string IdleState = "Base Layer.Idle";
        public AnimationClip IdleClip;
        public List<WorkshopAttack> List_Attacks = new List<WorkshopAttack>();

        public static readonly string[] EventNames =
        {
            "Attack_ExecuteHitbox", "Attack_OpenComboWindow", "Attack_CloseComboWindow", "Attack_Finish"
        };

        #region Draft
        public static CombatWorkshopDocument Create(WeaponLoadoutSO source)
        {
            var document = CreateInstance<CombatWorkshopDocument>();
            const HideFlags draftFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;
            document.hideFlags = draftFlags;
            document.Source = source;
            document.Loadout = source != null ? Instantiate(source) : CreateInstance<WeaponLoadoutSO>();
            document.Loadout.hideFlags = draftFlags;
            document.Combo = source != null && source.ComboAttackSet != null
                ? Instantiate(source.ComboAttackSet) : CreateInstance<ComboAttackSetSO>();
            document.Combo.hideFlags = draftFlags;
            document.BaseController = GetBaseController(document.Loadout.AnimatorController);
            var states = GetStates(document.BaseController);
            string idle = states.Keys.FirstOrDefault(x => x.EndsWith(".Idle", StringComparison.Ordinal));
            if (idle != null) document.IdleState = idle;
            document.IdleClip = document.GetEffectiveClip(document.IdleState);
            foreach (var attack in document.Combo.List_ComboAttackData)
            {
                var draft = new WorkshopAttack();
                SetClip(draft, document.GetEffectiveClip(attack.AnimationStateName));
                document.List_Attacks.Add(draft);
            }
            return document;
        }

        public void Dispose()
        {
            if (Loadout != null) DestroyImmediate(Loadout);
            if (Combo != null) DestroyImmediate(Combo);
            DestroyImmediate(this);
        }

        public static void SetClip(WorkshopAttack attack, AnimationClip clip)
        {
            attack.Clip = clip;
            attack.List_Markers.Clear();
            if (clip == null) return;
            foreach (var animationEvent in AnimationUtility.GetAnimationEvents(clip))
            {
                int index = Array.IndexOf(EventNames, animationEvent.functionName);
                if (index >= 0) attack.List_Markers.Add(new CombatMarker { Type = (CombatMarkerType)index, Time = animationEvent.time });
            }
        }

        public void AddAttack()
        {
            Undo.RecordObject(this, "Add workshop attack");
            var serialized = new SerializedObject(Combo);
            var list = serialized.FindProperty("_list_ComboAttackData");
            int index = list.arraySize++;
            var attack = list.GetArrayElementAtIndex(index);
            attack.FindPropertyRelative("_animationStateName").stringValue = "Attack " + (index + 1);
            attack.FindPropertyRelative("_transitionDuration").floatValue = 0.05f;
            attack.FindPropertyRelative("_hitboxCenterOffset").vector3Value = Vector3.forward;
            attack.FindPropertyRelative("_hitboxHalfExtents").vector3Value = Vector3.one * 0.5f;
            attack.FindPropertyRelative("_maxTargets").intValue = 8;
            attack.FindPropertyRelative("_targetLayerMask").intValue = LayerMask.GetMask("Enemy");
            serialized.ApplyModifiedProperties();
            List_Attacks.Add(new WorkshopAttack());
        }

        public void RemoveAttack(int index)
        {
            Undo.RecordObject(this, "Remove workshop attack");
            var serialized = new SerializedObject(Combo);
            serialized.FindProperty("_list_ComboAttackData").DeleteArrayElementAtIndex(index);
            serialized.ApplyModifiedProperties();
            List_Attacks.RemoveAt(index);
        }
        #endregion

        #region Animator Mapping
        public static AnimatorController GetBaseController(RuntimeAnimatorController controller)
        {
            while (controller is AnimatorOverrideController overrides) controller = overrides.runtimeAnimatorController;
            return controller as AnimatorController;
        }

        public static Dictionary<string, AnimationClip> GetStates(AnimatorController controller)
        {
            var result = new Dictionary<string, AnimationClip>();
            if (controller == null || controller.layers.Length == 0) return result;
            // ComboControllerBase plays attacks on layer zero.
            AddStates(controller.layers[0].stateMachine, controller.layers[0].name, result);
            return result;
        }

        private static void AddStates(AnimatorStateMachine machine, string path, Dictionary<string, AnimationClip> result)
        {
            foreach (var state in machine.states) result[path + "." + state.state.name] = state.state.motion as AnimationClip;
            foreach (var child in machine.stateMachines) AddStates(child.stateMachine, path + "." + child.stateMachine.name, result);
        }

        public static AnimationClip FindStateClip(Dictionary<string, AnimationClip> states, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            if (states.TryGetValue(name, out var clip)) return clip;
            var matches = states.Where(x => x.Key.EndsWith("." + name, StringComparison.Ordinal)).ToArray();
            return matches.Length == 1 ? matches[0].Value : null;
        }

        private AnimationClip GetEffectiveClip(string state)
        {
            AnimationClip original = FindStateClip(GetStates(BaseController), state);
            if (original == null) return null;
            if (Loadout.AnimatorController is AnimatorOverrideController overrides)
            {
                var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                overrides.GetOverrides(pairs);
                foreach (var pair in pairs)
                    if (pair.Key == original) return pair.Value != null ? pair.Value : original;
            }
            return original;
        }
        #endregion

        #region Validation
        public List<string> Validate()
        {
            var errors = new List<string>();
            if (BaseController == null) errors.Add("Select a base Animator Controller.");
            if (string.IsNullOrWhiteSpace(Loadout.ID)) errors.Add("Loadout ID is required.");
            if (IdleClip == null) errors.Add("Select an idle clip.");
            if (List_Attacks.Count == 0) errors.Add("Add at least one attack.");
            if (List_Attacks.Count != Combo.List_ComboAttackData.Count) errors.Add("Attack data and clips are out of sync.");
            var states = GetStates(BaseController);
            var used = new HashSet<AnimationClip>();
            ValidateSlot(states, IdleState, used, errors);
            for (int i = 0; i < List_Attacks.Count && i < Combo.List_ComboAttackData.Count; i++)
            {
                var draft = List_Attacks[i];
                var data = Combo.List_ComboAttackData[i];
                ValidateSlot(states, data.AnimationStateName, used, errors);
                if (draft.Clip == null) { errors.Add($"Attack {i + 1}: missing clip."); continue; }
                if (!draft.List_Markers.Any(x => x.Type == CombatMarkerType.FINISH)) errors.Add($"Attack {i + 1}: missing Finish marker.");
                if (draft.List_Markers.Count(x => x.Type == CombatMarkerType.FINISH) > 1) errors.Add($"Attack {i + 1}: only one Finish marker is allowed.");
                float finish = draft.List_Markers.Where(x => x.Type == CombatMarkerType.FINISH).Select(x => x.Time).DefaultIfEmpty(draft.Clip.length).First();
                if (draft.List_Markers.Any(x => x.Time < 0 || x.Time > draft.Clip.length || x.Time > finish)) errors.Add($"Attack {i + 1}: markers must be inside the clip and not after Finish.");
                float open = draft.List_Markers.Where(x => x.Type == CombatMarkerType.OPEN_COMBO).Select(x => x.Time).DefaultIfEmpty(-1).First();
                float close = draft.List_Markers.Where(x => x.Type == CombatMarkerType.CLOSE_COMBO).Select(x => x.Time).DefaultIfEmpty(-1).First();
                if ((open >= 0 || close >= 0) && (open < 0 || close < open)) errors.Add($"Attack {i + 1}: invalid combo window.");
                if (data.HitboxHalfExtents.x <= 0 || data.HitboxHalfExtents.y <= 0 || data.HitboxHalfExtents.z <= 0) errors.Add($"Attack {i + 1}: hitbox dimensions must be positive.");
            }
            foreach (var model in Loadout.List_WeaponModels)
            {
                if (model == null || model.WeaponPrefabReference == null || model.WeaponPrefabReference.editorAsset == null)
                    errors.Add("Weapon model is missing.");
                else if (AddressableAssetSettingsDefaultObject.Settings == null || AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(model.WeaponPrefabReference.AssetGUID, true) == null)
                    errors.Add($"Model {model.WeaponPrefabReference.editorAsset.name} is not Addressable.");
                if (model != null && string.IsNullOrWhiteSpace(model.SocketID)) errors.Add("Weapon socket ID is required.");
            }
            return errors;
        }

        private static void ValidateSlot(Dictionary<string, AnimationClip> states, string state, HashSet<AnimationClip> used, List<string> errors)
        {
            var clip = FindStateClip(states, state);
            if (clip == null) { errors.Add($"State '{state}' is missing or does not use a simple clip."); return; }
            if (!used.Add(clip) || states.Count(x => x.Value == clip) > 1)
                errors.Add($"State '{state}' shares its base clip. Assign a unique base clip in the Animator.");
        }
        #endregion

        #region Save
        public WeaponLoadoutSO Save(string newPath = null)
        {
            var errors = Validate();
            if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
            if (Source == null)
            {
                if (string.IsNullOrEmpty(newPath) || !newPath.StartsWith("Assets/", StringComparison.Ordinal) || !newPath.EndsWith(".asset", StringComparison.Ordinal))
                    throw new ArgumentException("Select an asset path inside Assets.");
                if (AssetDatabase.LoadMainAssetAtPath(newPath) != null) throw new InvalidOperationException("Asset already exists.");
                Source = CreateInstance<WeaponLoadoutSO>();
                AssetDatabase.CreateAsset(Source, newPath);
                Undo.RegisterCreatedObjectUndo(Source, "Create weapon loadout");
            }
            string sourcePath = AssetDatabase.GetAssetPath(Source);
            string parent = Path.GetDirectoryName(sourcePath).Replace('\\', '/');
            string folderName = Path.GetFileNameWithoutExtension(sourcePath) + "_Workshop_" + AssetDatabase.AssetPathToGUID(sourcePath).Substring(0, 8);
            string folder = parent + "/" + folderName;
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(parent, folderName);

            var combo = Source.ComboAttackSet;
            if (combo == null)
            {
                combo = CreateInstance<ComboAttackSetSO>();
                AssetDatabase.CreateAsset(combo, AssetDatabase.GenerateUniqueAssetPath(folder + "/ComboAttackSet.asset"));
                Undo.RegisterCreatedObjectUndo(combo, "Create attack set");
            }
            Undo.RecordObject(combo, "Save workshop attacks");
            EditorUtility.CopySerialized(Combo, combo);
            combo.hideFlags = HideFlags.None;

            string overridePath = folder + "/AnimatorOverride.overrideController";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(overridePath);
            if (controller == null)
            {
                controller = new AnimatorOverrideController(BaseController);
                AssetDatabase.CreateAsset(controller, overridePath);
                Undo.RegisterCreatedObjectUndo(controller, "Create weapon animations");
            }
            var oldPairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            if (Loadout.AnimatorController is AnimatorOverrideController previous && GetBaseController(previous) == BaseController)
                previous.GetOverrides(oldPairs);
            Undo.RecordObject(controller, "Save weapon animations");
            controller.runtimeAnimatorController = BaseController;
            var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            controller.GetOverrides(pairs);
            if (oldPairs.Count > 0)
            {
                for (int i = 0; i < pairs.Count; i++)
                {
                    var old = oldPairs.Find(x => x.Key == pairs[i].Key);
                    if (old.Value != null) pairs[i] = old;
                }
            }
            var states = GetStates(BaseController);
            SetOverride(pairs, FindStateClip(states, IdleState), IdleClip);
            for (int i = 0; i < List_Attacks.Count; i++)
            {
                AnimationClip clip = SaveClip(List_Attacks[i], folder + $"/Attack_{i + 1}.anim");
                SetOverride(pairs, FindStateClip(states, Combo.List_ComboAttackData[i].AnimationStateName), clip);
            }
            controller.ApplyOverrides(pairs);
            Undo.RecordObject(Source, "Save weapon loadout");
            EditorUtility.CopySerialized(Loadout, Source);
            Source.hideFlags = HideFlags.None;
            Source.name = Path.GetFileNameWithoutExtension(sourcePath);
            Source.ComboAttackSet = combo;
            Source.AnimatorController = controller;
            foreach (var asset in new UnityEngine.Object[] { combo, controller, Source })
            {
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
            }
            return Source;
        }

        private static AnimationClip SaveClip(WorkshopAttack attack, string path)
        {
            var target = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (target == null)
            {
                target = new AnimationClip();
                AssetDatabase.CreateAsset(target, path);
                Undo.RegisterCreatedObjectUndo(target, "Create workshop clip");
            }
            Undo.RecordObject(target, "Save workshop markers");
            if (target != attack.Clip) EditorUtility.CopySerialized(attack.Clip, target);
            target.hideFlags = HideFlags.None;
            var events = AnimationUtility.GetAnimationEvents(attack.Clip).Where(x => !EventNames.Contains(x.functionName)).ToList();
            events.AddRange(attack.List_Markers.Select(x => new AnimationEvent { functionName = EventNames[(int)x.Type], time = x.Time }));
            AnimationUtility.SetAnimationEvents(target, events.OrderBy(x => x.time).ToArray());
            var settings = AnimationUtility.GetAnimationClipSettings(target);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(target, settings);
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
            return target;
        }

        private static void SetOverride(List<KeyValuePair<AnimationClip, AnimationClip>> pairs, AnimationClip key, AnimationClip value)
        {
            int index = pairs.FindIndex(x => x.Key == key);
            if (index >= 0) pairs[index] = new KeyValuePair<AnimationClip, AnimationClip>(key, value);
        }
        #endregion
    }
}
