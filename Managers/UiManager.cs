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

        private static UiManager _instance;
        public UiManager(string menuName, Sprite menuSprite)
        {
            if (_instance != null)
            {
                throw new Exception("UiManager already exists.");
            }
            _instance = this;

            MainMenu = new ReMenuPage(menuName, true);
            ReTabButton.Create(menuName, $"Open the {menuName} menu.", menuName, menuSprite);

            TargetMenu = new ReMenuCategory($"{menuName}", QuickMenuEx.SelectedUserLocal.transform.Find("ScrollRect").GetComponent<ScrollRect>().content);
        }
    }
}
