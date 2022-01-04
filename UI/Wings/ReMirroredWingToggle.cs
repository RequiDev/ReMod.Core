using System;
using UnityEngine;

namespace ReMod.Core.UI.Wings
{
    public class ReMirroredWingToggle
    {
        private readonly ReWingToggle _leftToggle;
        private readonly ReWingToggle _rightToggle;
        
        public bool Interactable
        {
            get => _leftToggle.Interactable;
            set
            {
                _leftToggle.Interactable = value;
                _rightToggle.Interactable = value;
            }
        }

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