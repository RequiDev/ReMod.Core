using System;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
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
    }
}
