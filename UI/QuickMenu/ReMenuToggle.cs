using System;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements.Controls;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuToggle : UiElement
    {
        private readonly Toggle _toggleComponent;
        private readonly ToggleIcon _toggleIcon;

        public bool Interactable
        {
            get => _toggleComponent.interactable;
            set => _toggleComponent.interactable = value;
        }

        private bool _valueHolder;

        public ReMenuToggle(string text, string tooltip, Action<bool> onToggle, Transform parent, bool defaultValue = false) : base(QuickMenuEx.TogglePrefab, parent, $"Button_Toggle{text}")
        {
            var iconOn = RectTransform.Find("Icon_On").GetComponent<Image>();
            iconOn.sprite = QuickMenuEx.OnIconSprite;

            GameObject.DestroyImmediate(GameObject.GetComponent<UIInvisibleGraphic>()); // Fix for having clickable area overlap main quickmenu ui
            
            _toggleIcon = GameObject.GetComponent<ToggleIcon>();

            _toggleComponent = GameObject.GetComponent<Toggle>();
            _toggleComponent.onValueChanged = new Toggle.ToggleEvent();
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(_toggleIcon.OnValueChanged));
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(onToggle));

            var tmp = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.richText = true;
            tmp.color = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            tmp.m_fontColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            tmp.m_htmlColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiToggleTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;
            
            Toggle(defaultValue,false);

            EnableDisableListener.RegisterSafe();
            var edl = GameObject.AddComponent<EnableDisableListener>();
            edl.OnEnableEvent += UpdateToggleIfNeeded;
        }

        public ReMenuToggle(Transform transform) : base(transform)
        {
            _toggleIcon = GameObject.GetComponent<ToggleIcon>();
            _toggleComponent = GameObject.GetComponent<Toggle>();
        }

        public void Toggle(bool value, bool callback = true, bool updateVisually = false)
        {
            _valueHolder = value;
            _toggleComponent.Set(value, callback);
            if (updateVisually)
            {
                UpdateToggleIfNeeded();
            }
        }

        private void UpdateToggleIfNeeded()
        {
            _toggleIcon.OnValueChanged(_valueHolder);
        }
    }
}
