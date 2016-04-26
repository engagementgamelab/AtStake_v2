@Stake
=======

@Stake is a game that fosters democracy, empathy, and creative problem solving for civic issues. Players take on a variety of roles and pitch ideas under a time pressure, competing to produce the best idea in the eyes of the table's “Decider.” 

This is a mobile (iOS & Android) version of the [tabletop game](https://elab.emerson.edu/projects/participation-and-engagement/atstake/) designed by the [Engagement Lab](http://elab.emerson.edu/) at Emerson College.

# Developers

This section describes how to get the app up and running and provides an overview of the project structure.

### Project overview
This is a Unity3D 5.3 project. Be sure you are using the [correct version](http://unity3d.com/unity/whats-new/unity-5.3.3) of Unity before opening the project.

@Stake is a local multiplayer game played with WIFI-connected iOS and Android devices. Content is served by the Engagement Lab API, so "decks" can be added and removed from the game without users needing to update the app. Within Unity, the project is structured using a model-view-controller pattern, and there are a few custom editor windows to help aid in development and debugging.

### Setup
**1.** Clone this repository: `git clone https://github.com/engagementgamelab/AtStake_v2.git`

**2.** Install and run the [Engagement Lab API](https://github.com/engagementgamelab/EL-API). If you skip this step the game will still run, but it will use a local fallback and populate the content using whatever is in data.json. Once running, update the configuration in api.json.

**3.** Install and run the [master server](https://github.com/engagementgamelab/master-server). It is required that the master server be running in order to connect devices. Once running, update the configuration in api.json.

### Project structure
**1.** **Models:** [DataManager.cs](https://github.com/engagementgamelab/AtStake_v2/blob/master/Assets/Scripts/Utilities/DataManager.cs) downloads the data from the API (or loads locally as a fallback) and deserializes it into the patterns defined in [Models.cs](https://github.com/engagementgamelab/AtStake_v2/blob/master/Assets/Scripts/Data/Models.cs).

**2.** **Views:** [Views](https://github.com/engagementgamelab/AtStake_v2/tree/master/Assets/Scripts/Screen/Views) are populated from the data and interface with the GameController. Each view has a [template](https://github.com/engagementgamelab/AtStake_v2/tree/master/Assets/Scripts/Screen/Templates) associated with it, and the template decides how to display the information. Templates use Unity's UI.

**3.** **Controller:** The [GameController](https://github.com/engagementgamelab/AtStake_v2/blob/master/Assets/Scripts/Data/GameController.cs) handles most of the game's logic, much of which is frontloaded when the host selects a deck (for example, players' roles are randomly chosen for each round at the very beginning of the game, rather than dynamically at the start of each round)

### Template Editor

It is advisable to always use the Template Editor when working with the templates. To open the window, go to Window -> Template Editor. Opening the window will automatically create the necessary template prefabs (if they don't already exist in the scene).

![Editor Window](https://github.com/engagementgamelab/AtStake_v2/raw/master/docs/template_editor.png "Editor Window")

Here's a breakdown of the interface:
* **Templates:** A dropdown to select the template in the hierarchy

* **Screen Elements:** A (read only) list of screen elements associated with the template. You can cross reference these with the template's view.

* **Show debug info:** Toggle the debug info overlay. This is not visible in the production version of the game, but it is useful during development. Note that toggling this in the window does not determine whether or not it will be displayed in play mode. For that, use the `-define:SHOW_DEBUG_INFO` preprocessor (detailed below)

* **Connect/Disconnect Container:** **IMPORTANT** If you break the prefab while editing a template, the TemplateContainer prefab will need to have its modifications applied at the root of the hierarchy (otherwise, "Save changes" will revert the modifications). To do this, disconnect the TemplateContainer, click "Apply" in the TemplateContainer inspector window, reconnect the TemplateContainer, and click "Save changes"

* **Save changes:** Applies the prefab modifications. You must save your changes in order to have them reflected in the game.

#### Preprocessors
To aid with development there a few global preprocessors that can be defined in the [smcs.rsp file](https://github.com/engagementgamelab/AtStake_v2/blob/master/Assets/smcs.rsp):
* `-define:SINGLE_SCREEN` Enables "single screen" version of the game. Multiple instances of the game are played on a single screen rather than between devices.
* `-define:SIMULATE_LATENCY` Delays network messages by a random value (up to 1 second) to simulate poor connections
* `-define:SHOW_DEBUG_INFO` Overlays debug information during gameplay
* `-define:FAST_TIME` Timers will run 10x faster
* `-define:MUTE_AUDIO` Audio will not play