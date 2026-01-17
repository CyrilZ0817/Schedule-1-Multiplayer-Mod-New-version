# Schedule I Multiplayer Plus (v1.0.0)

A simple workaround to expand the multiplayer lobby limit of **Schedule I** from 4 to 16 players.

## ⚠️ Disclaimer
- **Beginner Code**: This is my first attempt at modding. It's designed to provide a temporary solution for friends who want to play together.
- **Experimental Status**: Expect bugs! For example, player avatars in the lobby may appear duplicated or incorrect. This is purely visual and doesn't affect the game start.
- **Wait for Original Author**: I encourage you to check the [original author's repository](https://github.com/MedicalMess/MonoFGMutliplayerPlus) for a more professional and updated version when it becomes available.
- **Console Warnings**: You will see **Yellow TypeLoadException warnings** in the MelonLoader console when the game starts. These are harmless artifacts of the Il2Cpp loading process and can be safely ignored.

## Features
- **Unlocks 16 Slots**: Extends Steam lobby capacity.
- **Join-Fix**: Bypasses the internal "Lobby Full" check that usually blocks the 5th player.
- **UI Sync**: Automatically replaces `x/4` text with `x/16`.

## Installation
1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader).
2. Place `ScheduleIMod.dll` into the `Mods` folder in your game directory.
3. **Mandatory**: Every player in the session must have this mod installed.

## Known Issues
- All extra players (5th to 16th) may display the Host's avatar in the lobby.
- Some yellow warnings appear in the console logs during startup.

## Credits
- Developed by: **cyrilz**
- Logic inspired by the original Mono mod for older versions.

## License
[MIT License](LICENSE)
