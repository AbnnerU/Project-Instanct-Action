using System;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.DashHandler;
using Scripts.Direction;
using Scripts.Weapon;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Scripts.Editor.CombatWorkshop
{
    public sealed class CombatWorkshopPreview : IDisposable
    {
        private readonly PreviewRenderUtility _preview = new PreviewRenderUtility();
        private readonly Dictionary<string, Transform> _dc_Sockets = new Dictionary<string, Transform>();
        private readonly List<GameObject> _list_Models = new List<GameObject>();
        private readonly Dictionary<Renderer, Material[]> _dc_OriginalMaterials = new Dictionary<Renderer, Material[]>();
        private Material _neutralMaterial;
        private bool _appliedOriginalMaterials;
        private GameObject _stage;
        private GameObject _character;
        private Animator _animator;
        private Transform _hitboxOrigin;
        //private Transform _movementReference;
        private Transform _visualTransform;
        private Vector3 _rightEuler = new Vector3(0, 90, 0);
        private Vector3 _leftEuler = new Vector3(0, -90, 0);
        private bool _useLocalDirection = true;
        private bool _rotateHitbox;
        private PlayableGraph _graph;
        private AnimationClipPlayable _clipPlayable;
        private AnimationClip _clip;
        private Vector3 _animatorPosition;
        private Quaternion _animatorRotation;
        private float _zoom = 1.65f;
        private Vector2 _orbit;
        private Vector3 _focus = new Vector3(0, 1, 0);
        private Vector3 _dragStart;
        private Vector3 _dragWorld;
        private Plane _dragPlane;
        private int _dragControl;

        public bool Ready => _animator != null;
        public bool Left;
        //public bool ShowAdvance = true;
        public bool OriginalMaterials;
        public Vector3 TargetPosition = new Vector3(1.7f, 1, 0);
        public Vector3 TargetSize = new Vector3(0.6f, 2, 0.6f);
        public List<string> List_Warnings { get; } = new List<string>();
        public IEnumerable<string> SocketNames => _dc_Sockets.Keys;

        public CombatWorkshopPreview()
        {
            _preview.camera.orthographic = true;
            _preview.camera.nearClipPlane = 0.01f;
            _preview.camera.farClipPlane = 100;
            _preview.camera.clearFlags = CameraClearFlags.SolidColor;
            _preview.camera.backgroundColor = new Color(0.14f, 0.15f, 0.17f);
            _preview.lights[0].intensity = 2.5f;
            _preview.lights[0].transform.rotation = Quaternion.Euler(40, 30, 0);
            _preview.lights[1].intensity = 1.5f;
            _preview.ambientColor = new Color(0.7f, 0.7f, 0.7f);
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader != null)
            {
                _neutralMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave, color = new Color(0.65f, 0.68f, 0.7f) };
                if (_neutralMaterial.HasProperty("_Smoothness")) _neutralMaterial.SetFloat("_Smoothness", 0.25f);
            }
        }

        #region Setup
        public void Build(GameObject source, WeaponLoadoutSO loadout, WeaponSocketRegistry externalRegistry = null)
        {
            ClearCharacter();
            List_Warnings.Clear();
            if (source == null) return;
            _stage = new GameObject("Combat Workshop Preview");
            _stage.hideFlags = HideFlags.HideAndDontSave;
            _stage.SetActive(false);
            _preview.AddSingleGO(_stage);
            // The inactive parent prevents gameplay Awake/OnEnable from running on the clone.
            _character = UnityEngine.Object.Instantiate(source, _stage.transform, false);
            _character.transform.position = Vector3.zero;
            _animator = _character.GetComponentInChildren<Animator>(true);
            var hitbox = _character.GetComponentInChildren<AttackHitbox>(true);
            if (hitbox != null)
            {
                var fields = new SerializedObject(hitbox);
                _hitboxOrigin = fields.FindProperty("_hitboxOrigin").objectReferenceValue as Transform;
                _rotateHitbox = fields.FindProperty("_useHitboxOriginRotation").boolValue;
            }
            else List_Warnings.Add("Character has no AttackHitbox; using character origin.");
            if (_hitboxOrigin == null) _hitboxOrigin = _character.transform;
            //var combo = _character.GetComponentInChildren<ComboControllerBase>(true);
            //if (combo != null) _movementReference = new SerializedObject(combo).FindProperty("_movementDirectionReference").objectReferenceValue as Transform;
            //if (_movementReference == null) _movementReference = _character.transform;
            var direction = _character.GetComponentInChildren<DirectionHandler>(true);
            if (direction != null)
            {
                var fields = new SerializedObject(direction);
                _visualTransform = fields.FindProperty("_visualTransform").objectReferenceValue as Transform;
                _rightEuler = fields.FindProperty("_rightRotation").FindPropertyRelative("EulerAngles").vector3Value;
                _leftEuler = fields.FindProperty("_leftRotation").FindPropertyRelative("EulerAngles").vector3Value;
                _useLocalDirection = fields.FindProperty("_useLocalEulerAngles").boolValue;
            }
            if (_visualTransform == null) _visualTransform = _character.transform;
            foreach (var registry in _character.GetComponentsInChildren<WeaponSocketRegistry>(true))
            {
                var sockets = new SerializedObject(registry).FindProperty("_list_WeaponSockets");
                for (int i = 0; i < sockets.arraySize; i++)
                {
                    var socket = sockets.GetArrayElementAtIndex(i);
                    string id = socket.FindPropertyRelative("SocketID").stringValue;
                    var reference = socket.FindPropertyRelative("SocketTransform").objectReferenceValue as Transform;
                    if (!string.IsNullOrWhiteSpace(id) && reference != null) _dc_Sockets[id] = reference;
                }
            }
            if (externalRegistry != null)
            {
                var sockets = new SerializedObject(externalRegistry).FindProperty("_list_WeaponSockets");
                for (int i = 0; i < sockets.arraySize; i++)
                {
                    var socket = sockets.GetArrayElementAtIndex(i);
                    string id = socket.FindPropertyRelative("SocketID").stringValue;
                    var original = socket.FindPropertyRelative("SocketTransform").objectReferenceValue as Transform;
                    if (string.IsNullOrWhiteSpace(id) || original == null) continue;
                    if (!original.IsChildOf(source.transform)) { List_Warnings.Add($"Socket {id} belongs to another character."); continue; }
                    string path = AnimationUtility.CalculateTransformPath(original, source.transform);
                    var reference = string.IsNullOrEmpty(path) ? _character.transform : _character.transform.Find(path);
                    if (reference != null) _dc_Sockets[id] = reference;
                }
            }
            StripGameplay(_character);
            foreach (var renderer in _character.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                _dc_OriginalMaterials[renderer] = renderer.sharedMaterials;
            _appliedOriginalMaterials = !OriginalMaterials;
            if (_animator != null)
            {
                _animator.runtimeAnimatorController = null;
                _animator.applyRootMotion = false;
                _animator.fireEvents = false;
                _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                _animatorPosition = _animator.transform.localPosition;
                _animatorRotation = _animator.transform.localRotation;
            }
            if (loadout != null)
            {
                foreach (var model in loadout.List_WeaponModels)
                {
                    GameObject instance = null;
                    var prefab = model?.WeaponPrefabReference?.editorAsset as GameObject;
                    Transform socket = model != null ? GetSocket(model.SocketID) : null;
                    if (prefab != null && socket != null)
                    {
                        instance = UnityEngine.Object.Instantiate(prefab, socket, false);
                        StripGameplay(instance);
                        ApplyModel(instance.transform, model);
                    }
                    else List_Warnings.Add($"Weapon model or socket not found: {model?.SocketID}");
                    _list_Models.Add(instance);
                }
            }
            _stage.SetActive(true);
            _character.SetActive(true);
            Frame();
        }

        private static void StripGameplay(GameObject root)
        {
            foreach (var script in root.GetComponentsInChildren<MonoBehaviour>(true))
                if (script != null) UnityEngine.Object.DestroyImmediate(script);
            foreach (var body in root.GetComponentsInChildren<Rigidbody>(true)) UnityEngine.Object.DestroyImmediate(body);
            foreach (var agent in root.GetComponentsInChildren<UnityEngine.AI.NavMeshAgent>(true)) UnityEngine.Object.DestroyImmediate(agent);
            foreach (var collider in root.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
            foreach (var canvas in root.GetComponentsInChildren<Canvas>(true)) canvas.gameObject.SetActive(false);
            foreach (var audio in root.GetComponentsInChildren<AudioSource>(true)) UnityEngine.Object.DestroyImmediate(audio);
            foreach (var particle in root.GetComponentsInChildren<ParticleSystem>(true)) particle.gameObject.SetActive(false);
            foreach (var camera in root.GetComponentsInChildren<Camera>(true)) camera.enabled = false;
        }

        private Transform GetSocket(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (_dc_Sockets.TryGetValue(id, out var socket)) return socket;
            foreach (var child in _character.GetComponentsInChildren<Transform>(true))
                if (child.name == id) { _dc_Sockets[id] = child; return child; }
            return null;
        }

        private static void ApplyModel(Transform model, WeaponModelSpawnData data)
        {
            model.localPosition = data.LocalPositionOffset;
            model.localEulerAngles = data.LocalEulerAngles;
            model.localScale = data.LocalScale;
        }

        public void Frame()
        {
            _orbit = Vector2.zero;
            _focus = new Vector3(0, 1, 0);
            _zoom = 1.65f;
            if (_character == null) return;
            var renderers = _character.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers.Length == 0) return;
            Bounds bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            _focus = bounds.center;
            _focus.x = (_focus.x + TargetPosition.x) * 0.5f;
            _zoom = Mathf.Max(1, bounds.size.y * 0.7f);
        }
        #endregion

        #region Sampling
        public void Sample(AnimationClip clip, float time, ComboAttackData attack)
        {
            if (!Ready) return;
            if (_appliedOriginalMaterials != OriginalMaterials)
            {
                foreach (var entry in _dc_OriginalMaterials)
                {
                    if (entry.Key == null) continue;
                    var materials = new Material[entry.Value.Length];
                    for (int i = 0; i < materials.Length; i++) materials[i] = OriginalMaterials || _neutralMaterial == null ? entry.Value[i] : _neutralMaterial;
                    entry.Key.sharedMaterials = materials;
                }
                _appliedOriginalMaterials = OriginalMaterials;
            }
            if (_clip != clip || !_graph.IsValid())
            {
                if (_graph.IsValid()) _graph.Destroy();
                _clip = clip;
                if (clip != null)
                {
                    _graph = PlayableGraph.Create("Combat Workshop");
                    _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
                    _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
                    _clipPlayable.SetApplyFootIK(false);
                    var output = AnimationPlayableOutput.Create(_graph, "Preview", _animator);
                    output.SetSourcePlayable(_clipPlayable);
                    _graph.Play();
                }
            }
            if (_graph.IsValid())
            {
                _clipPlayable.SetTime(time);
                _graph.Evaluate(0);
            }
            _animator.transform.localPosition = _animatorPosition;
            _animator.transform.localRotation = _animatorRotation;
            if (_useLocalDirection) _visualTransform.localEulerAngles = Left ? _leftEuler : _rightEuler;
            else _visualTransform.eulerAngles = Left ? _leftEuler : _rightEuler;
            /*
            Vector3 movement = Vector3.zero;
            if (ShowAdvance && attack != null && time >= attack.AdvanceDelay)
            {
                Vector3 direction = Vector3.ProjectOnPlane(_movementReference.forward, Vector3.up).normalized;
                float t = attack.AdvanceDuration <= 0 ? 1 : Mathf.Clamp01((time - attack.AdvanceDelay) / attack.AdvanceDuration);
                float value = t >= 1 ? 1 : attack.AdvanceCurve != null ? attack.AdvanceCurve.Evaluate(t) : t;
                movement = direction * (attack.AdvanceDistance * value);
            }
            _character.transform.position = movement;
            */
            _character.transform.position = Vector3.zero;
        }

        public Quaternion HitboxRotation => _rotateHitbox && _hitboxOrigin != null ? _hitboxOrigin.rotation : Quaternion.identity;
        public Vector3 GetHitboxCenter(ComboAttackData data) => _hitboxOrigin.position + HitboxRotation * data.HitboxCenterOffset;
        #endregion

        #region View And Handles
        public void Draw(Rect rect, ComboAttackData data, bool impact, bool editHitbox, WeaponModelSpawnData model, int modelIndex,
            Action<Vector3, Vector3> changeHitbox, Action<Vector3> changeWeapon, bool rotateWeapon, Action<Vector3> changeWeaponRotation)
        {
            if (rect.width < 2 || rect.height < 2) return;
            Event current = Event.current;
            if (rect.Contains(current.mousePosition))
            {
                if (current.type == EventType.ScrollWheel) { _zoom = Mathf.Clamp(_zoom * (1 + current.delta.y * 0.05f), 0.2f, 15); current.Use(); }
                if (current.type == EventType.MouseDrag && current.button == 1) { _orbit += current.delta * 0.4f; current.Use(); }
            }
            var camera = _preview.camera;
            camera.orthographicSize = _zoom;
            camera.aspect = rect.width / rect.height;
            camera.transform.rotation = Quaternion.Euler(_orbit.y, _orbit.x, 0);
            camera.transform.position = _focus - camera.transform.forward * 10;
            if (current.type == EventType.Repaint)
            {
                _preview.BeginPreview(rect, GUIStyle.none);
                _preview.Render(true);
                GUI.DrawTexture(rect, _preview.EndPreview(), ScaleMode.StretchToFill, false);
            }
            GUI.BeginGroup(rect);
            rect = new Rect(0, 0, rect.width, rect.height);
            Handles.BeginGUI();
            for (int x = -8; x <= 8; x++)
            {
                Handles.color = new Color(0.38f, 0.4f, 0.42f, 0.45f);
                Line(rect, new Vector3(x, 0, -4), new Vector3(x, 0, 4));
                Line(rect, new Vector3(-8, 0, x), new Vector3(8, 0, x));
            }
            bool targetHit = Ready && data != null && impact && OverlapsTarget(GetHitboxCenter(data), HitboxRotation, data.HitboxHalfExtents, TargetPosition, TargetSize * 0.5f);
            DrawBox(rect, TargetPosition, Quaternion.identity, TargetSize * 0.5f, targetHit ? new Color(1, 0.35f, 0.15f) : new Color(0.25f, 0.85f, 0.75f));
            if (Ready && data != null)
            {
                Vector3 center = GetHitboxCenter(data);
                DrawBox(rect, center, HitboxRotation, data.HitboxHalfExtents, impact ? new Color(1, 0.35f, 0.15f) : new Color(1, 0.83f, 0.2f));
                if (editHitbox)
                {
                    Vector3 moved = DragPoint(rect, center, 8101, new Color(1, 0.83f, 0.2f));
                    if (moved != center) changeHitbox(Quaternion.Inverse(HitboxRotation) * (moved - _hitboxOrigin.position), data.HitboxHalfExtents);
                    Vector3 corner = center + HitboxRotation * data.HitboxHalfExtents;
                    Vector3 resized = DragPoint(rect, corner, 8102, Color.white);
                    if (resized != corner)
                    {
                        Vector3 local = Quaternion.Inverse(HitboxRotation) * (resized - center);
                        changeHitbox(data.HitboxCenterOffset, new Vector3(Mathf.Max(0.01f, Mathf.Abs(local.x)), Mathf.Max(0.01f, Mathf.Abs(local.y)), Mathf.Max(0.01f, Mathf.Abs(local.z))));
                    }
                }
            }
            if (model != null && modelIndex >= 0 && modelIndex < _list_Models.Count && _list_Models[modelIndex] != null)
            {
                var transform = _list_Models[modelIndex].transform;
                ApplyModel(transform, model);
                if (rotateWeapon) DrawRotationHandle(rect, transform, changeWeaponRotation);
                else
                {
                    var point = DragPoint(rect, transform.position, 8103, new Color(0.35f, 0.75f, 1));
                    if (point != transform.position) changeWeapon(transform.parent.InverseTransformPoint(point));
                }
            }
            Handles.EndGUI();
            GUI.EndGroup();
        }

        private void DrawRotationHandle(Rect rect, Transform model, Action<Vector3> changeRotation)
        {
            Vector2 center = Project(rect, model.position);
            var points = new Vector3[65];
            for (int i = 0; i < points.Length; i++) points[i] = center + new Vector2(Mathf.Cos(i * Mathf.PI / 32), Mathf.Sin(i * Mathf.PI / 32)) * 45;
            Handles.color = new Color(0.35f, 0.75f, 1);
            Handles.DrawAAPolyLine(2, points);
            int id = GUIUtility.GetControlID(8104, FocusType.Passive);
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition) && Mathf.Abs(Vector2.Distance(center, e.mousePosition) - 45) < 7)
            { GUIUtility.hotControl = id; e.Use(); }
            if (GUIUtility.hotControl != id) return;
            if (e.type == EventType.MouseDrag)
            {
                Vector2 previous = e.mousePosition - e.delta - center;
                Vector2 next = e.mousePosition - center;
                float angle = -Vector2.SignedAngle(previous, next);
                Quaternion world = Quaternion.AngleAxis(angle, _preview.camera.transform.forward) * model.rotation;
                changeRotation((Quaternion.Inverse(model.parent.rotation) * world).eulerAngles);
                e.Use();
            }
            if (e.type == EventType.MouseUp) { GUIUtility.hotControl = 0; e.Use(); }
        }

        public static bool OverlapsTarget(Vector3 center, Quaternion rotation, Vector3 half, Vector3 targetCenter, Vector3 targetHalf)
        {
            Vector3[] box = { rotation * Vector3.right, rotation * Vector3.up, rotation * Vector3.forward };
            Vector3[] target = { Vector3.right, Vector3.up, Vector3.forward };
            var axes = new List<Vector3>(15);
            axes.AddRange(box);
            axes.AddRange(target);
            foreach (var a in box) foreach (var b in target) axes.Add(Vector3.Cross(a, b));
            foreach (var axis in axes)
            {
                if (axis.sqrMagnitude < 0.000001f) continue;
                float radius = 0;
                for (int i = 0; i < 3; i++) radius += Mathf.Abs(Vector3.Dot(axis, box[i])) * half[i] + Mathf.Abs(Vector3.Dot(axis, target[i])) * targetHalf[i];
                if (Mathf.Abs(Vector3.Dot(targetCenter - center, axis)) > radius + 0.00001f) return false;
            }
            return true;
        }

        private Vector2 Project(Rect rect, Vector3 point)
        {
            var v = _preview.camera.WorldToViewportPoint(point);
            return new Vector2(rect.x + rect.width * v.x, rect.y + rect.height * (1 - v.y));
        }

        private Ray GetRay(Rect rect, Vector2 point) => _preview.camera.ViewportPointToRay(new Vector3((point.x - rect.x) / rect.width, 1 - (point.y - rect.y) / rect.height, 0));

        private Vector3 DragPoint(Rect rect, Vector3 world, int hint, Color color)
        {
            int id = GUIUtility.GetControlID(hint, FocusType.Passive);
            Vector2 point = Project(rect, world);
            Rect handle = new Rect(point.x - 5, point.y - 5, 10, 10);
            if (rect.Contains(point)) EditorGUI.DrawRect(handle, color);
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition) && handle.Contains(e.mousePosition))
            {
                _dragPlane = new Plane(-_preview.camera.transform.forward, world);
                if (_dragPlane.Raycast(GetRay(rect, e.mousePosition), out float distance))
                {
                    _dragStart = GetRay(rect, e.mousePosition).GetPoint(distance);
                    _dragWorld = world;
                    _dragControl = id;
                    GUIUtility.hotControl = id;
                    e.Use();
                }
            }
            if (GUIUtility.hotControl == id && _dragControl == id)
            {
                if (e.type == EventType.MouseDrag && _dragPlane.Raycast(GetRay(rect, e.mousePosition), out float distance))
                {
                    Vector3 result = _dragWorld + GetRay(rect, e.mousePosition).GetPoint(distance) - _dragStart;
                    e.Use();
                    return result;
                }
                if (e.type == EventType.MouseUp) { GUIUtility.hotControl = 0; _dragControl = 0; e.Use(); }
            }
            return world;
        }

        private void Line(Rect rect, Vector3 a, Vector3 b) => Handles.DrawAAPolyLine(1.5f, Project(rect, a), Project(rect, b));

        private void DrawBox(Rect rect, Vector3 center, Quaternion rotation, Vector3 extents, Color color)
        {
            Handles.color = color;
            var corners = new Vector3[8];
            for (int i = 0; i < 8; i++) corners[i] = center + rotation * Vector3.Scale(extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
            for (int i = 0; i < 8; i++) for (int bit = 1; bit <= 4; bit *= 2) if ((i & bit) == 0) Line(rect, corners[i], corners[i | bit]);
        }
        #endregion

        public void Dispose()
        {
            ClearCharacter();
            _preview.Cleanup();
            if (_neutralMaterial != null) UnityEngine.Object.DestroyImmediate(_neutralMaterial);
        }

        private void ClearCharacter()
        {
            if (_graph.IsValid()) _graph.Destroy();
            if (_stage != null) UnityEngine.Object.DestroyImmediate(_stage);
            _character = null;
            _animator = null;
            _hitboxOrigin = null;
            //_movementReference = null;
            _visualTransform = null;
            _rotateHitbox = false;
            _clip = null;
            _dc_Sockets.Clear();
            _dc_OriginalMaterials.Clear();
            _list_Models.Clear();
        }
    }
}
