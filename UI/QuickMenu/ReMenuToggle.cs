using System;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Elements.Controls;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuToggle : UiElement
    {
        private readonly Toggle _toggleComponent;

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

            Object.DestroyImmediate(GameObject.GetComponent<UIInvisibleGraphic>()); // Fix for having clickable area overlap main quickmenu ui
            
            _toggleComponent = GameObject.GetComponent<Toggle>();
            _toggleComponent.onValueChanged = new Toggle.ToggleEvent();
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
        }

        public ReMenuToggle(Transform transform) : base(transform)
        {
            _toggleComponent = GameObject.GetComponent<Toggle>();
        }

        public void Toggle(bool value, bool callback = true, bool updateVisually = false)
        {
            _valueHolder = value;
            _toggleComponent.Set(value, callback);
        }
    }
}
