using System;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
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

        protected TextMeshProUGUI TextComponent;
        public string Title
        {
            get => TextComponent.text;
            set => TextComponent.text = value;
        }

        public ReMenuHeader(string title, Transform parent) : base(HeaderPrefab, (parent == null ? HeaderPrefab.transform.parent : parent), $"Header_{title}")
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            TextComponent.text = title;
            TextComponent.richText = true;

            TextComponent.transform.parent.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        }

        public ReMenuHeader(Transform transform) : base(transform)
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        protected ReMenuHeader(GameObject original, Transform parent, Vector3 pos, string name, bool defaultState = true) : base(original, parent, pos, name, defaultState) { }
        protected ReMenuHeader(GameObject original, Transform parent, string name, bool defaultState = true) : base(original, parent, name, defaultState) { }
    }

    public class ReMenuHeaderCollapsible : ReMenuHeader
    {
        private static GameObject _headerPrefab;
        private static GameObject HeaderPrefab
        {
            get
            {
                if (_headerPrefab == null)
                {
                    _headerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Settings/Panel_QM_ScrollRect").GetComponent<ScrollRect>().content
                        .Find("QM_Foldout_UI_Elements").gameObject;
                }
                return _headerPrefab;
            }
        }

        public Action<bool> OnToggle;

        public ReMenuHeaderCollapsible(string title, Transform parent) : base(HeaderPrefab, (parent == null ? HeaderPrefab.transform.parent : parent), $"Header_{title}")
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            TextComponent.text = title;
            TextComponent.richText = true;

            var foldout = GameObject.GetComponent<QMFoldout>();
            foldout.field_Private_String_0 = $"UI.ReMod.{GetCleanName(title)}";
            foldout.field_Private_Action_1_Boolean_0 = new Action<bool>(b => OnToggle?.Invoke(b));
        }

        public ReMenuHeaderCollapsible(Transform transform) : base(transform)
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
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

        public ReMenuButtonContainer(string name, Transform parent = null) : base(ContainerPrefab, parent == null ? ContainerPrefab.transform.parent : parent, $"Buttons_{name}")
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
        public readonly ReMenuHeader Header;
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

        public ReMenuCategory(string title, Transform parent = null, bool collapsible = true)
        {
            if (collapsible)
            {
                var header = new ReMenuHeaderCollapsible(title, parent);
                header.OnToggle += b => _buttonContainer!.GameObject.SetActive(b);
                Header = header;
            }

            else
            {
                var header = new ReMenuHeader(title, parent);
                Header = header;
            }
            _buttonContainer = new ReMenuButtonContainer(title, parent);

        }

        public ReMenuCategory(ReMenuHeader headerElement, ReMenuButtonContainer container)
        {
            Header = headerElement;
            _buttonContainer = container;
        }

        public ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            var button = new ReMenuButton(text, tooltip, onClick, _buttonContainer.RectTransform, sprite);
            return button;
        }
        
        public ReMenuButton AddSpacer(Sprite sprite = null) {
            var spacer = AddButton(string.Empty, string.Empty, null, sprite);
            spacer.GameObject.name = "Button_Spacer";
            spacer.Background.gameObject.SetActive(false);
            return spacer;
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
