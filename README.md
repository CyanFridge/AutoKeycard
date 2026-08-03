# AutoKeycard

A quality-of-life scripting mod for **SPT (Single Player Tarkov)** that simplifies keycard door interactions by automatically selecting and using the correct keycard from the player's inventory.

## Features

* Automatically detects locked keycard doors
* Replaces vanilla keycard interaction options with a custom AutoKeycard action
* Automatically selects the correct keycard
* Prevents incorrect keycard attempts
* Displays the required keycard name when missing one
* Shows an in-game notification when a keycard is used
* Uses EFT's native notification system
* Configurable logging options
* Debug logging support for troubleshooting

## How It Works

AutoKeycard uses Harmony patches to modify EFT's interaction system.

When a player interacts with a locked keycard door:

1. AutoKeycard checks the available interaction actions.
2. Vanilla keycard actions are removed.
3. A custom AutoKeycard action is added.
4. The player's inventory is searched for a matching keycard.
5. The keycard's `KeyId` is validated against the door's required `KeyId`.
6. EFT's own unlock system is called to handle the door opening.

The mod does not bypass the normal unlock process. It uses EFT's existing unlock operations and events to preserve vanilla behavior.

## Configuration

AutoKeycard includes configurable options through BepInEx Configuration Manager.

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

## Technical Details

AutoKeycard is built using:

* BepInEx
* Harmony
* EFT API classes
* EFT's built-in notification system

Main systems used:

* `GetActionsClass` - Used to modify available interaction actions
* `KeycardDoor` - Used for keycard door validation and unlocking
* `NotificationManagerClass` - Used for in-game notifications

## Project Structure

```
AutoKeycard/
│
├── Plugin.cs
│   └── BepInEx plugin initialization
│
├── KeycardDoorPatch.cs
│   └── Harmony patch handling keycard door interactions
│
├── AutoKeycardConfig.cs
│   └── BepInEx configuration settings
│
├── AutoKeycardNotification.cs
│   └── EFT notification implementation
│
└── PluginInfo.cs
    └── Mod metadata and version information
```

## Compatibility

Designed for:

* SPT 4.x
* Tarkov versions supported by the installed SPT release

Compatibility with future SPT versions is not guaranteed.

## Credits

Created by Cyan.

Special thanks to the SPT modding community for documentation, tools, and reverse-engineering resources.

## License

This project is licensed under the MIT License.

See `LICENSE` for details.
