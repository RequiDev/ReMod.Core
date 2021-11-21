using System;
using ReMod.Core.UI;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;

namespace ReMod.Core.Managers
{
    public class UiManager
    {
        public IButtonPage MainMenu { get; }
        public IButtonPage TargetMenu { get; }

        public UiManager(string menuName, Sprite menuSprite)
        {
            MainMenu = new ReMenuPage(menuName, true);
            ReTabButton.Create(menuName, $"Open the {menuName} menu.", menuName, menuSprite);

            TargetMenu = new ReMenuCategory($"{menuName}", QuickMenuEx.SelectedUserLocal.transform.Find("ScrollRect").GetComponent<ScrollRect>().content);
        }
    }
}
