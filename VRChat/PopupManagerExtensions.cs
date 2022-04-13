using MelonLoader.Preferences;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.Unity;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.UI;

namespace ReMod.Core.VRChat
{
    public static class PopupManagerExtensions
    {
        public delegate void ShowAlertDelegate(VRCUiPopupManager popupManager, string title, string body, float timeout);
        public delegate void ShowStandardPopupV2Fn(VRCUiPopupManager popupManager, string title, string body, string leftButtonText, Il2CppSystem.Action leftButtonAction, string rightButtonText, Il2CppSystem.Action rightButtonAction, Il2CppSystem.Action<VRCUiPopup> additionalSetup = null);
        public delegate void ShowStandardPopupV21Fn(VRCUiPopupManager popupManager, string title, string body, string buttonText, Il2CppSystem.Action onClick, Il2CppSystem.Action<VRCUiPopup> additionalSetup = null);

        private static ShowStandardPopupV21Fn _showStandardPopupV21Fn;

        private static ShowAlertDelegate _showAlertDelegate;

        private static ShowAlertDelegate ShowAlertFn
        {
            get
            {
                if (_showAlertDelegate != null)
                    return _showAlertDelegate;

                var showAlertFn = typeof(VRCUiPopupManager).GetMethods().Single(m =>
                {
                    if (m.ReturnType != typeof(void))
                        return false;

                    if (m.GetParameters().Length != 3)
                        return false;

                    return XrefScanner.XrefScan(m).Any(x => x.Type == XrefType.Global && x.ReadAsObject()?.ToString() ==
                        "UserInterface/MenuContent/Popups/AlertPopup");
                });

                _showAlertDelegate = (ShowAlertDelegate)Delegate.CreateDelegate(typeof(ShowAlertDelegate), showAlertFn);

                return _showAlertDelegate;
            }
        }

        private static ShowStandardPopupV21Fn ShowUiStandardPopupV21
        {
            get
            {
                if (_showStandardPopupV21Fn != null)
                {
                    return _showStandardPopupV21Fn;
                }
                var methodInfo = typeof(VRCUiPopupManager).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(it => {
                    if (it.GetParameters().Length == 5 && !it.Name.Contains("PDM"))
                    {
                        return XrefScanner.XrefScan(it).Any(delegate (XrefInstance jt)
                        {
                            if (jt.Type != XrefType.Global) return false;
                            var @object = jt.ReadAsObject();
                            return @object?.ToString() == "UserInterface/MenuContent/Popups/StandardPopupV2";
                        });
                    }
                    return false;
                });
                _showStandardPopupV21Fn = (ShowStandardPopupV21Fn)Delegate.CreateDelegate(typeof(ShowStandardPopupV21Fn), methodInfo);
                return _showStandardPopupV21Fn;
            }
        }

        public static void HideCurrentPopup(this VRCUiPopupManager vrcUiPopupManager)
        {
            VRCUiManagerEx.Instance.HideScreen("POPUP");
        }

        public static void ShowAlert(this VRCUiPopupManager popupManager, string title, string body, float timeout = 0f)
        {
            ShowAlertFn(popupManager, title, body, timeout);
        }

        private delegate void ShowInputPopupWithCancelDelegate(VRCUiPopupManager popupManager, string title,
            string preFilledText,
            InputField.InputType inputType, bool useNumericKeypad, string submitButtonText,
            Il2CppSystem.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text> submitButtonAction,
            Il2CppSystem.Action cancelButtonAction, string placeholderText = "Enter text....", bool hidePopupOnSubmit = true,
            Il2CppSystem.Action<VRCUiPopup> additionalSetup = null, bool param_11 = false, int param_12 = 0);

        private static ShowInputPopupWithCancelDelegate _showInputPopupWithCancelDelegate;

        private static ShowInputPopupWithCancelDelegate ShowInputPopupWithCancelFn
        {
            get
            {
                if (_showInputPopupWithCancelDelegate != null)
                    return _showInputPopupWithCancelDelegate;

                var method = typeof(VRCUiPopupManager).GetMethods().Single(m =>
                {
                    if (!m.Name.StartsWith(
                            "Method_Public_Void_String_String_InputType_Boolean_String_Action_3_String_List_1_KeyCode_Text_Action_String_Boolean_Action_1_VRCUiPopup_Boolean_Int32_") ||
                        m.Name.Contains("PDM"))
                        return false;

                    return XrefScanner.XrefScan(m).Any(x => x.Type == XrefType.Global && x.ReadAsObject()?.ToString() ==
                        "UserInterface/MenuContent/Popups/InputKeypadPopup");
                });

                _showInputPopupWithCancelDelegate = (ShowInputPopupWithCancelDelegate)Delegate.CreateDelegate(typeof(ShowInputPopupWithCancelDelegate), method);

                return _showInputPopupWithCancelDelegate;
            }
        }

        public static void ShowInputPopupWithCancel(this VRCUiPopupManager popupManager, string title, string preFilledText,
            InputField.InputType inputType, bool useNumericKeypad, string submitButtonText,
            Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text> submitButtonAction,
            Action cancelButtonAction, string placeholderText = "Enter text....", bool hidePopupOnSubmit = true,
            Action<VRCUiPopup> additionalSetup = null)
        {
            ShowInputPopupWithCancelFn(popupManager,
                    title,
                    preFilledText,
                    inputType, useNumericKeypad, submitButtonText, submitButtonAction, cancelButtonAction, placeholderText, hidePopupOnSubmit, additionalSetup);
        }

        public static void ShowStandardPopupV2(this VRCUiPopupManager popupManager, string title, string body, string leftButtonText,
            Action leftButtonAction, string rightButtonText, Action rightButtonAction,
            Action<VRCUiPopup> additonalSetup)
        {
            popupManager.Method_Public_Void_String_String_String_Action_String_Action_Action_1_VRCUiPopup_0(title, body, leftButtonText, leftButtonAction, rightButtonText, rightButtonAction, additonalSetup);
        }

        public static void ShowStandardPopupV2(this VRCUiPopupManager popupManager, string title, string body, string buttonText,
            Action onClick, Action<VRCUiPopup> onCreated=null)
        {
            ShowUiStandardPopupV21(popupManager, title, body, buttonText, onClick, onCreated);
        }

        public static void ShowColorInputPopup(this VRCUiPopupManager popupManager, ReMenuButton button, string who, ConfigValue<Color> configValue)
        {
            popupManager.ShowInputPopupWithCancel("Input hex color code",
                $"#{configValue.Value.ToHex()}", InputField.InputType.Standard, false, "Submit",
                (s, k, t) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;

                    if (!ColorUtility.TryParseHtmlString(s, out var color))
                        return;

                    color.a = 1f;
                    configValue.SetValue(color);

                    button.Text = $"<color=#{configValue.Value.ToHex()}>{who}</color> Color";
                }, null);
        }
        
        public static void ShowInputPopup<T>(this VRCUiPopupManager popupManager, string popupText, T oldValue, Action<T> callback, ReMenuButton menuButton = null, string buttonText = null, ValueRange<T> range = null) where T : IComparable
        {
            popupManager.ShowInputPopupWithCancel($"{popupText} {(range!=null ? $"Range: {range.MinValue}-{range.MaxValue}" : string.Empty)}",
                $"{oldValue}", InputField.InputType.Standard, false, "Submit",
                (s, _, _) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;

                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    if (!converter.IsValid(s)) return;
                    
                    T value = (T) converter.ConvertFromInvariantString(s);

                    if (range != null && !range.IsValid(value))
                        return;

                    if (menuButton != null)
                    {
                        menuButton.Text = $"{buttonText ?? "Value"}: {value}";
                    }
                    callback(value);
                }, null);
        }
        
        public static void ShowInputPopup<T>(this VRCUiPopupManager popupManager, string popupText, ConfigValue<T> configValue, ReMenuButton menuButton = null, string buttonText = null, ValueRange<T> range = null) where T : IComparable
        {
            popupManager.ShowInputPopupWithCancel($"{popupText} {(range!=null ? $"Range: {range.MinValue}-{range.MaxValue}" : string.Empty)}",
                $"{configValue.Value}", InputField.InputType.Standard, false, "Submit",
                (s, _, _) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;

                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    if (!converter.IsValid(s)) return;
                    
                    T value = (T) converter.ConvertFromInvariantString(s);

                    if (range != null && !range.IsValid(value))
                        return;
                    
                    if (menuButton != null)
                    {
                        menuButton.Text = $"{buttonText ?? "Value"}: {value}";
                    }
                    configValue.SetValue(value);
                }, null);
        }
    }
}
