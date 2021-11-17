using System;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace ReMod.Core.Unity
{
    [RegisterTypeInIl2Cpp]
    public class RenderObjectListener : MonoBehaviour
    {
        [method: HideFromIl2Cpp]
        public event Action RenderObject;
        public RenderObjectListener(IntPtr obj0) : base(obj0) { }

        public void OnRenderObject()
        {
            RenderObject?.Invoke();
        }
        
        private static bool _registered;
        [HideFromIl2Cpp]
        public static void RegisterSafe()
        {
            if (_registered) return;
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<RenderObjectListener>();
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
