using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using Object = UnityEngine.Object;

namespace ReMod.Core.VRChat
{
    public static class QuickMenuEx
    {
        private static VRC.UI.Elements.QuickMenu _quickMenuInstance;

        public static VRC.UI.Elements.QuickMenu Instance
        {
            get
            {
                if (_quickMenuInstance == null)
                {
                    _quickMenuInstance = Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>();
                }
                return _quickMenuInstance;
            }
        }

        public static MenuStateController MenuStateCtrl => Instance.field_Protected_MenuStateController_0;

        private static Wing[] _wings;
        private static Wing _leftWing;
        private static Wing _rightWing;

        private static Transform _cameraMenu;

        public static Wing[] Wings
        {
            get
            {
                if (_wings == null || _wings.Length == 0)
                {
                    _wings = Object.FindObjectsOfType<Wing>();
                }

                return _wings;
            }
        }

        public static Wing LeftWing
        {
            get
            {
                if (_leftWing == null)
                {
                    _leftWing = Wings.FirstOrDefault(w => w.field_Public_WingPanel_0 == Wing.WingPanel.Left);
                }
                return _leftWing;
            }
        }

        public static Wing RightWing
        {
            get
            {
                if (_rightWing == null)
                {
                    _rightWing = Wings.FirstOrDefault(w => w.field_Public_WingPanel_0 == Wing.WingPanel.Right);
                }
                return _rightWing;
            }
        }

        public static Transform CameraMenu
        {
            get
            {
                if (_cameraMenu == null)
                {
                    _cameraMenu = Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Camera");
                }
                return _cameraMenu;
            }
        }

        private static Sprite _onIconSprite;
        public static Sprite OnIconSprite
        {
            get
            {
                if (_onIconSprite == null)
                {
                    _onIconSprite = Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Notifications/Panel_NoNotifications_Message/Icon").GetComponent<Image>().sprite;
                }
                return _onIconSprite;
            }
        }

        private static Sprite _offIconSprite;
        public static Sprite OffIconSprite
        {
            get
            {
                if (_offIconSprite == null)
                {
                    _offIconSprite = TogglePrefab.transform.Find("Icon_Off").GetComponent<Image>().sprite;
                }
                return _offIconSprite;
            }
        }

        private static GameObject _togglePrefab;
        public static GameObject TogglePrefab
        {
            get
            {
                if (_togglePrefab == null)
                {
                    _togglePrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Settings/Panel_QM_ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_UI_Elements_Row_1/Button_ToggleQMInfo").gameObject;
                }
                return _togglePrefab;
            }
        }
    }
}
