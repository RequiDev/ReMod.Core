using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRC.SDKBase;
using VRC.UI;

namespace ReMod.Core.VRChat
{
    public static class PlayerExtensions
    {
        public static Player[] GetPlayers(this PlayerManager playerManager)
        {
            return playerManager.prop_ArrayOf_Player_0;
        }

        public static Player GetPlayer(this PlayerManager playerManager, string userId)
        {
            foreach (var player in playerManager.GetPlayers())
            {
                if (player == null)
                    continue;

                var apiUser = player.GetAPIUser();
                if (apiUser == null)
                    continue;

                if (apiUser.id == userId)
                    return player;
            }

            return null;
        }

        public static Player GetPlayer(this PlayerManager playerManager, int actorNr)
        {
            foreach (var player in playerManager.GetPlayers())
            {
                if (player == null)
                    continue;
                if (player.prop_Int32_0 == actorNr)
                    return player;
            }

            return null;
        }

        public static VRCPlayer GetVRCPlayer(this Player player)
        {
            return player._vrcplayer;
        }

        public static APIUser GetAPIUser(this Player player)
        {
            return player.field_Private_APIUser_0;
        }

        public static ApiAvatar GetApiAvatar(this Player player)
        {
            return player.prop_ApiAvatar_0;
        }

        public static Player GetPlayer(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer._player;
        }

        public static PlayerNet GetPlayerNet(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer._playerNet;
        }

        public static GameObject GetAvatarObject(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer.field_Internal_GameObject_0;
        }

        public static VRCPlayerApi GetPlayerApi(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer.field_Private_VRCPlayerApi_0;
        }

        public static string GetUserID(this IUser iUser)
        {
            return iUser.prop_String_0;
        }

        public static string GetAvatarID(this IUser iUser)
        {
            string avatarID = "avtr_00000000-0000-0000-0000-000000000000";

            iUser.prop_ILoadable_1_IAvatar_0
                .Method_Public_Abstract_Virtual_New_Void_Action_1_T_Action_1_String_0(new Action<IAvatar>(
                    activeAvatar => { avatarID = activeAvatar.prop_String_0; }));

            return avatarID;
        }

        public static bool IsStaff(this APIUser user)
        {
            if (user.hasModerationPowers)
                return true;
            if (user.developerType != APIUser.DeveloperType.None)
                return true;
            return user.tags.Contains("admin_moderator") || user.tags.Contains("admin_scripting_access") ||
                   user.tags.Contains("admin_official_thumbnail");
        }

        private static MethodInfo _reloadAvatarMethod;
        private static MethodInfo LoadAvatarMethod
        {
            get
            {
                if (_reloadAvatarMethod == null)
                {
                    _reloadAvatarMethod = typeof(VRCPlayer).GetMethods().First(mi => mi.Name.StartsWith("Method_Private_Void_Boolean_") && mi.Name.Length < 31 && mi.GetParameters().Any(pi => pi.IsOptional) && XrefUtils.CheckUsedBy(mi, "ReloadAvatarNetworkedRPC"));
                }

                return _reloadAvatarMethod;
            }
        }

        private static MethodInfo _reloadAllAvatarsMethod;
        private static MethodInfo ReloadAllAvatarsMethod
        {
            get
            {
                if (_reloadAllAvatarsMethod == null)
                {
                    _reloadAllAvatarsMethod = typeof(VRCPlayer).GetMethods().First(mi => mi.Name.StartsWith("Method_Public_Void_Boolean_") && mi.Name.Length < 30 && mi.GetParameters().All(pi => pi.IsOptional) && XrefUtils.CheckUsedBy(mi, "Method_Public_Void_", typeof(FeaturePermissionManager)));// Both methods seem to do the same thing;
                }

                return _reloadAllAvatarsMethod;
            }
        }
        public static void ReloadAvatar(this VRCPlayer instance)
        {
            LoadAvatarMethod.Invoke(instance, new object[] { true }); // parameter is forceLoad and has to be true
        }

        public static void ReloadAllAvatars(this VRCPlayer instance, bool ignoreSelf = false)
        {
            ReloadAllAvatarsMethod.Invoke(instance, new object[] { ignoreSelf });
        }

        public static void ChangeToAvatar(this VRCPlayer instance, string avatarId)
        {
            if (!instance.GetPlayer().GetAPIUser().IsSelf)
            {
                throw new ArgumentException("You can't change other peoples avatar.", nameof(instance));
            }

            var apiModelContainer = new ApiModelContainer<ApiAvatar>
            {
                OnSuccess = new Action<ApiContainer>(c =>
                {
                    var pageAvatar = Resources.FindObjectsOfTypeAll<PageAvatar>()[0];
                    var apiAvatar = new ApiAvatar
                    {
                        id = avatarId
                    };
                    pageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = apiAvatar;
                    pageAvatar.ChangeToSelectedAvatar();
                })
            };
            API.SendRequest($"avatars/{avatarId}", 0, apiModelContainer, null, true, true, 3600f, 2);
        }
    }
}
