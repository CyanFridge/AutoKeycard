using Diz.LanguageExtensions;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;

namespace AutoKeycard
{
    // Replaces the default keycard door interaction options with a custom action.
    // This allows AutoKeycard to automatically select and use the correct keycard.
    [HarmonyPatch(typeof(GetActionsClass))]
    public class KeycardDoorPatch
    {
        // EFT uses this method to generate available interactions for objects.
        // Allows me to modify the generated actions after vanilla creates them.
        [HarmonyPatch("smethod_13")]
        [HarmonyPostfix]
        public static void Postfix(
            GamePlayerOwner owner,
            KeycardDoor door,
            bool isProxy,
            ref ActionsReturnClass __result)
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

            // Removes vanilla "Try Keycard" interactions so they cannot conflict with the AutoKeycard action.
            int removedActions = __result.Actions.RemoveAll(
                action => action.Name.StartsWith("Try "));

            Plugin.LogDebug(
                $"[AutoKeycard] Removed {removedActions} vanilla keycard actions");

            // Add the custom interaction action.
            // It is disabled if the player does not have the required keycard.
            __result.Actions.Add(new ActionsTypesClass
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
                // This check prevents the original issue where incorrect cards could be selected.
                if (card.Key.Template.KeyId != door.KeyId)
                {
                    Plugin.LogDebug(
                        "[AutoKeycard] KeyId does not match door");
                    continue;
                }

                // EFT handles the actual unlock logic through UnlockOperation.
                // This keeps the door opening behavior identical to vanilla.
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
                owner.Player.vmethod_0(
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