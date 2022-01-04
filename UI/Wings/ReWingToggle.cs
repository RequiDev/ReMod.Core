using System;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;

namespace ReMod.Core.UI.Wings
{
    public class ReWingToggle
    {
        private readonly ReWingButton _button;
        private readonly Action<bool> _onToggle;

        private bool _state;
        
        public bool Interactable
        {
            get => _button.Interactable;
            set => _button.Interactable = value;
        }
        
        public ReWingToggle(string text, string tooltip, Action<bool> onToggle, Transform parent, bool defaultValue = false)
        {
            _onToggle = onToggle;
            _button = new ReWingButton(text, tooltip, () =>
            {
                Toggle(!_state);
            }, parent, GetCurrentIcon(), false);
            Toggle(defaultValue);
        }

        private Sprite GetCurrentIcon()
        {
            return _state ? QuickMenuEx.OnIconSprite : QuickMenuEx.OffIconSprite;
        }

        public void Toggle(bool b, bool callback = true)
        {
            if (_state == b) return;

            _state = b;
            _button.Sprite = GetCurrentIcon();
            if (callback)
            {
                _onToggle(_state);
            }
        }
    }
}
