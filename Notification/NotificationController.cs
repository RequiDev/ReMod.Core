using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using ReMod.Core.VRChat;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;

namespace ReMod.Core.Notification
{
    public class NotificationController : MonoBehaviour
    {
        public Sprite defaultSprite;
        
        //Objects
        private Animator _notificationAnimator;
        private Image _iconImage;
        private Image _backgroundImage;
        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _descriptionText;

        private Queue<NotificationObject> _notificationQueue = new Queue<NotificationObject>();
        private bool _isDisplaying;
        private object _timerToken;
        private static bool _registered;
        
        //Current NotificationObject details
        private NotificationObject _currentNotification;
        
        public NotificationController(IntPtr ptr) : base(ptr)
        {

        }

        [HideFromIl2Cpp]
        public void EnqueueNotification(NotificationObject notif)
        {
            if (!IsInWorld()) return;
            
            _notificationQueue.Enqueue(notif);
        }

        [HideFromIl2Cpp]
        public void ClearNotifications()
        {
            _notificationQueue.Clear();
            ClearNotification();
        }
        
        [HideFromIl2Cpp]
        public static void RegisterSafe()
        {
            if (_registered) return;
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<NotificationController>();
                _registered = true;
            }
            catch (Exception)
            {
                // we assume that due to an exception being thrown, that we're already registered.
                _registered = true;
            }
        }

        private void Start()
        {
            _notificationAnimator = gameObject.GetComponent<Animator>();
            _backgroundImage = gameObject.transform.Find("Content/Background").GetComponent<Image>();
            _iconImage = gameObject.transform.Find("Content/Icon").gameObject.GetComponent<Image>();
            _titleText = gameObject.transform.Find("Content/Title").gameObject.GetComponent<TextMeshProUGUI>();
            _descriptionText = gameObject.transform.Find("Content/Description").gameObject.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (_notificationQueue.Count <= 0 || _isDisplaying) return;
            if (!IsInWorld())
            {
                ClearNotification();
                return;
            }
            
            _currentNotification = _notificationQueue.Dequeue();

            if (NotificationSystem.UseVRChatNotificationSystem)
            {
                //Using VRChat HUD Messages
                VRCUiManager.prop_VRCUiManager_0.QueueHudMessage($"[{_currentNotification.Title}] {_currentNotification.Description}", Color.white, _currentNotification.DisplayLength);
                return;
            }
            
            //Update UI
            _titleText.text = _currentNotification.Title;
            _descriptionText.text = _currentNotification.Description;
            _iconImage.sprite = _currentNotification.Icon == null ? defaultSprite : _currentNotification.Icon;
            _iconImage.enabled = true;
            _currentNotification.BackgroundColor.a = NotificationSystem.NotificationAlpha.Value;
            _backgroundImage.color = _currentNotification.BackgroundColor;

            OpenNotification();
        }

        [HideFromIl2Cpp]
        public void ClearNotification()
        {
            _currentNotification = null;
            CloseNotification();
        }

        [HideFromIl2Cpp]
        private void OpenNotification()
        {
            _isDisplaying = true;
            if (_timerToken != null)
            {
                MelonCoroutines.Stop(_timerToken);
                _timerToken = null;
            }
                
            //Play slide in animation
            _notificationAnimator.Play("In");

            //Start notification timer
            _timerToken = MelonCoroutines.Start(StartTimer());
        }

        [HideFromIl2Cpp]
        private void CloseNotification()
        {
            if (!_isDisplaying) return;
            
            if (_timerToken != null)
            {
                MelonCoroutines.Stop(_timerToken);
                _timerToken = null;
            }

            _isDisplaying = false;
            //Play slide out
            _notificationAnimator.Play("Out");
        }

        [HideFromIl2Cpp]
        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(_currentNotification.DisplayLength);
            CloseNotification();
        }

        [HideFromIl2Cpp]
        private bool IsInWorld()
        {
            return RoomManager.field_Internal_Static_ApiWorld_0 != null && PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 0;
        }
    }
}