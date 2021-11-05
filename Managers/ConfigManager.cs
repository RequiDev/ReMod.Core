using System;

namespace ReMod.Core.Managers
{
    public class ConfigManager
    {
        public string CategoryName { get; }

        public static ConfigManager Instance { get; private set; }

        public ConfigManager(string categoryName)
        {
            if (Instance != null)
            {
                throw new Exception("ConfigManager already exists.");
            }

            Instance = this;
            CategoryName = categoryName;
        }
    }
}
