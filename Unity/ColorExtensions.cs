using UnityEngine;

namespace ReMod.Core
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
}
