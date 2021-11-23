using System;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements.Controls;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReTabButton : UiElement
    {
        private static GameObject _tabButtonPrefab;
        private static GameObject TabButtonPrefab
        {
            get
            {
                if (_tabButtonPrefab == null)
                {
                    _tabButtonPrefab = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_Settings").gameObject;
                }
                return _tabButtonPrefab;
            }
        }

        protected ReTabButton(string name, string tooltip, string pageName, Sprite sprite) : base(TabButtonPrefab, TabButtonPrefab.transform.parent, $"Page_{name}")
        {
            var menuTab = RectTransform.GetComponent<MenuTab>();
            menuTab.field_Public_String_0 = GetCleanName($"QuickMenuReMod{pageName}");
            menuTab.field_Private_MenuStateController_0 = QuickMenuEx.MenuStateCtrl;

            var button = GameObject.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new Action(menuTab.ShowTabContent));

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;

            var iconImage = RectTransform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = sprite;
            iconImage.overrideSprite = sprite;
        }

        public static ReTabButton Create(string name, string tooltip, string pageName, Sprite sprite)
        {
            return new ReTabButton(name, tooltip, pageName, sprite);
        }
    }
}
