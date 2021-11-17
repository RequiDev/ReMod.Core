using System;
using UnityEngine;

namespace ReMod.Core.Unity
{
    public static class UnityExtensions
    {
        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
                return "/" + current.name;
            return current.parent.GetPath() + "/" + current.name;
        }

        public static T[] GetComponentsInDirectChildren<T>(this GameObject gameObject)
        {
            var indexer = 0;

            foreach (var child in gameObject.transform)
            {
                var transform = child.Cast<Transform>();
                if (transform.GetComponent<T>() != null)
                {
                    indexer++;
                }
            }

            var returnArray = new T[indexer];

            indexer = 0;

            foreach (var child in gameObject.transform)
            {
                var transform = child.Cast<Transform>();
                if (transform.GetComponent<T>() != null)
                {
                    returnArray[indexer++] = transform.GetComponent<T>();
                }
            }

            return returnArray;
        }

        public static bool IsAbsurd(this float f)
        {
            return !(f > MaxAllowedValueBottom && f < MaxAllowedValueTop);
        }

        public static bool IsBad(this Vector3 v3)
        {
            return float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z) ||
                   float.IsInfinity(v3.x) || float.IsInfinity(v3.y) || float.IsInfinity(v3.z);
        }

        public const float MaxAllowedValueTop = 3.402823E+7f;
        public const float MaxAllowedValueBottom = -3.402823E+7f;
        public static bool IsAbsurd(this Vector3 v3)
        {
            return !(v3.x > MaxAllowedValueBottom && v3.x < MaxAllowedValueTop) ||
                   !(v3.y > MaxAllowedValueBottom && v3.y < MaxAllowedValueTop) ||
                   !(v3.z > MaxAllowedValueBottom && v3.z < MaxAllowedValueTop);
        }

        public static void Clamp(this Vector3 v3)
        {
            v3.x = Mathf.Clamp(v3.x, -512000f, 512000f);
            v3.y = Mathf.Clamp(v3.y, -512000f, 512000f);
            v3.z = Mathf.Clamp(v3.z, -512000f, 512000f);
        }

        public static void Clamp(this Quaternion v3)
        {
            v3.x = Mathf.Clamp(v3.x, -512000f, 512000f);
            v3.y = Mathf.Clamp(v3.y, -512000f, 512000f);
            v3.z = Mathf.Clamp(v3.z, -512000f, 512000f);
            v3.w = Mathf.Clamp(v3.w, -512000f, 512000f);
        }

        public static string ToCleanString(this Vector3 v3, string format="F4")
        {
            return v3.ToString(format).Replace(" ", string.Empty).Trim('(', ')');
        }

        /// <summary>
        /// Returns a copy of the float rounded to the given number.
        /// </summary>
        /// <param name="nearestFactor">The number the float should be rounded to</param>
        public static float RoundAmount(this float i, float nearestFactor)
        {
            return (float)Math.Round(i / nearestFactor) * nearestFactor;
        }

        /// <summary>
        /// Returns a copy of the vector rounded to the given number.
        /// </summary>
        /// <param name="nearestFactor">The number the vector should be rounded to</param>
        public static Vector3 RoundAmount(this Vector3 i, float nearestFactor)
        {
            return new Vector3(i.x.RoundAmount(nearestFactor), i.y.RoundAmount(nearestFactor), i.z.RoundAmount(nearestFactor));
        }

        /// <summary>
        /// Returns a copy of the vector rounded to the given number.
        /// </summary>
        /// <param name="nearestFactor">The number the vector should be rounded to</param>
        public static Vector2 RoundAmount(this Vector2 i, float nearestFactor)
        {
            return new Vector2(i.x.RoundAmount(nearestFactor), i.y.RoundAmount(nearestFactor));
        }
    }
}
