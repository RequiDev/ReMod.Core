using System;
using UnityEngine;

namespace ReMod.Core.UI.QuickMenu
{
    public interface IButtonPage
    {
        ReMenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null);
        ReMenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false);
        ReMenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue);
        ReMenuSlider AddSlider(string text, string tooltip, Action<float> onSlide, float defaultValue = 0, float minValue = 0, float maxValue = 10);
        ReMenuSlider AddSlider(string text, string tooltip, ConfigValue<float> configValue, float defaultValue = 0, float minValue = 0, float maxValue = 10);
        ReMenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null);
        ReCategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null);
        ReMenuPage GetMenuPage(string name);
        ReCategoryPage GetCategoryPage(string name);
    }
}
