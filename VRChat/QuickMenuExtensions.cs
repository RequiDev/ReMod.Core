using System;
using System.Linq;
using UnhollowerRuntimeLib.XrefScans;
using VRC.UI.Elements;

namespace ReMod.Core.VRChat
{
    public static class QuickMenuExtensions
    {
        public delegate void ShowConfirmDialogDelegate(UIMenu uiMenu, string title, string body, Il2CppSystem.Action yesAction, Il2CppSystem.Action noAction=null);

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
        
        public static void ShowConfirmDialog(this UIMenu uiMenu, string title, string body, Action yesAction, Action noAction=null)
        {
            ShowConfirmDialogFn.Invoke(uiMenu, title, body, yesAction, noAction);
        }
    }
}