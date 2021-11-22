using System;
using ReMod.Core.VRChat;
using UnityEngine;

namespace ReMod.Core.UI.Wings
{
    public class ReMirroredWingMenu
    {
        private readonly ReWingMenu _leftMenu;
        private readonly ReWingMenu _rightMenu;

        public bool Active
        {
            get => _leftMenu.Active && _rightMenu.Active;
            set
            {
                _leftMenu.Active = value;
                _rightMenu.Active = value;
            }
        }

        public ReMirroredWingMenu(string text, string tooltip, Transform leftParent, Transform rightParent, Sprite sprite = null, bool arrow = true, bool background = true, bool separator = false)
        {
            _leftMenu = new ReWingMenu(text);
            _rightMenu = new ReWingMenu(text, false);

            ReWingButton.Create(text, tooltip, _leftMenu.Open, leftParent, sprite, arrow, background, separator);
            ReWingButton.Create(text, tooltip, _rightMenu.Open, rightParent, sprite, arrow, background, separator);
        }

        public static ReMirroredWingMenu Create(string text, string tooltip, Sprite sprite = null, bool arrow = true,
            bool background = true, bool separator = false)
        {
            return new ReMirroredWingMenu(text, tooltip,
                QuickMenuEx.LeftWing.field_Public_RectTransform_0.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"),
                QuickMenuEx.RightWing.field_Public_RectTransform_0.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"),
                sprite, arrow, background, separator);
        }

        public ReMirroredWingButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null, bool arrow = true, bool background = true,
            bool separator = false)
        {
            return new ReMirroredWingButton(text, tooltip, onClick, _leftMenu.Container, _rightMenu.Container, sprite, arrow, background, separator);
        }

        public ReMirroredWingToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue)
        {
            return new ReMirroredWingToggle(text, tooltip, onToggle, _leftMenu.Container, _rightMenu.Container,
                defaultValue);
        }

        public ReMirroredWingMenu AddSubMenu(string text, string tooltip, Sprite sprite = null, bool arrow = true,
            bool background = true, bool separator = false)
        {
            return new ReMirroredWingMenu(text, tooltip, _leftMenu.Container, _rightMenu.Container, sprite, arrow,
                background, separator);
        }
    }
}