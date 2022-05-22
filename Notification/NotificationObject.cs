using UnityEngine;

namespace ReMod.Core.Notification
{
    public class NotificationObject
    {
        public string Title;
        public string Description;
        public Sprite Icon;
        public float DisplayLength;

        public NotificationObject(string title, string description, Sprite icon, float displayLength)
        {
            Title = title;
            Description = description;
            Icon = icon;
            DisplayLength = displayLength;
        }
    }
}