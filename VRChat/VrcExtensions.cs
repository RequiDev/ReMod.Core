using System;
using System.Linq;
using System.Reflection;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;
using VRCSDK2;
using VRC_EventHandler = VRC.SDKBase.VRC_EventHandler;

namespace ReMod.Core.VRChat
{
    public static class VrcExtensions
    {
        private static MethodInfo _destroyPortalMethod;

        public static void Destroy(this PortalInternal instance)
        {
            if (_destroyPortalMethod == null)
            {
                _destroyPortalMethod = typeof(PortalInternal).GetMethods().First(m =>
                {
                    if (m.ReturnType != typeof(void))
                        return false;
                    return m.GetParameters().Length <= 0 && XrefScanner.XrefScan(m).Any(x => x.Type == XrefType.Global && x.ReadAsObject().ToString().Contains("Destroying static portal"));
                });
            }

            Networking.SetOwner(Networking.LocalPlayer, instance.gameObject);
            _destroyPortalMethod.Invoke(instance, new object[] { });
        }

        public static string GetParametersFromBytes(byte[] bytes, string prefix = "")
        {
            var parameters = ParameterSerialization.Method_Public_Static_ArrayOf_Object_ArrayOf_Byte_0(bytes);
            if (parameters == null)
            {
                return string.Empty;
            }

            var decodedAsString = string.Empty;
            var i = 0;
            foreach (var obj in parameters)
            {
                Il2CppSystem.Type type;
                string text;
                if (obj == null)
                {
                    type = Il2CppType.Of<Il2CppSystem.Object>();
                    text = "null";
                }
                else
                {
                    type = obj.GetIl2CppType();
                    if (type == Il2CppType.Of<byte>())
                        text = obj.Unbox<byte>().ToString();
                    else if (type == Il2CppType.Of<ushort>())
                        text = obj.Unbox<ushort>().ToString();
                    else if (type == Il2CppType.Of<short>())
                        text = obj.Unbox<short>().ToString();
                    else if (type == Il2CppType.Of<uint>())
                        text = obj.Unbox<uint>().ToString();
                    else if (type == Il2CppType.Of<int>())
                        text = obj.Unbox<int>().ToString();
                    else if (type == Il2CppType.Of<float>())
                        text = obj.Unbox<float>().ToString();
                    else if (type == Il2CppType.Of<long>())
                        text = obj.Unbox<long>().ToString();
                    else if (type == Il2CppType.Of<double>())
                        text = obj.Unbox<double>().ToString();
                    else if (type == Il2CppType.Of<bool>())
                        text = obj.Unbox<bool>().ToString();
                    else if (type == Il2CppType.Of<string>())
                        text = $"\"{obj.ToString()}\"";
                    else if (type == Il2CppType.Of<Vector3>())
                        text = obj.Unbox<Vector3>().ToString();
                    else if (type == Il2CppType.Of<Quaternion>())
                        text = obj.Unbox<Quaternion>().ToString();
                    else
                        text = obj.ToString();
                }

                if (type.IsArray && type.GetElementType() == Il2CppType.Of<string>())
                {
                    string[] array = Il2CppStringArray.WrapNativeGenericArrayPointer(obj.Pointer);
                    text = array.Length > 0 ? $"[{array.Length}]{{\n        \"{string.Join($"\",\n        \"", array)}\"\n    }}" : "[0]{}";
                }

                if (type.IsArray && type.GetElementType().IsArray && type.GetElementType().GetElementType() == Il2CppType.Of<string>())
                {
                    Il2CppStringArray[] arrayOut = Il2CppArrayBase<Il2CppStringArray>.WrapNativeGenericArrayPointer(obj.Pointer);
                    if (arrayOut.Length > 0)
                    {
                        text = $"[{arrayOut.Length}]{{";
                        foreach (string[] array in arrayOut)
                            text += array.Length > 0 ? $"\n        [{array.Length}]{{\n            \"{string.Join($"\",\n            \"", array)}\"\n        }}" : $"\n        [0]{{}}";
                        text += $"\n    }}";
                    }
                    else
                        text = "[0]{}";
                }

                if (type.IsArray && type.GetElementType() == Il2CppType.Of<VRC_SyncVideoPlayer.VideoEntry>())
                {
                    var array = Il2CppArrayBase<VRC_SyncVideoPlayer.VideoEntry>.WrapNativeGenericArrayPointer(obj.Pointer);
                    if (array.Length > 0)
                    {
                        text = $"[{array.Length}]{{\n";
                        text = array.Aggregate(text, (current, entry) => current + $"      \"{entry.URL}\",\n");
                        text += "    }}";
                    }
                    else
                    {
                        text = "[0]{}";
                    }
                }

                if (type.IsArray && type.GetElementType() == Il2CppType.Of<byte>())
                {
                    byte[] array = Il2CppArrayBase<byte>.WrapNativeGenericArrayPointer(obj.Pointer);
                    text = $"[{array.Length}]{BitConverter.ToString(array)}";
                }

                if (type.IsArray && type.GetElementType() == Il2CppType.Of<int>())
                {
                    int[] array = Il2CppArrayBase<int>.WrapNativeGenericArrayPointer(obj.Pointer);
                    text = array.Length > 0 ? $"[{array.Length}]{{\n        {prefix}{string.Join($",\n        {prefix}", array)}\n    {prefix}}}" : "[0]{}";
                }

                decodedAsString += $"{prefix}({type.Name}){i} = {text}\n";
                ++i;
            }

            return decodedAsString;
        }

        public static string Dump(this VRC_EventHandler.VrcEvent evt, string name)
        {
            var parameters = GetParametersFromBytes(evt.ParameterBytes);

            var eventName = evt.Name ?? string.Empty;
            var paramString = evt.ParameterString ?? string.Empty;

           return ($"Received VrcEvent from {name} -> \n" +
                       $"Name: {eventName}\n" +
                       $"EventType: {evt.EventType}\n" +
                       $"ParameterString: {paramString}\n" +
                       $"ParameterBoolOp: {evt.ParameterBoolOp}\n" +
                       $"ParameterBool: {evt.ParameterBool}\n" +
                       $"ParameterFloat: {evt.ParameterFloat}\n" +
                       $"ParameterInt: {evt.ParameterInt}\n" +
                       $"ParameterObject: {evt.ParameterObject?.name}\n" +
                       $"ParameterBytes: \n{parameters}");
        }
    }
}
