using System;
using System.IO;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReMod.Core.Notification
{
    public class NotificationSystem
    {
        public static Sprite DefaultIcon;
        public static bool UseVRChatNotificationSystem;
        
        //AssetBundle Parts
        private static AssetBundle _notifBundle;
        private static GameObject _notificationPrefab;
        
        private static GameObject _hudContent;
        private static GameObject _notificationGO;
        private static NotificationController _controllerInstance;
        
        public static void SetupNotifications()
        {
            _hudContent = GameObject.Find("/UserInterface/UnscaledUI/HudContent");

            var notificationTransform = _hudContent.transform.Find("Notification (Clone)");
            if (notificationTransform != null)
            {
                //Notification system already initialized
                _notificationGO = notificationTransform.gameObject;
                _controllerInstance = _notificationGO.GetComponent<NotificationController>();
            }

            LoadAssets();
            
            NotificationController.RegisterSafe();

            if (_notificationPrefab == null)
                throw new Exception("NotificationSystem failed to load, prefab missing!");

            //Instantiate prefab and let NotificationController setup!
            _notificationGO = Object.Instantiate(_notificationPrefab, _hudContent.transform);
            _controllerInstance = _notificationGO.AddComponent<NotificationController>();

            _controllerInstance.defaultSprite = DefaultIcon;
        }

        /// <summary>
        /// Enqueue a new notification
        /// </summary>
        /// <param name="title">Title shown in the top of the notification</param>
        /// <param name="description">Main description, scales based on size</param>
        /// <param name="displayLength">How long in seconds you want it shown</param>
        /// <param name="icon">Optional icon sprite, defaults to Megaphone</param>
        public static void EnqueueNotification(string title, string description, float displayLength = 5f, Sprite icon = null)
        {
            var notif = new NotificationObject(title, description, icon, displayLength);
            
            if(_controllerInstance == null)
                SetupNotifications();
            
            _controllerInstance.EnqueueNotification(notif);
        }

        public static void ClearNotification()
        {
            if(_controllerInstance == null)
                SetupNotifications();
            
            _controllerInstance.ClearNotifications();
        }

        private static void LoadAssets()
        {
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReMod.Core.Notification.notification"))
            {
                if (assetStream != null)
                {
                    using var tempStream = new MemoryStream((int) assetStream.Length);
                    assetStream.CopyTo(tempStream);

                    _notifBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                    _notifBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
            }

            if (_notifBundle != null)
            {
                //Load Sprites
                DefaultIcon = _notifBundle.LoadAsset_Internal("Megaphone", Il2CppType.Of<Sprite>()).Cast<Sprite>();
                DefaultIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                //Prefab
                _notificationPrefab = _notifBundle.LoadAsset_Internal("Notification", Il2CppType.Of<GameObject>()).Cast<GameObject>();
                _notificationPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
        }
    }
}