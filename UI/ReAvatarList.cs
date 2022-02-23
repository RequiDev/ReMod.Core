﻿using System;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using AvatarList = Il2CppSystem.Collections.Generic.List<VRC.Core.ApiAvatar>;

namespace ReMod.Core.UI
{
    public interface IAvatarListOwner
    {
        AvatarList GetAvatars(ReAvatarList avatarList);
        void Clear(ReAvatarList avatarList);
    }

    public class ReAvatarList : UiElement
    {
        private static GameObject _legacyAvatarList;
        private static GameObject LegacyAvatarList
        {
            get
            {
                if (_legacyAvatarList == null)
                {
                    _legacyAvatarList = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Legacy Avatar List");
                }

                return _legacyAvatarList;
            }
        }

        private readonly UiAvatarList _avatarList;

        private readonly bool _hasPagination;
        private readonly ReUiButton _refreshButton;
        private readonly ReUiButton _nextPageButton;
        private readonly ReUiButton _prevPageButton;
        private readonly ReUiText _pageCount;

        private int _currentPage;

        private int _maxAvatarsPerPage = 100;

        public SimpleAvatarPedestal AvatarPedestal => _avatarList.field_Public_SimpleAvatarPedestal_0;

        private readonly Text _textComponent;

        public string Title
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        public event Action OnEnable;

        private readonly string _title;

        private readonly IAvatarListOwner _owner;
        public ReAvatarList(string title, IAvatarListOwner owner, bool addClearButton = true, bool addPagination = true) : base(
            LegacyAvatarList,
            LegacyAvatarList.transform.parent,
            $"{title}AvatarList",
            false)
        {
            _hasPagination = addPagination;
            _owner = owner;
            _title = title;

            _avatarList = GameObject.GetComponent<UiAvatarList>();
            _avatarList.clearUnseenListOnCollapse = false;
            _avatarList.field_Public_Category_0 = UiAvatarList.Category.SpecificList;

            GameObject.transform.SetSiblingIndex(0);

            EnableDisableListener.RegisterSafe();
            var enableDisableListener = GameObject.AddComponent<EnableDisableListener>();
            enableDisableListener.OnEnableEvent += () =>
            {
                OnEnable?.Invoke();
                RefreshAvatars();
            };

            var expandButton = GameObject.GetComponentInChildren<Button>(true);
            _textComponent = expandButton.GetComponentInChildren<Text>();
            Title = title;

            var offset = 0f;
            if (addClearButton)
            {
                var clearButton = new ReUiButton("Clear", new Vector3(975, 0f), new Vector2(0.3f, 1), () => { _owner.Clear(this); }, expandButton.transform);
                offset = 85f;
            }

            _refreshButton = new ReUiButton("↻", new Vector3(980f - offset, 0f), new Vector2(0.25f, 1), RefreshAvatars, expandButton.transform);
            if (_hasPagination)
            {
                _nextPageButton = new ReUiButton("→", new Vector2(900f - offset, 0f), new Vector2(0.25f, 1f), () =>
                {
                    _currentPage += 1;
                    RefreshAvatars();
                }, expandButton.transform);

                _prevPageButton = new ReUiButton("←", new Vector2(750f - offset, 0f), new Vector2(0.25f, 1f), () =>
                {
                    _currentPage -= 1;
                    RefreshAvatars();
                }, expandButton.transform);

                _pageCount = new ReUiText("0 / 0", new Vector2(825f - offset, 0f), new Vector2(0.25f, 1f), () =>
                {
                    VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowInputPopupWithCancel("Goto Page", string.Empty, InputField.InputType.Standard, true, "Submit",
                        (s, k, t) =>
                        {
                            if (string.IsNullOrEmpty(s))
                                return;

                            _currentPage = int.Parse(s) - 1;
                            RefreshAvatars();
                        }, null, "Enter page...");
                }, expandButton.transform);
            }
        }

        public void SetMaxAvatarsPerPage(int value)
        {
            _maxAvatarsPerPage = value;
            RefreshAvatars();
        }
        
        public void RefreshAvatars()
        {
            Refresh(_owner.GetAvatars(this));
        }

        public void Refresh(AvatarList avatars)
        {
            if (_hasPagination)
            {
                var pagesCount = 0;
                if (avatars.Count != 0)
                {
                    pagesCount = (avatars.Count - 1) / _maxAvatarsPerPage;
                }
                _currentPage = Mathf.Clamp(_currentPage, 0, pagesCount);

                _pageCount.Text = $"{_currentPage + 1} / {pagesCount + 1}";
                var cutDown = avatars.GetRange(_currentPage * _maxAvatarsPerPage,
                    Math.Abs(_currentPage * _maxAvatarsPerPage - avatars.Count));
                if (cutDown.Count > _maxAvatarsPerPage)
                {
                    cutDown.RemoveRange(_maxAvatarsPerPage, cutDown.Count - _maxAvatarsPerPage);
                }

                _prevPageButton.Interactable = _currentPage > 0;
                _nextPageButton.Interactable = _currentPage < pagesCount;

                Title = $"{_title} ({cutDown.Count * (_currentPage + 1)}/{avatars.Count})";

                _avatarList.StartRenderElementsCoroutine(cutDown);
            }
            else
            {
                _avatarList.StartRenderElementsCoroutine(avatars);
            }
        }
    }
}
