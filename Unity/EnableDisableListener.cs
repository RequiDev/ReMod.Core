using System;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace ReMod.Core.Unity
{
    [RegisterTypeInIl2Cpp]
    public class EnableDisableListener : MonoBehaviour
    {
        [method: HideFromIl2Cpp]
        public event Action OnEnableEvent;
        [method: HideFromIl2Cpp]
        public event Action OnDisableEvent;

        public EnableDisableListener(IntPtr obj) : base(obj) { }
        public void OnEnable() => OnEnableEvent?.Invoke();
        public void OnDisable() => OnDisableEvent?.Invoke();

        private static bool _registered;
        [HideFromIl2Cpp]
        public static void RegisterSafe()
        {
            if (_registered) return;
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<EnableDisableListener>();
                _registered = true;
            }
            catch (Exception)
            {
                // we assume that due to an exception being thrown, that we're already registered.
                _registered = true;
            }
        }
    }
}
