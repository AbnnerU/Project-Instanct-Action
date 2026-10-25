using System;
using Scripts.Combat;
using Scripts.DashHandler;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor.Combat
{
    public static class PostDashAttackChecks
    {
        [MenuItem("Tools/Combat/Check Post-Dash Controller (Play Mode)")]
        public static void Run()
        {
            var player = GameObject.Find("Player");
            if (!EditorApplication.isPlaying || player == null)
                throw new InvalidOperationException("The check requires Player in Play Mode.");

            var combo = player.GetComponent<PostDashAttackController>();
            var dash = player.GetComponent<Dash2D>();
            var animator = player.GetComponentInChildren<Animator>();
            if (combo == null || dash == null || animator == null || combo.List_ComboAttackData.Count < 2)
                throw new InvalidOperationException("Player needs a normal combo and a configured post-dash attack.");
            if (player.GetComponents<ComboControllerBase>().Length != 1)
                throw new InvalidOperationException("Player must have exactly one combat controller.");

            Vector3 position = player.transform.position;
            AnimatorCullingMode culling = animator.cullingMode;
            bool background = Application.runInBackground;
            player.transform.position += Vector3.right * 1000f;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            Application.runInBackground = true;

            int normalHits = 0, specialHits = 0, stage = 0;
            float retainedTime = 0f;
            double startedAt = EditorApplication.timeSinceStartup;
            ComboAttackData requested = null;
            Action<ComboAttackData> before = data => requested = data;
            Action<AttackExecutionData> hit = data =>
            {
                if (combo.IsPostDashAttack) specialHits++;
                else normalHits++;
            };
            combo.OnBeforeAttack += before;
            combo.OnAttackHitboxExecutionStarted += hit;

            EditorApplication.CallbackFunction tick = null;
            Action<string, bool> finish = (message, success) =>
            {
                EditorApplication.update -= tick;
                if (combo != null)
                {
                    combo.OnBeforeAttack -= before;
                    combo.OnAttackHitboxExecutionStarted -= hit;
                    combo.CancelAttack(false);
                }
                if (player != null) player.transform.position = position;
                if (animator != null) animator.cullingMode = culling;
                Application.runInBackground = background;
                if (success) Debug.Log("[PostDash Checks] PASS: " + message);
                else Debug.LogError("[PostDash Checks] FAILED: " + message);
            };

            tick = () =>
            {
                try
                {
                    if (!EditorApplication.isPlaying || combo == null || EditorApplication.timeSinceStartup - startedAt > 20)
                        throw new InvalidOperationException($"Timeout/interruption at stage {stage}; normal hits={normalHits}, special hits={specialHits}.");
                    EditorApplication.QueuePlayerLoopUpdate();

                    if (stage == 0 && normalHits > 0)
                    {
                        Check(dash.TryDash(DashInputSide.RIGHT), "Dash starts");
                        combo.RequestAttack();
                        Check(combo.HasQueuedAttack && !combo.IsAttacking, "Attack buffers during dash");
                        stage = 1;
                    }
                    else if (stage == 1 && combo.IsPostDashAttack)
                    {
                        retainedTime = combo.ComboRetentionRemaining;
                        Check(retainedTime > 0f && combo.CurrentComboIndex == 1, "Dash retains next normal attack");
                        stage = 2;
                    }
                    else if (stage == 2)
                    {
                        if (combo.IsPostDashAttack)
                            Check(Mathf.Abs(combo.ComboRetentionRemaining - retainedTime) < 0.001f, "Timer stays paused");
                        else if (!combo.IsAttacking)
                        {
                            Check(specialHits == 1, "Special fires exactly one hitbox");
                            Check(combo.CurrentComboIndex == 1 && combo.ComboRetentionRemaining > 0f, "Normal combo restored");
                            combo.RequestAttack();
                            Check(combo.IsAttacking && requested == combo.List_ComboAttackData[1], "Attack 2 resumes");
                            stage = 3;
                        }
                    }
                    else if (stage == 3 && normalHits >= 2)
                    {
                        combo.CancelAttack(false);
                        Check(combo.CurrentComboIndex == 0 && combo.ComboRetentionRemaining == 0f, "Hit cancellation resets retention");
                        finish("normal hitbox, dash buffer, special hitbox, paused retention, resumed attack 2 and cancellation.", true);
                    }
                }
                catch (Exception exception) { finish(exception.Message, false); }
            };

            combo.CancelAttack(false);
            combo.RequestAttack();
            if (!combo.IsAttacking || combo.IsPostDashAttack)
            {
                finish("Normal attack did not start.", false);
                return;
            }
            EditorApplication.update += tick;
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
