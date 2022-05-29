using UnityEngine;

namespace ReMod.Core.Notification
{
    public class NotificationObject
    {
        public string Title;
        public string Description;
        public Sprite Icon;
        public float DisplayLength;
        public Color BackgroundColor;

        public NotificationObject(string title, string description, Sprite icon, float displayLength, Color backgroundColor)
        {
            Title = title;
            Description = description;
            Icon = icon;
            DisplayLength = displayLength;
            BackgroundColor = backgroundColor;
        }
    }
}