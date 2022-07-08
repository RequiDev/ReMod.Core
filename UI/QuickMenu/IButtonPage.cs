using System;
using UnityEngine;

namespace ReMod.Core.UI.QuickMenu
{
    public interface IButtonPage
    {
        ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null);
        ReMenuButton AddSpacer(Sprite sprite = null);
        ReMenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null);
        ReCategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null);
        ReMenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false, Sprite iconOn = null, Sprite iconOff = null);
        ReMenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue, Sprite iconOn = null, Sprite iconOff = null);
        ReMenuPage GetMenuPage(string name);
        ReCategoryPage GetCategoryPage(string name);
        void AddCategoryPage(string text, string tooltip, Action<ReCategoryPage> onPageBuilt, Sprite sprite = null);
        void AddMenuPage(string text, string tooltip, Action<ReMenuPage> onPageBuilt, Sprite sprite = null);
    }
}
