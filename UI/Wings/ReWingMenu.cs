using System;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.Wings
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

        public Transform Container { get; }

        private readonly Wing _wing;
        private readonly string _menuName;

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

            Container = content;

            var menuStateCtrl = _wing.GetComponent<MenuStateController>();

            var uiPage = GameObject.GetComponent<UIPage>();
            uiPage.field_Public_String_0 = _menuName;
            uiPage.field_Private_Boolean_1 = true;
            uiPage.field_Protected_MenuStateController_0 = menuStateCtrl;
            uiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            uiPage.field_Private_List_1_UIPage_0.Add(uiPage);

            menuStateCtrl.field_Private_Dictionary_2_String_UIPage_0.Add(uiPage.field_Public_String_0, uiPage);
        }

        public void Open()
        {
            _wing.field_Private_MenuStateController_0.PushPage(_menuName);
        }

        public ReWingButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null, bool arrow = true, bool background = true, bool separator = false)
        {
            return new ReWingButton(text, tooltip, onClick, Container, sprite, arrow, background, separator);
        }

        public ReWingToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false)
        {
            return new ReWingToggle(text, tooltip, onToggle, Container, defaultValue);
        }

        public ReWingMenu AddSubMenu(string text, string tooltip)
        {
            var menu = new ReWingMenu(text, _wing.field_Public_WingPanel_0 == Wing.WingPanel.Left);
            AddButton(text, tooltip, menu.Open);
            return menu;
        }
    }
}
