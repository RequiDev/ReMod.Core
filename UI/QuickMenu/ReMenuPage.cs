using System;
using System.Linq;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuPage : UiElement, IButtonPage
    {
        private static GameObject _menuPrefab;

        private static GameObject MenuPrefab
        {
            get
            {
                if (_menuPrefab == null)
                {
                    _menuPrefab = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_DevTools").gameObject;
                }
                return _menuPrefab;
            }
        }

        private static int SiblingIndex => QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Modal_AddMessage").GetSiblingIndex();

        public event Action OnOpen;
        public event Action OnClose;
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public ReMenuPage(string text, bool isRoot = false) : base(MenuPrefab, QuickMenuEx.MenuParent, $"Menu_{text}", false)
        {
            Object.DestroyImmediate(GameObject.GetComponent<DevMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            var menuName = GetCleanName(text);
            _isRoot = isRoot;
            var headerTransform = RectTransform.GetChild(0);
            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = text;
            titleText.richText = true;

            if (!_isRoot)
            {
                var backButton = headerTransform.GetComponentInChildren<Button>(true);
                backButton.gameObject.SetActive(true);
            }

            headerTransform.name = $"Header_{menuName}";

            var buttonContainer = RectTransform.Find("Scrollrect/Viewport/VerticalLayoutGroup/Buttons");
            foreach (var obj in buttonContainer)
            {
                var control = obj.Cast<Transform>();
                if (control == null)
                {
                    continue;
                }
                Object.Destroy(control.gameObject);
            }

            // Set up UIPage
            UiPage = GameObject.AddComponent<UIPage>();
            UiPage.field_Public_String_0 = $"QuickMenuReMod{menuName}";
            UiPage.field_Private_Boolean_1 = true;
            UiPage.field_Protected_MenuStateController_0 = QuickMenuEx.MenuStateCtrl;
            UiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            UiPage.field_Private_List_1_UIPage_0.Add(UiPage);

            // Get scroll stuff
            var scrollRect = RectTransform.Find("Scrollrect").GetComponent<ScrollRect>();
            _container = scrollRect.content;

            // copy properties of old grid layout
            var gridLayoutGroup = _container.Find("Buttons").GetComponent<GridLayoutGroup>();

            Object.DestroyImmediate(_container.GetComponent<VerticalLayoutGroup>());
            var glp = _container.gameObject.AddComponent<GridLayoutGroup>();
            glp.spacing = gridLayoutGroup.spacing;
            glp.cellSize = gridLayoutGroup.cellSize;
            glp.constraint = gridLayoutGroup.constraint;
            glp.constraintCount = gridLayoutGroup.constraintCount;
            glp.startAxis = gridLayoutGroup.startAxis;
            glp.startCorner = gridLayoutGroup.startCorner;
            glp.childAlignment = TextAnchor.UpperLeft;
            glp.padding = gridLayoutGroup.padding;
            glp.padding.top = 8;
            glp.padding.left = 64;

            // delete components we're not using
            Object.DestroyImmediate(_container.Find("Buttons").gameObject);
            Object.DestroyImmediate(_container.Find("Spacer_8pt").gameObject);

            // Fix scrolling
            var scrollbar = scrollRect.transform.Find("Scrollbar");
            scrollbar.gameObject.SetActive(true);

            scrollRect.enabled = true;
            scrollRect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;

            QuickMenuEx.MenuStateCtrl.field_Private_Dictionary_2_String_UIPage_0.Add(UiPage.field_Public_String_0, UiPage);

            if (isRoot)
            {
                var rootPages = QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0.ToList();
                rootPages.Add(UiPage);
                QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0 = rootPages.ToArray();
            }

            EnableDisableListener.RegisterSafe();
            var listener = GameObject.AddComponent<EnableDisableListener>();
            listener.OnEnableEvent += () => OnOpen?.Invoke();
            listener.OnDisableEvent += () => OnClose?.Invoke();
        }

        public ReMenuPage(Transform transform) : base(transform)
        {
            UiPage = GameObject.GetComponent<UIPage>();
            _isRoot = QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0.Contains(UiPage);
            var scrollRect = RectTransform.Find("Scrollrect").GetComponent<ScrollRect>();
            _container = scrollRect.content;
        }

        public void Open()
        {
            if (_isRoot)
            {
                QuickMenuEx.MenuStateCtrl.SwitchToRootPage(UiPage.field_Public_String_0);
            }
            else
            {
                QuickMenuEx.MenuStateCtrl.PushPage(UiPage.field_Public_String_0);
            }

            OnOpen?.Invoke();
        }

        public ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            return new ReMenuButton(text, tooltip, onClick, _container, sprite);
        }

        public ReMenuButton AddSpacer(Sprite sprite = null)
        {
            var spacer = AddButton(string.Empty, string.Empty, null, sprite);
            spacer.GameObject.name = "Button_Spacer";
            spacer.Background.gameObject.SetActive(false);
            return spacer;
        }

        public ReMenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false)
        {
            return new ReMenuToggle(text, tooltip, onToggle, _container, defaultValue);
        }

        public ReMenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue)
        {
            return new ReMenuToggle(text, tooltip, configValue.SetValue, _container, configValue);
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

        public ReMenuPage GetMenuPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(GetCleanName($"Menu_{name}"));
            return transform == null ? null : new ReMenuPage(transform);
        }

        public ReCategoryPage GetCategoryPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(GetCleanName($"Menu_{name}"));
            return transform == null ? null : new ReCategoryPage(transform);
        }

        public static ReMenuPage Create(string text, bool isRoot)
        {
            return new ReMenuPage(text, isRoot);
        }
    }
}
