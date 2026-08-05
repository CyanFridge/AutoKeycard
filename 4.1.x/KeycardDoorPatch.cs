using Diz.LanguageExtensions;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;

namespace AutoKeycard
{
    // Adds a custom keycard door interaction while preserving EFT's existing interaction system.
    // This allows AutoKeycard to automatically select and use the correct keycard.
    [HarmonyPatch(typeof(InteractionContextHelper))]
    public class KeycardDoorPatch
    {
        // EFT uses this method to generate available interactions for keycard doors.
        // Allows AutoKeycard to modify the generated actions after vanilla creates them.
        [HarmonyPatch(
            "GetAvailableActions",
            typeof(GamePlayerOwner),
            typeof(KeycardDoor),
            typeof(bool))]
        [HarmonyPostfix]
        public static void Postfix(
            GamePlayerOwner owner,
            KeycardDoor door,
            bool isProxy,
            ref AvailableInteractionState __result)
        {
            // Only modify locked keycard doors.
            // Normal doors and already unlocked doors should behave normally.
            if (door.DoorState != EDoorState.Locked)
                return;

            Plugin.LogInfo("[AutoKeycard] Locked keycard door detected");
            Plugin.LogDebug($"[AutoKeycard] Door KeyId: {door.KeyId}");

            bool hasRequiredCard = false;

            // Check the player's inventory for a keycard matching this door.
            // KeyId validation prevents incorrect keycards from being attempted.
            foreach (var card in owner.GetAllKeyCards())
            {
                Plugin.LogDebug(
                    $"[AutoKeycard] Inventory card: {card.Item.TemplateId}, " +
                    $"KeyId: {card.Key.Template.KeyId}");

                if (card.Key.Template.KeyId == door.KeyId)
                {
                    hasRequiredCard = true;
                    break;
                }
            }

            string actionName = "Auto Keycard";

            // Changes the interaction text depending on whether the player has the required keycard.
            if (!hasRequiredCard)
            {
                if (AutoKeycardConfig.ShowRequiredKeycardName.Value)
                {
                    string requiredCardName = GetKeycardName(door.KeyId);

                    actionName = $"Missing {requiredCardName}";
                }
                else
                {
                    actionName = "Missing Required Keycard";
                }

                Plugin.LogInfo($"[AutoKeycard] {actionName}");
            }
            else
            {
                Plugin.LogInfo("[AutoKeycard] Required keycard found");
            }

            // Removes vanilla keycard interactions so they cannot conflict with the AutoKeycard action.
            int removedActions = __result.Actions.RemoveAll(
                action => action.Name.StartsWith("Try "));

            Plugin.LogDebug(
                $"[AutoKeycard] Removed {removedActions} vanilla keycard actions");

            // Add the custom interaction action.
            // It is disabled if the player does not have the required keycard.
            __result.Actions.Add(new InteractionAction
            {
                Name = actionName,
                Disabled = !hasRequiredCard,
                Action = new System.Action(() =>
                {
                    TryUnlock(owner, door);
                })
            });
        }

        // Attempts to unlock the door using the correct keycard from the player's inventory.
        private static void TryUnlock(
            GamePlayerOwner owner,
            KeycardDoor door)
        {
            Plugin.LogInfo(
                "[AutoKeycard] Searching inventory for compatible keycard...");

            foreach (var card in owner.GetAllKeyCards())
            {
                Plugin.LogDebug(
                    $"[AutoKeycard] Testing card: {card.Item.TemplateId}, " +
                    $"KeyId: {card.Key.Template.KeyId}");

                // Ignore any keycards that do not belong to this door.
                // This prevents incorrect cards from being selected.
                if (card.Key.Template.KeyId != door.KeyId)
                {
                    Plugin.LogDebug(
                        "[AutoKeycard] KeyId does not match door");
                    continue;
                }

                // EFT handles keycard validation and creates the unlock interaction result.
                // This keeps the door behavior identical to vanilla.
                var result = door.UnlockOperation(
                    card.Key,
                    owner.Player,
                    door);
                if (result.Failed)
                {
                    Plugin.LogDebug(
                        "[AutoKeycard] Matching keycard was rejected");
                    continue;
                }

                string usedCardName = GetKeycardName(card.Key.Template.KeyId);

                Plugin.LogInfo(
                    $"[AutoKeycard] Using valid keycard: {usedCardName}");

                // Uses EFT's notification system instead of a custom UI element.
                if (AutoKeycardConfig.ShowUsedKeycardMessage.Value)
                {
                    Plugin.ShowNotification($"Used {usedCardName}");
                }

                // Trigger vanilla inventory events.
                result.Value.RaiseEvents(
                    owner.Player.InventoryController,
                    CommandStatus.Begin);

                // Play the normal unlock animation and complete the operation.
                owner.Player.StartInteraction(
                    door,
                    result.Value,
                    () =>
                    {
                        Plugin.LogInfo(
                            "[AutoKeycard] Unlock animation finished");

                        result.Value.RaiseEvents(
                            owner.Player.InventoryController,
                            CommandStatus.Succeed);
                    });

                return;
            }

            Plugin.LogInfo(
                "[AutoKeycard] No valid keycard found");
        }


        // Retrieves a readable keycard name using EFT's localization system.
        // Falls back to "Unknown" if the localization entry does not exist.
        private static string GetKeycardName(string keyId)
        {
            string name = $"{keyId} ShortName".Localized(null);

            if (string.IsNullOrEmpty(name) ||
                name == $"{keyId} ShortName")
            {
                name = "Unknown";
            }

            if (!name.ToLower().Contains("keycard"))
            {
                name += " Keycard";
            }

            return name;
        }
    }
}