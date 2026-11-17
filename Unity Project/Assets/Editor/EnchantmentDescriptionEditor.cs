using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Enchantment;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnchantmentSOBase), true)]
public sealed class EnchantmentDescriptionEditor : Editor
{
    private const string ControlName = "EnchantmentDescription";
    private readonly List<string> _suggestions = new List<string>();
    private int _start = -1, _cursor, _selected;
    private int _textControlId;
    private string _dismissed;
    private Vector2 _scroll;
    private PreviewRenderUtility _preview;
    private TextMeshPro _text;
    private TMP_FontAsset _font;
    private TMP_SpriteAsset _sprites;
    private float _descriptionHeight = 160f;
    private float _previewHeight = 280f;
    private float _previewFontSize = 20f;

    private void OnEnable()
    {
        _font = TMP_Settings.defaultFontAsset;
        _sprites = TMP_Settings.defaultSpriteAsset;
    }

    private void OnDisable()
    {
        _preview?.Cleanup();
        _preview = null;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var property = serializedObject.GetIterator();
        bool children = true;
        while (property.NextVisible(children))
        {
            children = false;
            if (property.name == "Description") DrawDescription(property);
            else
            {
                using (new EditorGUI.DisabledScope(property.name == "m_Script"))
                    EditorGUILayout.PropertyField(property, true);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDescription(SerializedProperty property)
    {
        EditorGUILayout.LabelField("Description");
        _descriptionHeight = EditorGUILayout.Slider("Description Height", _descriptionHeight, 70f, 600f);
        var e = Event.current;
        if (GUI.GetNameOfFocusedControl() == ControlName && _suggestions.Count > 0 && e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.UpArrow)
            {
                _selected = (_selected + (e.keyCode == KeyCode.DownArrow ? 1 : _suggestions.Count - 1)) % _suggestions.Count;
                e.Use();
            }
            else if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Tab)
            {
                Insert(property, _suggestions[_selected]);
                e.Use();
            }
            else if (e.keyCode == KeyCode.Escape)
            {
                _dismissed = property.stringValue;
                _suggestions.Clear();
                e.Use();
            }
        }
        var textRect = GUILayoutUtility.GetRect(new GUIContent(property.stringValue), EditorStyles.textArea,
            GUILayout.Height(_descriptionHeight), GUILayout.ExpandWidth(true));
        GUI.SetNextControlName(ControlName);
        // GUI.TextArea uses the per-control TextEditor; EditorGUI uses a shared recycled editor.
        property.stringValue = GUI.TextArea(textRect, property.stringValue, EditorStyles.textArea);
        if (GUI.GetNameOfFocusedControl() == ControlName)
        {
            _textControlId = GUIUtility.keyboardControl;
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), _textControlId);
            _cursor = Mathf.Clamp(editor.cursorIndex, 0, property.stringValue.Length);
            string prefix = property.stringValue.Substring(0, _cursor);
            _start = prefix.LastIndexOf('<');
            _suggestions.Clear();
            if (_start >= 0 && prefix.LastIndexOf('>') < _start && _dismissed != property.stringValue)
            {
                string query = prefix.Substring(_start);
                _suggestions.AddRange(GetTags().Where(t => t.StartsWith(query, StringComparison.OrdinalIgnoreCase)));
            }
            _selected = Mathf.Clamp(_selected, 0, Math.Max(0, _suggestions.Count - 1));
        }
        if (_suggestions.Count > 0)
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(Mathf.Min(130, _suggestions.Count * 24)));
            for (int i = 0; i < _suggestions.Count; i++)
            {
                string tag = _suggestions[i];
                var style = new GUIStyle(EditorStyles.miniButton) { alignment = TextAnchor.MiddleLeft };
                if (i == _selected) style.normal.textColor = Color.cyan;
                if (GUILayout.Button(tag, style)) { Insert(property, tag); break; }
            }
            EditorGUILayout.EndScrollView();
        }
        serializedObject.ApplyModifiedProperties();
        DrawPreview();
    }

    private void Insert(SerializedProperty property, string tag)
    {
        if (_start < 0 || _cursor < _start || _cursor > property.stringValue.Length) return;
        property.stringValue = property.stringValue.Substring(0, _start) + tag + property.stringValue.Substring(_cursor);
        GUI.FocusControl(ControlName);
        GUIUtility.keyboardControl = _textControlId;
        var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), _textControlId);
        editor.text = property.stringValue;
        editor.cursorIndex = editor.selectIndex = _start + tag.Length;
        _suggestions.Clear();
        _start = -1;
    }

    private IEnumerable<string> GetTags()
    {
        if (target is StatusUpgradeSO status)
            for (int i = 0; i < status.List_StatusModifiers.Count; i++) yield return $"<statusmodifier index={i}>";
        if (target is AbilityEnchantmentSO ability)
        {
            for (int i = 0; i < ability.List_DamageDatas.Count; i++) yield return $"<damage index={i}>";
            for (int i = 0; i < ability.List_BuffDatas.Count; i++)
            {
                yield return $"<buff index={i}>";
                yield return $"<buffduration index={i}>";
            }
        }
        if (target is WeaponEnchantmentSO postDashWeapon && postDashWeapon.WeaponLoadout != null)
        {
            var loadout = postDashWeapon.WeaponLoadout;
            if (loadout.HasPostDashAttack && loadout.PostDashAttack != null)
                for (int h = 0; h < loadout.PostDashAttack.Hits.Count; h++)
                    yield return $"<postdashdamage hit={h}>";
        }
        if (target is WeaponEnchantmentSO weapon && weapon.WeaponLoadout != null && weapon.WeaponLoadout.ComboAttackSet != null)
        {
            var attacks = weapon.WeaponLoadout.ComboAttackSet.List_ComboAttackData;
            for (int i = 0; i < attacks.Count; i++)
                if (attacks[i] != null)
                    for (int h = 0; h < attacks[i].Hits.Count; h++)
                    {
                        yield return $"<attackdamage index={i} hit={h}>";
                        yield return $"<attackSet index={i} hit={h}>";
                    }
        }
        if (_sprites != null)
            foreach (var sprite in _sprites.spriteCharacterTable)
                yield return $"<sprite name=\"{sprite.name}\">";
        foreach (string tag in new[] { "b", "i", "u", "s", "sub", "sup", "uppercase", "lowercase", "smallcaps", "nobr", "noparse" })
        {
            yield return "<" + tag + ">";
            yield return "</" + tag + ">";
        }
        foreach (string tag in new[] { "<color=#FFFFFF>", "</color>", "<size=120%>", "</size>", "<br>", "<alpha=#FF>", "<mark=#FFFF0080>", "</mark>", "<align=center>", "</align>", "<space=10>", "<cspace=1>", "</cspace>", "<indent=10%>", "</indent>", "<line-height=120%>", "</line-height>", "<voffset=1em>", "</voffset>", "<font=\"FontName\">", "</font>", "<link=\"id\">", "</link>", "<sprite index=0>" })
            yield return tag;
    }

    private void DrawPreview()
    {
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        _font = (TMP_FontAsset)EditorGUILayout.ObjectField("Preview Font", _font, typeof(TMP_FontAsset), false);
        _sprites = (TMP_SpriteAsset)EditorGUILayout.ObjectField("Preview Sprites", _sprites, typeof(TMP_SpriteAsset), false);
        _previewHeight = EditorGUILayout.Slider("Preview Height", _previewHeight, 100f, 800f);
        _previewFontSize = EditorGUILayout.Slider("Preview Font Size", _previewFontSize, 10f, 36f);
        if (_font == null) { EditorGUILayout.HelpBox("Select a TMP font for the preview.", MessageType.Info); return; }
        var rect = GUILayoutUtility.GetRect(100, _previewHeight, GUILayout.ExpandWidth(true));
        if (Event.current.type != EventType.Repaint) return;
        if (_preview == null)
        {
            _preview = new PreviewRenderUtility();
            var go = new GameObject("Description Preview", typeof(TextMeshPro));
            go.hideFlags = HideFlags.HideAndDontSave;
            _preview.AddSingleGO(go);
            _text = go.GetComponent<TextMeshPro>();
            _preview.camera.orthographic = true;
            _preview.camera.clearFlags = CameraClearFlags.SolidColor;
            _preview.camera.backgroundColor = new Color(0.12f, 0.12f, 0.12f);
        }
        _text.font = _font;
        _text.spriteAsset = _sprites;
        _text.fontSize = _previewFontSize;
        _text.color = Color.white;
        _text.richText = true;
        // World-space TMP uses one tenth of the UI text scale.
        _text.transform.localScale = Vector3.one * 10f;
        _text.rectTransform.sizeDelta = new Vector2(rect.width - 16, rect.height - 16) / 10f;
        _text.alignment = TextAlignmentOptions.TopLeft;
        _text.text = ((EnchantmentSOBase)target).GetResolvedDescription();
        _text.ForceMeshUpdate();
        _preview.camera.transform.position = new Vector3(0, 0, -100);
        _preview.camera.transform.rotation = Quaternion.identity;
        _preview.camera.orthographicSize = rect.height / 2;
        _preview.camera.nearClipPlane = 0.1f;
        _preview.camera.farClipPlane = 200;
        _preview.BeginPreview(rect, GUIStyle.none);
        _preview.camera.Render();
        GUI.DrawTexture(rect, _preview.EndPreview(), ScaleMode.StretchToFill);
    }
}
