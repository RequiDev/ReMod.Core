using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine;
using VRC.Core;
using VRC.UI.Core;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;

namespace ReMod.Core.VRChat
{
    public static class VrcUiExtensions
    {
        public static GameObject MenuContent(this VRCUiManager uiManager)
        {
            return uiManager.field_Public_GameObject_0;
        }

        public static void StartRenderElementsCoroutine(this UiVRCList instance, Il2CppSystem.Collections.Generic.List<ApiAvatar> avatarList, int offset = 0, bool endOfPickers = true, VRCUiContentButton contentHeaderElement = null)
        {
            MelonCoroutines.Start(RenderElements(0.1f));

            IEnumerator RenderElements(float delay)
            {
                while (!instance.gameObject.activeInHierarchy || !instance.isActiveAndEnabled || instance.isOffScreen ||
                    !instance.enabled)
                    yield return new WaitForSeconds(delay);

                if (instance.scrollRect != null)
                {
                    instance.scrollRect.normalizedPosition = new Vector2(0f, 0f);
                }
                instance.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0(avatarList, offset, endOfPickers, contentHeaderElement);
            };
        }

        private delegate void OnValueChangedDelegate(ToggleIcon toggleIcon, bool arg0);
        private static OnValueChangedDelegate _onValueChanged;

        public static void OnValueChanged(this ToggleIcon toggleIcon, bool arg0)
        {
            if (_onValueChanged == null)
            {
                _onValueChanged = (OnValueChangedDelegate)Delegate.CreateDelegate(typeof(OnValueChangedDelegate),
                    typeof(ToggleIcon).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Private_Void_Boolean_PDM_") && XrefUtils.CheckMethod(m, "Toggled")));
            }

            _onValueChanged(toggleIcon, arg0);
        }

        public static void QueueHudMessage(this VRCUiManager uiManager, string notification, Color color, float duration = 5f,
            float delay = 0f)
        {
            uiManager.field_Public_Text_0.color = color; // DisplayTextColor
            uiManager.field_Public_Text_0.text = string.Empty;
            uiManager.field_Private_Single_0 = 0f; // HudMessageDisplayTime
            uiManager.field_Private_Single_1 = duration; // HudMessageDisplayDuration
            uiManager.field_Private_Single_2 = delay; // DelayBeforeHudMessage

            uiManager.field_Private_List_1_String_0.Add(notification);
        }
        
        private delegate void PushPageDelegate(MenuStateController menuStateCtrl, string pageName, UIContext uiContext,
            bool clearPageStack);
        private static PushPageDelegate _pushPage;

        public static void PushPage(this MenuStateController menuStateCtrl, string pageName, UIContext uiContext = null,
            bool clearPageStack = false)
        {
            if (_pushPage == null)
            {
                _pushPage = (PushPageDelegate)Delegate.CreateDelegate(typeof(PushPageDelegate),
                    typeof(MenuStateController).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Public_Void_String_UIContext_Boolean_") && XrefUtils.CheckMethod(m, "No page named")));
            }

            _pushPage(menuStateCtrl, pageName, uiContext, clearPageStack);
        }
        
        private delegate void SwitchToRootPageDelegate(MenuStateController menuStateCtrl, string pageName, UIContext uiContext,
            bool clearPageStack, bool inPlace);
        private static SwitchToRootPageDelegate _switchToRootPage;

        public static void SwitchToRootPage(this MenuStateController menuStateCtrl, string pageName, UIContext uiContext = null,
            bool clearPageStack = false, bool inPlace = false)
        {
            if (_switchToRootPage == null)
            {
                _switchToRootPage = (SwitchToRootPageDelegate)Delegate.CreateDelegate(typeof(SwitchToRootPageDelegate),
                    typeof(MenuStateController).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Public_Void_String_UIContext_Boolean_") && XrefUtils.CheckMethod(m, "UIPage not in root page list:")));
            }

            _switchToRootPage(menuStateCtrl, pageName, uiContext, clearPageStack, inPlace);
        }
    }
}
