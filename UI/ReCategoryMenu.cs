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
    public class ReCategoryPage : UiElement
    {
        private static GameObject _menuPrefab;

        private static GameObject MenuPrefab
        {
            get
            {
                if (_menuPrefab == null)
                {
                    _menuPrefab = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard").gameObject;
                }
                return _menuPrefab;
            }
        }

        private static int SiblingIndex => QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Modal_AddMessage").GetSiblingIndex();

        private readonly List<ReMenuCategory> _categories = new List<ReMenuCategory>();

        public event Action OnOpen;
        private readonly string _menuName;
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public ReCategoryPage(string text, bool isRoot = false) : base(MenuPrefab, MenuPrefab.transform.parent, $"Menu_ReMod{text}", false)
        {
            Object.DestroyImmediate(GameObject.GetComponent<LaunchPadQMMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            _menuName = GetCleanName(text);
            _isRoot = isRoot;
            var headerTransform = RectTransform.GetChild(0);
            Object.DestroyImmediate(headerTransform.Find("RightItemContainer/Button_QM_Expand").gameObject);

            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = text;
            titleText.richText = true;


            if (!_isRoot)
            {
                var backButton = headerTransform.GetComponentInChildren<Button>(true);
                backButton.gameObject.SetActive(true);
            }

            _container = RectTransform.GetComponentInChildren<ScrollRect>().content;
            foreach (var obj in _container)
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

        public ReMenuCategory AddCategory(string title)
        {
            var existingCategory = GetCategory(title);
            if (existingCategory != null)
            {
                return existingCategory;
            }

            var category = new ReMenuCategory(title, _container);
            _categories.Add(category);
            return category;
        }

        public ReMenuCategory GetCategory(string name)
        {
            return _categories.FirstOrDefault(c => c.Name == GetCleanName(name));
        }

        public static ReCategoryPage Create(string text, bool isRoot)
        {
            return new ReCategoryPage(text, isRoot);
        }
    }
}
