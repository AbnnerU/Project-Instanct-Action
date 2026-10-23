using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Combat;
using Scripts.Weapon;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Scripts.Editor.CombatWorkshop
{
    public static class CombatWorkshopChecks
    {
        [MenuItem("Tools/Combat Workshop Checks")]
        public static void RunMenu() => Debug.Log(Run());

        public static string Run()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play Mode before running checks.");
            int checks = 0;
            string folder = "Assets/WorkshopChecks_" + Guid.NewGuid().ToString("N");
            AssetDatabase.CreateFolder("Assets", folder.Substring("Assets/".Length));
            CombatWorkshopDocument document = null;
            try
            {
                var idle = MakeClip(folder + "/Idle.anim");
                var attackClip = MakeClip(folder + "/Attack.anim");
                AnimationUtility.SetAnimationEvents(attackClip, new[]
                {
                    new AnimationEvent { time = 0.1f, functionName = "ExistingEvent" },
                    new AnimationEvent { time = 0.2f, functionName = "Attack_OpenComboWindow" }
                });
                var controller = AnimatorController.CreateAnimatorControllerAtPath(folder + "/Base.controller");
                var machine = controller.layers[0].stateMachine;
                machine.AddState("Idle").motion = idle;
                machine.AddState("Attack 1").motion = attackClip;
                document = CombatWorkshopDocument.Create(null);
                Assert((document.Loadout.hideFlags & HideFlags.NotEditable) == 0 && (document.Combo.hideFlags & HideFlags.NotEditable) == 0, "Draft Inspector fields editable", ref checks);
                document.BaseController = controller;
                document.IdleClip = idle;
                document.Loadout.ID = "test_weapon";
                document.Loadout.DisplayName = "Test Weapon";
                document.AddAttack();
                CombatWorkshopDocument.SetClip(document.List_Attacks[0], attackClip);
                Assert(document.List_Attacks[0].List_Markers.Count == 0, "Legacy opening marker ignored", ref checks);
                document.List_Attacks[0].List_Markers.Add(new CombatMarker { Type = CombatMarkerType.HIT, Time = 0.3f });
                document.List_Attacks[0].List_Markers.Add(new CombatMarker { Type = CombatMarkerType.CLOSE_COMBO, Time = 0.6f });
                document.List_Attacks[0].List_Markers.Add(new CombatMarker { Type = CombatMarkerType.FINISH, Time = 0.8f });
                Assert(document.Validate().Count == 0, "Valid draft", ref checks);
                //Assert(document.Combo.List_ComboAttackData[0].AdvanceDistance == 0, "Initialized advance data", ref checks);
                var saved = document.Save(folder + "/Weapon.asset");
                Assert(saved.ComboAttackSet != null && saved.AnimatorController is AnimatorOverrideController, "Generated runtime references", ref checks);
                string comboGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(saved.ComboAttackSet));
                string overrideGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(saved.AnimatorController));
                var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                ((AnimatorOverrideController)saved.AnimatorController).GetOverrides(pairs);
                var copy = pairs.First(x => x.Key == attackClip).Value;
                Assert(copy != attackClip && AssetDatabase.Contains(copy), "Own attack clip asset", ref checks);
                Assert(AnimationUtility.GetAnimationEvents(attackClip).Length == 2, "Original events unchanged", ref checks);
                Assert(AnimationUtility.GetAnimationEvents(copy).Length == 4, "Preserved unrelated event and added combat markers", ref checks);
                Assert(!AnimationUtility.GetAnimationEvents(copy).Any(x => x.functionName == "Attack_OpenComboWindow"), "Legacy opening marker not exported", ref checks);
                Assert(!copy.isLooping, "Attack copy does not loop", ref checks);
                string copyGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(copy));
                document.Dispose();
                document = CombatWorkshopDocument.Create(saved);
                Assert(document.List_Attacks[0].Clip == copy, "Reopened effective override", ref checks);
                Assert(document.List_Attacks[0].List_Markers.Count == 3, "Reopened markers", ref checks);
                document.List_Attacks[0].List_Markers[0].Time = 0.25f;
                Assert(Mathf.Approximately(AnimationUtility.GetAnimationEvents(copy).First(x => x.functionName == "Attack_ExecuteHitbox").time, 0.3f), "Draft does not change source", ref checks);
                document.Save();
                Assert(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(saved.ComboAttackSet)) == comboGuid, "Stable combo reference", ref checks);
                Assert(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(saved.AnimatorController)) == overrideGuid, "Stable controller reference", ref checks);
                Assert(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(copy)) == copyGuid, "Stable clip reference", ref checks);
                Assert(AnimationUtility.GetAnimationEvents(copy).Count(x => x.functionName == "Attack_ExecuteHitbox") == 1, "No duplicate impact events after saving twice", ref checks);
                Assert(Mathf.Approximately(AnimationUtility.GetAnimationEvents(copy).First(x => x.functionName == "Attack_ExecuteHitbox").time, 0.25f), "Updated saved marker", ref checks);
                document.Loadout.HasPostDashAttack = true;
                document.CreatePostDashSlot();
                CombatWorkshopDocument.SetPostDashClip(document.PostDashAttack, copy);
                Assert(document.PostDashAttack.List_Markers.All(x => x.Type != CombatMarkerType.CLOSE_COMBO), "Post-dash ignores combo window", ref checks);
                Assert(document.Validate().Count == 0, "Valid post-dash draft", ref checks);
                saved = document.Save();
                document.Dispose();
                document = CombatWorkshopDocument.Create(saved);
                Assert(saved.HasPostDashAttack && document.PostDashAttack.Clip != copy, "Post-dash has its own saved clip", ref checks);
                Assert(saved.PostDashAttack.AnimationStateName == CombatWorkshopDocument.PostDashStateName, "Post-dash state saved", ref checks);
                Assert(document.PostDashAttack.List_Markers.Count == 2, "Post-dash markers reopened", ref checks);
                var postClip = document.PostDashAttack.Clip;
                document.Save();
                Assert(AnimationUtility.GetAnimationEvents(postClip).Count(x => x.functionName == "Attack_ExecuteHitbox") == 1, "Post-dash save does not duplicate hit events", ref checks);
                document.List_Attacks[0].List_Markers.RemoveAll(x => x.Type == CombatMarkerType.FINISH);
                Assert(document.Validate().Any(x => x.Contains("Finish")), "Missing Finish rejected", ref checks);
                machine.AddState("Shared Attack").motion = attackClip;
                Assert(document.Validate().Any(x => x.Contains("shares")), "Ambiguous override slots rejected", ref checks);
                Assert(CombatWorkshopPreview.OverlapsTarget(Vector3.zero, Quaternion.Euler(0, 90, 0), new Vector3(0.2f, 1, 2), new Vector3(1.5f, 0, 0), Vector3.one * 0.1f), "Rotated hitbox reaches target", ref checks);
                Assert(!CombatWorkshopPreview.OverlapsTarget(Vector3.zero, Quaternion.identity, new Vector3(0.2f, 1, 2), new Vector3(1.5f, 0, 0), Vector3.one * 0.1f), "Unrotated hitbox misses target", ref checks);
                return $"Combat Workshop: {checks} checks passed.";
            }
            finally
            {
                if (document != null) document.Dispose();
                AssetDatabase.DeleteAsset(folder);
            }
        }

        private static AnimationClip MakeClip(string path)
        {
            var clip = new AnimationClip();
            clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.Linear(0, 0, 1, 1));
            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        private static void Assert(bool condition, string name, ref int count)
        {
            if (!condition) throw new InvalidOperationException("Workshop check failed: " + name);
            count++;
        }
    }
}
