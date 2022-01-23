using System;
using System.Collections.Generic;
using System.Linq;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuSliderContainer : UiElement
    {
        private static GameObject _ContainerPrefab;
        internal static GameObject ContainerPrefab
        {                       
            get
            {
                if (_ContainerPrefab == null)
                {
                    _ContainerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_AudioSettings/Content").gameObject;
                }
                return _ContainerPrefab;
            }
        }

        public ReMenuSliderContainer(string name, Transform parent = null) : base(ContainerPrefab, parent == null ? ContainerPrefab.transform.parent : parent, $"Sliders_{name}")
        {
            foreach (var obj in RectTransform)
            {
                var control = obj.Cast<Transform>();
                if (control == null)
                {
                    continue;
                }
                Object.Destroy(control.gameObject);
            }

            var vlg = GameObject.GetComponent<VerticalLayoutGroup>();
            vlg.m_Padding = new RectOffset(64, 64, 0, 0);
        }

        public ReMenuSliderContainer(Transform transform) : base(transform)
        {
        }
    }

    public class ReMenuSliderCategory
    {
        public readonly ReMenuHeader Header;
        private readonly ReMenuSliderContainer _sliderContainer;

        public string Title
        {
            get => Header.Title;
            set => Header.Title = value;
        }

        public ReMenuSliderCategory(string title, Transform parent = null, bool collapsible = true)
        {
            if (collapsible)
            {
                var header = new ReMenuHeaderCollapsible(title, parent);
                header.OnToggle += b => _sliderContainer!.GameObject.SetActive(b);
                Header = header;
            }

            else
            {
                var header = new ReMenuHeader(title, parent);
                Header = header;
            }
            _sliderContainer = new ReMenuSliderContainer(title, parent);

        }

        public ReMenuSliderCategory(ReMenuHeader headerElement, ReMenuSliderContainer container)
        {
            Header = headerElement;
            _sliderContainer = container;
        }

        public ReMenuSlider AddSlider(string text, string tooltip, Action<float> onSlide, float defaultValue = 0, float minValue = 0, float maxValue = 10)
        {
            var slider = new ReMenuSlider(text, tooltip, onSlide, _sliderContainer.RectTransform, defaultValue, minValue, maxValue);
            return slider;
        }

        public ReMenuSlider AddSlider(string text, string tooltip, ConfigValue<float> configValue, float defaultValue = 0, float minValue = 0, float maxValue = 10)
        {
            var slider = new ReMenuSlider(text, tooltip, configValue.SetValue, _sliderContainer.RectTransform, configValue, minValue, maxValue);
            return slider;
        }

    }
}
