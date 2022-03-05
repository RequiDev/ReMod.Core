using System;
using UnityEngine;

namespace ReMod.Core.UI.QuickMenu
{
    public interface IButtonPage
    {
        ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null);
        ReMenuButton AddSpacer(Sprite sprite = null);
        ReMenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false);
        ReMenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue);
        ReMenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null);
        ReCategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null);
        ReMenuPage GetMenuPage(string name);
        ReCategoryPage GetCategoryPage(string name);
    }
}
