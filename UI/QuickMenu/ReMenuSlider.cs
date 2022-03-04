using System;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements.Controls;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuSlider : UiElement
    {
        private readonly Slider _sliderComponent;

        private float _valueHolder;

        public ReMenuSlider(string text, string tooltip, Action<float> onSlide, Transform parent, float defaultValue = 0, float minValue = 0, float maxValue = 10) : base(QuickMenuEx.SliderPrefab, parent, $"Slider_{text}")
        {
            GameObject.DestroyImmediate(GameObject.GetComponent<UIInvisibleGraphic>()); // Fix for having clickable area overlap main quickmenu ui

            var name = GameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            name.text = text;

            var value = GameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            value.text = defaultValue.ToString();  //This shit don't work

            _sliderComponent = GameObject.GetComponentInChildren<Slider>();
            _sliderComponent.onValueChanged = new Slider.SliderEvent();
            _sliderComponent.onValueChanged.AddListener(new Action<float>(onSlide));
            _sliderComponent.onValueChanged.AddListener((Action<float>)delegate (float val) { value.text = val.ToString("F"); });
            _sliderComponent.minValue = minValue;
            _sliderComponent.maxValue = maxValue;
            _sliderComponent.value = defaultValue;

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;
            
            Slide(defaultValue,false);

            EnableDisableListener.RegisterSafe();
            var edl = GameObject.AddComponent<EnableDisableListener>();
        }

        public void Slide(float value, bool callback = true)
        {
            _valueHolder = value;
            _sliderComponent.Set(value, callback);
        }
    }
}
