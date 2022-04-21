using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Reflection;
using MelonLoader;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI.QuickMenu
{
    public class ReMenuToggle : UiElement
    {
        private readonly Toggle _toggleComponent;

        public bool Interactable
        {
            get => _toggleComponent.interactable;
            set
            {
                _toggleComponent.interactable = value;
                            
                if(_toggleStyleElement != null)
                    _toggleStyleElement.OnEnable();
            }
        }

        private bool _valueHolder;
        
        private StyleElement _toggleStyleElement;

        private object _toggleIcon;

        private TextMeshProUGUI _textComponent;
        public string Text
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        public ReMenuToggle(string text, string tooltip, Action<bool> onToggle, Transform parent, bool defaultValue = false) : base(QuickMenuEx.TogglePrefab, parent, $"Button_Toggle{text}")
        {
            var iconOn = RectTransform.Find("Icon_On").GetComponent<Image>();
            iconOn.sprite = QuickMenuEx.OnIconSprite;

            Object.DestroyImmediate(GameObject.GetComponent<UIInvisibleGraphic>()); // Fix for having clickable area overlap main quickmenu ui

            FindToggleIcon();

            _toggleComponent = GameObject.GetComponent<Toggle>();
            _toggleComponent.onValueChanged = new Toggle.ToggleEvent();
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(OnValueChanged));
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(onToggle));
            
            _toggleStyleElement = GameObject.GetComponent<StyleElement>();

            _textComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _textComponent.text = text;
            _textComponent.richText = true;
            _textComponent.color = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            _textComponent.m_fontColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            _textComponent.m_htmlColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiToggleTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;
            
            Toggle(defaultValue,false);

            EnableDisableListener.RegisterSafe();
            var edl = GameObject.AddComponent<EnableDisableListener>();
            edl.OnEnableEvent += UpdateToggleIfNeeded;
        }

        public ReMenuToggle(Transform transform) : base(transform)
        {
            FindToggleIcon();
            _toggleComponent = GameObject.GetComponent<Toggle>();
        }

        public void Toggle(bool value, bool callback = true, bool updateVisually = false)
        {
            _valueHolder = value;
            _toggleComponent.Set(value, callback);
            if (updateVisually)
            {
                UpdateToggleIfNeeded();
            }
        }
        private void UpdateToggleIfNeeded()
        {
           OnValueChanged(_valueHolder);
        }

        private void FindToggleIcon()
        {
            var components = new Il2CppSystem.Collections.Generic.List<Component>();
            GameObject.GetComponents(components);

            foreach (var c in components)
            {
                var il2CppType = c.GetIl2CppType();
                var il2CppFields = il2CppType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                if (il2CppFields.Count != 1)
                    continue;

                if (!il2CppFields.Any(t => t.IsPublic && t.FieldType == Il2CppType.Of<Toggle>()))
                    continue;

                var realType = GetUnhollowedType(il2CppType);
                if (realType == null)
                {
                    MelonLogger.Error("SHITS FUCKED!");
                    break;
                }
                _toggleIcon = Activator.CreateInstance(realType, c.Pointer);
                break;
            }
        }

        private List<Action<bool>> _onValueChanged;

        private void OnValueChanged(bool arg0)
        {
            if (_onValueChanged == null)
            {
                _onValueChanged = new List<Action<bool>>();
                foreach (var methodInfo in _toggleIcon.GetType().GetMethods().Where(m =>
                             m.Name.StartsWith("Method_Private_Void_Boolean_PDM_") && XrefUtils.CheckMethod(m, "Toggled")))
                {
                    _onValueChanged.Add((Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), _toggleIcon, methodInfo));
                }
            }

            foreach (var onValueChanged in _onValueChanged)
            {
                onValueChanged(arg0);
            }
        }


        private static readonly Dictionary<string, Type> DeobfuscatedTypes = new Dictionary<string, Type>();
        private static readonly Dictionary<string, string> ReverseDeobCache = new Dictionary<string, string>();

        private static void BuildDeobfuscationCache()
        {
            if (DeobfuscatedTypes.Count > 0)
                return;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.TryGetTypes())
                    TryCacheDeobfuscatedType(type);
            }
        }

        private static void TryCacheDeobfuscatedType(Type type)
        {
            try
            {
                if (!type.CustomAttributes.Any())
                    return;

                foreach (var att in type.CustomAttributes)
                {
                    // Thanks to Slaynash for this

                    if (att.AttributeType == typeof(ObfuscatedNameAttribute))
                    {
                        string obfuscatedName = att.ConstructorArguments[0].Value.ToString();

                        DeobfuscatedTypes.Add(obfuscatedName, type);
                        ReverseDeobCache.Add(type.FullName, obfuscatedName);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
        public static Type GetUnhollowedType(Il2CppSystem.Type cppType)
        {
            if (DeobfuscatedTypes.Count == 0)
            {
                BuildDeobfuscationCache();
            }

            var fullname = cppType.FullName;

            if (DeobfuscatedTypes.TryGetValue(fullname, out var deob))
                return deob;

            if (fullname.StartsWith("System."))
                fullname = $"Il2Cpp{fullname}";

            return null;
        }
    }
}
