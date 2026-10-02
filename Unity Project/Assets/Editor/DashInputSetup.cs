using System;
using System.Linq;
using Scripts.Combat;
using Scripts.DashHandler;
using Scripts.Direction;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Editor
{
    public static class DashInputSetup
    {
        [MenuItem("Tools/Project Instanct Action/Dash/Configure Selected Player")]
        public static void ConfigureSelectedPlayer()
        {
            GameObject player = Selection.activeGameObject;
            if (player == null || !player.scene.IsValid())
                throw new InvalidOperationException("Select the player in the scene or Prefab Mode first.");

            Dash2D dash = player.GetComponent<Dash2D>();
            DirectionHandler direction = player.GetComponent<DirectionHandler>();
            ComboController combo = player.GetComponent<ComboController>();
            if (dash == null || direction == null || combo == null)
                throw new InvalidOperationException("Selected player must have Dash2D, DirectionHandler and ComboController.");

            const string inputPath = "Assets/Input/Inputs.inputactions";
            InputActionReference[] references = AssetDatabase.LoadAllAssetsAtPath(inputPath).OfType<InputActionReference>().ToArray();
            InputActionReference Find(string name) => references.FirstOrDefault(r => r.action != null && r.action.name == name);
            InputActionReference attack = Find("Attack"), lookLeft = Find("Look Left"), lookRight = Find("Look Right"),
                dashLeft = Find("Dash Left"), dashRight = Find("Dash Right");
            if (new[] { attack, lookLeft, lookRight, dashLeft, dashRight }.Any(r => r == null))
                throw new InvalidOperationException("Input action references missing in " + inputPath + ". Reimport the asset first.");

            Transform visual = new SerializedObject(direction).FindProperty("_visualTransform").objectReferenceValue as Transform;
            if (visual == null)
                throw new InvalidOperationException("DirectionHandler needs its Visual Transform assigned before setup.");

            Undo.SetCurrentGroupName("Configure player dash input");
            int group = Undo.GetCurrentGroup();
            Dash_DirectionHandler bridge = player.GetComponent<Dash_DirectionHandler>();
            if (bridge == null) bridge = Undo.AddComponent<Dash_DirectionHandler>(player);
            SetReference(dash, "_leftInput", dashLeft);
            SetReference(dash, "_rightInput", dashRight);
            SetReference(direction, "_leftInput", lookLeft);
            SetReference(direction, "_rightInput", lookRight);
            SetReference(bridge, "_dash", dash);
            SetReference(bridge, "_directionHandler", direction);
            SetReference(dash, "_directionReference", visual);
            SetReference(combo, "_attackInput", attack);

            Dash_ComboControllerHandler comboBridge = player.GetComponent<Dash_ComboControllerHandler>();
            if (comboBridge == null) comboBridge = Undo.AddComponent<Dash_ComboControllerHandler>(player);
            SetReference(comboBridge, "_dashHandler", dash);
            SetReference(comboBridge, "_comboController", combo);

            DashAnimation dashAnimation = player.GetComponent<DashAnimation>();
            if (dashAnimation != null) SetReference(dashAnimation, "_playerDash", dash);
            DashInvulnerability invulnerability = player.GetComponent<DashInvulnerability>();
            if (invulnerability != null) SetReference(invulnerability, "_playerDash", dash);
            HitReaction_BlockPlayerDash hitReaction = player.GetComponent<HitReaction_BlockPlayerDash>();
            if (hitReaction != null) SetReference(hitReaction, "_playerDashComponent", dash);

            Animator animator = new SerializedObject(combo).FindProperty("_animator").objectReferenceValue as Animator;
            RuntimeAnimatorController controller = animator != null ? animator.runtimeAnimatorController : null;
            while (controller is AnimatorOverrideController overrides)
                controller = overrides.runtimeAnimatorController;
            if (controller is AnimatorController animatorController && !animatorController.parameters.Any(p => p.name == "DashDirection"))
            {
                Undo.RecordObject(animatorController, "Add DashDirection parameter");
                animatorController.AddParameter("DashDirection", AnimatorControllerParameterType.Float);
            }
            Undo.CollapseUndoOperations(group);
            Debug.Log("Player input configured. Save the scene/prefab and Animator Controller after reviewing changes.", player);
        }

        private static void SetReference(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null) throw new MissingFieldException(target.GetType().Name, field);
            property.objectReferenceValue = value;
            serialized.ApplyModifiedProperties();
        }
    }
}
