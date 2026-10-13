using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Combat;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Editor.Combat
{
    [InitializeOnLoad]
    public static class ComboControllerChecks
    {
        #region Runner

        public static string Result
        {
            get => SessionState.GetString("ComboControllerChecks.Result", "Not started.");
            private set => SessionState.SetString("ComboControllerChecks.Result", value);
        }
        private static IEnumerator _routine;
        private static double _startedAt;
        private const string RUN_ON_PLAY = "ComboControllerChecks.RunOnPlay";

        static ComboControllerChecks()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(RUN_ON_PLAY, false)) return;
                SessionState.SetBool(RUN_ON_PLAY, false);
                EditorApplication.delayCall += Run;
            };
        }

        [MenuItem("Tools/Combat/Run Combo Checks (Play Mode)")]
        public static void RunInPlayMode()
        {
            if (EditorApplication.isPlaying) { Run(); return; }
            SessionState.SetBool(RUN_ON_PLAY, true);
            EditorApplication.EnterPlaymode();
        }

        public static void Run()
        {
            if (!EditorApplication.isPlaying || EditorApplication.isPaused)
                throw new InvalidOperationException("Enter Play Mode and unpause before running combo checks.");
            if (_routine != null) throw new InvalidOperationException("Combo checks are already running.");
            Result = "Running.";
            _startedAt = EditorApplication.timeSinceStartup;
            _routine = CheckCombo();
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            try
            {
                if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup - _startedAt > 30)
                    throw new InvalidOperationException("Combo checks interrupted or timed out.");
                EditorApplication.QueuePlayerLoopUpdate();
                if (_routine.MoveNext()) return;
            }
            catch (Exception exception)
            {
                Result = "FAILED: " + exception.Message;
                Debug.LogError(Result);
            }
            EditorApplication.update -= Tick;
            (_routine as IDisposable)?.Dispose();
            _routine = null;
        }

        #endregion

        #region Checks

        private static IEnumerator CheckCombo()
        {
            using (var fixture = new Fixture())
            {
                int checks = 0;
                var combo = fixture.Combo;
                combo.RequestAttack();
                Check(combo.IsAttacking && combo.IsComboWindowOpen, "Window opens on attack start", ref checks);
                combo.RequestAttack();
                combo.RequestAttack();
                combo.AnimationEvent_CloseComboWindow();
                combo.AnimationEvent_OpenComboWindow();
                Check(!combo.IsComboWindowOpen && combo.HasQueuedNextAttack, "Close preserves one queued input; legacy Open is ignored", ref checks);
                combo.AnimationEvent_FinishAttack();
                Check(combo.CurrentComboIndex == 1 && !combo.HasQueuedNextAttack, "Exactly one next attack", ref checks);
                combo.AnimationEvent_CloseComboWindow();
                combo.RequestAttack();
                Check(!combo.HasQueuedNextAttack, "Input after Close is ignored", ref checks);

                combo.Disable();
                combo.CancelAttack(false);
                Check(!combo.IsAttacking && combo.CurrentComboIndex == 0, "Dash clears interrupted index", ref checks);
                combo.RequestAttack();
                Check(!combo.IsAttacking && !combo.HasQueuedNextAttack, "Dash does not buffer input", ref checks);
                combo.Enable();
                combo.RequestAttack();
                Check(combo.IsAttacking && combo.CurrentComboIndex == 0 && !combo.HasQueuedNextAttack,
                    "Attack after dash starts a new combo", ref checks);
                combo.RequestAttack();
                combo.AnimationEvent_FinishAttack();
                combo.CancelAttack(false);
                combo.RequestAttack();
                Check(combo.CurrentComboIndex == 0, "Hit resets the combo", ref checks);
                combo.RequestAttack();
                combo.AnimationEvent_FinishAttack();
                combo.SetComboAttackSet(fixture.Attacks);
                Check(!combo.IsAttacking && combo.CurrentComboIndex == 0, "Weapon change resets the combo", ref checks);
                combo.RequestAttack();
                combo.RequestAttack();
                combo.AnimationEvent_FinishAttack();
                combo.enabled = false;
                Check(!combo.IsAttacking && combo.CurrentComboIndex == 0, "Disable resets the combo", ref checks);
                combo.enabled = true;

                combo.RequestAttack();
                combo.RequestAttack();
                combo.AnimationEvent_FinishAttack();
                combo.RequestAttack();
                combo.CancelAttack(playIdle: false, maintainComboIndex: true);
                Check(!combo.IsAttacking && combo.CurrentComboIndex == 1, "Cancel can retain the current index", ref checks);
                Check(!combo.IsComboWindowOpen && !combo.HasQueuedNextAttack, "Retaining index still clears execution and queued input", ref checks);
                combo.RequestAttack();
                Check(combo.IsAttacking && combo.CurrentComboIndex == 1 && combo.IsComboWindowOpen,
                    "New request starts the retained attack", ref checks);
                combo.AnimationEvent_FinishAttack();
                Check(!combo.IsAttacking && combo.CurrentComboIndex == 0, "Normal finish resets the retained index", ref checks);

                combo.RequestAttack();
                combo.RequestAttack();
                combo.AnimationEvent_FinishAttack();
                combo.CancelAttack(maintainComboIndex: true);
                Check(combo.CurrentComboIndex == 1 && fixture.Animator.GetBool("Idle"), "Idle and index retention are independent", ref checks);
                combo.CancelAttack();
                Check(combo.CurrentComboIndex == 0, "Default cancel clears a retained index even while idle", ref checks);

                Action<ComboAttackData> cancelBeforeAttack = _ => combo.CancelAttack(playIdle: false);
                combo.OnBeforeAttack += cancelBeforeAttack;
                combo.RequestAttack();
                combo.OnBeforeAttack -= cancelBeforeAttack;
                Check(!combo.IsAttacking, "Cancellation inside OnBeforeAttack is respected", ref checks);

                combo.RequestAttack();
                combo.RequestAttack();
                combo.AnimationEvent_FinishAttack();
                int finishCalls = 0;
                Action cancelDuringFinish = () =>
                {
                    finishCalls++;
                    combo.AnimationEvent_FinishAttack();
                    combo.RequestAttack();
                    combo.CancelAttack(playIdle: false, maintainComboIndex: true);
                };
                combo.OnAttackFinished += cancelDuringFinish;
                combo.AnimationEvent_FinishAttack();
                combo.OnAttackFinished -= cancelDuringFinish;
                Check(finishCalls == 1, "Finishing phase prevents recursive Finish", ref checks);
                Check(!combo.IsAttacking && combo.CurrentComboIndex == 1 && !combo.HasQueuedNextAttack,
                    "Cancellation during Finish is not overwritten", ref checks);
                combo.CancelAttack(playIdle: false);

                fixture.Animator.SetTrigger("Idle");
                combo.RequestAttack();
                fixture.Advance(0);
                Check(!fixture.Animator.GetBool("Idle") && fixture.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"),
                    "Pending Idle is cleared before attack", ref checks);
                int frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                fixture.Advance(0.3f);
                Check(fixture.Hits == 1 && combo.IsAttacking, "Animator hit event accepted", ref checks);
                combo.RequestAttack();
                fixture.Advance(0.35f);
                Check(!combo.IsComboWindowOpen && combo.HasQueuedNextAttack, "Animator Close event accepted", ref checks);
                fixture.Advance(0.2f);
                Check(combo.IsAttacking && combo.CurrentComboIndex == 1, "Animator Finish advances queue", ref checks);
                fixture.Advance(0);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                fixture.Advance(0.85f);
                Check(!combo.IsAttacking, "Animator Finish ends combo without queued input", ref checks);

                int hits = fixture.Hits;
                combo.RequestAttack();
                fixture.Advance(0);
                fixture.Advance(0.3f);
                Check(fixture.Hits == hits + 1, "Impact on the starting frame is not lost", ref checks);
                combo.CancelAttack(false);

                fixture.SetTransitionDuration(0.5f);
                combo.RequestAttack();
                fixture.Advance(0);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                fixture.Advance(0.65f);
                combo.CancelAttack(false);
                combo.RequestAttack();
                fixture.Advance(0);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                hits = fixture.Hits;
                fixture.Advance(0.22f);
                Check(combo.IsAttacking && combo.CurrentComboIndex == 0, "Outgoing Finish cannot finish the restarted attack", ref checks);
                Check(fixture.Hits == hits + 1, "Incoming hit accepted during a same-state blend", ref checks);
                fixture.Advance(0.65f);
                Check(!combo.IsAttacking, "Incoming Finish accepted after blend", ref checks);
                fixture.SetTransitionDuration(0);

                combo.RequestAttack();
                fixture.Advance(0);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                fixture.Animator.Play("Idle", 0, 0);
                fixture.Advance(0);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                Check(!combo.IsAttacking && !fixture.Animator.GetBool("Idle"), "Unexpected state exit cancels without forcing Idle", ref checks);

                AnimationUtility.SetAnimationEvents(fixture.AttackClip, Array.Empty<AnimationEvent>());
                combo.RequestAttack();
                fixture.Advance(0);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                fixture.Advance(1.1f);
                frame = Time.frameCount;
                while (Time.frameCount <= frame + 1) yield return null;
                Check(!combo.IsAttacking, "Missing Finish cannot leave the combo stuck", ref checks);
                Check(!combo.CanAcceptAnimationEvent(fixture.Animator, new AnimationEvent()), "Unrelated event rejected", ref checks);
                Result = $"Combo controller: {checks} checks passed.";
            }
        }

        private static void Check(bool condition, string label, ref int count)
        {
            if (!condition) throw new InvalidOperationException(label);
            count++;
        }

        #endregion

        #region Isolated Fixture

        private sealed class Fixture : IDisposable
        {
            public EnemyComboController Combo;
            public Animator Animator;
            public ComboAttackSetSO Attacks;
            public AnimationClip AttackClip;
            public int Hits;

            private readonly List<Object> _list_Objects = new List<Object>();
            private GameObject _root;
            private readonly bool _wasRunningInBackground = Application.runInBackground;

            public Fixture()
            {
                try
                {
                    Application.runInBackground = true;
                    _root = new GameObject("Combo Checks (temporary)");
                    _root.transform.position = Vector3.one * -10000f;
                    Animator = _root.AddComponent<Animator>();
                    Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    var controller = new AnimatorController();
                    _list_Objects.Add(controller);
                    controller.AddLayer("Base Layer");
                    controller.AddParameter("Idle", AnimatorControllerParameterType.Trigger);
                    var machine = controller.layers[0].stateMachine;
                    _list_Objects.Add(machine);
                    var idle = machine.AddState("Idle");
                    _list_Objects.Add(idle);
                    idle.motion = MakeClip("Idle");
                    machine.defaultState = idle;
                    AttackClip = MakeClip("Attack");
                    AnimationUtility.SetAnimationEvents(AttackClip, new[]
                    {
                        new AnimationEvent { time = 0.2f, functionName = "Attack_ExecuteHitbox" },
                        new AnimationEvent { time = 0.6f, functionName = "Attack_CloseComboWindow" },
                        new AnimationEvent { time = 0.8f, functionName = "Attack_Finish" }
                    });
                    for (int i = 1; i <= 3; i++)
                    {
                        var state = machine.AddState("Attack " + i);
                        _list_Objects.Add(state);
                        state.motion = AttackClip;
                    }
                    var transition = machine.AddAnyStateTransition(idle);
                    _list_Objects.Add(transition);
                    transition.hasExitTime = false;
                    transition.duration = 0;
                    transition.canTransitionToSelf = false;
                    transition.AddCondition(AnimatorConditionMode.If, 0, "Idle");
                    Animator.runtimeAnimatorController = controller;
                    Animator.speed = 0;
                    Animator.Update(0);
                    _root.AddComponent<AttackHitbox>();
                    Combo = _root.AddComponent<EnemyComboController>();
                    _root.AddComponent<ComboAnimationEventReceiver>();
                    Attacks = ScriptableObject.CreateInstance<ComboAttackSetSO>();
                    _list_Objects.Add(Attacks);
                    var serialized = new SerializedObject(Attacks);
                    var attacks = serialized.FindProperty("_list_ComboAttackData");
                    attacks.arraySize = 3;
                    for (int i = 0; i < attacks.arraySize; i++)
                    {
                        var attack = attacks.GetArrayElementAtIndex(i);
                        attack.FindPropertyRelative("_animationStateName").stringValue = "Attack " + (i + 1);
                        attack.FindPropertyRelative("_transitionDuration").floatValue = 0;
                        attack.FindPropertyRelative("_targetLayerMask").intValue = 0;
                    }
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                    Combo.SetComboAttackSet(Attacks);
                    Combo.OnAttackHitboxExecutionStarted += _ => Hits++;
                }
                catch { Dispose(); throw; }
            }

            private AnimationClip MakeClip(string name)
            {
                var clip = new AnimationClip { name = name };
                clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.Constant(0, 1, -10000));
                _list_Objects.Add(clip);
                return clip;
            }

            public void Advance(float seconds)
            {
                Animator.speed = 1;
                Animator.Update(seconds);
                Animator.speed = 0;
            }

            public void SetTransitionDuration(float duration)
            {
                var serialized = new SerializedObject(Attacks);
                var attacks = serialized.FindProperty("_list_ComboAttackData");
                for (int i = 0; i < attacks.arraySize; i++)
                    attacks.GetArrayElementAtIndex(i).FindPropertyRelative("_transitionDuration").floatValue = duration;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Combo.SetComboAttackSet(Attacks);
            }

            public void Dispose()
            {
                Application.runInBackground = _wasRunningInBackground;
                if (_root != null) Object.DestroyImmediate(_root);
                for (int i = _list_Objects.Count - 1; i >= 0; i--)
                    if (_list_Objects[i] != null) Object.DestroyImmediate(_list_Objects[i]);
                _list_Objects.Clear();
            }
        }

        #endregion
    }
}
