@Stake
=======

@Stake is a game that fosters democracy, empathy, and creative problem solving for civic issues. Players take on a variety of roles and pitch ideas under a time pressure, competing to produce the best idea in the eyes of the table's “Decider.” 

This is a mobile (iOS & Android) version of the [tabletop game](http://engagementgamelab.org/games/@stake/) designed by the [Engagement Lab](http://elab.emerson.edu/) at Emerson College.

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
**1.** **Models:** DataManager.cs downloads the data from the API (or loads locally as a fallback) and deserializes it into the patterns defined in Models.cs.

**2.** **Views:** Views are populated from the data and interface with the GameController. Each view has a template associeated with it, and the template decides how to display the information. Templates use Unity's UI.

**3.** **Controller:** The GameController handles most of the game's logic, much of which is frontloaded when the host selects a deck (for example, players' roles are randomly chosen for each round at the very beginning of the game, rather than dynamically at the start of each round)

#### Preprocessors
To aid with development there a few global preprocessors that can be defined in the smcs.rsp file:
* `-define:SINGLE_SCREEN` Enables "single screen" version of the game. Multiple instances of the game are played on a single screen rather than between devices.
* `-define:SIMULATE_LATENCY` Delays network messages by a random value (up to 1 second) to simulate poor connections
* `-define:SHOW_DEBUG_INFO` Overlays debug information during gameplay
* `-define:FAST_TIME` Timers will run 10x faster
