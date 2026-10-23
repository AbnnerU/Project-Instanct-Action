using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Combat;
using Scripts.Weapon;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scripts.Editor.CombatWorkshop
{
    public class CombatWorkshopWindow : EditorWindow
    {
        [SerializeField] private CombatWorkshopDocument _document;
        [SerializeField] private GameObject _character;
        [SerializeField] private WeaponSocketRegistry _socketRegistry;
        [SerializeField] private int _attackIndex;
        [SerializeField] private bool _editPostDash;
        [SerializeField] private int _modelIndex;
        [SerializeField] private int _tab;
        private CombatWorkshopPreview _preview;
        private Vector2 _scroll;
        private bool _playing;
        private bool _loop = true;
        private bool _playCombo;
        private bool _previewIdle;
        private bool _editHitbox = true;
        private bool _rotateWeapon;
        [SerializeField] private float _time;
        private float _playbackSpeed = 100;
        private double _lastUpdate;
        private int _dragMarker = -1;
        private string _status = "Ready";
        private List<string> _list_Errors = new List<string>();

        private WorkshopAttack Attack => _document == null ? null : _editPostDash
            ? (_document.Loadout.HasPostDashAttack ? _document.PostDashAttack : null)
            : (_attackIndex >= 0 && _attackIndex < _document.List_Attacks.Count ? _document.List_Attacks[_attackIndex] : null);
        private ComboAttackData AttackData => Attack == null ? null : _editPostDash ? _document.Loadout.PostDashAttack
            : (_attackIndex < _document.Combo.List_ComboAttackData.Count ? _document.Combo.List_ComboAttackData[_attackIndex] : null);
        private AnimationClip CurrentClip => _previewIdle ? _document?.IdleClip : Attack?.Clip;

        #region Lifecycle
        [MenuItem("Tools/Combat Workshop")]
        public static void Open()
        {
            var window = GetWindow<CombatWorkshopWindow>();
            window.titleContent = new GUIContent("Combat Workshop");
            window.minSize = new Vector2(960, 620);
            if (window._document == null)
            {
                var selected = Selection.activeObject as WeaponLoadoutSO;
                if (selected == null)
                {
                    var paths = AssetDatabase.FindAssets("t:WeaponLoadoutSO").Select(AssetDatabase.GUIDToAssetPath).OrderBy(x => x).ToArray();
                    string path = paths.FirstOrDefault(x => x.IndexOf("Hand", StringComparison.OrdinalIgnoreCase) >= 0) ?? paths.FirstOrDefault();
                    if (path != null) selected = AssetDatabase.LoadAssetAtPath<WeaponLoadoutSO>(path);
                }
                var player = GameObject.Find("Player");
                window.LoadLoadout(selected, player);
            }
            window.Show();
        }

        public void LoadLoadout(WeaponLoadoutSO source, GameObject character)
        {
            if (_document != null) _document.Dispose();
            _document = CombatWorkshopDocument.Create(source);
            _character = character;
            FindSocketRegistry();
            _attackIndex = 0;
            _time = 0;
            _playing = false;
            hasUnsavedChanges = false;
            _list_Errors.Clear();
            RebuildPreview();
            Repaint();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Combat Workshop");
            minSize = new Vector2(960, 620);
            saveChangesMessage = "Save Combat Workshop changes before closing?";
            _preview = new CombatWorkshopPreview();
            _lastUpdate = EditorApplication.timeSinceStartup;
            EditorApplication.update += Tick;
            Undo.undoRedoPerformed += OnUndo;
            if (_document != null) RebuildPreview();
        }

        private void OnDisable()
        {
            EditorApplication.update -= Tick;
            Undo.undoRedoPerformed -= OnUndo;
            _preview?.Dispose();
            _preview = null;
        }

        private void OnDestroy()
        {
            if (_document != null) _document.Dispose();
        }

        private void OnUndo()
        {
            if (_document == null) return;
            _attackIndex = Mathf.Clamp(_attackIndex, 0, Mathf.Max(0, _document.List_Attacks.Count - 1));
            RebuildPreview();
            Dirty();
        }

        private void RebuildPreview()
        {
            if (_preview == null) _preview = new CombatWorkshopPreview();
            _preview.Build(_character, _document?.Loadout, _socketRegistry);
        }

        private void FindSocketRegistry()
        {
            _socketRegistry = _character != null ? _character.GetComponentInChildren<WeaponSocketRegistry>(true) : null;
            if (_character == null || _socketRegistry != null) return;
            foreach (var registry in FindObjectsByType<WeaponSocketRegistry>(FindObjectsInactive.Include))
            {
                var sockets = new SerializedObject(registry).FindProperty("_list_WeaponSockets");
                for (int i = 0; i < sockets.arraySize; i++)
                {
                    var socket = sockets.GetArrayElementAtIndex(i).FindPropertyRelative("SocketTransform").objectReferenceValue as Transform;
                    if (socket == null || !socket.IsChildOf(_character.transform)) continue;
                    _socketRegistry = registry;
                    return;
                }
            }
        }

        private void Dirty()
        {
            hasUnsavedChanges = true;
            _status = "Unsaved changes";
            Repaint();
        }
        #endregion

        #region Layout
        private void OnGUI()
        {
            if (_document == null) { if (GUILayout.Button("Open Combat Workshop")) Open(); return; }
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                DrawToolbar();
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope(GUILayout.Width(365)))
                    {
                        _tab = GUILayout.Toolbar(_tab, new[] { "Loadout", "Weapons", "Attacks", "Checks" });
                        _scroll = EditorGUILayout.BeginScrollView(_scroll);
                        EditorGUI.BeginChangeCheck();
                        switch (_tab)
                        {
                            case 0: DrawLoadout(); break;
                            case 1: DrawWeapons(); break;
                            case 2: DrawAttacks(); break;
                            case 3: DrawValidation(); break;
                        }
                        if (EditorGUI.EndChangeCheck()) Dirty();
                        EditorGUILayout.EndScrollView();
                    }
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawViewToolbar();
                        Rect view = GUILayoutUtility.GetRect(100, 10000, 240, 10000, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        _preview.Sample(CurrentClip, _time, _previewIdle ? null : AttackData);
                        bool impact = !_previewIdle && Attack != null && Attack.List_Markers.Any(x => x.Type == CombatMarkerType.HIT && Mathf.Abs(x.Time - _time) <= 0.055f);
                        var models = _document.Loadout.List_WeaponModels;
                        var model = _tab == 1 && _modelIndex >= 0 && _modelIndex < models.Count ? models[_modelIndex] : null;
                        _preview.Draw(view, _previewIdle ? null : AttackData, impact, _tab == 2 && _editHitbox, model, _modelIndex, ChangeHitbox, ChangeWeapon, _rotateWeapon, ChangeWeaponRotation);
                        DrawTimeline();
                    }
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    GUILayout.Label(_status, EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("v0.1", EditorStyles.miniLabel);
                }
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode) EditorGUILayout.HelpBox("Workshop editing is available in Edit Mode.", MessageType.Info);
        }

        private static GUIContent Icon(string name, string tooltip)
        {
            GUIContent content = new GUIContent(EditorGUIUtility.IconContent(name));
            content.tooltip = tooltip;
            return content;
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var source = (WeaponLoadoutSO)EditorGUILayout.ObjectField(_document.Source, typeof(WeaponLoadoutSO), false, GUILayout.Width(270));
                if (source != _document.Source && ConfirmDiscard()) LoadLoadout(source, _character);
                if (GUILayout.Button(Icon("Toolbar Plus", "New loadout"), EditorStyles.toolbarButton, GUILayout.Width(30)) && ConfirmDiscard())
                {
                    var controller = _document.BaseController;
                    var idle = _document.IdleClip;
                    LoadLoadout(null, _character);
                    _document.BaseController = controller;
                    _document.IdleClip = idle;
                    _document.Loadout.ID = "new_weapon";
                    _document.Loadout.DisplayName = "New Weapon";
                    Dirty();
                }
                if (GUILayout.Button(Icon("SaveActive", "Save loadout and generated clips"), EditorStyles.toolbarButton, GUILayout.Width(30))) SaveChanges();
                if (GUILayout.Button(Icon("Refresh", "Discard draft and reload saved assets"), EditorStyles.toolbarButton, GUILayout.Width(30)) && ConfirmDiscard()) LoadLoadout(_document.Source, _character);
                GUILayout.FlexibleSpace();
                GUILayout.Label(_document.Loadout.DisplayName ?? "Loadout", EditorStyles.boldLabel);
                GUILayout.Space(12);
            }
        }

        private bool ConfirmDiscard() => !hasUnsavedChanges || EditorUtility.DisplayDialog("Combat Workshop", "Discard unsaved changes?", "Discard", "Cancel");

        private void DrawLoadout()
        {
            EditorGUILayout.LabelField("Character", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var character = (GameObject)EditorGUILayout.ObjectField("Preview character", _character, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck()) { _character = character; FindSocketRegistry(); RebuildPreview(); }
            EditorGUI.BeginChangeCheck();
            var registry = (WeaponSocketRegistry)EditorGUILayout.ObjectField("Socket registry", _socketRegistry, typeof(WeaponSocketRegistry), true);
            if (EditorGUI.EndChangeCheck()) { _socketRegistry = registry; RebuildPreview(); }
            if (_character == null) EditorGUILayout.HelpBox("Select a character prefab or scene instance.", MessageType.Info);
            EditorGUILayout.Space();
            var fields = new SerializedObject(_document.Loadout);
            fields.Update();
            EditorGUILayout.PropertyField(fields.FindProperty("ID"));
            EditorGUILayout.PropertyField(fields.FindProperty("DisplayName"));
            fields.ApplyModifiedProperties();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
            var documentFields = new SerializedObject(_document);
            documentFields.Update();
            EditorGUILayout.PropertyField(documentFields.FindProperty("BaseController"));
            documentFields.ApplyModifiedProperties();
            string idle = StatePopup("Idle state", _document.IdleState);
            if (idle != _document.IdleState) { Undo.RecordObject(_document, "Change idle state"); _document.IdleState = idle; }
            var clip = (AnimationClip)EditorGUILayout.ObjectField("Idle clip", _document.IdleClip, typeof(AnimationClip), false);
            if (clip != _document.IdleClip) { Undo.RecordObject(_document, "Change idle clip"); _document.IdleClip = clip; }
            _previewIdle = EditorGUILayout.Toggle("Preview idle", _previewIdle);
            if (_document.Source != null)
            {
                EditorGUILayout.Space();
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField("Attack set", _document.Source.ComboAttackSet, typeof(ComboAttackSetSO), false);
                    EditorGUILayout.ObjectField("Controller", _document.Source.AnimatorController, typeof(RuntimeAnimatorController), false);
                }
            }
        }
        #endregion

        #region Weapons
        private void DrawWeapons()
        {
            var models = _document.Loadout.List_WeaponModels;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Weapon models", EditorStyles.boldLabel);
                if (GUILayout.Button(Icon("Toolbar Plus", "Add weapon model"), GUILayout.Width(28)))
                {
                    Undo.RecordObject(_document.Loadout, "Add weapon model");
                    models.Add(new WeaponModelSpawnData());
                    Dirty();
                    _modelIndex = models.Count - 1;
                    RebuildPreview();
                }
            }
            if (models.Count == 0) { EditorGUILayout.LabelField("Unarmed", EditorStyles.miniLabel); return; }
            _modelIndex = EditorGUILayout.Popup("Model", Mathf.Clamp(_modelIndex, 0, models.Count - 1), models.Select((x, i) => $"{i + 1}  {x.SocketID}").ToArray());
            var model = models[_modelIndex];
            var current = model.WeaponPrefabReference?.editorAsset as GameObject;
            var prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", current, typeof(GameObject), false);
            if (prefab != current)
            {
                Undo.RecordObject(_document.Loadout, "Change weapon model");
                model.WeaponPrefabReference = prefab == null ? null : new AssetReferenceGameObject(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab)));
                RebuildPreview();
            }
            string socket = EditorGUILayout.DelayedTextField("Socket ID", model.SocketID);
            string[] sockets = new[] { "Select socket..." }.Concat(_preview.SocketNames).ToArray();
            int choice = EditorGUILayout.Popup("Available sockets", 0, sockets);
            if (choice > 0) socket = sockets[choice];
            if (socket != model.SocketID) { Undo.RecordObject(_document.Loadout, "Change weapon socket"); model.SocketID = socket; RebuildPreview(); }
            var fields = new SerializedObject(_document.Loadout);
            fields.Update();
            var item = fields.FindProperty("List_WeaponModels").GetArrayElementAtIndex(_modelIndex);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(item.FindPropertyRelative("LocalPositionOffset"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("LocalEulerAngles"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("LocalScale"));
            if (EditorGUI.EndChangeCheck()) { fields.ApplyModifiedProperties(); RebuildPreview(); }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Register Addressable") && prefab != null)
                {
                    var settings = AddressableAssetSettingsDefaultObject.Settings;
                    if (settings == null || settings.DefaultGroup == null) _status = "Addressables settings/default group missing.";
                    else
                    {
                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                        if (settings.FindAssetEntry(guid, true) == null)
                        {
                            Undo.RecordObject(settings, "Register weapon Addressable");
                            Undo.RecordObject(settings.DefaultGroup, "Register weapon Addressable");
                            settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                            AssetDatabase.SaveAssetIfDirty(settings.DefaultGroup);
                            AssetDatabase.SaveAssetIfDirty(settings);
                        }
                        _status = "Weapon model registered.";
                    }
                }
                if (GUILayout.Button(Icon("Toolbar Minus", "Remove model from loadout"), GUILayout.Width(28)))
                {
                    Undo.RecordObject(_document.Loadout, "Remove weapon model");
                    models.RemoveAt(_modelIndex);
                    Dirty();
                    _modelIndex = Mathf.Max(0, _modelIndex - 1);
                    RebuildPreview();
                }
            }
            foreach (string warning in _preview.List_Warnings) EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }
        #endregion

        #region Attacks
        private void DrawAttacks()
        {
            int mode = GUILayout.Toolbar(_editPostDash ? 1 : 0, new[] { "Combo", "Post-dash" });
            if ((mode == 1) != _editPostDash)
            {
                _editPostDash = mode == 1;
                _time = 0;
                _previewIdle = false;
                _playing = false;
            }
            if (_editPostDash)
            {
                var loadoutFields = new SerializedObject(_document.Loadout);
                EditorGUILayout.PropertyField(loadoutFields.FindProperty("HasPostDashAttack"), new GUIContent("Enabled"));
                loadoutFields.ApplyModifiedProperties();
                if (!_document.Loadout.HasPostDashAttack) return;
                var states = CombatWorkshopDocument.GetStates(_document.BaseController);
                if (CombatWorkshopDocument.FindStateClip(states, _document.Loadout.PostDashAttack.AnimationStateName) == null)
                {
                    using (new EditorGUI.DisabledScope(_document.BaseController == null))
                        if (GUILayout.Button("Create post-dash Animator slot"))
                        {
                            _document.CreatePostDashSlot();
                            Dirty();
                        }
                }
            }
            else
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Combo", EditorStyles.boldLabel);
                    if (GUILayout.Button(Icon("Toolbar Plus", "Add attack"), GUILayout.Width(28)))
                    {
                        _document.AddAttack();
                        Dirty();
                        _attackIndex = _document.List_Attacks.Count - 1;
                        _previewIdle = false;
                        _time = 0;
                    }
                }
                if (_document.List_Attacks.Count > 0)
                {
                    int selected = EditorGUILayout.Popup("Attack", _attackIndex, _document.Combo.List_ComboAttackData.Select((x, i) => $"{i + 1}  {x.AnimationStateName}").ToArray());
                    if (selected != _attackIndex) { _attackIndex = selected; _time = 0; _previewIdle = false; }
                }
            }
            if (Attack == null) return;
            var fields = GetAttackFields();
            fields.Update();
            var attack = GetAttackProperty(fields);
            var state = attack.FindPropertyRelative("_animationStateName");
            state.stringValue = StatePopup("Animator state", state.stringValue);
            EditorGUILayout.PropertyField(attack.FindPropertyRelative("_transitionDuration"), new GUIContent("Blend (normalized)", "Existing runtime CrossFade duration, relative to the source state."));
            var clip = (AnimationClip)EditorGUILayout.ObjectField("Clip", Attack.Clip, typeof(AnimationClip), false);
            if (clip != Attack.Clip)
            {
                Undo.RecordObject(_document, "Replace attack clip");
                if (_editPostDash) CombatWorkshopDocument.SetPostDashClip(Attack, clip);
                else CombatWorkshopDocument.SetClip(Attack, clip);
                _previewIdle = false;
                _time = 0;
            }
            EditorGUILayout.Space();
            DrawMarkers();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hitbox", EditorStyles.boldLabel);
            _editHitbox = EditorGUILayout.Toggle("Edit in preview", _editHitbox);
            foreach (string property in new[] { "_hitboxCenterOffset", "_hitboxHalfExtents", "_targetLayerMask", "_queryTriggerInteraction", "_maxTargets" })
                EditorGUILayout.PropertyField(attack.FindPropertyRelative(property));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(attack.FindPropertyRelative("_damageData"), true);
            //EditorGUILayout.PropertyField(attack.FindPropertyRelative("_attackAdvance"), true);
            EditorGUILayout.PropertyField(attack.FindPropertyRelative("_attackKnockback"), true);
            fields.ApplyModifiedProperties();
            if (!_editPostDash && GUILayout.Button("Remove attack"))
            {
                _document.RemoveAttack(_attackIndex);
                Dirty();
                _attackIndex = Mathf.Max(0, _attackIndex - 1);
                _time = 0;
            }
        }

        private SerializedObject GetAttackFields() => new SerializedObject(_editPostDash
            ? (UnityEngine.Object)_document.Loadout : _document.Combo);

        private SerializedProperty GetAttackProperty(SerializedObject fields) => _editPostDash
            ? fields.FindProperty("PostDashAttack")
            : fields.FindProperty("_list_ComboAttackData").GetArrayElementAtIndex(_attackIndex);

        private string StatePopup(string label, string current)
        {
            var names = CombatWorkshopDocument.GetStates(_document.BaseController).Keys.ToList();
            int index = names.IndexOf(current);
            if (index < 0) index = names.FindIndex(x => x.EndsWith("." + current, StringComparison.Ordinal));
            if (index < 0) { names.Insert(0, current ?? ""); index = 0; }
            return names[EditorGUILayout.Popup(label, index, names.ToArray())];
        }

        private void DrawMarkers()
        {
            EditorGUILayout.LabelField("Animation events", EditorStyles.boldLabel);
            if (Attack.Clip == null) return;
            var markers = Attack.List_Markers;
            for (int i = 0; i < markers.Count; i++)
            {
                var marker = markers[i];
                using (new EditorGUILayout.HorizontalScope())
                {
                    var type = _editPostDash
                        ? (EditorGUILayout.Popup(marker.Type == CombatMarkerType.FINISH ? 1 : 0, new[] { "HIT", "FINISH" }, GUILayout.Width(113)) == 1
                            ? CombatMarkerType.FINISH : CombatMarkerType.HIT)
                        : (CombatMarkerType)EditorGUILayout.EnumPopup(marker.Type, GUILayout.Width(113));
                    float time = EditorGUILayout.FloatField(marker.Time);
                    if (type != marker.Type || time != marker.Time)
                    {
                        Undo.RecordObject(_document, "Edit attack marker");
                        marker.Type = type;
                        marker.Time = Mathf.Clamp(time, 0, Attack.Clip.length);
                    }
                    if (GUILayout.Button(new GUIContent("@", "Place marker at playhead"), GUILayout.Width(24)))
                    { Undo.RecordObject(_document, "Move marker to playhead"); marker.Time = _time; Dirty(); }
                    if (GUILayout.Button(Icon("Toolbar Minus", "Remove marker"), GUILayout.Width(24)))
                    { Undo.RecordObject(_document, "Remove marker"); markers.RemoveAt(i); Dirty(); break; }
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add marker"))
                { Undo.RecordObject(_document, "Add marker"); markers.Add(new CombatMarker { Type = CombatMarkerType.HIT, Time = _time }); Dirty(); }
                if (markers.Count == 0 && GUILayout.Button("Default markers"))
                {
                    Undo.RecordObject(_document, "Add default markers");
                    float length = Attack.Clip.length;
                    markers.Add(new CombatMarker { Type = CombatMarkerType.HIT, Time = length * 0.35f });
                    if (!_editPostDash) markers.Add(new CombatMarker { Type = CombatMarkerType.CLOSE_COMBO, Time = length * 0.75f });
                    markers.Add(new CombatMarker { Type = CombatMarkerType.FINISH, Time = length * 0.95f });
                    Dirty();
                }
            }
        }

        private void ChangeHitbox(Vector3 offset, Vector3 extents)
        {
            if (AttackData == null) return;
            var fields = GetAttackFields();
            var attack = GetAttackProperty(fields);
            attack.FindPropertyRelative("_hitboxCenterOffset").vector3Value = offset;
            attack.FindPropertyRelative("_hitboxHalfExtents").vector3Value = extents;
            fields.ApplyModifiedProperties();
            Dirty();
        }

        private void ChangeWeapon(Vector3 position)
        {
            Undo.RecordObject(_document.Loadout, "Move weapon");
            _document.Loadout.List_WeaponModels[_modelIndex].LocalPositionOffset = position;
            Dirty();
        }

        private void ChangeWeaponRotation(Vector3 rotation)
        {
            Undo.RecordObject(_document.Loadout, "Rotate weapon");
            _document.Loadout.List_WeaponModels[_modelIndex].LocalEulerAngles = rotation;
            Dirty();
        }
        #endregion

        #region Playback
        private void DrawViewToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button(Icon("SceneViewOrtho", "Reset side view"), EditorStyles.toolbarButton, GUILayout.Width(28))) _preview.Frame();
                _preview.Left = GUILayout.Toggle(_preview.Left, "Left", EditorStyles.toolbarButton, GUILayout.Width(45));
                //_preview.ShowAdvance = GUILayout.Toggle(_preview.ShowAdvance, "Advance", EditorStyles.toolbarButton, GUILayout.Width(65));
                _previewIdle = GUILayout.Toggle(_previewIdle, "Idle", EditorStyles.toolbarButton, GUILayout.Width(40));
                _preview.OriginalMaterials = GUILayout.Toggle(_preview.OriginalMaterials, "Materials", EditorStyles.toolbarButton, GUILayout.Width(67));
                if (_tab == 1)
                {
                    if (GUILayout.Toggle(!_rotateWeapon, Icon("MoveTool", "Move weapon"), EditorStyles.toolbarButton, GUILayout.Width(28))) _rotateWeapon = false;
                    if (GUILayout.Toggle(_rotateWeapon, Icon("RotateTool", "Rotate weapon"), EditorStyles.toolbarButton, GUILayout.Width(28))) _rotateWeapon = true;
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label(CurrentClip != null ? CurrentClip.name : "No clip", EditorStyles.miniLabel);
            }
        }

        private void DrawTimeline()
        {
            AnimationClip clip = CurrentClip;
            float length = clip != null ? Mathf.Max(clip.length, 0.001f) : 1;
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button(Icon(_playing ? "PauseButton" : "PlayButton", "Play / pause preview"), EditorStyles.toolbarButton, GUILayout.Width(30))) _playing = !_playing;
                if (GUILayout.Button(Icon("Animation.FirstKey", "Previous frame"), EditorStyles.toolbarButton, GUILayout.Width(28))) { _playing = false; _time = Mathf.Max(0, _time - 1 / (clip != null ? clip.frameRate : 60)); }
                if (GUILayout.Button(Icon("Animation.LastKey", "Next frame"), EditorStyles.toolbarButton, GUILayout.Width(28))) { _playing = false; _time = Mathf.Min(length, _time + 1 / (clip != null ? clip.frameRate : 60)); }
                _loop = GUILayout.Toggle(_loop, "Loop", EditorStyles.toolbarButton, GUILayout.Width(43));
                _playCombo = GUILayout.Toggle(_playCombo, "Combo", EditorStyles.toolbarButton, GUILayout.Width(55));
                GUILayout.FlexibleSpace();
                _playbackSpeed = Mathf.Clamp(EditorGUILayout.FloatField(_playbackSpeed, GUILayout.Width(45)), 1, 400);
                GUILayout.Label("%", GUILayout.Width(15));
                GUILayout.Label($"{_time:0.00} / {length:0.00}s", GUILayout.Width(100));
            }
            Rect timeline = GUILayoutUtility.GetRect(100, 68, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(timeline, new Color(0.1f, 0.11f, 0.12f));
            Handles.BeginGUI();
            for (int i = 0; i <= 10; i++)
            {
                float x = timeline.x + i * timeline.width / 10;
                Handles.color = new Color(0.35f, 0.35f, 0.35f);
                Handles.DrawLine(new Vector3(x, timeline.y + 18), new Vector3(x, timeline.yMax));
                GUI.Label(new Rect(x + 2, timeline.y, 40, 18), (length * i / 10).ToString("0.0"), EditorStyles.miniLabel);
            }
            var markers = !_previewIdle ? Attack?.List_Markers : null;
            if (markers != null)
            {
                for (int i = 0; i < markers.Count; i++)
                {
                    float x = timeline.x + markers[i].Time / length * timeline.width;
                    var color = MarkerColor(markers[i].Type);
                    EditorGUI.DrawRect(new Rect(x - 2, timeline.y + 20, 4, 46), color);
                    GUI.Label(new Rect(Mathf.Min(x + 3, timeline.xMax - 75), timeline.y + 22 + (i % 2) * 18, 75, 18), markers[i].Type.ToString(), EditorStyles.miniLabel);
                }
            }
            float playhead = timeline.x + _time / length * timeline.width;
            EditorGUI.DrawRect(new Rect(playhead - 1, timeline.y, 2, timeline.height), Color.white);
            Handles.EndGUI();
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && timeline.Contains(e.mousePosition))
            {
                _playing = false;
                _dragMarker = -1;
                if (markers != null && e.mousePosition.y >= timeline.y + 20)
                    for (int i = 0; i < markers.Count; i++) if (Mathf.Abs(timeline.x + markers[i].Time / length * timeline.width - e.mousePosition.x) < 7) { _dragMarker = i; Undo.RecordObject(_document, "Drag attack marker"); break; }
                _time = Mathf.Clamp01((e.mousePosition.x - timeline.x) / timeline.width) * length;
                e.Use();
            }
            if (e.type == EventType.MouseDrag && e.button == 0 && (_dragMarker >= 0 || timeline.Contains(e.mousePosition)))
            {
                _time = Mathf.Clamp01((e.mousePosition.x - timeline.x) / timeline.width) * length;
                if (_dragMarker >= 0 && markers != null) { markers[_dragMarker].Time = _time; Dirty(); }
                e.Use();
            }
            if (e.type == EventType.MouseUp) _dragMarker = -1;
            EditorGUI.BeginChangeCheck();
            _time = EditorGUILayout.Slider(_time, 0, length);
            if (EditorGUI.EndChangeCheck()) _playing = false;
        }

        private static Color MarkerColor(CombatMarkerType type)
        {
            switch (type)
            {
                case CombatMarkerType.HIT: return new Color(1, 0.4f, 0.15f);
                case CombatMarkerType.CLOSE_COMBO: return new Color(0.35f, 0.65f, 1);
                default: return Color.white;
            }
        }

        private void Tick()
        {
            double now = EditorApplication.timeSinceStartup;
            float delta = Mathf.Min(0.1f, (float)(now - _lastUpdate));
            _lastUpdate = now;
            if (!_playing || CurrentClip == null || EditorApplication.isPlayingOrWillChangePlaymode) return;
            _time += delta * _playbackSpeed / 100;
            float end = CurrentClip.length;
            if (_playCombo && !_previewIdle && Attack != null)
                end = Attack.List_Markers.Where(x => x.Type == CombatMarkerType.FINISH).Select(x => x.Time).DefaultIfEmpty(end).Min();
            if (_time >= end)
            {
                if (_playCombo && !_editPostDash && !_previewIdle && _attackIndex + 1 < _document.List_Attacks.Count) { _attackIndex++; _time = 0; }
                else if (_loop) { _time = 0; if (_playCombo && !_editPostDash) _attackIndex = 0; }
                else { _time = end; _playing = false; }
            }
            Repaint();
        }
        #endregion

        #region Save And Checks
        private void DrawValidation()
        {
            if (GUILayout.Button("Validate loadout")) _list_Errors = _document.Validate();
            foreach (string error in _list_Errors) EditorGUILayout.HelpBox(error, MessageType.Error);
            foreach (string warning in _preview.List_Warnings) EditorGUILayout.HelpBox(warning, MessageType.Warning);
            if (_list_Errors.Count == 0) EditorGUILayout.LabelField("No reported asset errors", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview target", EditorStyles.boldLabel);
            _preview.TargetPosition = EditorGUILayout.Vector3Field("Position", _preview.TargetPosition);
            _preview.TargetSize = EditorGUILayout.Vector3Field("Size", _preview.TargetSize);
            if (GUILayout.Button("Ping saved loadout") && _document.Source != null) EditorGUIUtility.PingObject(_document.Source);
        }

        public override void SaveChanges()
        {
            if (_document == null) return;
            _list_Errors = _document.Validate();
            if (_list_Errors.Count > 0) { _tab = 3; _status = "Resolve validation errors before saving."; return; }
            string path = null;
            if (_document.Source == null)
            {
                path = EditorUtility.SaveFilePanelInProject("Save weapon loadout", "WeaponLoadoutSO", "asset", "Choose a folder for the loadout.");
                if (string.IsNullOrEmpty(path)) return;
            }
            try
            {
                WeaponLoadoutSO saved = _document.Save(path);
                LoadLoadout(saved, _character);
                _status = "Saved: " + AssetDatabase.GetAssetPath(saved);
                base.SaveChanges();
            }
            catch (Exception exception)
            {
                _status = "Save failed: " + exception.Message;
                Debug.LogException(exception);
            }
        }

        public override void DiscardChanges()
        {
            if (_document != null) LoadLoadout(_document.Source, _character);
            base.DiscardChanges();
        }
        #endregion
    }
}
