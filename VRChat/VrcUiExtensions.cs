using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;
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
            return uiManager.field_Private_Transform_0.gameObject;
        }

        public static void StartRenderElementsCoroutine(this UiVRCList instance, Il2CppSystem.Collections.Generic.List<ApiAvatar> avatarList, int offset = 0, bool endOfPickers = true, VRCUiContentButton contentHeaderElement = null)
        {
            if (!instance.gameObject.activeInHierarchy || !instance.isActiveAndEnabled || instance.isOffScreen ||
                !instance.enabled)
                return;

            if (instance.scrollRect != null)
            {
                instance.scrollRect.normalizedPosition = new Vector2(0f, 0f);
            }
            instance.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0(avatarList, offset, endOfPickers, contentHeaderElement);
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
            bool clearPageStack, UIPage.TransitionType transitionType);
        private static PushPageDelegate _pushPage;

        public static void PushPage(this MenuStateController menuStateCtrl, string pageName, UIContext uiContext = null,
            bool clearPageStack = false, UIPage.TransitionType transitionType = UIPage.TransitionType.Right)
        {
            if (_pushPage == null)
            {
                _pushPage = (PushPageDelegate)Delegate.CreateDelegate(typeof(PushPageDelegate),
                    typeof(MenuStateController).GetMethods().FirstOrDefault(m => m.GetParameters().Length == 4 && m.Name.StartsWith("Method_Public_Void_String_UIContext_Boolean_TransitionType_") && XrefUtils.CheckMethod(m, "No page named")));
            }

            _pushPage(menuStateCtrl, pageName, uiContext, clearPageStack, transitionType);
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

        private delegate void CloseUiDelegate(VRCUiManager uiManager, bool what, bool what2);
        private static CloseUiDelegate _closeUi;
        
        public static void CloseUi(this VRCUiManager uiManager)
        {
            if (_closeUi == null)
            {
                _closeUi = (CloseUiDelegate)Delegate.CreateDelegate(typeof(CloseUiDelegate),
                    typeof(VRCUiManager).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Public_Void_Boolean_Boolean") && !m.Name.Contains("PDM") && XrefUtils.CheckUsing(m, "TrimCache")));
            }

            _closeUi(uiManager, true, false);
        }

        private delegate void ShowUiDelegate(VRCUiManager uiManager, bool showDefaultScreen, bool showBackdrop);
        private static ShowUiDelegate _showUi;

        public static void ShowUi(this VRCUiManager uiManager, bool showDefaultScreen = true, bool showBackdrop = true)
        {
            if (_showUi == null)
            {
                _showUi = (ShowUiDelegate)Delegate.CreateDelegate(typeof(ShowUiDelegate), typeof(VRCUiManager).GetMethods()
                    .First(mb => mb.Name.StartsWith("Method_Public_Void_Boolean_Boolean_") && !mb.Name.Contains("_PDM_") && XrefUtils.CheckMethod(mb, "UserInterface/MenuContent/Backdrop/Backdrop")));
            }

            _showUi(uiManager, showDefaultScreen, showBackdrop);
        }

        public static void ShowScreen(this VRCUiManager uiManager, string screen, bool addToScreenStack = false)
        {
            uiManager.Method_Public_Void_String_Boolean_0(screen, addToScreenStack);
        }

        public static void ShowScreen(this VRCUiManager uiManager, QuickMenu.MainMenuScreenIndex menuIndex,
            bool addToScreenStack = false)
        {
            ShowScreen(uiManager, BigMenuIndexToPathTable[menuIndex], addToScreenStack);
        }

        public static Transform GetScreen(this VRCUiManager uiManager, QuickMenu.MainMenuScreenIndex menuIndex)
        {
            if (!BigMenuIndexToPathTable.ContainsKey(menuIndex))
                return null;

            return uiManager.MenuContent().transform.Find($"Screens/{BigMenuIndexToNameTable[menuIndex]}");
        }

        private static MethodInfo[] _radioSetTitleMethods; 

        public static void SetTitle(this RadioButtonSelector selector, string key, string displayName)
        {
            _radioSetTitleMethods ??= typeof(RadioButtonSelector).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name.Contains("Method_Public_Void_String_String_PDM") && XrefScanner.UsedBy(x).Any()).ToArray();

            foreach (var method in _radioSetTitleMethods)
            {
                method.Invoke(selector, new object[] {key, displayName});
            }
        }

        private static readonly Dictionary<QuickMenu.MainMenuScreenIndex, string> BigMenuIndexToPathTable = new Dictionary<QuickMenu.MainMenuScreenIndex, string>()
        {
            { QuickMenu.MainMenuScreenIndex.Unknown, "" },
            { QuickMenu.MainMenuScreenIndex.WorldsMenu, "UserInterface/MenuContent/Screens/WorldInfo" },
            { QuickMenu.MainMenuScreenIndex.AvatarMenu, "UserInterface/MenuContent/Screens/Avatar" },
            { QuickMenu.MainMenuScreenIndex.SocialMenu, "UserInterface/MenuContent/Screens/Social" },
            { QuickMenu.MainMenuScreenIndex.SettingsMenu, "UserInterface/MenuContent/Screens/Settings" },
            { QuickMenu.MainMenuScreenIndex.UserDetailsMenu, "UserInterface/MenuContent/Screens/UserInfo" },
            { QuickMenu.MainMenuScreenIndex.DetailsMenu_Obsolete, "UserInterface/MenuContent/Screens/ImageDetails" },
            { QuickMenu.MainMenuScreenIndex.SafetyMenu, "UserInterface/MenuContent/Screens/Settings_Safety" },
            { QuickMenu.MainMenuScreenIndex.CurrentUserFavoriteListMenu, "UserInterface/MenuContent/Screens/Playlists" },
            { QuickMenu.MainMenuScreenIndex.OtherUserFavoriteListMenu, "UserInterface/MenuContent/Screens/Playlists" },
            { QuickMenu.MainMenuScreenIndex.VRCPlusMenu, "UserInterface/MenuContent/Screens/VRC+" },
            { QuickMenu.MainMenuScreenIndex.GalleryMenu, "UserInterface/MenuContent/Screens/Gallery" },
        };


        private static readonly Dictionary<QuickMenu.MainMenuScreenIndex, string> BigMenuIndexToNameTable = new Dictionary<QuickMenu.MainMenuScreenIndex, string>()
        {
            { QuickMenu.MainMenuScreenIndex.Unknown, "" },
            { QuickMenu.MainMenuScreenIndex.WorldsMenu, "WorldInfo" },
            { QuickMenu.MainMenuScreenIndex.AvatarMenu, "Avatar" },
            { QuickMenu.MainMenuScreenIndex.SocialMenu, "Social" },
            { QuickMenu.MainMenuScreenIndex.SettingsMenu, "Settings" },
            { QuickMenu.MainMenuScreenIndex.UserDetailsMenu, "UserInfo" },
            { QuickMenu.MainMenuScreenIndex.DetailsMenu_Obsolete, "ImageDetails" },
            { QuickMenu.MainMenuScreenIndex.SafetyMenu, "Settings_Safety" },
            { QuickMenu.MainMenuScreenIndex.CurrentUserFavoriteListMenu, "Playlists" },
            { QuickMenu.MainMenuScreenIndex.OtherUserFavoriteListMenu, "Playlists" },
            { QuickMenu.MainMenuScreenIndex.VRCPlusMenu, "VRC+" },
            { QuickMenu.MainMenuScreenIndex.GalleryMenu, "Gallery" },
        };
    }
}
