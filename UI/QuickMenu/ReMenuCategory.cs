using System;
using System.Collections.Generic;
using System.Linq;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuHeader : UiElement
    {
        private static GameObject _headerPrefab;
        private static GameObject HeaderPrefab
        {
            get
            {
                if (_headerPrefab == null)
                {
                    _headerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Header_QuickActions").gameObject;
                }
                return _headerPrefab;
            }
        }

        private readonly TextMeshProUGUI _text;
        public string Title
        {
            get => _text.text;
            set => _text.text = value;
        }

        public ReMenuHeader(string title, Transform parent) : base(HeaderPrefab, (parent == null ? HeaderPrefab.transform.parent : parent), $"Header_{title}")
        {
            _text = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _text.text = title;
            _text.richText = true;

            _text.transform.parent.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        }

        public ReMenuHeader(Transform transform) : base(transform)
        {
            _text = GameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public class ReMenuButtonContainer : UiElement
    {
        private static GameObject _containerPrefab;
        private static GameObject ContainerPrefab
        {
            get
            {
                if (_containerPrefab == null)
                {
                    _containerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_QuickActions").gameObject;
                }
                return _containerPrefab;
            }
        }

        public ReMenuButtonContainer(string name, Transform parent = null) : base(ContainerPrefab, (parent == null ? ContainerPrefab.transform.parent : parent), $"Buttons_{name}")
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

            var gridLayout = GameObject.GetComponent<GridLayoutGroup>();

            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.padding.top = 8;
            gridLayout.padding.left = 64;
        }

        public ReMenuButtonContainer(Transform transform) : base(transform)
        {
        }
    }

    public class ReMenuCategory : IButtonPage
    {
        public ReMenuHeader Header;
        private readonly ReMenuButtonContainer _buttonContainer;

        public string Title
        {
            get => Header.Title;
            set => Header.Title = value;
        }

        public bool Active
        {
            get => _buttonContainer.GameObject.activeInHierarchy;
            set
            {
                Header.Active = value;
                _buttonContainer.Active = value;
            }
        }

        public ReMenuCategory(string title, Transform parent = null)
        {
            Header = new ReMenuHeader(title, parent);
            _buttonContainer = new ReMenuButtonContainer(title, parent);
        }

        public ReMenuCategory(ReMenuHeader header, ReMenuButtonContainer container)
        {
            Header = header;
            _buttonContainer = container;
        }

        public ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            var button = new ReMenuButton(text, tooltip, onClick, _buttonContainer.RectTransform, sprite);
            return button;
        }

        public ReMenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false)
        {
            var toggle = new ReMenuToggle(text, tooltip, onToggle, _buttonContainer.RectTransform, defaultValue);
            return toggle;
        }

        public ReMenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue)
        {
            var toggle = new ReMenuToggle(text, tooltip, configValue.SetValue, _buttonContainer.RectTransform, configValue);
            return toggle;
        }

        public ReMenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null)
        {
            var existingPage = GetMenuPage(text);
            if (existingPage != null)
            {
                return existingPage;
            }

            var menu = new ReMenuPage(text);
            AddButton(text, string.IsNullOrEmpty(tooltip) ? $"Open the {text} menu" : tooltip, menu.Open, sprite);
            return menu;
        }

        public ReCategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null)
        {
            var existingPage = GetCategoryPage(text);
            if (existingPage != null)
            {
                return existingPage;
            }

            var menu = new ReCategoryPage(text);
            AddButton(text, string.IsNullOrEmpty(tooltip) ? $"Open the {text} menu" : tooltip, menu.Open, sprite);
            return menu;
        }

        public RectTransform RectTransform => _buttonContainer.RectTransform;

        public ReMenuPage GetMenuPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(UiElement.GetCleanName($"Menu_{name}"));
            return transform == null ? null : new ReMenuPage(transform);
        }

        public ReCategoryPage GetCategoryPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(UiElement.GetCleanName($"Menu_{name}"));
            return transform == null ? null : new ReCategoryPage(transform);
        }
    }
}
