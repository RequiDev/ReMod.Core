using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ReMod.Core.Managers
{
    public class ResourceManager
    {
        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        private readonly Assembly _ourAssembly;

        private readonly string _resourcePath;

        public static ResourceManager Instance { get; private set; }
        public ResourceManager(Assembly ourAssembly, string resourcePath)
        {
            if (Instance != null)
            {
                throw new Exception("ResourceManager already exists.");
            }
            Instance = this;

            _ourAssembly = ourAssembly;
            _resourcePath = resourcePath;
        }

        public Texture2D GetTexture(string resourceName)
        {
            if (_textures.ContainsKey(resourceName))
            {
                return _textures[resourceName];
            }

            var resourcePath = $"{_resourcePath}.{resourceName}.png";
            var stream = _ourAssembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                throw new ArgumentException($"Resource \"{resourcePath}\" doesn't exist", nameof(resourceName));
            }

            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            var texture = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture, ms.ToArray());
            texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            texture.wrapMode = TextureWrapMode.Clamp;

            _textures.Add(resourceName, texture);

            return texture;
        }

        public Sprite GetSprite(string resourceName)
        {
            if (_sprites.ContainsKey(resourceName))
            {
                return _sprites[resourceName];
            }

            var texture = GetTexture(resourceName);

            var rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            var border = Vector4.zero;
            var sprite = Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, 100.0f, 0, SpriteMeshType.Tight, ref border, false);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            _sprites.Add(resourceName, sprite);

            return sprite;
        }
    }
}
