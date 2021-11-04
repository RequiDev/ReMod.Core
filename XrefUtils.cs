using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnhollowerRuntimeLib.XrefScans;

namespace ReMod.Core
{
    public static class XrefUtils
    {

        /// <summary>
        /// Returns if a string is contained within the given method's body.
        /// </summary>
        /// <param name="method">The method to check</param>
        /// <param name="match">The string to check</param>
        public static bool CheckMethod(MethodInfo method, string match)
        {
            try
            {
                foreach (var instance in XrefScanner.XrefScan(method))
                {
                    if (instance.Type == XrefType.Global && instance.ReadAsObject().ToString().Contains(match))
                        return true;
                }

                return false;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        /// <summary>
        /// Returns if the given method is called by the other given method.
        /// </summary>
        /// <param name="method">The method to check</param>
        /// <param name="methodName">The name of the method that uses the given method</param>
        /// <param name="type">The type of the method that uses the given method</param>
        public static bool CheckUsedBy(MethodInfo method, string methodName, Type type = null)
        {
            foreach (var instance in XrefScanner.UsedBy(method))
            {
                if (instance.Type == XrefType.Method)
                {
                    try
                    {
                        if ((type == null || instance.TryResolve().DeclaringType == type) && instance.TryResolve().Name.Contains(methodName))
                            return true;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns whether the given method is using another the other given method.
        /// </summary>
        /// <param name="method">The method to check</param>
        /// <param name="methodName">The name of the method that is used by the given method</param>
        /// <param name="type">The type of the method that is used by the given method</param>
        public static bool CheckUsing(MethodInfo method, string methodName, Type type = null)
        {
            foreach (var instance in XrefScanner.XrefScan(method))
            {
                if (instance.Type == XrefType.Method)
                {
                    try
                    {
                        if ((type == null || instance.TryResolve().DeclaringType == type) && instance.TryResolve().Name.Contains(methodName))
                            return true;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            return false;
        }

        public static void DumpXRefs(this Type type)
        {
            MelonLogger.Msg($"{type.Name} XRefs:");
            foreach (var m in AccessTools.GetDeclaredMethods(type))
            {
                m.DumpXRefs(1);
            }
        }

        public static void DumpXRefs(this MethodInfo method, int depth = 0)
        {
            var indent = new string('\t', depth);
            MelonLogger.Msg($"{indent}{method.Name} XRefs:");
            foreach (var x in XrefScanner.XrefScan(method))
            {
                if (x.Type == XrefType.Global)
                {
                    MelonLogger.Msg($"\tString = {x.ReadAsObject()?.ToString()}");
                }
                else
                {
                    var resolvedMethod = x.TryResolve();
                    if (resolvedMethod != null)
                    {
                        MelonLogger.Msg($"{indent}\tMethod -> {resolvedMethod.DeclaringType?.Name}.{resolvedMethod.Name}");
                    }
                }
            }
        }
    }
}
