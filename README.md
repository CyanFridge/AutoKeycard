# AutoKeycard

A quality-of-life mod for **SPT (Single Player Tarkov)** that automatically selects and uses the correct keycard when interacting with keycard doors.

No more searching through your inventory to find the right Labs keycard. Simply interact with the door, and AutoKeycard handles the rest.

## ✨ Features

* 🔑 Automatically uses the correct keycard from your inventory
* 🚪 Replaces vanilla keycard interactions with a single **AutoKeycard** action
* 🛡️ Prevents incorrect keycard attempts
* 📋 Shows the required keycard name when you are missing one
* 🔔 Optional notification showing which keycard was used
* ⚙️ Configurable logging and debug options
* ✅ Uses EFT's existing unlock system to preserve vanilla behavior

## Compatibility

AutoKeycard versions are tied to specific SPT versions:

| SPT Version | AutoKeycard Version |
| ----------- | ------------------- |
| SPT 4.0.x   | v1.0.0              |
| SPT 4.1.x   | v1.0.1              |

⚠️ Compatibility with future SPT versions is not guaranteed.

## Installation

1. Download the version of AutoKeycard that matches your SPT version.
2. Extract the mod into your SPT `BepInEx/plugins` folder.
3. Launch SPT and use keycard doors normally.

## Does AutoKeycard bypass keycard requirements?

No.

AutoKeycard does **not** unlock doors without the required keycard. It does not remove keycard requirements or skip EFT's normal unlock process.

The mod simply automates the normal process:

1. Finds the correct keycard in your inventory.
2. Selects the keycard for the interaction.
3. Lets EFT handle the actual unlock operation.

## Configuration

AutoKeycard includes configurable options through **BepInEx Configuration Manager**.

### General

| Setting                    | Default | Description                                         |
| -------------------------- | ------- | --------------------------------------------------- |
| Show Required Keycard Name | Enabled | Shows the required keycard name when one is missing |
| Show Used Keycard Message  | Enabled | Displays a notification when a keycard is used      |

### Logging

| Setting        | Default  | Description                           |
| -------------- | -------- | ------------------------------------- |
| Enable Logging | Enabled  | Enables standard AutoKeycard logging  |
| Debug Logging  | Disabled | Enables additional diagnostic logging |

## How It Works (Technical)

AutoKeycard uses Harmony patches to modify EFT's interaction system.

When interacting with a locked keycard door:

1. AutoKeycard detects the available keycard door interactions.
2. Vanilla keycard actions are removed.
3. A custom AutoKeycard action is added.
4. The player's inventory is searched for a matching keycard.
5. The keycard's `KeyId` is validated against the door's required `KeyId`.
6. EFT's existing unlock operations and inventory events handle the rest.

## Technical Details

AutoKeycard is built using:

* BepInEx
* Harmony
* EFT API classes
* EFT's built-in notification system

Main systems used:

* `InteractionContextHelper` - Handles available interaction actions
* `KeycardDoor` - Handles keycard door validation and unlocking
* `NotificationManager` - Handles in-game notifications

## Credits

Created by **CyanFridge**.

Special thanks to the SPT modding community for documentation, tools, and reverse-engineering resources.

## License

This project is licensed under the MIT License.

See `LICENSE` for details.
