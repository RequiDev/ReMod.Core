using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using ReMod.Core.Managers;
using System;
using System.Reflection;
using VRC.Core;
using VRC.DataModel;
using VRC.SDKBase;
using Player = VRC.Player;

namespace ReMod.Core
{
    public class ComponentDisabled : Attribute
    {
    }

    public class ComponentPriority : Attribute
    {
        public int Priority;
        public ComponentPriority(int priority = 0) => Priority = priority;
    }

    public class ModComponent
    {
        public virtual void OnUiManagerInitEarly(){}
        public virtual void OnUiManagerInit(UiManager uiManager){}
        public virtual void OnFixedUpdate(){}
        public virtual void OnUpdate(){}
        public virtual void OnLateUpdate(){}
        public virtual void OnGUI(){}
        public virtual void OnSceneWasLoaded(int buildIndex, string sceneName){}
        public virtual void OnSceneWasInitialized(int buildIndex, string sceneName){}
        public virtual void OnApplicationQuit(){}
        public virtual void OnPreferencesSaved(){}
        public virtual void OnPreferencesLoaded(){ }
        public virtual void OnPhotonPlayerJoined(int actorNr, Il2CppSystem.Collections.Hashtable propertiesTable) { }
        public virtual void OnPhotonPlayerLeft(int actorNr, Photon.Realtime.Player player) { }
        public virtual void OnPlayerJoined(Player player){}
        public virtual void OnPlayerLeft(Player player){}
        public virtual void OnAvatarIsReady(VRCPlayer player){ }
        public virtual void OnAvatarChanged(APIUser apiUser, ApiAvatar apiAvatar) { }
        public virtual void OnEnterWorld(ApiWorld world, ApiWorldInstance instance){}
        public virtual void OnSelectUser(IUser user, bool isRemote){ }
        public virtual void OnSetupUserInfo(APIUser apiUser){ }

        // return value determines whether it found something malicious and should block the original function
        public virtual bool ExecuteEvent(Player player, VRC_EventHandler.VrcEvent evt, VRC_EventHandler.VrcBroadcastType broadcastType, int instagatorId, float fastForward) { return false; }
        public virtual bool OnPhotonEvent(LoadBalancingClient loadBalancingClient, ref EventData eventData) { return false; }
        public virtual bool OnDownloadAvatar(ApiAvatar apiAvatar) { return false; }
        public virtual bool OnRaiseEvent(byte eventCode, ref Il2CppSystem.Object content, RaiseEventOptions raiseEventOptions, SendOptions sendOptions) { return false; }
        
        public virtual void OnOwnershipTransferred(Photon.Realtime.Player player, PhotonView photonView, bool isMaster, bool isRequest){}
        public virtual void OnBlockStateChange(Photon.Realtime.Player instigator, bool blocked){ }
        public virtual void OnMuteStateChange(Photon.Realtime.Player instigator, bool muted) { }
        public virtual void OnRenderObject() { }
        public virtual void OnOperationResponse(LoadBalancingClient loadBalancingClient, OperationResponse operationResponse) { }
        public virtual void OnJoinedRoom() { }
        public virtual void OnLeftRoom() { }
        public virtual void OnModulesLoaded() { }

        protected HarmonyMethod GetLocalPatch(string methodName)
        {
            return GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
        }
    }
}
