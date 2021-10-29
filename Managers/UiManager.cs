using System;
using ReMod.Core.UI;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;

namespace ReMod.Core.Managers
{
    public class UiManager
    {
        public ReMenuPage MainMenu { get; }
        public ReMenuCategory TargetMenu { get; }

        private static UiManager _instance;
        public UiManager(string menuName, Sprite menuSprite)
        {
            if (_instance != null)
            {
                throw new Exception("UiManager already exists.");
            }
            _instance = this;

            FixLaunchpadScrolling();

            var resourceManager = ResourceManager.Instance;
            MainMenu = new ReMenuPage(menuName, true);
            ReTabButton.Create(menuName, $"Open the {menuName} menu.", menuName, menuSprite);

            MainMenu.AddCategoryPage("Movement", sprite: resourceManager.GetSprite("running"));

            var visualCatPage = MainMenu.AddCategoryPage("Visuals", sprite: resourceManager.GetSprite("eye"));

            visualCatPage.AddCategory("ESP/Highlights");
            visualCatPage.AddCategory("Flashlight");
            visualCatPage.AddCategory("Wireframe");
            visualCatPage.AddCategory("Bone ESP");
            visualCatPage.AddCategory("Nametags");

            MainMenu.AddMenuPage("Dynamic Bones", sprite: resourceManager.GetSprite("bone"));
            MainMenu.AddMenuPage("Avatars", sprite: resourceManager.GetSprite("hanger"));

            var utilCatPage = MainMenu.AddCategoryPage("Utility", sprite: resourceManager.GetSprite("tools"));

            utilCatPage.AddCategory("Quality of Life");
            utilCatPage.AddCategory("Local Clone");
            utilCatPage.AddCategory("Objects");
            utilCatPage.AddCategory("Spoofing");
            utilCatPage.AddCategory("Protection");
            utilCatPage.AddCategory("Emojis");
            utilCatPage.AddCategory("Media Controls");
            utilCatPage.AddCategory("Staff Alerts");
            utilCatPage.AddCategory("Near Clipping Plane");
            utilCatPage.AddCategory("Application");

            MainMenu.AddMenuPage("Logging", sprite: resourceManager.GetSprite("log"));
            MainMenu.AddMenuPage("FBT", sprite: resourceManager.GetSprite("arms-up"));
            MainMenu.AddMenuPage("Hotkeys", sprite: resourceManager.GetSprite("keyboard"));

            TargetMenu = new ReMenuCategory($"{menuName}", ExtendedQuickMenu.Instance.field_Private_UIPage_1.transform.Find("ScrollRect").GetComponent<ScrollRect>().content);
        }

        private void FixLaunchpadScrolling()
        {
            var dashboard = ExtendedQuickMenu.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard").GetComponent<UIPage>();
            var scrollRect = dashboard.GetComponentInChildren<ScrollRect>();
            var dashboardScrollbar = scrollRect.transform.Find("Scrollbar").GetComponent<Scrollbar>();

            var dashboardContent = scrollRect.content;
            dashboardContent.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
            dashboardContent.Find("Carousel_Banners").gameObject.SetActive(false);

            scrollRect.enabled = true;
            scrollRect.verticalScrollbar = dashboardScrollbar;
            scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;
        }
    }
}
