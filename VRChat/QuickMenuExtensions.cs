using System;
using System.Linq;
using UnhollowerRuntimeLib.XrefScans;
using VRC.UI.Elements;

namespace ReMod.Core.VRChat
{
    public static class QuickMenuExtensions
    {
        public delegate void ShowConfirmDialogDelegate(UIMenu uiMenu, string title, string body, Il2CppSystem.Action onYes, Il2CppSystem.Action onNo=null);
        private static ShowConfirmDialogDelegate _showConfirmDialogDelegate;

        private static ShowConfirmDialogDelegate ShowConfirmDialogFn
        {
            get
            {
                if (_showConfirmDialogDelegate != null)
                    return _showConfirmDialogDelegate;

                var showConfirmDialogFn = typeof(UIMenu).GetMethods().FirstOrDefault(m =>
                {
                    if (!m.Name.Contains("Public_Void_String_String_Action_Action_PDM_"))
                        return false;
                    
                    return XrefUtils.CheckMethod(m, "ConfirmDialog");
                });

                _showConfirmDialogDelegate = (ShowConfirmDialogDelegate)Delegate.CreateDelegate(typeof(ShowConfirmDialogDelegate), showConfirmDialogFn);

                return _showConfirmDialogDelegate;
            }
        }
        
        public delegate void ShowConfirmDialogWithCancelDelegate(UIMenu uiMenu, string title, string body, string yesLabel, string noLabel, string cancelLabel, Il2CppSystem.Action onYes, Il2CppSystem.Action onNo, Il2CppSystem.Action onCancel);
        private static ShowConfirmDialogWithCancelDelegate _showConfirmDialogWithCancelDelegate;
        
        private static ShowConfirmDialogWithCancelDelegate ShowConfirmDialogWithCancelFn
        {
            get
            {
                if (_showConfirmDialogWithCancelDelegate != null)
                    return _showConfirmDialogWithCancelDelegate;

                var showConfirmDialogWithCancelFn = typeof(UIMenu).GetMethods().FirstOrDefault(m =>
                {
                    if (!m.Name.Contains("Method_Public_Void_String_String_String_String_String_Action_Action_Action"))
                        return false;
                    
                    return XrefUtils.CheckMethod(m, "ConfirmDialog");
                });

                _showConfirmDialogWithCancelDelegate = (ShowConfirmDialogWithCancelDelegate)Delegate.CreateDelegate(typeof(ShowConfirmDialogWithCancelDelegate), showConfirmDialogWithCancelFn);

                return _showConfirmDialogWithCancelDelegate;
            }
        }
        
        public static void ShowConfirmDialog(this UIMenu uiMenu, string title, string body, Action onYes, Action onNo=null)
        {
            ShowConfirmDialogFn.Invoke(uiMenu, title, body, onYes, onNo);
        }
        
        public static void ShowConfirmDialogWithCancel(this UIMenu uiMenu, string title, string body, string yesLabel, string noLabel, string cancelLabel, Action onYes, Action onNo, Action onCancel)
        {
            ShowConfirmDialogWithCancelFn.Invoke(uiMenu, title, body, yesLabel, noLabel, cancelLabel, onYes, onNo, onCancel);
        }
    }
}