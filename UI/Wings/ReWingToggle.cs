using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReMod.Core.Managers;
using ReMod.Core.VRChat;
using UnityEngine;

namespace ReMod.Core.UI.Wings
{
    public class ReWingToggle
    {
        private readonly ReWingButton _button;
        private readonly Action<bool> _onToggle;

        private bool _state;
        
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

    public class ReMirroredWingToggle
    {
        private readonly ReWingToggle _leftToggle;
        private readonly ReWingToggle _rightToggle;

        public ReMirroredWingToggle(string text, string tooltip, Action<bool> onToggle, Transform leftParent,
            Transform rightParent, bool defaultValue = false)
        {
            _leftToggle = new ReWingToggle(text, tooltip, b =>
            {
                _rightToggle?.Toggle(b, false);
                onToggle(b);
            }, leftParent, defaultValue);
            _rightToggle = new ReWingToggle(text, tooltip, b =>
            {
                _leftToggle.Toggle(b, false);
                onToggle(b);
            }, rightParent, defaultValue);
        }

        public void Toggle(bool b, bool callback = true)
        {
            _leftToggle.Toggle(b, callback);
            _rightToggle.Toggle(b, callback);
        }
    }
}
