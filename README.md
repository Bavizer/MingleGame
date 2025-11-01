# ℹ Mingle Game
**A plugin for SCP: Secret Laboratory, that implements the Mingle Game inspired by Netflix series "Squid Game: Season 2":**
- 🏠 Special schematic - Mingle Game location spawns in the game.
- 🤵 Players spawn in the game location.
- 🤖 Plugin randomly selects game's calm part duration and players room amount.
- ⏱ The event ends when one or no players remain.

# ⭐ Features
- 🏠 **Schematic**: interactable location with many structured objects.
- ⚙ **Configurable**: change some event settings in the config file and schematic.
- 🤖 **Automatic**: you just need to start the event. That's all.
- ⌨ **Console commands**: you can start and end the event using Remote Admin Console commands.
- 🌐 **API**: access plugin's code features.

> [!NOTE]
> **Interactions with doors are done using NoClip key (ALT by default).**

# ✨ Starting and Ending event
To start or end the event you can use one of the methods below:
- Using commands in **Remote Admin Console**:
   - `start_mgame` command to start the event.
   - `end_mgame` command to end the event.
- Using **API**:
  - Call `MingleGame.Instance.StartEvent()` method to start the event.
  - Call `MingleGame.Instance.EndEvent()` method to end the event.

> [!NOTE]
> **You need `RoundEvents` permission to execute event commnads.**
 
> [!NOTE]
> **Players whose role is `Class-D` are automatically event players. That means, if your respawn player with `Class-D` role during the event, he will become event player.**

> [!TIP]
> **Before starting the event, assign players role to `Class-D`.**

# 📁Installation and Configuration
> [!IMPORTANT]
> **For this plugin to work correctly, you need to install (if you didn't) [ProjectMER](https://github.com/Michal78900/ProjectMER) and [AudioPlayerApi](https://github.com/Killers0992/AudioPlayerApi) plugins.**

> [!NOTE]
> **This plugin doesn't include audio files for license reasons. You need to include them by yourself and specify paths in the config file. Technically, you can use any audio file, even not related to the game.
> See audio requirements [here](https://github.com/Killers0992/AudioPlayerApi#audio-requirements).
> <br><ins>EVENT WILL NOT START IF AUDIO FILES WERE NOT FOUND!</ins>**

> [!NOTE]
> **Note that duration of audio files matters, for instance:**
> - **game's calm part is calculated as a random duration (from (duration / 2) to duration) of `CalmPart` audio file;**
> - **game's danger part is calculated as a duration of `DangerPart` audio file.**

##

Don't forget to put content from `Schematics` folder into your `Schematics` folder.

##

- [Installation Guide](https://github.com/northwood-studios/LabAPI/wiki/Installing-Plugins)  
- [Configuration Guide](https://github.com/northwood-studios/LabAPI/wiki/Configuring-Plugins)

# 👨‍💻 Authors
Plugin and game location schematic originally created by [Bavizer](https://github.com/Bavizer/).

# 🖼 Media

https://github.com/user-attachments/assets/14625bdc-0106-437f-ba13-c5774215768d

![MingleGame](https://github.com/user-attachments/assets/b7dc799d-b352-4e92-80a0-c44b5a3b70b6)
![MingleGameUnity](https://github.com/user-attachments/assets/1d3b7563-4e41-4ac9-b522-936b5994172a)
