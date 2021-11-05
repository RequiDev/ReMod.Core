using System;
using System.Collections.Generic;
using System.Linq;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI
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

        private readonly List<ReMenuPage> _subMenuPages = new List<ReMenuPage>();
        private readonly List<ReCategoryPage> _subCategoryPages = new List<ReCategoryPage>();

        public event Action OnOpen;
        private readonly string _menuName;
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public ReMenuPage(string text, bool isRoot = false) : base(MenuPrefab, MenuPrefab.transform.parent, $"Menu_ReMod{text}", false)
        {
            Object.DestroyImmediate(GameObject.GetComponent<DevMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            _menuName = GetCleanName(text);
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

            headerTransform.name = $"Header_{_menuName}";

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
            UiPage.field_Public_String_0 = $"QuickMenuReMod{_menuName}";
            UiPage.field_Private_Boolean_1 = true;
            UiPage.field_Private_MenuStateController_0 = QuickMenuEx.MenuStateCtrl;
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
        }

        public void Open()
        {
            if (_isRoot)
            {
                QuickMenuEx.MenuStateCtrl.SwitchToRootPage($"QuickMenuReMod{_menuName}");
            }
            else
            {
                QuickMenuEx.MenuStateCtrl.PushPage($"QuickMenuReMod{_menuName}");
            }

            OnOpen?.Invoke();
        }

        public ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            return new ReMenuButton(text, tooltip, onClick, _container, sprite);
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
            _subMenuPages.Add(menu);
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
            _subCategoryPages.Add(menu);
            return menu;
        }

        public ReMenuPage GetMenuPage(string name)
        {
            return _subMenuPages.FirstOrDefault(m => m.Name == GetCleanName($"Menu_ReMod{name}"));
        }

        public ReCategoryPage GetCategoryPage(string name)
        {
            return _subCategoryPages.FirstOrDefault(m => m.Name == GetCleanName($"Menu_ReMod{name}"));
        }

        public static ReMenuPage Create(string text, bool isRoot)
        {
            return new ReMenuPage(text, isRoot);
        }
    }
}
