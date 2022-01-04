using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.Wings
{
    public class ReMirroredWingButton
    {
        private readonly ReWingButton _leftButton;
        private readonly ReWingButton _rightButton;

        public ReMirroredWingButton(string text, string tooltip, Action onClick, Transform leftParent, Transform rightParent, Sprite sprite = null, bool arrow = true, bool background = true,
            bool separator = false)
        {
            _leftButton = new ReWingButton(text, tooltip, onClick, leftParent, sprite, arrow, background, separator);
            _rightButton = new ReWingButton(text, tooltip, onClick, rightParent, sprite, arrow, background, separator);
        }

        public void Destroy()
        {
            Object.DestroyImmediate(_leftButton.GameObject);
            Object.DestroyImmediate(_rightButton.GameObject);
        }
    }
}
