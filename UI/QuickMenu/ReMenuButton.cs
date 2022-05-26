using System;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuButton : UiElement
    {
        private static GameObject _buttonPrefab;

        private static GameObject ButtonPrefab
        {
            get
            {
                if (_buttonPrefab == null)
                {
                    _buttonPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_QuickActions/Button_Respawn").gameObject;
                }
                return _buttonPrefab;
            }
        }

        private readonly TextMeshProUGUI _text;

        public string Text
        {
            get => _text.text;
            set => _text.SetText(value);
        }

        private readonly StyleElement _styleElement;
        private readonly Button _button;
        public bool Interactable
        {
            get => _button.interactable;
            set
            {
                _button.interactable = value;
                _styleElement.Method_Private_Void_Boolean_Boolean_0(value);
            }
        }

        public Image Background { get; }

        public ReMenuButton(string text, string tooltip, Action onClick, Transform parent, Sprite sprite = null, bool resizeTextNoSprite = true) : base(ButtonPrefab, parent,
            $"Button_{text}")
        {
            _text = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _text.text = text;
            _text.richText = true;

            Background = RectTransform.Find("Background").GetComponent<Image>();

            if (sprite == null)
            {
                if (resizeTextNoSprite)
                {
                    _text.fontSize = 35;
                    _text.enableAutoSizing = true;
                    _text.color = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
                    _text.m_fontColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
                    _text.m_htmlColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
                    _text.transform.localPosition = new Vector3(_text.transform.localPosition.x, -30f);

                    var layoutElement = Background.gameObject.AddComponent<LayoutElement>();
                    layoutElement.ignoreLayout = true;

                    var horizontalLayout = GameObject.AddComponent<HorizontalLayoutGroup>();
                    horizontalLayout.padding.right = 10;
                    horizontalLayout.padding.left = 10;
                    var styleElement = _text.GetComponent<StyleElement>();
                    styleElement.field_Public_String_1 = "H1";
                    Object.DestroyImmediate(RectTransform.Find("Icon").gameObject);
                }
                else
                {
                    var iconImage = RectTransform.Find("Icon").GetComponent<Image>();
                    iconImage.sprite = null;
                    iconImage.overrideSprite = null;
                }
            }
            else
            {
                var iconImage = RectTransform.Find("Icon").GetComponent<Image>();
                iconImage.sprite = sprite;
                iconImage.overrideSprite = sprite;
            }

            _styleElement = GameObject.GetComponent<StyleElement>();

            Object.DestroyImmediate(RectTransform.Find("Icon_Secondary").gameObject);
            Object.DestroyImmediate(RectTransform.Find("Badge_Close").gameObject);
            Object.DestroyImmediate(RectTransform.Find("Badge_MMJump").gameObject);

            var uiTooltips = GameObject.GetComponents<VRC.UI.Elements.Tooltips.UiTooltip>();
            VRC.UI.Elements.Tooltips.UiTooltip uiTooltip = null;
            if (uiTooltips.Length > 0)
            {
                //Fuck tooltips, all my friends hate tooltips
                uiTooltip = uiTooltips[0];
                
                for(int i=1; i<uiTooltips.Length; i++)
                    Object.DestroyImmediate(uiTooltips[i]);
            }

            if (uiTooltip != null)
            {
                uiTooltip.field_Public_String_0 = tooltip;
                uiTooltip.field_Public_String_1 = tooltip;
            }

            if (onClick != null)
            {
                _button = GameObject.GetComponent<Button>();
                _button.onClick = new Button.ButtonClickedEvent();
                _button.onClick.AddListener(new Action(onClick));
            }
        }

        public ReMenuButton(Transform transform) : base(transform)
        {
            _text = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _styleElement = GameObject.GetComponent<StyleElement>();
            _button = GameObject.GetComponent<Button>();
            Background = RectTransform.Find("Background").GetComponent<Image>();
        }

        public static ReMenuButton Create(string text, string tooltip, Action onClick, Transform parent, Sprite sprite = null)
        {
            return new ReMenuButton(text, tooltip, onClick, parent, sprite);
        }
    }
}
