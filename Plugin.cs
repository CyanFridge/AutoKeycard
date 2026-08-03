using BepInEx;
using HarmonyLib;
using EFT.Communications;

namespace AutoKeycard
{
    [BepInPlugin(
        PluginInfo.GUID,
        PluginInfo.NAME,
        PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public static BepInEx.Logging.ManualLogSource Log;


        private void Awake()
        {
            Instance = this;
            Log = Logger;


            AutoKeycardConfig.Bind(Config);


            LogInfo(
                $"{PluginInfo.NAME} {PluginInfo.VERSION} loaded successfully!");


            Harmony harmony = new Harmony("com.cyan.autokeycard");
            harmony.PatchAll();
        }

        public static void ShowNotification(string message)
        {
            try
            {
                NotificationManagerClass.DisplayNotification(
                    new AutoKeycardNotification(message));
            }
            catch (System.Exception ex)
            {
                LogDebug($"[AutoKeycard] Failed to show notification: {ex}");
            }
        }


        public static void LogInfo(string message)
        {
            if (AutoKeycardConfig.EnableLogging.Value)
            {
                Log.LogInfo(message);
            }
        }


        public static void LogDebug(string message)
        {
            if (AutoKeycardConfig.DebugLogging.Value)
            {
                Log.LogDebug(message);
            }
        }
    }
}