using System;
using System.Collections.Generic;
using MelonLoader;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;
using Object = System.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReRadioTogglePage : UiElement
    {
        private static GameObject _menuPrefab;
        
        private static GameObject MenuPrefab
        {
            get
            {
                if (_menuPrefab == null)
                {
                    _menuPrefab = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_ChangeAudioInputDevice").gameObject;
                }
                return _menuPrefab;
            }
        }
        
        private static int SiblingIndex => QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Modal_AddMessage").GetSiblingIndex();
        
        public event Action OnOpen;
        public event Action OnClose;
        public event Action<Object> OnSelect;

        public string TitleText
        {
            set => _titleText.text = value;
        }

        private TextMeshProUGUI _titleText;
        private GameObject _toggleGroupRoot;
        private List<Tuple<String, Object>> _radioElementSource = new();
        private List<ReRadioToggle> _radioElements = new();
        private bool _isUpdated;
        
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public ReRadioTogglePage(string name) : base(MenuPrefab, QuickMenuEx.MenuParent, $"Menu_{name}", false)
        {
            var headerTransform = RectTransform.GetChild(0);
            
            _titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            _titleText.text = name;
            _titleText.richText = true;
            
            _container = RectTransform.GetComponentInChildren<ScrollRect>().content;

            var inputMenu = RectTransform.GetComponent<AudioInputDeviceMenu>();

            _toggleGroupRoot = inputMenu.field_Public_ListBinding_0.gameObject;
            
            UnityEngine.Object.DestroyImmediate(_toggleGroupRoot.gameObject.GetComponent<ListBinding>());
            UnityEngine.Object.DestroyImmediate(_toggleGroupRoot.gameObject.GetComponent<RadioButtonSelectorGroup>());

            //Get rid of the AudioInputDeviceMenu component
            UnityEngine.Object.DestroyImmediate(inputMenu);

            // Set up UIPage
            UiPage = GameObject.GetComponent<UIPage>();
            UiPage.field_Public_String_0 = $"QuickMenuReMod{GetCleanName(name)}";
            UiPage.field_Private_Boolean_1 = true;
            UiPage.field_Protected_MenuStateController_0 = QuickMenuEx.MenuStateCtrl;
            UiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            UiPage.field_Private_List_1_UIPage_0.Add(UiPage);

            QuickMenuEx.MenuStateCtrl.field_Private_Dictionary_2_String_UIPage_0.Add(UiPage.field_Public_String_0, UiPage);

            EnableDisableListener.RegisterSafe();
            var listener = GameObject.AddComponent<EnableDisableListener>();
            listener.OnEnableEvent += () => OnOpen?.Invoke();
            listener.OnDisableEvent += () => OnClose?.Invoke();
        }

        /// <summary>
        /// Opens and configures the ReRadioTogglePage, optionally with default object to set toggles active
        /// </summary>
        /// <param name="selected">Default object matching ones on toggle elements</param>
        public void Open(Object selected = null)
        {
            QuickMenuEx.MenuStateCtrl.PushPage(UiPage.field_Public_String_0);

            if (_isUpdated)
            {
                _isUpdated = false;

                foreach (var element in _radioElements)
                    UnityEngine.Object.DestroyImmediate(element.GameObject);
                _radioElements.Clear();

                foreach (var newElement in _radioElementSource)
                {
                    var toggle = new ReRadioToggle(_toggleGroupRoot.transform, newElement.Item1, newElement.Item1, newElement.Item2);
                    toggle.ToggleStateUpdated += OnToggleSelect;
                    _radioElements.Add(toggle);
                }
            }
            
            if(selected == null)
                return;

            //Update the toggles to display the current active state
            foreach (var element in _radioElements)
            {
                element.SetToggle(element.ToggleData.Equals(selected));
            }
        }

        private void OnToggleSelect(ReRadioToggle toggle, bool state)
        {
            foreach (var element in _radioElements)
            {
                if(element == toggle)
                    continue;
                
                element.SetToggle(false);
            }
            
            OnSelect?.Invoke(toggle.ToggleData);
        }

        /// <summary>
        /// Adds a item to the radio element source
        /// </summary>
        /// <param name="name">Name that will appear on radio toggle</param>
        /// <param name="obj">Object to be send in OnSelect event</param>
        public void AddItem(string name, Object obj)
        {
            _radioElementSource.Add(new Tuple<string, Object>(name, obj));
            _isUpdated = true;
        }

        public void ClearItems()
        {
            _radioElementSource.Clear();
            _isUpdated = true;
        }
    }
}