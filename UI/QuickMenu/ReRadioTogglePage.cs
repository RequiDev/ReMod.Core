using System;
using System.Collections.Generic;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;
using Object = Il2CppSystem.Object;

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
        private ListBinding ListBinding;
        private GameObject RadioButtonPrefab;
        private RadioButtonSelectorGroup RadioButtonSelectorGroup;
        private Dictionary<string, Tuple<string, Object, Action>> _radioElementSource = new();
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
            RadioButtonPrefab = inputMenu.field_Public_GameObject_0;
            ListBinding = inputMenu.field_Public_ListBinding_0;
            RadioButtonSelectorGroup = ListBinding.gameObject.GetComponent<RadioButtonSelectorGroup>();

            ListBinding.field_Protected_Dictionary_2_Object_GameObject_0 = new Il2CppSystem.Collections.Generic.Dictionary<Object, GameObject>();

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

        public void Open()
        {
            QuickMenuEx.MenuStateCtrl.PushPage(UiPage.field_Public_String_0);

            if (!_isUpdated)
                return;

            _isUpdated = false;
            
            foreach(var element in ListBinding.field_Protected_Dictionary_2_Object_GameObject_0)
                UnityEngine.Object.DestroyImmediate(element.value);
            ListBinding.field_Protected_Dictionary_2_Object_GameObject_0.Clear();

            foreach (var newElement in _radioElementSource)
            {
                var radioButton = UnityEngine.Object.Instantiate(RadioButtonPrefab, ListBinding.gameObject.transform);
                radioButton.active = true;
                var radioButtonSelector = radioButton.GetComponent<RadioButtonSelector>();
                radioButtonSelector.field_Public_TextMeshProUGUI_0 = radioButtonSelector.GetComponentInChildren<TextMeshProUGUI>();
                radioButtonSelector.field_Private_Button_0 = radioButtonSelector.GetComponent<Button>();
                UnityEngine.Object.DestroyImmediate(radioButtonSelector.GetComponent<AudioDeviceButton>());
                radioButtonSelector.field_Private_Button_0.onClick.AddListener(new Action(() => OnSelect?.Invoke(newElement.Value.Item2)));
                if(newElement.Value.Item3 != null)
                    radioButtonSelector.field_Private_Button_0.onClick.AddListener(newElement.Value.Item3);
                radioButtonSelector.field_Public_String_0 = newElement.Value.Item1;
                radioButtonSelector.SetTitle(newElement.Key, newElement.Value.Item1);
                radioButtonSelector.prop_RadioButtonSelectorGroup_0 = RadioButtonSelectorGroup;
                ListBinding.field_Protected_Dictionary_2_Object_GameObject_0.Add(newElement.Key, radioButton);
            }
        }

        /// <summary>
        /// Adds a item to the radio element source
        /// </summary>
        /// <param name="name">Name that will appear on radio toggle</param>
        /// <param name="obj">Object to be send in OnSelect event</param>
        /// <param name="onClick">OnClick when the toggle is selected in menu</param>
        public void AddItem(string name, Object obj, Action onClick = null)
        {
            _radioElementSource.Add($"{_radioElementSource.Count}_{name}", new Tuple<string, Object, Action>(name, obj, onClick));
            _isUpdated = true;
        }

        public void ClearItems()
        {
            _radioElementSource.Clear();
            _isUpdated = true;
        }
    }
}