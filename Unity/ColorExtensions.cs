using UnityEngine;

namespace ReMod.Core.Unity
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
}
