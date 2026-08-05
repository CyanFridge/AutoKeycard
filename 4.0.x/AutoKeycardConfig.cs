using BepInEx.Configuration;

namespace AutoKeycard
{
    public static class AutoKeycardConfig
    {
        public static ConfigEntry<bool> ShowRequiredKeycardName;
        public static ConfigEntry<bool> ShowUsedKeycardMessage;
        public static ConfigEntry<bool> EnableLogging;
        public static ConfigEntry<bool> DebugLogging;

        public static void Bind(ConfigFile config)
        {
            ShowRequiredKeycardName = config.Bind(
                "General",
                "Show Required Keycard Name",
                true,
                "Shows the required keycard name when missing one.");

            ShowUsedKeycardMessage = config.Bind(
                "General",
                "Show Used Keycard Message",
                true,
                "Shows a notification when a keycard is used.");

            EnableLogging = config.Bind(
                "Logging",
                "Enable Logging",
                true,
                "Enables normal AutoKeycard logging.");

            DebugLogging = config.Bind(
                "Logging",
                "Debug Logging",
                false,
                "Enables extra debugging information.");
        }
    }
}