@Stake
=======

@Stake is a game that fosters democracy, empathy, and creative problem solving for civic issues. Players take on a variety of roles and pitch ideas under a time pressure, competing to produce the best idea in the eyes of the table's “Decider.” 

This is a mobile (iOS & Android) version of the [tabletop game](http://engagementgamelab.org/games/@stake/) designed by the [Engagement Lab](http://elab.emerson.edu/) at Emerson College.

## Developers
This is a Unity3D 5.3 project. Clone the repository and open the project in Unity to make edits.

### Preprocessors
To aid with development there a few global preprocessors that can be defined in the smcs.rs file:
* `-define:SINGLE_SCREEN` Enables "single screen" version of the game. Multiple instances of the game are played on a single screen rather than between devices.
* `-define:SIMULATE_LATENCY` Delays network messages by a random value (up to 1 second) to simulate poor connections
* `-define:SHOW_DEBUG_INFO` Overlays debug information during gameplay
* `-define:FAST_TIME` Timers will run 10x faster