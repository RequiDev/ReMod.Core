using System;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI
{
    [Flags]
    public enum WingSide
    {
        Left = (1 << 0),
        Right = (1 << 1),
        Both = Left | Right
    }

    public class ReWingMenu : UiElement
    {
        private static GameObject _wingMenuPrefab;

        private static GameObject WingMenuPrefab
        {
            get
            {
                if (_wingMenuPrefab == null)
                {
                    _wingMenuPrefab = QuickMenuEx.LeftWing.field_Public_RectTransform_0.Find("WingMenu").gameObject;
                }
                return _wingMenuPrefab;
            }
        }

        private readonly Wing _wing;
        private readonly string _menuName;
        private readonly Transform _container;
        
        public ReWingMenu(string text, bool left = true) : base(WingMenuPrefab, (left ? QuickMenuEx.LeftWing : QuickMenuEx.RightWing).field_Public_RectTransform_0, text, false)
        {
            _menuName = GetCleanName(text);
            _wing = left ? QuickMenuEx.LeftWing : QuickMenuEx.RightWing;

            var headerTransform = RectTransform.GetChild(0);
            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = text;
            titleText.richText = true;

            var backButton = headerTransform.GetComponentInChildren<Button>(true);
            backButton.gameObject.SetActive(true);

            var backIcon = backButton.transform.Find("Icon");
            backIcon.gameObject.SetActive(true);
            var components = new Il2CppSystem.Collections.Generic.List<Behaviour>();
            backButton.GetComponents(components);

            foreach (var comp in components)
            {
                comp.enabled = true;
            }

            var content = RectTransform.GetComponentInChildren<ScrollRect>().content;
            foreach (var obj in content)
            {
                var control = obj.Cast<Transform>();
                if (control == null)
                {
                    continue;
                }

                Object.Destroy(control.gameObject);
            }

            _container = content;

            var uiPage = GameObject.GetComponent<UIPage>();
            uiPage.field_Public_String_0 = _menuName;
            uiPage.field_Private_Boolean_1 = true;
            uiPage.field_Private_MenuStateController_0 = _wing.field_Private_MenuStateController_0;
            uiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            uiPage.field_Private_List_1_UIPage_0.Add(uiPage);

            _wing.field_Private_MenuStateController_0.field_Private_Dictionary_2_String_UIPage_0.Add(uiPage.field_Public_String_0, uiPage);
        }

        public void Open()
        {
            _wing.field_Private_MenuStateController_0.Method_Public_Void_String_UIContext_Boolean_0(_menuName);
        }

        public ReWingButton AddButton(string text, string tooltip, Action onClick, bool arrow = true, bool background = true, bool separator = false)
        {
            return new ReWingButton(text, tooltip, onClick, _container, arrow, background, separator);
        }

        public ReWingMenu AddSubMenu(string text, string tooltip)
        {
            var menu = new ReWingMenu(text, _wing.field_Public_WingPanel_0 == Wing.WingPanel.Left);
            AddButton(text, tooltip, menu.Open);
            return menu;
        }
    }

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

        public ReWingButton(string text, string tooltip, Action onClick, Sprite sprite = null, bool left = true, bool arrow = true, bool background = true,
            bool separator = false) : base(WingButtonPrefab, (left ? QuickMenuEx.LeftWing : QuickMenuEx.RightWing).field_Public_RectTransform_0.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"), $"Button_{text}")
        {
            var container = RectTransform.Find("Container").transform;
            container.Find("Background").gameObject.SetActive(background);
            container.Find("Icon_Arrow").gameObject.SetActive(arrow);
            RectTransform.Find("Separator").gameObject.SetActive(separator);
            var iconImage = container.Find("Icon").GetComponent<Image>();
            if (sprite != null)
            {
                iconImage.sprite = sprite;
                iconImage.overrideSprite = sprite;
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }

            var tmp = container.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.richText = true;

            var button = GameObject.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new Action(onClick));

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;
        }

        public ReWingButton(string text, string tooltip, Action onClick, Transform parent, bool arrow = true, bool background = true,
            bool separator = false) : base(WingButtonPrefab, parent, $"Button_{text}")
        {
            var container = RectTransform.Find("Container").transform;
            container.Find("Background").gameObject.SetActive(background);
            container.Find("Icon_Arrow").gameObject.SetActive(arrow);
            RectTransform.Find("Separator").gameObject.SetActive(separator);
            container.Find("Icon").gameObject.SetActive(false);

            var tmp = container.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.richText = true;

            var button = GameObject.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new Action(onClick));

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;
        }

        public static void Create(string text, string tooltip, Action onClick, Sprite sprite = null,
            WingSide wingSide = WingSide.Both, bool arrow = true, bool background = true, bool separator = true)
        {
            if ((wingSide & WingSide.Left)==WingSide.Left)
            {
                new ReWingButton(text, tooltip, onClick, sprite, true, arrow, background, separator);
            }

            if ((wingSide & WingSide.Right)==WingSide.Right)
            {
                new ReWingButton(text, tooltip, onClick, sprite, false, arrow, background, separator);
            }
        }
    }
}
