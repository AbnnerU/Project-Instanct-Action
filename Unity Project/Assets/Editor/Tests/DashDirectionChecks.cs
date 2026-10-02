using System;
using System.Linq;
using System.Reflection;
using Scripts.Combat;
using Scripts.DashHandler;
using Scripts.Direction;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Editor.Tests
{
    public static class DashDirectionChecks
    {
        private static int _checks;

        [MenuItem("Tools/Project Instanct Action/Dash/Run Direction Checks")]
        public static void Run()
        {
            _checks = 0;
            GameObject owner = new GameObject("DashChecks");
            GameObject visual = new GameObject("DashChecksVisual");
            try
            {
                Dash2D dash = owner.AddComponent<Dash2D>();
                SetField(dash, "_targetTransform", owner.transform);
                SetField(dash, "_directionReference", visual.transform);
                foreach (DashReferenceAxis axis in Enum.GetValues(typeof(DashReferenceAxis)))
                {
                    SetField(dash, "_referenceAxis", axis);
                    visual.transform.rotation = Quaternion.Euler(17f, 63f, 11f);
                    Vector3 forward = axis == DashReferenceAxis.FORWARD ? visual.transform.forward
                        : axis == DashReferenceAxis.RIGHT ? visual.transform.right : visual.transform.up;
                    Vector3 perpendicular = axis == DashReferenceAxis.UP ? visual.transform.right : visual.transform.up;
                    Near(1f, dash.GetDirectionDot(forward * 7f), axis + " forward normalization");
                    Near(-1f, dash.GetDirectionDot(-forward), axis + " backward");
                    Near(0f, dash.GetDirectionDot(perpendicular), axis + " perpendicular");
                }
                foreach (DashMovementAxis axis in Enum.GetValues(typeof(DashMovementAxis)))
                {
                    SetField(dash, "_movementAxis", axis);
                    Vector3 vector = axis == DashMovementAxis.X ? Vector3.right
                        : axis == DashMovementAxis.Y ? Vector3.up : Vector3.forward;
                    Expect(dash.GetDirectionVector(DashInputSide.RIGHT) == vector, axis + " positive input");
                    Expect(dash.GetDirectionVector(DashInputSide.LEFT) == -vector, axis + " negative input");
                }

                MethodInfo startCooldown = typeof(Dash2D).GetMethod("StartCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
                startCooldown.Invoke(dash, new object[] { DashInputSide.LEFT });
                Expect(dash.IsOnCooldown(DashInputSide.LEFT), "Left cooldown starts");
                Expect(!dash.IsOnCooldown(DashInputSide.RIGHT), "Right cooldown independent");
                Near(1f, dash.GetCooldownFillAmount(DashInputSide.LEFT), "Initial cooldown fill");
                visual.transform.Rotate(0f, 180f, 0f);
                Expect(dash.IsOnCooldown(DashInputSide.LEFT), "Turning does not switch cooldown");

                DirectionHandler direction = owner.AddComponent<DirectionHandler>();
                SetField(direction, "_visualTransform", visual.transform);
                direction.LookRight();
                direction.BlockDirection(owner);
                direction.BlockDirection(visual);
                direction.LookLeft();
                Expect(direction.CurrentDirection == DirectionSide.RIGHT, "Direction blocked");
                direction.UnblockDirection(owner);
                Expect(!direction.Active, "Other lock retained");
                direction.UnblockDirection(visual);
                direction.LookLeft();
                Expect(direction.CurrentDirection == DirectionSide.LEFT, "Direction released");
                direction.Disable();
                direction.BlockDirection(owner);
                direction.UnblockDirection(owner);
                Expect(!direction.Active, "Previously disabled state retained");

                Expect(!typeof(ComboController).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Any(f => f.FieldType == typeof(DirectionHandler)), "Combo has no DirectionHandler");
                Expect(!typeof(DashBase).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Any(f => f.FieldType == typeof(InputActionReference) || f.Name.ToLowerInvariant().Contains("cooldown")),
                    "Dash base has no input or cooldown fields");
                InputActionAsset input = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/Inputs.inputactions");
                Expect(input != null, "Input asset exists");
                foreach (string side in new[] { "Left", "Right" })
                {
                    InputAction action = input.FindAction("Gameplay/Dash " + side, true);
                    Expect(action.interactions.Contains("MultiTap") && action.interactions.Contains("tapCount=2"), side + " double tap");
                    Expect(input.FindAction("Gameplay/Look " + side, true) != null, side + " look input");
                }
                Expect(input.FindAction("Gameplay/Attack", true) != null, "Single attack action");
                Expect(input.FindAction("Gameplay/Attack Left") == null && input.FindAction("Gameplay/Attack Right") == null,
                    "No directional attack actions");
                Debug.Log($"Dash direction checks PASSED: {_checks} assertions.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(owner);
                UnityEngine.Object.DestroyImmediate(visual);
            }
        }

        private static void SetField(object target, string name, object value)
        {
            for (Type type = target.GetType(); type != null; type = type.BaseType)
            {
                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field == null) continue;
                field.SetValue(target, value);
                return;
            }
            throw new MissingFieldException(target.GetType().Name, name);
        }

        private static void Near(float expected, float actual, string label) =>
            Expect(Mathf.Abs(expected - actual) < 0.0001f, label + $" ({expected} != {actual})");

        private static void Expect(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException("Dash check failed: " + label);
            _checks++;
        }
    }
}
