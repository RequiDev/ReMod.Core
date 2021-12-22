using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReMod.Core.UI
{
    public class UiElement
    {
        public string Name { get; }
        public GameObject GameObject { get; }
        public RectTransform RectTransform { get; }

        public Vector3 Position
        {
            get => RectTransform.localPosition;
            set => RectTransform.localPosition = value;
        }

        public bool Active
        {
            get => GameObject.activeSelf;
            set => GameObject.SetActive(value);
        }

        public UiElement(Transform transform)
        {
            RectTransform = transform.GetComponent<RectTransform>();
            if (RectTransform == null)
                throw new ArgumentException("Transform has to be a RectTransform.", nameof(transform));

            GameObject = transform.gameObject;
            Name = GameObject.name;
        }

        public UiElement(GameObject original, Transform parent, Vector3 pos, string name, bool defaultState = true) : this(original, parent, name, defaultState)
        {
            GameObject.transform.localPosition = pos;
        }

        public UiElement(GameObject original, Transform parent, string name, bool defaultState = true)
        {
            GameObject = Object.Instantiate(original, parent);
            GameObject.name = GetCleanName(name);
            Name = GameObject.name;

            GameObject.SetActive(defaultState);
            RectTransform = GameObject.GetComponent<RectTransform>();
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
        }

        public static string GetCleanName(string name)
        {
            return Regex.Replace(Regex.Replace(name, "<.*?>", string.Empty), @"[^0-9a-zA-Z_]+", string.Empty);
        }
    }
}
