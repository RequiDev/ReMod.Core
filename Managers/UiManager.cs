using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;

namespace ReMod.Core.Managers
{
    public class UiManager
    {
        public IButtonPage MainMenu { get; }
        public IButtonPage TargetMenu { get; }

        public UiManager(string menuName, Sprite menuSprite, bool createTargetMenu = true)
        {
            MainMenu = new ReMenuPage(menuName, true);
            ReTabButton.Create(menuName, $"Open the {menuName} menu.", menuName, menuSprite);

            if (createTargetMenu)
            {
                var localMenu = new ReCategoryPage(QuickMenuEx.SelectedUserLocal.transform);
                TargetMenu = localMenu.AddCategory($"{menuName}");
            }
        }
    }
}
