using System;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;

namespace ReMod.Core.UI.Wings
{
    public class ReWingButton : UiElement
    {
        private static GameObject _wingButtonPrefab;
        private static GameObject WingButtonPrefab
        {
            get
            {
                if (_wingButtonPrefab == null)
                {
                    _wingButtonPrefab = QuickMenuEx.LeftWing.transform.Find("Container/InnerContainer/WingMenu/ScrollRect/Viewport/VerticalLayoutGroup/Button_Profile").gameObject;
                }
                return _wingButtonPrefab;
            }
        }

        private readonly Image _iconImage;
        public Sprite Sprite
        {
            get => _iconImage.sprite;
            set
            {
                if (value != null)
                {
                    _iconImage.sprite = value;
                    _iconImage.overrideSprite = value;
                }
                _iconImage.gameObject.SetActive(value != null);
            }
        }

        private readonly StyleElement _styleElement;
        private readonly Button _button;
        public bool Interactable
        {
            get => _button.interactable;
            set
            {
                _button.interactable = value;
                _styleElement.Method_Private_Void_Boolean_0(value);
            }
        }

        
        public ReWingButton(string text, string tooltip, Action onClick, Sprite sprite = null, bool left = true,
            bool arrow = true, bool background = true, bool separator = false) :
            this(text, tooltip, onClick,
                (left ? QuickMenuEx.LeftWing : QuickMenuEx.RightWing).field_Public_RectTransform_0.Find(
                    "WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"), sprite, arrow, background, separator)
        {
        }

        public ReWingButton(string text, string tooltip, Action onClick, Transform parent, Sprite sprite = null, bool arrow = true, bool background = true,
            bool separator = false) : base(WingButtonPrefab, parent, $"Button_{text}")
        {
            var container = RectTransform.Find("Container").transform;
            container.Find("Background").gameObject.SetActive(background);
            container.Find("Icon_Arrow").gameObject.SetActive(arrow);
            RectTransform.Find("Separator").gameObject.SetActive(separator);

            _iconImage = container.Find("Icon").GetComponent<Image>();
            Sprite = sprite;

            var tmp = container.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.richText = true;

            _styleElement = GameObject.GetComponent<StyleElement>();

            _button = GameObject.GetComponent<Button>();
            _button.onClick = new Button.ButtonClickedEvent();
            _button.onClick.AddListener(new Action(onClick));

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;

            if (sprite == null && !arrow)
            {
                container.gameObject.AddComponent<HorizontalLayoutGroup>();
                tmp.enableAutoSizing = true;
            }
        }

        public static void Create(string text, string tooltip, Action onClick, Sprite sprite = null,
            WingSide wingSide = WingSide.Both, bool arrow = true, bool background = true, bool separator = true)
        {
            if ((wingSide & WingSide.Left) == WingSide.Left)
            {
                new ReWingButton(text, tooltip, onClick, sprite, true, arrow, background, separator);
            }

            if ((wingSide & WingSide.Right) == WingSide.Right)
            {
                new ReWingButton(text, tooltip, onClick, sprite, false, arrow, background, separator);
            }
        }
        public static void Create(string text, string tooltip, Action onClick, Transform parent, Sprite sprite = null, bool arrow = true, bool background = true, bool separator = true)
        {
            new ReWingButton(text, tooltip, onClick, parent, sprite, arrow, background, separator);
        }
    }
}